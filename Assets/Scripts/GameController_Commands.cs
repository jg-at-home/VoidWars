
using System.Collections;
using UnityEngine;

namespace VoidWars {
    public partial class GameController {
        private delegate void ActionFunc(ShipController ship, bool status);

        /// <summary>
        /// Executes the command string provided.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        public void ExecuteCommand(string command) {
            Debug.LogFormat("GameController.ExecuteCommand{0})", command);

            var parts = command.ToLower().Split(' ');
            switch (parts[0]) {
                case "pass":
                    // Nothing to do, carry on.
                    _actionComplete = true;
                    break;

                case "shields": {
                        var status = bool.Parse(parts[1]);
                        StartCoroutine(executeCloseup(_activeShipID, status, executeShields, 1f));
                    }
                    break;

                case "aux":
                    handleAuxCommand(parts);
                    _actionComplete = true;
                    break;

                // TODO: everything else
            }
        }

        private void handleAuxCommand(string[] parts) {
            switch(parts[1]) {
                case "shinobi": {
                        var status = bool.Parse(parts[2]);
                        executeCloak(_activeShip, status);
                    }
                    break;

                // TODO: everything else
            }
        }

        private void executeCloak(ShipController shipController, bool status) {
            if (status) {
                shipController.AudioPlayer.PlayOneShot(shipController.ShipClass.CloakClip);
            }
            else {
                // TODO: uncloak
            }
            _communicator.CmdSetCloakStatus(shipController.ID, status);
        }

        private void executeShields(ShipController shipController, bool status) {
            if (status) {
                shipController.AudioPlayer.PlayOneShot(shipController.ShipClass.ShieldsClip);
            }
            else {
                // TODO: shields down.
            }
            _communicator.CmdSetShieldStatus(shipController.ID, status);
        }

        /// <summary>
        /// Provides a means of briefly focusing the 3D view on a ship to see a status change.
        /// </summary>
        /// <param name="shipID">The ship's ID.</param>
        /// <param name="duration">The duration of the close-up in s.</param>
        public void ZoomCloseUp(int shipID, float duration) {
            StartCoroutine(executeCloseup(shipID, false, null, duration));
        }

        private IEnumerator executeCloseup(int shipID, bool status, ActionFunc actionFunc, float holdTime) {
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

            // Do the thing.
            if (actionFunc != null) {
                actionFunc(shipController, status);
            }

            // Wait a little.
            yield return new WaitForSeconds(holdTime);

            // Restore camera.
            CameraRig.transform.SetPositionAndRotation(oldCameraPos, oldCameraRot);

            // Signal finished.
            _actionComplete = true;
        }    
    }
}