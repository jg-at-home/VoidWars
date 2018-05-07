using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    public class ContentPanelController : MonoBehaviour {
        [SerializeField]
        private Text _infoText;

        [SerializeField]
        private float _charDelay = 0.05f;

        [SerializeField]
        private Button _doneButton;

        [SerializeField]
        private RectTransform _promptPanel;

        /// <summary>
        /// Sets the info text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void SetInfoText(string text) {
            _textQueue.Clear();
            _sb.Length = 0;
            _infoText.text = string.Empty;
            foreach (char c in text) {
                _textQueue.Enqueue(c);
            }

            if (_writer != null) {
                StopCoroutine(_writer);
            }

            _writer = StartCoroutine(writeText());
        }

        /// <summary>
        /// Sets the text on the Done button.
        /// </summary>
        /// <param name="caption">The button text.</param>
        public void SetDoneButtonCaption(string caption) {
            _doneButton.GetComponentInChildren<Text>().text = caption;
        }

        /// <summary>
        /// Enable or disable the 'done' button.
        /// </summary>
        /// <param name="enable">If true, enable the button.</param>
        public void EnableDoneButton(bool enable) {
            _doneButton.interactable = enable;
            if (_promptPanel != null) {
                _promptPanel.gameObject.SetActive(enable);
            }
        }

        private IEnumerator writeText() {
            while (_textQueue.Count > 0) {
                _sb.Append(_textQueue.Dequeue());
                _infoText.text = _sb.ToString();
                yield return new WaitForSeconds(_charDelay);
            }

            _writer = null;
        }

        /// <summary>
        /// Called when the Done button is clicked.
        /// </summary>
        public virtual void OnDoneButtonClicked() {
            SendMessageUpwards("PlayButtonClick");
            var gameController = Util.GetGameController();
            gameController.NextShip();
        }

        private readonly Queue<char> _textQueue = new Queue<char>();
        private StringBuilder _sb = new StringBuilder();
        private Coroutine _writer;

    }
}