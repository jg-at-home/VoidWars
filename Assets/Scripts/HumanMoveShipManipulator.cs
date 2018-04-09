using UnityEngine;
using System.Collections.Generic;
using System;

namespace VoidWars {
    public class HumanMoveShipManipulator : MonoBehaviour {
        void Start() {
            // TODO: cache nodes.
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
                next = _currentMove - 1;
                if (next < 0) {
                    next = _legalMoves.Count - 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow)) {
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

                var position = node.transform.position;
                // Offset in y to appear above the active ship marker. TODO: maybe use layer order?
                position.y += 0.01f;
                _moveTemplate = Instantiate(templatePrefab, position, node.transform.rotation);
            }

            _gameController.InfoPanel.NotifyContent("SetMoveName", current.ToString());
        }

        private void generateLegalMoves() {
            // For all possible moves, determine if it will take the ship OOB or collide with another
            // ship on the way, and whether the ship has sufficient energy.
            var frontNode = gameObject.transform.Find("NodeFront");
            var rearNode = gameObject.transform.Find("NodeRear");
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
            var shipClass = _shipController.ShipClass;
            if ((type == MoveType.Reverse) && !shipClass.HasReverseThrust) {
                return false;
            }

            if ((type == MoveType.SharpTurnLeft || type == MoveType.SharpTurnRight) && (shipClass.Maneuverability == 1)) {
                return false;
            }

            if ((type == MoveType.TurnAbout ) && (shipClass.Maneuverability != 3)) {
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
            var layer = LayerMask.NameToLayer("ActiveObjects");
            var overlaps = Physics.OverlapSphere(worldEndPosition, radius, layer);
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
            var energyRequired = _shipController.MassRatio * shipClass.MoveDrainRate * moveScale(type, size);
            var energyAvailable = _shipController.GetEnergyFor(EnergyConsumer.Propulsion);
            if (energyRequired > energyAvailable) {
                return false;
            }

            return true;
        }

        private static float moveScale(MoveType type, int size) {
            switch(type) {
                case MoveType.Forward:
                case MoveType.Reverse:
                    return size;

                case MoveType.GentleTurnLeft:
                case MoveType.GentleTurnRight:
                    return size * Mathf.PI/4f;

                case MoveType.SharpTurnLeft:
                case MoveType.SharpTurnRight:
                    return size * Mathf.PI/2f;

                case MoveType.TurnAbout:
                    return size * 3f;

                default:
                    Debug.Assert(false, "Huh?");
                    return size;
            }
        }

        private int _currentMove;
        private MoveTemplate _moveTemplate;
        private readonly List<ShipMove> _legalMoves = new List<ShipMove>();
        private ShipController _shipController;
        private GameController _gameController;
        private Rect _bounds;
    }
}