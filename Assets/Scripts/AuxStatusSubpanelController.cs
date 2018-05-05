using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    /// <summary>
    /// Panel showing the status of the auxiliary items equipped.
    /// </summary>
    public class AuxStatusSubpanelController : SubpanelController {
        public Sprite OkImage;
        public Sprite BrokenImage;
        public Sprite OverheatImage;
        public Sprite OnImage;
        public Sprite OffImage;

        [SerializeField] private TextMeshProUGUI[] _text;
        [SerializeField] private Image[] _icons;
        [SerializeField] private RectTransform[] _rows;
        
        protected override void initialize() {
            refresh();
        }

        protected override void refresh() {
            var ship = activeShip;
            if (ship != null) {
                var numAux = ship.GetAuxiliaryCount();
                int i = 0;
                for(; i < numAux; ++i) {
                    _rows[i].gameObject.SetActive(true);
                    var auxItem = ship.GetAuxiliaryItemC(i);
                    _text[i].text = auxItem.Name;
                    var auxState = ship.GetAuxiliaryItemState(i);
                    if (auxState == AuxState.Broken) {
                        _icons[i].sprite = BrokenImage;
                    }
                    else if (auxState == AuxState.Overheated) {
                        _icons[i].sprite = OverheatImage;
                    }
                    else {
                        switch (auxItem.Mode) {
                            case AuxMode.OneShot:
                            case AuxMode.Continuous:
                                _icons[i].sprite = OkImage;
                                break;

                            case AuxMode.Switchable:
                                if (auxState == AuxState.Operational) {
                                    _icons[i].sprite = OnImage;
                                }
                                else {
                                    _icons[i].sprite = OffImage;
                                }
                                break;
                        }
                    }
                }
                for(; i < 4; ++i) {
                    _rows[i].gameObject.SetActive(false);
                }
            }
        }

        protected override void updateInner() {
        }
    }
}