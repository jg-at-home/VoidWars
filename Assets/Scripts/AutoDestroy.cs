using UnityEngine;

namespace VoidWars {
    public class AutoDestroy : MonoBehaviour {
        [SerializeField] private float _lifetime;

        private void Start() {
            Destroy(gameObject, _lifetime);
        }
    }
}