using System.Collections;
using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Controller for the camera.
    /// </summary>
    public class CameraRigController : MonoBehaviour {
        [SerializeField] private Transform _start;
        [SerializeField] private Transform _zoomedOut;
        [SerializeField] private Transform _zoomedIn;
        [SerializeField] private float _smoothTime = 1.0f;
        [SerializeField] private float _tolerance = 0.01f;

        private void Start() {
            gameObject.transform.position = _start.position;
        }

        /// <summary>
        /// Zoom out to the zoomed out position.
        /// </summary>
        public void ZoomOut() {
            StartCoroutine(moveCamera(_zoomedOut.position));
        }

        /// <summary>
        /// Zooms to the start position (pretty close to the sun, looks cool).
        /// </summary>
        public void ZoomToStart() {
            StartCoroutine(moveCamera(_start.position));
        }

        /// <summary>
        /// Zooms to be above the target position.
        /// </summary>
        /// <param name="target"></param>
        public void ZoomTo(Vector3 target) {
            Vector3 actualTarget;
            actualTarget.x = target.x;
            actualTarget.y = _zoomedIn.position.y;
            actualTarget.z = target.z;
            StartCoroutine(moveCamera(actualTarget));
        }

        private IEnumerator moveCamera(Vector3 target) {
            Vector3 velocity = Vector3.zero;
            while (Vector3.Distance(gameObject.transform.position, target) > _tolerance) {
                gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, target, ref velocity, _smoothTime);
                yield return null;
            }
        }
    }
}