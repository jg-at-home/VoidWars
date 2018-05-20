using UnityEngine;

namespace VoidWars {
    public class Tumbler : MonoBehaviour {
        public float Rate = 90;

        // Update is called once per frame
        void Update() {
            transform.Rotate(new Vector3(0.5f, 0.5f, 0.5f), Time.deltaTime * Rate);
        }
    }
}