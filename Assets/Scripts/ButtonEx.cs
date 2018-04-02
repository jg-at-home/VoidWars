using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    /// <summary>
    /// Class for setting button text colours when the button becomes disabled.
    /// </summary>
    public class ButtonEx : MonoBehaviour {
        void Start() {
            _button = GetComponent<Button>();
            _text = _button.GetComponentInChildren<Text>();
            if (_text == null) {
                // No text, nothing to do.
                enabled = false;
                return;
            }
            _activeColor = _button.colors.normalColor;
            _inactiveColor = new Color(_activeColor.r / 2, _activeColor.g / 2, _activeColor.b / 2);
            onStateChange();
        }

        void Update() {
            if (_button.interactable != _wasInteractable) {
                onStateChange();
            }
        }

        private void onStateChange() {
            _wasInteractable = _button.interactable;
            if (_wasInteractable) {
                _text.color = _activeColor;
            }
            else {
                _text.color = _inactiveColor;
            }
        }

        private bool _wasInteractable;
        private Color _activeColor;
        private Color _inactiveColor;
        private Button _button;
        private Text _text;
    }
}