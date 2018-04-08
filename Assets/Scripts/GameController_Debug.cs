﻿using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Game controller debug code.
    /// </summary>
    public partial class GameController {
        private void OnGUI() {
            if (_communicator != null) {
                var text = string.Format("Controller ID: {0}", _communicator.ID);
                GUI.Label(new Rect(800, 20, 200, 20), text);
                text = string.Format("Active Ship ID: {0}", _activeShipID);
                GUI.Label(new Rect(800, 40, 200, 20), text);
                text = string.Format("Game State: {0}", _state);
                GUI.Label(new Rect(800, 60, 200, 20), text);
            }
        }
    }
}