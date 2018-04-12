using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    public class EnergySubpanelController : SubpanelController {
        public Image ShieldImage;
        public Image LifeSupportImage;
        public Image WeaponsImage;
        public Image PropulsionImage;
        public Image TotalEnergyImage;

        protected override void updateInner() {            
            maybeUpdateLevel(ShieldImage, _shieldLevel);
            maybeUpdateLevel(WeaponsImage, _weaponsLevel);
            maybeUpdateLevel(LifeSupportImage, _lifeSupportLevel);
            maybeUpdateLevel(PropulsionImage, _propulsionLevel);
            maybeUpdateLevel(TotalEnergyImage, _energyLevel);
        }

        protected override void refresh() {
            var shipController = controller.GetActiveShip();
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

        private float _shieldLevel;
        private float _weaponsLevel;
        private float _lifeSupportLevel;
        private float _propulsionLevel;
        private float _energyLevel;
    }
}