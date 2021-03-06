﻿
using System;
using System.Collections;
using UnityEngine;

namespace VoidWars {
    public partial class GameController {
        private delegate void ActionFunc(ShipController ship, bool status);


        /// <summary>
        /// Executes the command string provided.
        /// </summary>
        /// <param name="ship">The ship to affect.</param>
        /// <param name="command">The command to execute.</param>
        public void ExecuteCommand(ShipController ship, string command) {
            Debug.LogFormat("GameController.ExecuteCommand({0})", command);

            var parts = command.Split(' ');
            switch (parts[0].ToLower()) {
                case "pass":
                    // Nothing to do, carry on.
                    _actionComplete = true;
                    break;

                case "shields": {
                        var status = bool.Parse(parts[1]);
                        StartCoroutine(executeCloseup(ship.ID, status, executeShields, 1f));
                    }
                    break;

                case "aux":
                    if (handleAuxCommand(ship, parts)) {
                        _actionComplete = true;
                    }
                    break;

                case "repair":
                    createRepairTask(ship, parts);
                    _actionComplete = true;
                    break;

                case "yaw180":
                    StartCoroutine(executeCloseup(ship.ID, true, (s,x) => { s.CmdYaw180();} , 2f));
                    break;

                case "refuel": {
                        // TODO: some kind of animation.
                        var targetID = int.Parse(parts[1]);
                        ship.CmdTransferEnergy(targetID);
                        _actionComplete = true;
                    }
                    break;

                case "heal": {
                        // TODO: some kind of animation.
                        var targetID = int.Parse(parts[1]);
                        ship.CmdTransferHealth(targetID);
                        _actionComplete = true;
                    }
                    break;

                // TODO: everything else
            }
        }

        private void createRepairTask(ShipController ship, string[] parts) {
            if (parts[1] == "propulsion") {
                ship.CmdScheduleEngineRepairs();
            }
            else {
                var itemType = (AuxType)Enum.Parse(typeof(AuxType), parts[1]);
                ship.CmdScheduleRepairForItems(itemType);
            }
        }

        private bool handleAuxCommand(ShipController ship, string[] parts) {
            var complete = true;
            switch (parts[1]) {
                case "shinobi": {
                        var status = bool.Parse(parts[2]);
                        executeCloak(ship, status);
                    }
                    break;

                case "scanners": {
                        var status = bool.Parse(parts[2]);
                        executeScanners(ship, status);
                    }
                    break;

                case "flarelauncher": {
                        // It's a one-shot so no need to parse the argument.
                        ship.UseOneShotAuxiliary(AuxType.FlareLauncher, () => _actionComplete = true);
                        complete = false;
                    }
                    break;

                case "erbinducer": {
                        _communicator.CmdTeleportShip(ship.ID);
                        complete = false;
                    }
                    break;

                case "chafflauncher": {
                        _communicator.CmdLaunchChaff(ship.ID);
                        complete = false;
                    }
                    break;

                case "minelauncher": {
                        _communicator.CmdDeployMine(ship.ID);
                        complete = false;
                    }
                    break;

                case "empgenerator": {
                        _communicator.CmdGenerateEMP(ship.ID);
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