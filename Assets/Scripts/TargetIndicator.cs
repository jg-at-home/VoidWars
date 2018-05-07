using UnityEngine;

namespace VoidWars {
    public class TargetIndicator : MonoBehaviour {
        public float ScrollSpeed = 2.5f;
        public float Radius;
        public Material Material;
        public float LineWidth = 0.1f;
        public int NumCirclePoints = 32;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private LineRenderer _circleRenderer;

        /// <summary>
        /// Gets the target object.
        /// </summary>
        public GameObject Target {
            get { return _target; }
        }

        /// <summary>
        /// Initializes the indicator.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="target">Target object.</param>
        public void Initialize(GameObject source, GameObject target) {
            var delta = target.transform.position - source.transform.position;
            var direction = delta.normalized;

            // Create the line.
            _lineRenderer.positionCount = 2;
            Vector3 lineStart = source.transform.position + Radius * direction;
            _lineRenderer.SetPosition(0, lineStart);
            Vector3 lineEnd = target.transform.position - Radius * direction;
            _lineRenderer.SetPosition(1, lineEnd);

            // Now the circle.
            _circleRenderer.positionCount = NumCirclePoints;
            _circleRenderer.loop = true;
            Vector3 point;
            point.y = target.transform.position.y;
            float angleStep = Mathf.PI * 2f / NumCirclePoints;
            float angle = 0f;
            for(int i = 0; i < NumCirclePoints; ++i) {
                point.x = target.transform.position.x + Mathf.Cos(angle) * Radius;
                point.z = target.transform.position.z + Mathf.Sin(angle) * Radius;
                _circleRenderer.SetPosition(i, point);
                angle += angleStep;
            }

            _target = target;
        }

        private void Start() {
            setShared(_lineRenderer);
            setShared(_circleRenderer);
        }

        private void Update() {
            _offset.x -= ScrollSpeed * Time.deltaTime;
            if (_offset.x < 0f) {
                _offset.x += 1f;
            }

            Material.SetTextureOffset("_MainTex", _offset);
        }

        private void setShared(LineRenderer renderer) {
            renderer.material = Material;
            renderer.startWidth = LineWidth;
            renderer.endWidth = LineWidth;
            renderer.useWorldSpace = true;
        }

        [SerializeField] private GameObject _target;
        private Vector2 _offset = new Vector2(0f, 0f);
    }
}