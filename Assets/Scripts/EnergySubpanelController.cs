using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    public class EnergySubpanelController : SubpanelController {
        public Image ShieldImage;
        public Image LifeSupportImage;
        public Image WeaponsImage;
        public Image PropulsionImage;
        public Image TotalEnergyImage;
        public float SamplePeriod = 1.0f;
        public float MeterSpeed = 0.7f;

        public override void OnActivation() {
            _timer = 0f;    
        }

        private void Awake() {
            _controller = Util.GetGameController();
        }

        private void Update() {
            _timer += Time.deltaTime;
            if (_timer >= SamplePeriod) {
                _timer -= SamplePeriod;
                readValues();
            }

            maybeUpdateLevel(ShieldImage, _shieldLevel);
            maybeUpdateLevel(WeaponsImage, _weaponsLevel);
            maybeUpdateLevel(LifeSupportImage, _lifeSupportLevel);
            maybeUpdateLevel(PropulsionImage, _propulsionLevel);
            maybeUpdateLevel(TotalEnergyImage, _energyLevel);
        }

        private void maybeUpdateLevel(Image image, float level) {
            var current = image.fillAmount;
            if (current < level) {
                image.fillAmount = Mathf.Min(image.fillAmount + MeterSpeed * Time.deltaTime, level);
            }
            else if (current > level) {
                image.fillAmount = Mathf.Max(image.fillAmount - MeterSpeed * Time.deltaTime, level);
            }
        }

        private void readValues() {
            var shipController = _controller.GetActiveShip();
            if (shipController != null) {
                var shieldEnergy = shipController.ShieldEnergy;
                var weaponsEnergy = shipController.WeaponsEnergy;
                var lifeSupportEnergy = shipController.LifeSupportEnergy;
                var propulsionEnergy = shipController.PropulsionEnergy;
                var maxEnergy = Mathf.Max(shieldEnergy, weaponsEnergy, lifeSupportEnergy, propulsionEnergy);
                var energyScale = maxEnergy > 0f ? 1.0f / maxEnergy : 0f;
                _shieldLevel = shieldEnergy * energyScale;
                _weaponsLevel = weaponsEnergy * energyScale;
                _lifeSupportLevel = lifeSupportEnergy * energyScale;
                _propulsionLevel = propulsionEnergy * energyScale;
                _energyLevel = shipController.Energy / shipController.MaxEnergy;
            }
        }

        private GameController _controller;
        private float _timer;
        private float _shieldLevel;
        private float _weaponsLevel;
        private float _lifeSupportLevel;
        private float _propulsionLevel;
        private float _energyLevel;
    }
}