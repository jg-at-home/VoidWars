﻿using TMPro;
using UnityEngine.UI;

namespace VoidWars {
    public class ActionDetailImageController : ActionDetailPanelController {
        public TextMeshProUGUI DetailText;
        public Image DetailImage;

        protected override void initializeInner(ActionItem item, string[] args) {
            DetailImage.sprite = ImageManager.GetImage(args[1]);
            DetailText.text = item.Detail;

            // Allow OK for these panels.
            enableSelectButton(true);
        }

        public override void SelectAction() {
            var gameController = Util.GetGameController();
            gameController.ExecuteCommand(Item.Action);
        }
    }
}