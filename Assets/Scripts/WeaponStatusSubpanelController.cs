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
                switch(ship.GetWeaponState(slot)) {
                    case AuxState.Broken:
                        // TODO: consider the possibility of breaking weapons.
                    case AuxState.Idle:
                        icon.sprite = InactiveSprite;
                        break;

                    case AuxState.Operational:
                        icon.sprite = ActiveSprite;
                        break;

                    case AuxState.Overheated:
                        icon.sprite = OverheatSprite;
                        break;
                }
            }
        }

        protected override void updateInner() {
        }

        private bool _populated;
    }
}