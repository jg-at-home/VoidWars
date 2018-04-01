using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Comoponent that allows a human to set their ships up.
    /// </summary>
    public class HumanSetupManipulator : MonoBehaviour {
        [Tooltip("Speed of ship rotation")]
        public float TurnSpeed = 22.0f;

        [Tooltip("Acceleration for left-right motion")]
        public float MoveAcceleration = 4.0f;

        [Tooltip("Initial horizontal speed of ship")]
        public float MoveSpeed = 3.0f;

        [Tooltip("Max left-right speed.")]
        public float MaxSpeed = 8.0f;

        private void Start() {
            _shipController = gameObject.GetComponent<ShipController>();
            _gameController = Util.GetGameController();
            _bounds = _gameController.GetStartPositionBoundary(_shipController.StartPointIndex);
        }

        private void Update() {
            updatePosition();
            updateRotation();
        }

        private void updateRotation() {
            if (Input.GetKey(KeyCode.UpArrow)) {
                _rotation.y = Time.deltaTime * TurnSpeed;
                gameObject.transform.Rotate(_rotation);
            }
            else if (Input.GetKey(KeyCode.DownArrow)) {
                _rotation.y = -Time.deltaTime * TurnSpeed;
                gameObject.transform.Rotate(_rotation);
            }
        }

        private void updatePosition() {
            var position = gameObject.transform.position;
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
                // A left / right key just went down.
                _moveSpeed = MoveSpeed;
            }

            bool positionChanged = false;
            if (Input.GetKey(KeyCode.LeftArrow)) {
                _moveSpeed += MoveAcceleration * Time.deltaTime;
                if (_moveSpeed > MaxSpeed) {
                    _moveSpeed = MaxSpeed;
                }

                position.x -= _moveSpeed * Time.deltaTime;
                if (position.x < _bounds.xMin) {
                    position.x = _bounds.xMin;
                }
                positionChanged = true;
            }
            else if (Input.GetKey(KeyCode.RightArrow)) {
                _moveSpeed += MoveAcceleration * Time.deltaTime;
                if (_moveSpeed > MaxSpeed) {
                    _moveSpeed = MaxSpeed;
                }

                position.x += _moveSpeed * Time.deltaTime;
                if (position.x < _bounds.xMin) {
                    position.x = _bounds.xMin;
                }
                positionChanged = true;
            }

            if (positionChanged) {
                gameObject.transform.position = position;
            }
        }
    
        private ShipController _shipController;
        private GameController _gameController;
        private Rect _bounds;
        private float _moveSpeed;
        private Vector3 _rotation = new Vector3(0f, 1f, 0f);
    }
}