using System.Collections;
using UnityEngine;
using TMPro;

namespace VoidWars {
    public class DamageIndicator : MonoBehaviour {
        [SerializeField] private Animation _animation;
        private float _duration;
        private RectTransform _myRT;
        private RectTransform _canvasRT;
        private TextMeshProUGUI _text;
        private Canvas _canvas;

        private void Start() {
            _duration = _animation.clip.length;
            _text = _animation.GetComponent<TextMeshProUGUI>();
            _myRT = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            _canvasRT = _canvas.GetComponent<RectTransform>();
            //_text.enabled = false;
        }

        public void SetValue(Vector3 position, int value) {
            var viewportPosition = Camera.main.WorldToViewportPoint(position);

            // Bias towards centre of screen.
            var yStep = Mathf.Sign(0.5f - viewportPosition.y);
            viewportPosition.y += 0.1f * yStep;

            // Compute UI position.
            var uiPosition = new Vector2((viewportPosition.x * _canvasRT.sizeDelta.x) - (_canvasRT.sizeDelta.x * 0.5f),
                                         (viewportPosition.y * _canvasRT.sizeDelta.y) - (_canvasRT.sizeDelta.y * 0.5f));
            _myRT.anchoredPosition = uiPosition;

            _text.text = value.ToString();
            _text.enabled = true;
            _animation.Play();
            StartCoroutine(disableOnFinish());
        }

        private IEnumerator disableOnFinish() {
            yield return new WaitForSeconds(_duration);
            _text.enabled = false;
        }
    }
}