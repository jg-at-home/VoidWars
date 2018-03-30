using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    public class Communicator : NetworkBehaviour {
        /// <summary>
        /// Gets the unique ID of this instance.
        /// </summary>
        public int ID {
            get { return (int)netId.Value; }
        }

        public override void OnStartLocalPlayer() {
            base.OnStartLocalPlayer();
            _controller = Util.GetGameController();
            _controller.SetCommunicator(this);
        }

        /// <summary>
        /// Spawns all the ships in the game. Must be called from server context.
        /// </summary>
        /// <param name="config">Game configuration data.</param>
        public void SpawnShips(GameConfig config) {
            _shipIndex = 0;

            // Spawn any AI ships in the server context.
            var aiConfigs = config.PlayerConfigs.FindAll(c => c.ControlType == ControlType.AI);
            spawnAIShips(aiConfigs);

            // For humans, bounce the spawn request down to all the clients so they can create with the
            // correct authority.
            var humanConfigs = config.PlayerConfigs.FindAll(c => c.ControlType == ControlType.HUMAN);
            spawnHumanShips(humanConfigs);
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
                    var startPos = _controller.StartPositions[_shipIndex];
                    ship.transform.localPosition = startPos.transform.position;
                    ship.transform.localRotation = startPos.transform.rotation;
                    ++_shipIndex;
                    var controller = ship.GetComponent<ShipController>();
                    controller.ControlType = ControlType.AI;
                    controller.Faction = config.Faction;
                    NetworkServer.Spawn(ship);
                }
            }
        }

        [ClientRpc]
        void RpcMaybeSpawnPlayerShip(int playerID, string prefabName, Faction faction) {
            if (playerID == ID) {
                CmdSpawnPlayerShip(prefabName, faction);
            }
        }

        [Command]
        void CmdSpawnPlayerShip(string prefabName, Faction faction) {
            var prefabPath = "Prefabs/" + prefabName;
            var prefab = (GameObject)Resources.Load(prefabPath);
            var ship = Instantiate(prefab);
            var startPos = _controller.StartPositions[_shipIndex];
            ship.transform.localPosition = startPos.transform.position;
            ship.transform.localRotation = startPos.transform.rotation;
            ++_shipIndex;
            var controller = ship.GetComponent<ShipController>();
            controller.ControlType = ControlType.HUMAN;
            controller.Faction = faction;
            NetworkServer.SpawnWithClientAuthority(ship, connectionToClient);
        }

        public void NotifyGameStateChange(GameState newState) {
            Debug.Assert(isServer);

            RpcNotifyGameStateChange(newState);
        }

        [ClientRpc]
        void RpcNotifyGameStateChange(GameState newState) {
            _controller.SetState(newState);
        }

        public void NotifyPlayPhaseChange(PlayPhase newPhase) {
            Debug.Assert(isServer);

            RpcNotifyPlayPhaseChange(newPhase);
        }

        [ClientRpc]
        void RpcNotifyPlayPhaseChange(PlayPhase newPhase) {
            _controller.SetPlayPhase(newPhase);
        }

        private void Update() {
            if (isServer) {
                if (_controller != null) {
                    _controller.UpdateServer();
                }
            }
        }

        private GameController _controller;
        private int _shipIndex;
    }
}