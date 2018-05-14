using System.Collections;

namespace VoidWars {
    public class ActionInfoPanelController : ContentPanelController {
        public void OnActiveShipChanged(bool active) {
            string text;
            if (active) {
                // It's the local ship.
                // TODO: change prompt depending on the number of actions the player has.
                text = "Please choose an action for the selected ship";
            }
            else {
                // It's the other guy's ship.
                text = "Please wait whilst opponent chooses their next action(s)";
            }
            SetInfoText(text);
        }

        /// <summary>
        /// Called when the Done button is clicked.
        /// </summary>
        public override void OnDoneButtonClicked() {
        }
    }
}