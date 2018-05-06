using UnityEngine.UI;

namespace VoidWars {
    /// <summary>
    /// Status panel indicating ship health.
    /// </summary>
    public class HealthSubpanelController : SubpanelController {
        public Image GaugeImage;
        public Text GaugeText;

        protected override void initialize() {
            _health = 1f;
            GaugeImage.fillAmount = 1f;
            refresh();
        }

        protected override void refresh() {
            if (activeShip != null) {
                _health = activeShip.Health / activeShip.ShipData.MaxHealth;
                GaugeText.text = string.Format("{0}%", (int)(_health * 100.0f));
            }
        }

        protected override void updateInner() {
            maybeUpdateLevel(GaugeImage, _health, true);
        }

        private float _health;
    }
}