using UnityEngine.UI;
using TMPro;

namespace VoidWars {
    public class RerouteEnergyPanelController : ActionDetailPanelController{
        public Slider[] Sliders;
        public TextMeshProUGUI[] Percentages;

        public override void Setup(ActionItem item, string[] args) {
            //var gameController = Util.GetGameController();
            //var shipController = gameController.GetActiveShip();
            //_budget = shipController.EnergyBudget;
            //_values[0] = _budget.Available(EnergyConsumer.Shields);
            //_values[1] = _budget.Available(EnergyConsumer.Weapons);
            //_values[2] = _budget.Available(EnergyConsumer.LifeSupport);
            //_values[3] = _budget.Available(EnergyConsumer.Propulsion);
            refresh();
        }

        public override void SelectAction() {
            // TODO.
        }

        // TODO: remove 
        private void Start() {
            _lock = true;
            for(int i = 0; i < 4; ++i) {
                Sliders[i].value = _values[i];
                Percentages[i].text = string.Format("{0}%", (int)(_values[i] * 100f));
            }
            _lock = false;
        }

        private void refresh() {
            var total = 0f;
            for (int i = 0; i < Sliders.Length; ++i) {
                _values[i] = Sliders[i].value;
                total += _values[i];
            }

            _lock = true;
            var scale = 1f / total;
            for (int i = 0; i < Sliders.Length; ++i) {
                _values[i] *= scale;
                Sliders[i].value = _values[i];
                Percentages[i].text = string.Format("{0}%", (int)(_values[i] * 100f));
            }
            _lock = false;
        }

        public void OnSliderChanged() {
            if (!_lock) {
                refresh();
            }
        }

        private bool _lock;
        private EnergyBudget _budget;
        private readonly float[] _values = new[] { .25f, .25f, .25f, .25f }; 
    }
}