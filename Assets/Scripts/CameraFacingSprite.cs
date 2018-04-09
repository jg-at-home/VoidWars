using UnityEngine;

namespace VoidWars {
    public class CameraFacingSprite : MonoBehaviour {
        private void Update() {
            var cameraTx = Camera.main.transform;
            transform.LookAt(transform.position + cameraTx.rotation * Vector3.forward, cameraTx.rotation * Vector3.up);
        }
    }
}