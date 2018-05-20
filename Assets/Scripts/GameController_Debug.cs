using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Game controller debug code.
    /// </summary>
    public partial class GameController {
        private bool _showDebug;
        private Rect _printRect;

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Tab)) {
                _showDebug = !_showDebug;
            }
        }

        private void print(string msg) {
            GUI.Label(_printRect, msg);
            _printRect.y += 20;
        }

        private void OnGUI() {
            if (_communicator != null && _showDebug) {
                _printRect = new Rect(50, 40, 200, 20);

                print(string.Format("Turn #{0}", _round + 1));
                print("");
                print(string.Format("Controller ID: {0}", _communicator.ID));
                print(string.Format("Active Ship ID: {0}", _activeShipID));
                print(string.Format("Game State: {0}", _state));
                print(string.Format("Play phase: {0}", _playPhase));

                foreach(var ship in _ships) {
                    print("");
                    print(string.Format("Ship #{0}", ship.ID));
                    print(string.Format("Energy: {0}", ship.Energy));
                    print(string.Format("Health: {0}", ship.Health));
                    print(string.Format("Temp: {0}", ship.HullTemperature));
                }
            }
        }
    }
}