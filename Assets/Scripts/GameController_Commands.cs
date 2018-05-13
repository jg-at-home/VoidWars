
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
                    if (handleAuxCommand(parts)) {
                        _actionComplete = true;
                    }
                    break;

                // TODO: everything else
            }
        }

        private bool handleAuxCommand(string[] parts) {
            var complete = true;
            switch(parts[1]) {
                case "shinobi": {
                        var status = bool.Parse(parts[2]);
                        executeCloak(_activeShip, status);
                    }
                    break;

                case "scanners": {
                        var status = bool.Parse(parts[2]);
                        executeScanners(_activeShip, status);
                    }
                    break;

                case "flarelauncher": {
                        // It's a one-shot so no need to parse the argument.
                        _activeShip.UseOneShotAuxiliary(AuxType.FlareLauncher, () => _actionComplete = true);
                    }
                    break;

                case "erbinducer": {
                        _activeShip.UseOneShotAuxiliary(AuxType.ERBInducer, () => _actionComplete = true);
                        complete = false;
                    }
                    break;
                // TODO: everything else
            }

            return complete;
        }

        private void executeCloak(ShipController shipController, bool status) {
            if (status) {
                shipController.AudioPlayer.PlayOneShot(shipController.ShipData.CloakClip);
            }
            else {
                // TODO: uncloak
            }
            _communicator.CmdSetCloakStatus(shipController.ID, status);
        }

        private void executeShields(ShipController shipController, bool status) {
            if (status) {
                shipController.AudioPlayer.PlayOneShot(shipController.ShipData.ShieldsClip);
            }
            else {
                // TODO: shields down.
            }
            _communicator.CmdSetShieldStatus(shipController.ID, status);
        }

        private void executeScanners(ShipController shipController, bool status) {
            if (status) {
                shipController.AudioPlayer.PlayOneShot(shipController.ShipData.ScannersClip);
            }
            _communicator.CmdEnableAuxiliary(shipController.ID, AuxType.Scanners, status);

            // On this client only, show / hide the scanner data.
            foreach (var ship in _ships) {
                // TODO: only ships in range?
                // Target all ships owned by everyone else.
                if (ship.OwnerID != shipController.OwnerID) {
                    if (status) {
                        Instantiate(ScannerInfoPrefab, ship.transform);
                    }
                    else {
                        var scannerInfo = ship.gameObject.FindChildrenWithTag("scanner");
                        Debug.Assert(scannerInfo.Length == 1);
                        scannerInfo[0].transform.SetParent(null, false);
                        Destroy(scannerInfo[0]);
                    }
                }
            }
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