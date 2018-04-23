using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    public class ShieldSubpanelController : SubpanelController {
        public Image ShieldPowerBar;
        public Image ShieldLevelBar;
        public Text ShieldLevelText;

        protected override void initialize() {
            refresh();
            ShieldPowerBar.fillAmount = _shieldPower;
            ShieldLevelBar.fillAmount = _shieldLevel / 100f;
        }

        protected override void refresh() {
            var activeShip = controller.GetActiveShip();
            _shieldPower = activeShip.ShieldEnergy / activeShip.MaxEnergy;
            if (activeShip.ShieldsActive) {
                _shieldLevel = activeShip.ShieldPercent;
                ShieldLevelText.text = string.Format("{0}%", (int)Mathf.Clamp(_shieldLevel, 0f, 100f));
            }
            else {
                ShieldLevelText.text = "OFF";
            }
        }

        protected override void updateInner() {
            maybeUpdateLevel(ShieldPowerBar, _shieldPower);
            maybeUpdateLevel(ShieldLevelBar, _shieldLevel / 100f, true);
        }

        private float _shieldPower;
        private float _shieldLevel;
    }
}