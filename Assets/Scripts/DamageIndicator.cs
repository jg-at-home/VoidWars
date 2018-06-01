using UnityEngine;
using TMPro;

namespace VoidWars {
    public class DamageIndicator : MonoBehaviour {
        [SerializeField] private Animation _animation;

        public void SetValue(Vector3 position, string value, Color color) {
            var myRT = GetComponent<RectTransform>();
            var canvas = GetComponentInParent<Canvas>();
            var canvasRT = canvas.GetComponent<RectTransform>();
            var viewportPosition = Camera.main.WorldToViewportPoint(position);

            // Bias towards centre of screen.
            var yStep = Mathf.Sign(0.5f - viewportPosition.y);
            viewportPosition.y += 0.1f * yStep;

            // Compute UI position.
            var uiPosition = new Vector2((viewportPosition.x * canvasRT.sizeDelta.x) - (canvasRT.sizeDelta.x * 0.5f),
                                         (viewportPosition.y * canvasRT.sizeDelta.y) - (canvasRT.sizeDelta.y * 0.5f));
            myRT.anchoredPosition = uiPosition;

            var text = _animation.GetComponent<TextMeshProUGUI>();
            text.text = value;
            text.color = color;
            text.enabled = true;
            _animation.Play();
            var duration = _animation.clip.length;
            Destroy(gameObject, duration);
        }
    }
}