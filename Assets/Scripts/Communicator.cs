using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    /// <summary>
    /// Basically, the networking side of the game controller.
    /// </summary>
    public class Communicator : NetworkBehaviour {
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
        /// Called server-side to set the active ship.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="shipID"></param>
        public void NotifyActiveShip(int shipID) {
            RpcNotifyActiveShip(shipID);
        }

        [ClientRpc]
        void RpcNotifyActiveShip(int shipID) {
            controller.SetActiveShip(shipID);
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
                _alienSpawnPoints = _controller.GetStartPointIndices(Faction.ALIENS, numAIPlayers, numShipsPerPlayer);
                spawnAIShips(aiConfigs);
            }

            // For humans, bounce the spawn request down to all the clients so they can create with the
            // correct authority.
            var humanConfigs = config.PlayerConfigs.FindAll(c => c.ControlType == ControlType.HUMAN);
            var numHumanPlayers = humanConfigs.Count;
            if (numHumanPlayers > 0) {
                _humanSpawnPoints = _controller.GetStartPointIndices(Faction.HUMANS, numHumanPlayers, numShipsPerPlayer);
                spawnHumanShips(humanConfigs);
            }
        }

        private void spawnHumanShips(List<PlayerConfig> humanConfigs) {
            foreach (var config in humanConfigs) {
                // Only spawn the things I control.
                foreach (var prefabName in config.ShipPrefabs) {
                    // TODO: oh hackity hack!            
                    RpcMaybeSpawnPlayerShip(1, prefabName, config.Faction);
//                    RpcMaybeSpawnPlayerShip(config.ControllerID, prefabName, config.Faction);
                }
            }
        }

        private void spawnAIShips(List<PlayerConfig> aiConfigs) {
            foreach(var config in aiConfigs) {
                foreach (var prefabName in config.ShipPrefabs) {
                    var prefabPath = "Prefabs/" + prefabName;
                    var prefab = (GameObject)Resources.Load(prefabPath);
                    var ship = Instantiate(prefab);
                    var spawnIndex = _alienSpawnPoints[_alienShipIndex++];
                    var startPos = _controller.StartPositions[spawnIndex];
                    ship.transform.localPosition = startPos.transform.position;
                    ship.transform.localRotation = startPos.transform.rotation;
                    var shipController = ship.GetComponent<ShipController>();
                    shipController.ControlType = ControlType.AI;
                    shipController.Faction = config.Faction;
                    NetworkServer.Spawn(ship);
                }
            }
        }

        [ClientRpc]
        void RpcMaybeSpawnPlayerShip(int playerID, string prefabName, Faction faction) {
            if (hasAuthority && playerID == ID) {
                // This causes problems client-side XXX
                CmdSpawnPlayerShip(prefabName, faction);
            }
        }

        [Command]
        void CmdSpawnPlayerShip(string prefabName, Faction faction) {
            var prefabPath = "Prefabs/" + prefabName;
            var prefab = (GameObject)Resources.Load(prefabPath);
            var ship = Instantiate(prefab);
            var spawnIndex = _humanSpawnPoints[_humanShipIndex++];
            var startPos = _controller.StartPositions[spawnIndex];
            ship.transform.localPosition = startPos.transform.position;
            ship.transform.localRotation = startPos.transform.rotation;
            var shipController = ship.GetComponent<ShipController>();
            shipController.ControlType = ControlType.HUMAN;
            shipController.Faction = faction;
            NetworkServer.SpawnWithClientAuthority(ship, connectionToClient);
        }

        public void NotifyGameStateChange(GameState newState) {
            Debug.Assert(isServer);

            RpcNotifyGameStateChange(newState);
        }

        [ClientRpc]
        void RpcNotifyGameStateChange(GameState newState) {
            controller.SetState(newState, false);
        }

        public void NotifyPlayPhaseChange(PlayPhase newPhase) {
            Debug.Assert(isServer);

            RpcNotifyPlayPhaseChange(newPhase);
        }

        [ClientRpc]
        void RpcNotifyPlayPhaseChange(PlayPhase newPhase) {
            controller.SetPlayPhase(newPhase, false);
        }

        private void Update() {
            if (isServer) {
                if (_controller != null) {
                    _controller.UpdateServer();
                }
            }
        }

        private GameController controller {
            get {
                if (_controller == null) {
                    var controllerObj = GameObject.FindGameObjectWithTag("GameController");
                    _controller = controllerObj.GetComponent<GameController>();
                }
                return _controller;
            }

        }
        private GameController _controller;
        private int _humanShipIndex;
        private int _alienShipIndex;
        private int[] _humanSpawnPoints;
        private int[] _alienSpawnPoints;
    }
}