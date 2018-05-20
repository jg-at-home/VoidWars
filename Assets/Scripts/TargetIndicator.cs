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
        /// Initializes the indicator.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="target">Target object.</param>
        public void Initialize(Vector3 source, Vector3 target, Color color) {
            _targetPosition = target;

            _matProps = new MaterialPropertyBlock();
            setShared(_lineRenderer);
            setShared(_circleRenderer);

            var delta = target - source;
            var direction = delta.normalized;

            // Create the line.
            _lineRenderer.positionCount = 2;
            Vector3 lineStart = source + Radius * direction;
            _lineRenderer.SetPosition(0, lineStart);
            Vector3 lineEnd = target - Radius * direction;
            _lineRenderer.SetPosition(1, lineEnd);

            // Now the circle.
            _circleRenderer.positionCount = NumCirclePoints;
            _circleRenderer.loop = true;
            Vector3 point;
            point.y = target.y;
            float angleStep = Mathf.PI * 2f / NumCirclePoints;
            float angle = 0f;
            _circlePoints = new Vector3[NumCirclePoints];
            for(int i = 0; i < NumCirclePoints; ++i) {
                var rcos = Radius*Mathf.Cos(angle);
                var rsin = Radius*Mathf.Sin(angle);
                _circlePoints[i] = new Vector3(rcos, 0f, rsin);
                point.x = target.x + rcos;
                point.z = target.z + rsin;
                _circleRenderer.SetPosition(i, point);
                angle += angleStep;
            }

            _color = color;
            checkLineValid();
        }

        /// <summary>
        /// Rebuilds the indicator with new start and end positions.
        /// </summary>
        /// <param name="start">Start position (world)</param>
        /// <param name="target">Target position (world)</param>
        public void Rebuild(Vector3 start, Vector3 target) {
            _targetPosition = target;
            _lineRenderer.SetPosition(0, start);
            var direction = (target - start).normalized;
            Vector3 lineEnd = target - Radius * direction;
            _lineRenderer.SetPosition(1, lineEnd);
            var xScale = Mathf.Max((target - start).magnitude / 2, 1f);
            _lineRenderer.material.mainTextureScale = new Vector2(xScale, 1f);
            checkLineValid();
            for (int i = 0; i < NumCirclePoints; ++i) {
                var point = target + _circlePoints[i];
                _circleRenderer.SetPosition(i, point);
            }
        }

        /// <summary>
        /// Sets the brightness of the lines.
        /// </summary>
        /// <param name="color">The line brightness in [0,1].</param>
        public void SetBrightness(float level) {
            _color.a = level;
            setLineColor(_lineRenderer);
            setLineColor(_circleRenderer);
        }

        private void checkLineValid() {
            var start = _lineRenderer.GetPosition(0);
            if (Vector3.Distance(start, _targetPosition) <= Radius) {
                _lineRenderer.enabled = false;
            }
            else {
                _lineRenderer.enabled = true;
            }
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

            _lineRenderer.material.SetTextureOffset("_MainTex", _offset);
            _circleRenderer.material.SetTextureOffset("_MainTex", _offset);
        }

        private void setShared(LineRenderer renderer) {
            renderer.material = Material;
            renderer.startWidth = LineWidth;
            renderer.endWidth = LineWidth;
            renderer.useWorldSpace = true;
            setLineColor(renderer);
        }

        private void setLineColor(LineRenderer renderer) {
            renderer.GetPropertyBlock(_matProps);
            _matProps.SetColor("_Color", _color);
            renderer.SetPropertyBlock(_matProps);
        }

        private Vector2 _offset = new Vector2(0f, 0f);
        private Vector3 _targetPosition;
        private Color _color;
        private Vector3[] _circlePoints;
        private MaterialPropertyBlock _matProps;
    }
}