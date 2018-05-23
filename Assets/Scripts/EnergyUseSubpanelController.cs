using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    public class EnergyUseSubpanelController : SubpanelController {
        public Image CurrEnergyImage;
        public Image NextEnergyImage;

        protected override void initialize() {
            refresh();
        }

        protected override void updateInner() {
            maybeUpdateLevel(CurrEnergyImage, _currEnergyFraction);
            maybeUpdateLevel(NextEnergyImage, _nextEnergyFraction);
        }

        protected override void refresh() {
            var shipController = activeShip;
            if (shipController != null) {
                var maxEnergy = shipController.MaxEnergy;
                var currentEnergy = shipController.Energy;
                _currEnergyFraction = shipController.Energy / maxEnergy;
                _nextEnergyFraction = controller.ShipEnergyAfterSelection / maxEnergy;
            }
        }

        private float _currEnergyFraction;
        private float _nextEnergyFraction;
    }
}