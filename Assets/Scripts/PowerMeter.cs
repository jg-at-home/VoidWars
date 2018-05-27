using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    public class PowerMeter : MonoBehaviour {
        public float Speed = 0.7f;
        public float Acceleration = 0.026f;
        public Image FillImage;

        public float Power {  get { return _power; } }

        private void OnEnable() {
            _time = 0f;    
        }

        private void Update() {
            _time = _time + Time.deltaTime;
            _power = Mathf.PingPong(_time*(Speed + 0.5f*Acceleration*_time), 1f);
            FillImage.fillAmount = _power;
        }

        private float _power;
        private float _time;
    }
}