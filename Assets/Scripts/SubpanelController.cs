using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    public abstract class SubpanelController : MonoBehaviour {
        public string Name;
        public float RefreshPeriod = 1.0f;
        public float MeterSpeed = 0.7f;
        public Gradient MeterColor;

        private void Awake() {
            _controller = Util.GetGameController();

            var images = new List<MaskableGraphic>();
            findComponents(gameObject.transform, images);
            _images = images.ToArray();
        }

        private static void findComponents<T>(Transform element, List<T> items) {
            foreach (Transform child in element) {
                var comp = child.GetComponent<T>();
                if (comp != null) {
                    items.Add(comp);
                }
                findComponents(child, items);
            }
        }

        public void SetAlpha(float alpha) {
            _fadeColor.a = alpha;
            foreach(var image in _images) {
                image.color = _fadeColor;
            }
        }

        private void OnEnable() {
            initialize();    
        }

        private void Update() {
            _timer += Time.deltaTime;
            if (_timer >= RefreshPeriod) {
                _timer -= RefreshPeriod;
                refresh();
            }

            updateInner();
        }

        protected void maybeUpdateLevel(Image image, float level, bool recolor=false) {
            bool changed = false;
            var current = image.fillAmount;
            if (current < level) {
                image.fillAmount = Mathf.Min(image.fillAmount + MeterSpeed * Time.deltaTime, level);
                changed = true;
            }
            else if (current > level) {
                image.fillAmount = Mathf.Max(image.fillAmount - MeterSpeed * Time.deltaTime, level);
                changed = true;
            }

            if (changed && recolor) {
                image.color = MeterColor.Evaluate(level);
            }
        }

        protected abstract void initialize();
        protected abstract void refresh();
        protected abstract void updateInner();

        protected GameController controller {
            get { return _controller; }
        }

        public virtual void OnActivation() {
            _timer = 0.0f;
            gameObject.SetActive(true);
        }

        public virtual void OnDeactivation() {
            gameObject.SetActive(false);
        }

        private GameController _controller;
        private float _timer;
        private MaskableGraphic[] _images;
        private Color _fadeColor = new Color(1, 1, 1, 1);
    }
}
