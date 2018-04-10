using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Game controller debug code.
    /// </summary>
    public partial class GameController {
        private void OnGUI() {
            if (_communicator != null) {
                var text = string.Format("Controller ID: {0}", _communicator.ID);
                GUI.Label(new Rect(700, 20, 200, 20), text);
                text = string.Format("Active Ship ID: {0}", _activeShipID);
                GUI.Label(new Rect(700, 40, 200, 20), text);
                text = string.Format("Game State: {0}", _state);
                GUI.Label(new Rect(700, 60, 200, 20), text);
                text = string.Format("Play phase: {0}", _playPhase);
                GUI.Label(new Rect(700, 80, 200, 20), text);
            }
        }
    }
}