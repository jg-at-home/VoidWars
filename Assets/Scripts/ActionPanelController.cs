using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

namespace VoidWars {
    /// <summary>
    /// Top-level controller for the action selection panel.
    /// </summary>
    public class ActionPanelController : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private ActionListController _actionList;
        [SerializeField] private Image _flasher;
        [SerializeField] private float _flashPeriod = 1.0f;
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;

        /// <summary>
        /// Refreshes the list of actions.
        /// </summary>
        public void Refresh() {
            _actionList.Refresh();
        }

        /// <summary>
        /// Selects the current action to execute.
        /// </summary>
        public void SelectCurrentAction() {
            _actionList.SelectCurrentAction();
        }

        /// <summary>
        /// Sets the caption on the panel.
        /// </summary>
        /// <param name="title">Thr caption text.</param>
        public void SetTitle(string title) {
            _titleText.text = title;
        }

        private void OnEnable() {
            _flashOn = false;
            StartCoroutine(flash());
        }

        private void OnDisable() {
            StopAllCoroutines();
        }

        private IEnumerator flash() {
            while(true) {
                yield return new WaitForSeconds(_flashPeriod);

                _flashOn = !_flashOn;
                _flasher.sprite = _flashOn ? _onSprite : _offSprite;
            }
        }

        private bool _flashOn;
    }
}