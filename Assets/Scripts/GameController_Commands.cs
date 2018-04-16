
using UnityEngine;

namespace VoidWars {
    public partial class GameController {
        /// <summary>
        /// Executes the command string provided.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        public void ExecuteCommand(string command) {
            Debug.LogFormat("GameController.ExecuteCommand{0})", command);

            var parts = command.Split(' ');
            switch (parts[0]) {
                case "pass":
                    // Nothing to do, carry on.
                    break;

                case "shields": {
                        var status = bool.Parse(parts[1]);
                        _communicator.CmdSetShieldStatus(_activeShipID, status);
                    }
                    break;

                // TODO: everything else
            }
        }
    }
}