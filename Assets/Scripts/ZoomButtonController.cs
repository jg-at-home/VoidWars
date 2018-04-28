using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    /// <summary>
    /// Controller for the zoom in/out button. Changes icons like a proper toggle.
    /// </summary>
    public class ZoomButtonController : MonoBehaviour {
        private void Start() {
            refreshUI();
        }

        public void ResetZoom() {
            _zoomLevel = 0;
            refreshUI();
            var gameController = Util.GetGameController();
            gameController.ResetZoom();
        }

        public void ZoomOut() {
            if (_zoomLevel > 0) {
                --_zoomLevel;
                refresh();                
            }
        }

        public void ZoomIn() {
            if (_zoomLevel < 2) {
                ++_zoomLevel;
                refresh();
            }
        }

        private void refresh() {
            refreshUI();
            var gameController = Util.GetGameController();
            gameController.SetZoomLevel(_zoomLevel);
        }

        private void refreshUI() {
            _inButton.interactable = _zoomLevel < 2;
            _outButton.interactable = _zoomLevel > 0;
        }

        [SerializeField] private Button _inButton;
        [SerializeField] private Button _outButton;
        private int _zoomLevel;
    }
}