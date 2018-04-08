using UnityEngine;
using UnityEngine.UI;

namespace VoidWars {
    public class MovePanelController : ContentPanelController {
        [SerializeField]
        private Text _moveText;

        public override void OnDoneButtonClicked() {
            var gameController = Util.GetGameController();
            gameController.NextShip();
        }

        /// <summary>
        /// Sets the name of the currently selected move.
        /// </summary>
        /// <param name="moveName"></param>
        public void SetMoveName(string moveName) {
            _moveText.text = moveName;
        }
    }
}