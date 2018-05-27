using UnityEngine;
using System.Collections.Generic;
using System;

namespace VoidWars {
    public class HumanMoveShipManipulator : Manipulator {
        public override void OnDeactivation() {
            if (_moveTemplate != null) {
                Destroy(_moveTemplate.gameObject);
                _moveTemplate = null;
            }
        }

        private void Start() {
            var aoLayerMask = 1 << LayerMask.NameToLayer("ActiveObjects");
            var shipLayerMask = 1 << LayerMask.NameToLayer("Ships");
            _layerMask = aoLayerMask | shipLayerMask;
            _shipController = gameObject.GetComponent<ShipController>();
            _gameController = Util.GetGameController();
            _bounds = _gameController.GetBoardBounds();
            generateLegalMoves();
            _currentMove = 0;
            showTemplate();
        }

        private void Update() {
            int next = _currentMove;
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                _gameController.UIAudioPlayer.PlayButtonClick();
                next = _currentMove - 1;
                if (next < 0) {
                    next = _legalMoves.Count - 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                _gameController.UIAudioPlayer.PlayButtonClick();
                next = _currentMove + 1;
                if (next == _legalMoves.Count) {
                    next = 0;
                }
            }

            if (next != _currentMove) {
                _currentMove = next;
                showTemplate();
            }
        }

        private void showTemplate() {
            if (_moveTemplate != null) {
                Destroy(_moveTemplate.gameObject);
                _moveTemplate = null;
            }

            var current = _legalMoves[_currentMove];
            var moveInstance = new ShipMoveInstance {
                Move = current,
                ShipID = _shipController.ID
            };
            if (current.MoveType != MoveType.None) {
                var templatePrefab = Array.Find(_gameController.MoveTemplates,
                    t => t.MoveType == current.MoveType && t.Size == current.Size);
                Transform node;
                if (current.MoveType == MoveType.Reverse) {
                    node = gameObject.transform.Find("NodeRear");
                }
                else {
                    node = gameObject.transform.Find("NodeFront");
                }

                // TODO: cache templates.
                _moveTemplate = Instantiate(templatePrefab, node.transform.position, node.transform.rotation);
                moveInstance.Position = _moveTemplate.EndPoint.transform.position;
                moveInstance.Rotation = _moveTemplate.EndPoint.transform.rotation;
            }
            _gameController.SelectedMove = moveInstance;
            _gameController.InfoPanel.NotifyContent("SetMoveName", current.ToString());
        }

        private void generateLegalMoves() {
            // For all possible moves, determine if it will take the ship OOB or collide with another
            // ship on the way, and whether the ship has sufficient energy.
            var frontNode = _shipController.FrontNode;
            var rearNode = _shipController.RearNode;
            foreach(var template in _gameController.MoveTemplates) { 
                if (checkMove(frontNode, rearNode, template)) {
                    _legalMoves.Add(new ShipMove(template.MoveType, template.Size));
                }
            }

            // TODO review doing nothing is always an option?
            var noMove = new ShipMove(MoveType.None);
            _legalMoves.Add(noMove);

        }

        private bool checkMove(Transform frontNode, Transform rearNode, MoveTemplate template) {
            var type = template.MoveType;
            var size = template.Size;

            // Can the ship do this?
            var shipClass = _shipController.ShipData;
            if ((type == MoveType.Reverse) && !shipClass.HasReverseThrust) {
                return false;
            }

            if ((type == MoveType.SharpTurnLeft || type == MoveType.SharpTurnRight) && (_shipController.ShipData.Maneuverability == 1)) {
                return false;
            }

            if ((type == MoveType.TurnAbout ) && (_shipController.ShipData.Maneuverability != 3)) {
                return false;
            }

            // Work out where the ship would end up.
            float direction;
            Transform startNode;
            if (type == MoveType.Reverse) {
                startNode = rearNode;
                direction = -1f;
            }
            else {
                startNode = frontNode;
                direction = 1f;
            }

            var startPos = startNode.transform.localPosition;
            var endObj = template.EndPoint;
            var localEndPosition = startPos + direction*endObj.localPosition;
            var worldEndPosition = gameObject.transform.TransformPoint(localEndPosition);

            // Is this within the board bounds?
            if (!_bounds.Contains(new Vector2(worldEndPosition.x, worldEndPosition.z))) { 
                // TODO: less harsh condition, say "any of the ship can be in bounds"? 
                return false;
            }

            // Do I overlap with any ships or objects at the end?
            var collider = gameObject.GetComponent<Collider>();
            var colliderSize = collider.bounds.size;
            var radius = Mathf.Max(colliderSize.x, colliderSize.y, colliderSize.z) / 2f;
            var overlaps = Physics.OverlapSphere(worldEndPosition, radius, _layerMask);
            if ((overlaps != null) && (overlaps.Length > 0)) {
                // Colliding with something - discount oneself, though.
                foreach(var overlap in overlaps) {
                    if (!ReferenceEquals(overlap.gameObject, gameObject)) {
                        return false;
                    }
                }
            }

            // Can the ship do it at all?
            if (_shipController.MaxMoveSize < size) {
                return false;
            }

            // Compute the energy requirement of the move and see if we've got enough to do it.
            var move = new ShipMove { MoveType = type, Size = size };
            var energyRequired = _shipController.GetEnergyForMove(move);
            var energyAvailable = _shipController.GetEnergyBudgetFor(EnergyConsumer.Propulsion);
            if (energyRequired > energyAvailable) {
                return false;
            }

            return true;
        }

        private int _currentMove;
        private MoveTemplate _moveTemplate;
        private readonly List<ShipMove> _legalMoves = new List<ShipMove>();
        private ShipController _shipController;
        private GameController _gameController;
        private Rect _bounds;
        private int _layerMask;
    }
}