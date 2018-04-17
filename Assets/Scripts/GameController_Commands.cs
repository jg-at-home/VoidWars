
using System.Collections;
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
                        StartCoroutine(executeShields(_activeShipID, status));
                    }
                    break;

                // TODO: everything else
            }
        }

        private IEnumerator executeShields(int shipID, bool status) {
            // Stash the current camera transform.
            var oldCameraPos = CameraRig.transform.position;
            var oldCameraRot = CameraRig.transform.rotation;

            // Put the camera above the ship.
            var shipController = GetShip(shipID);
            var shipGob = shipController.gameObject;

            var newCameraPosition = shipGob.transform.position + Vector3.up * 4f;
            CameraRig.transform.position = newCameraPosition;
            CameraRig.transform.LookAt(shipGob.transform.position);

            // Wait a frame for the move.
            yield return null;

            // Enable the shields.
            _communicator.CmdSetShieldStatus(shipID, status);

            // Wait a little.
            yield return new WaitForSeconds(1.0f);

            // Restore camera.
            CameraRig.transform.SetPositionAndRotation(oldCameraPos, oldCameraRot);
        }
    }
}