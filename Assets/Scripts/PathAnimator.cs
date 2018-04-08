using UnityEngine;

namespace VoidWars {
    [RequireComponent(typeof(MoveTemplate))]
    public class PathAnimator : MonoBehaviour {
        public float ScrollSpeed = 1.0f;

        private void Start() {
            var lineRenderer = gameObject.GetComponent<LineRenderer>();
            _material = lineRenderer.material;
            var template = gameObject.GetComponent<MoveTemplate>();
            _material.SetTextureScale("_MainTex", new Vector2(4f * template.Size, 1f));
        }

        private void Update() {
            _offset.x = -Time.time * ScrollSpeed;
            _material.SetTextureOffset("_MainTex", _offset);
        }

        private Material _material;
        private Vector2 _offset = new Vector2();
    }
}