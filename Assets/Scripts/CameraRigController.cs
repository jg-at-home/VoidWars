using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private float _tolerance = 0.1f;

        private void Start() {
            gameObject.transform.position = _start.position;
        }

        public void ZoomOut() {
            StartCoroutine(moveCamera(_zoomedOut.position));
        }

        public void ZoomToStart() {
            StartCoroutine(moveCamera(_start.position));
        }

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