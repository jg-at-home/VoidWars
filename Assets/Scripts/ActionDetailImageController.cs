using TMPro;
using UnityEngine.UI;

namespace VoidWars {
    public class ActionDetailImageController : ActionDetailPanelController {
        public TextMeshProUGUI DetailText;
        public Image DetailImage;

        public override void Initialize(ActionItem item, string[] args) {
            DetailImage.sprite = ImageManager.GetImage(args[1]);
            DetailText.text = item.Detail;            
        }

        public override void SelectAction() {
            // TODO
        }
    }
}