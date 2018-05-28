using TMPro;
using UnityEngine.UI;

namespace VoidWars {
    public class AbilityPanelController : ActionDetailPanelController {
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI DetailText;
        public TextMeshProUGUI TurnsLeftText;
        public Image Icon;

        public override void SelectAction() {
            var controller = Util.GetGameController();
            var ship = controller.GetActiveShip();
            controller.ExecuteCommand(ship, Item.Action);
        }

        protected override void initializeInner(ActionItem item, string[] args) {
            // TODO: setup the fields.
        }
    }
}