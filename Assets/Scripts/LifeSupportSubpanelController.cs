using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    public class LifeSupportSubpanelController : SubpanelController {
        public Text StatusText;

        protected override void initialize() {
            refresh();
        }

        protected override void refresh() {
            if (activeShip != null) {
                if (activeShip.LifeSupportEnergy > 2f*activeShip.ShipClass.LifeSupportDrainRate) {
                    StatusText.color = Color.white;
                    StatusText.text = "NOMINAL";
                }
                else if (activeShip.LifeSupportEnergy > activeShip.ShipClass.LifeSupportDrainRate) {
                    StatusText.color = orange;
                    StatusText.text = "CRITICAL";
                }
                else {
                    StatusText.color = Color.red;
                    StatusText.text = "FAILED";
                }
            }
        }

        protected override void updateInner() {
        }

        private static readonly Color orange = new Color(1, 0.5f, 0.1f, 1f);
    }
}