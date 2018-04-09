namespace VoidWars {
    public class SetupPanelController : ContentPanelController {
        public void OnActiveShipChanged(bool active) {
            string text;
            if (active) {
                // It's the local ship.
                text = "Please set the start position and rotation of your ship";
            }
            else {
                // It's the other guy's ship.
                text = "Please wait whilst opponent sets up their next ship";
            }
            SetInfoText(text);
        }
    }
}