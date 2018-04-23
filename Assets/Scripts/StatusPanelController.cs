using System.Collections;
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
        public Image FadePanel;
        public float PanelScrollDelay = 10f;
        public float PanelScrollInterval = 3f;
        public float FadeTime = 0.5f;

        public void OnNextButton() {
            onButtonClicked();
            nextPanel();
        }

        public void OnPrevButton() {
            onButtonClicked();
            var next = _currentPanel - 1;
            if (next < 0) {
                next = PanelPrefabs.Length-1;
            }
            setCurrentPanel(next, false);
        }

        private void onButtonClicked() {
            _timer = 0;
            if (_scrolling) {
                StopCoroutine("scrollPanels");
                FadePanel.color = Color.clear;
                _scrolling = false;
            }
        }

        private void nextPanel() {
            var next = _currentPanel + 1;
            if (next >= PanelPrefabs.Length) {
                next = 0;
            }
            setCurrentPanel(next, false);
        }

        private void Start() {
            _panels = new SubpanelController[PanelPrefabs.Length];
            setCurrentPanel(0, true);
            if (PanelPrefabs.Length < 2) {
                NextButton.interactable = false;
                PrevButton.interactable = false;
            }
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
                    var color = Color.Lerp(Color.clear, Color.black, t / FadeTime);
                    FadePanel.color = color;
                    yield return null;
                }

                nextPanel();

                for (var t = 0f; t < FadeTime; t += Time.deltaTime) {
                    var color = Color.Lerp(Color.black, Color.clear, t / FadeTime);
                    FadePanel.color = color;
                    yield return null;
                }

                yield return new WaitForSeconds(PanelScrollInterval);
            }
        }

        private void setCurrentPanel(int panel, bool force) {
            if ((_currentPanel != panel) || force) {
                var panelInstance = _panels[_currentPanel];
                if (panelInstance != null) {
                    panelInstance.OnDeactivation();
                    panelInstance.GetComponent<RectTransform>().SetParent(null, false);
                }

                _currentPanel = panel;
                if (_panels[panel] == null) {
                    _panels[panel] = Instantiate(PanelPrefabs[panel]);
                }

                panelInstance = _panels[panel];
                panelInstance.GetComponent<RectTransform>().SetParent(PanelRoot,false);
                panelInstance.OnActivation();

                TitleText.text = panelInstance.Name;
            }
        }

        private SubpanelController[] _panels;
        private int _currentPanel;
        private float _timer;
        private bool _scrolling;
    }
}