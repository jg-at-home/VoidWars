﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    /// <summary>
    /// Top level controller for the status panels.
    /// </summary>
    public class StatusPanelController : MonoBehaviour {
        public Image GaugeFill;
        public float GaugePeriod = 1.0f;
        public SubpanelController[] PanelPrefabs;
        public RectTransform PanelRoot;
        public Button NextButton;
        public Button PrevButton;
        public Text TitleText;
        public float PanelScrollDelay = 10f;
        public float PanelScrollInterval = 3f;
        public float FadeTime = 0.5f;

        /// <summary>
        /// Gets the ship to show status for.
        /// </summary>
        public ShipController ActiveShip {
            get { return _activeShip; }
        }

        public void OnNextButton() {
            onButtonClicked();
            nextPanel();
        }

        public void GoToPanel(string panelName) {
            for(int i = 0; i < PanelPrefabs.Length; ++i) {
                if (PanelPrefabs[i].Name == panelName) {
                    onButtonClicked(false);
                    setCurrentPanel(i, false);
                }
            }
        }

        public void OnPrevButton() {
            onButtonClicked();
            var next = _currentPanelIndex - 1;
            if (next < 0) {
                next = PanelPrefabs.Length-1;
            }
            setCurrentPanel(next, false);
        }

        private void onButtonClicked(bool click = true) {
            if (click) {
                SendMessageUpwards("PlayButtonClick");
            }
            _timer = 0;
            _canvasGroup.alpha = 1f;
            if (_scrolling) {
                StopCoroutine("scrollPanels");                
                _scrolling = false;
            }
        }

        private void nextPanel() {
            var next = _currentPanelIndex + 1;
            if (next >= PanelPrefabs.Length) {
                next = 0;
            }
            setCurrentPanel(next, false);
        }

        private void Start() {
            _canvasGroup = PanelRoot.GetComponent<CanvasGroup>();
            _panels = new SubpanelController[PanelPrefabs.Length];
            setCurrentPanel(0, true);
            if (PanelPrefabs.Length < 2) {
                NextButton.interactable = false;
                PrevButton.interactable = false;
            }
        }

        private void OnEnable() {
            _scrolling = false;
            _activeShip = Util.GetGameController().GetActiveShip();
        }

        private void Update() {
            GaugeFill.fillAmount = Mathf.Repeat(Time.time, GaugePeriod);
            if (!_scrolling) {
                _timer += Time.deltaTime;
                if (_timer >= PanelScrollDelay) {
                    _timer = 0f;
                    _scrolling = true;
                    StartCoroutine("scrollPanels");
                }
            }
        }

        private IEnumerator scrollPanels() {
            while (_scrolling) {
                // Fade current panel out.
                for (var t = 0f; t < FadeTime; t += Time.deltaTime) {
                    var alpha = 1f - Mathf.Clamp01(t / FadeTime);
                    _canvasGroup.alpha = alpha;
                    yield return null;
                }

                // Move along.
                nextPanel();

                // Fade up.
                for (var t = 0f; t < FadeTime; t += Time.deltaTime) {
                    var alpha = Mathf.Clamp01(t / FadeTime);
                    _canvasGroup.alpha = alpha;
                    yield return null;
                }

                // Wait.
                yield return new WaitForSeconds(PanelScrollInterval);
            }
        }

        private void setCurrentPanel(int panel, bool force) {
            if ((_currentPanelIndex != panel) || force) {
                var panelInstance = _panels[_currentPanelIndex];
                if (panelInstance != null) {
                    var rt = panelInstance.gameObject.GetComponent<RectTransform>();
                    rt.SetParent(null, false);
                    panelInstance.OnDeactivation();
                }

                _currentPanelIndex = panel;
                if (_panels[panel] == null) {
                    _panels[panel] = Instantiate(PanelPrefabs[panel]);
                }

                panelInstance = _panels[panel];
                panelInstance.GetComponent<RectTransform>().SetParent(PanelRoot,false);
                panelInstance.OnActivation(this);

                TitleText.text = panelInstance.Name;
            }
        }

        private SubpanelController[] _panels;
        private int _currentPanelIndex;
        private float _timer;
        private bool _scrolling;
        public ShipController _activeShip;
        private CanvasGroup _canvasGroup;
    }
}