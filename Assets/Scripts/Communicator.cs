using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    /// <summary>
    /// Basically, the networking side of the game controller. Acts as a bridge between clients and server.
    /// </summary>
    public class Communicator : VoidNetworkBehaviour {
        /// <summary>
        /// Gets the unique ID of this instance.
        /// </summary>
        public int ID {
            get { return (int)netId.Value; }
        }

        public override void OnStartLocalPlayer() {
            base.OnStartLocalPlayer();
            controller.SetCommunicator(this);
        }

        /// <summary>
        /// Advances the game at the end of a phase or state.
        /// </summary>
        [Command]
        public void CmdNextShip() {
            controller.NextShipServer();
        }

        [Command]
        public void CmdSetActiveShip(int index) {
            Debug.LogFormat("CmdSetActiveShip({0}", index);
            controller.SetActiveShipByIndex(index, false);
        }

        /// <summary>
        /// Called server-side to set the active ship.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="shipID"></param>
        public void NotifyActiveShip(int ownerID, int shipID) {
            Debug.Assert(isServer);
            Debug.LogFormat("NotifyActiveShip({0}, {1}) (ID={2})", ownerID, shipID, ID);
            RpcNotifyActiveShip(ownerID, shipID);
        }

        [ClientRpc]
        void RpcNotifyActiveShip(int ownerID, int shipID) {
            Debug.LogFormat("RpcNotifyActiveShip({0}, {1}) (ID={2})", ownerID, shipID, ID);
            controller.NotifyActiveShip(ownerID, shipID);
        }

        /// <summary>
        /// Spawns all the ships in the game. Must be called from server context.
        /// </summary>
        /// <param name="config">Game configuration data.</param>
        public void SpawnShips(GameConfig config) {
            _humanShipIndex = 0;
            _alienShipIndex = 0;

            // Spawn any AI ships in the server context.
            var numShipsPerPlayer = config.NumberOfShips / config.PlayerConfigs.Count;
            var aiConfigs = config.PlayerConfigs.FindAll(c => c.ControlType == ControlType.AI);
            var numAIPlayers = aiConfigs.Count;
            if (numAIPlayers > 0) {
                _alienSpawnPoints = controller.GetStartPointIndices(Faction.ALIENS, numAIPlayers, numShipsPerPlayer);
                spawnAIShips(aiConfigs);
            }

            // For humans, bounce the spawn request down to all the clients so they can create with the
            // correct authority.
            var humanConfigs = config.PlayerConfigs.FindAll(c => c.ControlType == ControlType.HUMAN);
            var numHumanPlayers = humanConfigs.Count;
            if (numHumanPlayers > 0) {
                _humanSpawnPoints = controller.GetStartPointIndices(Faction.HUMANS, numHumanPlayers, numShipsPerPlayer);
                spawnHumanShips(humanConfigs);
            }
        }

        private void spawnHumanShips(List<PlayerConfig> humanConfigs) {
            foreach (var config in humanConfigs) {
                // Only spawn the things I control.
                foreach (var shipConfig in config.ShipConfigs) {
                    RpcMaybeSpawnPlayerShip(config.ControllerID, shipConfig, config.Faction);
                }
            }
        }

        private void spawnAIShips(List<PlayerConfig> aiConfigs) {
            foreach(var config in aiConfigs) {
                foreach (var shipConfig in config.ShipConfigs) {
                    var className = shipConfig.ClassName;
                    var shipClass = controller.GetShipClassByName(className);
                    var prefabName = shipClass.ModelName;
                    var prefabPath = "Prefabs/Ships/" + prefabName;
                    var prefab = (GameObject)Resources.Load(prefabPath);
                    var ship = Instantiate(prefab);
                    var spawnIndex = _alienSpawnPoints[_alienShipIndex++];
                    var startPos = controller.StartPositions[spawnIndex];
                    ship.transform.localPosition = startPos.transform.position;
                    ship.transform.localRotation = startPos.transform.rotation;
                    var shipController = ship.GetComponent<ShipController>();
                    shipController.ControlType = ControlType.AI;
                    shipController.Faction = config.Faction;
                    shipController.StartPointIndex = spawnIndex;
                    shipController.OwnerID = ShipController.AI_OWNER;
                    shipController.ClassID = className;
                    shipController.PrimaryWeaponType = shipConfig.PrimaryWeapon;
                    NetworkServer.Spawn(ship);
                }
            }
        }

        [ClientRpc]
        void RpcMaybeSpawnPlayerShip(int playerID, ShipConfig config, Faction faction) {
            if (hasAuthority/* && playerID == ID*/) {
                CmdSpawnPlayerShip(playerID, config, faction);
            }
        }

        [Command]
        void CmdSpawnPlayerShip(int ownerID, ShipConfig config, Faction faction) {
            var shipClass = controller.GetShipClassByName(config.ClassName);
            var prefabPath = "Prefabs/Ships/" + shipClass.ModelName;
            var prefab = (GameObject)Resources.Load(prefabPath);
            var ship = Instantiate(prefab);
            var spawnIndex = _humanSpawnPoints[_humanShipIndex++];
            var startPos = controller.StartPositions[spawnIndex];
            ship.transform.localPosition = startPos.transform.position;
            ship.transform.localRotation = startPos.transform.rotation;
            var shipController = ship.GetComponent<ShipController>();
            shipController.ControlType = ControlType.HUMAN;
            shipController.Faction = faction;
            shipController.StartPointIndex = spawnIndex;
            shipController.OwnerID = ownerID;
            shipController.ClassID = config.ClassName;
            shipController.PrimaryWeaponType = config.PrimaryWeapon;
            var player = controller.GetPlayer(ownerID);
            NetworkServer.SpawnWithClientAuthority(ship, player.Connection);
        }

        /// <summary>
        /// Notifies clients of a game state change.
        /// </summary>
        /// <param name="newState">The new state.</param>
        public void NotifyGameStateChange(GameState newState) {
            Debug.Assert(isServer);

            RpcNotifyGameStateChange(newState);
        }

        [ClientRpc]
        void RpcNotifyGameStateChange(GameState newState) {
            controller.SetState(newState, false);
        }

        /// <summary>
        /// Notifies clients of a game phase change.
        /// </summary>
        /// <param name="newPhase">The new phase.</param>
        public void NotifyPlayPhaseChange(PlayPhase newPhase) {
            Debug.Assert(isServer);

            RpcNotifyPlayPhaseChange(newPhase);
        }

        [ClientRpc]
        void RpcNotifyPlayPhaseChange(PlayPhase newPhase) {
            controller.SetPlayPhase(newPhase, false);
        }

        #region UI
        /// <summary>
        /// Tells clients to enable their info panels.
        /// </summary>
        /// <param name="caption">Info panel caption.</param>
        /// <param name="prefabName">The prefab of the panel content.</param>
        [Command]
        public void CmdEnableInfoPanel(string caption, string prefabName) {
            RpcEnableInfoPanel(caption, prefabName);
        }

        [ClientRpc]
        void RpcEnableInfoPanel(string caption, string prefabName) {
            controller.EnableInfoPanel(caption, prefabName);
        }

        /// <summary>
        /// Tells all clients to clear their info panel.
        /// </summary>
        [Command]
        public void CmdDisableInfoPanel() {
            RpcDisableInfoPanel();
        }

        [ClientRpc]
        void RpcDisableInfoPanel() {
            controller.DisableInfoPanel();
        }

        [Command]
        public void CmdEnableControlPanel(bool enable) {
            RpcEnableControlPanel(enable);
        }

        [ClientRpc]
        void RpcEnableControlPanel(bool enable) {
            controller.EnableControlPanel(enable);
        }
        #endregion UI


        private void Update() {
            if (isServer) {
                controller.UpdateServer();
            }
        }

        private int _humanShipIndex;
        private int _alienShipIndex;
        private int[] _humanSpawnPoints;
        private int[] _alienSpawnPoints;
    }
}