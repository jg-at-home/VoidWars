using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Comoponent that allows a human to set their ships up.
    /// </summary>
    public class HumanSetupManipulator : Manipulator {
        [Tooltip("Speed of ship rotation")]
        public float TurnSpeed = 22.0f;

        [Tooltip("Horizontal speed of ship")]
        public float MoveSpeed = 30.0f;

        private void Start() {
            _shipController = gameObject.GetComponent<ShipController>();
            _rb = gameObject.GetComponent<Rigidbody>();
            _gameController = Util.GetGameController();
            _bounds = _gameController.GetStartPositionBoundary(_shipController.StartPointIndex);
        }

        private void FixedUpdate() {
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
            var axis = Input.GetAxis("Horizontal");
            var motion = new Vector3(axis, 0f, 0f);
            _rb.velocity = motion * MoveSpeed;
            _rb.position += _rb.velocity * Time.deltaTime;
            _rb.position = new Vector3(Mathf.Clamp(_rb.position.x, _bounds.xMin, _bounds.xMax), _rb.position.y, _rb.position.z);
        }
    
        private ShipController _shipController;
        private GameController _gameController;
        private Rect _bounds;
        private Vector3 _rotation = new Vector3(0f, 1f, 0f);
        private Rigidbody _rb;
    }
}