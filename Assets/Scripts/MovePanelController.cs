using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    /// <summary>
    /// Controller script for the movement panel.
    /// </summary>
    public class MovePanelController : ContentPanelController {
        [SerializeField]
        private Text _moveText;

        public override void OnDoneButtonClicked() {
            var gameController = Util.GetGameController();
            gameController.StoreSelectedMove();
            gameController.NextShip();
        }

        /// <summary>
        /// Sets the name of the currently selected move.
        /// </summary>
        /// <param name="moveName"></param>
        public void SetMoveName(string moveName) {
            _moveText.text = moveName;
        }

        public void OnActiveShipChanged(bool active) {
            string text;
            if (active) {
                // It's the local ship.
                text = "Please select a move for the highlighted ship";
            }
            else {
                // It's the other guy's ship.
                text = "Please wait whilst opponent moves their next ship";
            }
            SetInfoText(text);
        }
    }
}