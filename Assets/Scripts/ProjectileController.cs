using UnityEngine;

namespace VoidWars {
    public delegate void CollisionHandler(GameObject collider);

    public class ProjectileController : MonoBehaviour {
        public CollisionHandler CollisionHandler;
        [HideInInspector] public GameObject SourceShip;
        public float Speed;

        private void OnCollisionEnter(Collision collision) {
            if (!ReferenceEquals(collision.gameObject, SourceShip)) {
                gameObject.SetActive(false);
                if (CollisionHandler != null) {
                    CollisionHandler(collision.gameObject);
                }
            }
        }
    }
}