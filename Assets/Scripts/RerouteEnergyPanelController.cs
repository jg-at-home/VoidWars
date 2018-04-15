using UnityEngine.UI;
using TMPro;
using UnityEngine;

namespace VoidWars {
    public class RerouteEnergyPanelController : ActionDetailPanelController {
        public Slider[] Sliders;
        public TextMeshProUGUI[] Percentages;
        public TextMeshProUGUI DescriptionText;

        /// <summary>
        /// Initializes the panel controls.
        /// </summary>
        /// <param name="item">The item the panel relates to.</param>
        /// <param name="args">List of arguments to the panel.</param>
        public override void Initialize(ActionItem item, string[] args) {
            var gameController = Util.GetGameController();
            var shipController = gameController.GetActiveShip();
            _budget = shipController.EnergyBudget;
            _values[0] = _budget.Available(EnergyConsumer.Shields);
            _values[1] = _budget.Available(EnergyConsumer.Weapons);
            _values[2] = _budget.Available(EnergyConsumer.LifeSupport);
            _values[3] = _budget.Available(EnergyConsumer.Propulsion);
            updateSlidersAndText();
            DescriptionText.text = item.Detail;

            // Don't allow user to select OK until they change something.
            gameController.InfoPanel.NotifyContent("EnableDone", false);
        }

        public override void SelectAction() {
            Debug.Log("RerouteEnergyPanelController.SelectAction()");

            _budget.SetFractions(_values);
        }

        private void refresh() {
            var total = 0f;
            for (int i = 0; i < Sliders.Length; ++i) {
                _values[i] = Sliders[i].value;
                total += _values[i];
            }

            var scale = 1f / total;
            for (int i = 0; i < Sliders.Length; ++i) {
                _values[i] *= scale;
            }

            updateSlidersAndText();
        }

        private void updateSlidersAndText() {
            _lock = true;
            for (int i = 0; i < Sliders.Length; ++i) {
                Sliders[i].value = _values[i];
                Percentages[i].text = string.Format("{0}%", (int)(_values[i] * 100f));
            }
            _lock = false;
        }

        public void OnSliderChanged() {
            if (!_lock) {
                refresh();
                // Allow selection of OK.
                var gameController = Util.GetGameController();
                gameController.InfoPanel.NotifyContent("EnableDone", true);
            }
        }

        public void OnResetClicked() {
            _lock = true;
            for(int i = 0; i < 4; ++i) {
                Sliders[i].value = 0.25f;
                _values[i] = 0.25f;
                Percentages[i].text = "25%";
            }
            _lock = false;
        }

        private bool _lock;
        private EnergyBudget _budget;
        private readonly float[] _values = new[] { .25f, .25f, .25f, .25f }; 
    }
}