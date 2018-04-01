using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Script that controls a player start zone.
    /// </summary>
    public class StartZoneController : MonoBehaviour {
        public float PulsePeriod = 0.5f;
        public float MinEmissive = 0.5f;
        public float MaxEmissive = 2.0f;

        private void OnEnable() {
            var renderer = gameObject.GetComponent<MeshRenderer>();
            _material = renderer.material;
        }

        public void SetColor(Color color) {
            _material.color = color;
        }

        private void Update() {
            var x = MinEmissive + ((MaxEmissive-MinEmissive) * Mathf.PingPong(Time.time, PulsePeriod));
            var emissiveColor = x * _material.color;
            _material.SetColor("_EmissionColor", emissiveColor);       
        }

        private Material _material;
    }
}