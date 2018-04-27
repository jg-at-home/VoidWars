using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VoidWars {
    /// <summary>
    /// Panel showing the status of the life support systems.
    /// </summary>
    public class LifeSupportSubpanelController : SubpanelController {
        public TextMeshProUGUI StatusText;
        public GraphSim[] Graphs;

        protected override void initialize() {
            refresh();
        }

        protected override void refresh() {
            var failed = false;
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
                    failed = true;
                }
                foreach (var graph in Graphs) {
                    graph.FlatLine(failed);
                }
            }
        }

        protected override void updateInner() {
        }

        private static readonly Color orange = new Color(1, 0.5f, 0.1f, 1f);
    }
}