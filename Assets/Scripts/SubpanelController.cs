﻿using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    public abstract class SubpanelController : MonoBehaviour {
        public string Name;
        public float RefreshPeriod = 1.0f;
        public float MeterSpeed = 0.7f;
        public Gradient MeterColor;

        private void Awake() {
            _controller = Util.GetGameController();
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
                image.fillAmount = Mathf.Min(current + MeterSpeed * Time.deltaTime, level);
                changed = true;
            }
            else if (current > level) {
                image.fillAmount = Mathf.Max(current - MeterSpeed * Time.deltaTime, level);
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

        protected ShipController activeShip {
            get {
                if (_root != null) {
                    return _root.ActiveShip;
                }
                else {
                    return null;
                }
            }
        }

        public virtual void OnActivation(StatusPanelController root) {
            _timer = 0.0f;
            gameObject.SetActive(true);
            _root = root;
            initialize();
        }

        public virtual void OnDeactivation() {
            gameObject.SetActive(false);
            _root = null;
        }

        private GameController _controller;
        private StatusPanelController _root;
        private float _timer;
    }
}
