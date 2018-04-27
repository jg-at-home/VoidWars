using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VoidWars {
    public class WeaponStatusSubpanelController : SubpanelController {
        public RectTransform FrontPane;
        public RectTransform RearPane;
        public TextMeshProUGUI FrontType;
        public TextMeshProUGUI RearType;
        public Image FrontIcon;
        public Image RearIcon;
        public Sprite ActiveSprite;
        public Sprite InactiveSprite;
        public Sprite OverheatSprite;

        protected override void initialize() {
            _populated = false;
            refresh();
        }

        protected override void refresh() {
            if (activeShip != null) {
                if (!_populated) {
                    populateSlot(FrontPane, activeShip, 0, FrontType, FrontIcon);
                    populateSlot(RearPane, activeShip, 1, RearType, RearIcon);
                    _populated = true;
                }
            }
        }

        private void populateSlot(RectTransform row, ShipController ship, int slot, TextMeshProUGUI text, Image icon) {
            var type = ship.GetWeaponType(slot);
            if (type == WeaponType.None) {
                row.gameObject.SetActive(false);
            }
            else {
                var gameController = Util.GetGameController();
                row.gameObject.SetActive(true);
                text.text = gameController.GetWeaponClass(type).Name;
                if (ship.IsPrimaryWeaponOK) {
                    icon.sprite = ActiveSprite;
                }
                else {
                    icon.sprite = InactiveSprite;
                }
            }
        }

        protected override void updateInner() {
        }

        private bool _populated;
    }
}