using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    public class StatusPanelController : MonoBehaviour {
        public Image GaugeFill;
        public float GaugePeriod = 1.0f;
        public SubpanelController[] PanelPrefabs;
        public RectTransform PanelRoot;
        public Button NextButton;
        public Button PrevButton;
        public Text TitleText;

        public void OnNextButton() {
            var next = _currentPanel+1;
            if (next >= PanelPrefabs.Length) {
                next = 0;
            }
            setCurrentPanel(next, false);
        }

        public void OnPrevButton() {
            var next = _currentPanel - 1;
            if (next < 0) {
                next = PanelPrefabs.Length-1;
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
        }

        private void setCurrentPanel(int panel, bool force) {
            if ((_currentPanel != panel) || force) {
                var panelInstance = _panels[_currentPanel];
                if (panelInstance != null) {
                    panelInstance.OnDeactivation();
                    panelInstance.GetComponent<RectTransform>().SetParent(null);
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
    }
}