using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    public class ZoomButtonController : MonoBehaviour {
        [SerializeField] private Sprite _magPlusSprite;
        [SerializeField] private Sprite _magMinusSprite;
        private Image _icon;
        private Text _text;

        private void Start() {
            _icon = GetComponent<Image>();
            _text = GetComponentInChildren<Text>();
            refreshUI();
        }

        public void OnButtonPressed() {
            _magnify = !_magnify;
            refreshUI();
            var gameController = Util.GetGameController();
            if (_magnify) {
                gameController.ZoomIn();
            }
            else {
                gameController.ZoomOut();
            }
        }

        private void refreshUI() {
            if (_magnify) {
                _icon.sprite = _magMinusSprite;
                _text.text = "Zoom out";
            }
            else {
                _icon.sprite = _magPlusSprite;
                _text.text = "Zoom in";
            }
        }

        private bool _magnify;
    }
}