using System;
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
        /// Called when a round ends.
        /// </summary>
        public void EndThisRound() {
            controller.EndRoundServer();
            RpcEndThisRound();
        }

        [ClientRpc]
        void RpcEndThisRound() {
            controller.EndRoundClient();
        }

        /// <summary>
        /// Called when a new round begins.
        /// </summary>
        public void BeginNextRound() {
            controller.BeginRoundServer();
            RpcBeginNextRound();
        }

        [ClientRpc]
        void RpcBeginNextRound() {
            controller.BeginRoundClient();
        }

        /// <summary>
        /// Called by clients to add a move to the server.
        /// </summary>
        /// <param name="move">The move to add.</param>
        [Command]
        public void CmdAddPlayerMove(ShipMoveInstance move) {
            controller.AddPlayerMove(move);
        }

        /// <summary>
        /// Performs an attack.
        /// </summary>
        /// <param name="sourceID">The ID of the attacking ship.</param>
        /// <param name="targetID">The ID of the target ship.</param>
        /// <param name="weaponSlot">0=front, 1=rear</param>
        [Command]
        public void CmdPerformAttack(int sourceID, int targetID, int weaponSlot) {
            // Generate a random seed. All client must agree on what will happen next, so all
            // the stochastic bits of calculations must be identical. This seed then is used
            // to init the .NET rng on all clients to get the desired result.
            var rngSeed = (int)Time.time;
            RpcPerformAttack(sourceID, targetID, weaponSlot, rngSeed);
        }

        [ClientRpc]
        void RpcPerformAttack(int sourceID, int targetID, int weaponSlot, int rngSeed) {
            Statistics.Reseed(rngSeed);
            controller.PerformAttack(sourceID, targetID, weaponSlot);
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

        [Command]
        public void CmdTeleportShip(int shipID) {
            var ship = controller.GetShip(shipID);
            var destination = controller.GetTeleportDestination(ship);
            RpcTeleportShip(shipID, destination);
        }

        [ClientRpc]
        void RpcTeleportShip(int shipID, Vector3 destination) {
            var ship = controller.GetShip(shipID);
            var teleporter = (Teleporter)ship.GetAuxiliaryItem(AuxType.ERBInducer);
            teleporter.SetDestination(destination);
            ship.UseOneShotAuxiliary(AuxType.ERBInducer, () => controller.OnActionComplete());
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
        /// Applies damage to a ship on the server.
        /// </summary>
        /// <param name="shipID">The ship's ID</param>
        /// <param name="damage">The amount of damage to do.</param>
        /// <param name="dT">The temperature effect/</param>
        [Command]
        public void CmdApplyDamageToShip(int shipID, float damage, float dT) {
            var ship = controller.GetShip(shipID);
            damage = ship.ComputeDamage(damage, dT);
            RpcShowDamagePopup(shipID, damage);
        }

        [ClientRpc]
        void RpcShowDamagePopup(int shipID, float damage) {
            controller.ShowDamage(shipID, damage);
        }

        [Command]
        public void CmdUpdateNPCs() {
            Debug.Log("CmdUpdateNPCs()");
            controller.UpdateNPCs();
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
                    shipController.SecondaryWeaponType = shipConfig.SecondaryWeapon;
                    shipController.EquipmentMask = makeEquipmentMask(shipConfig.Equipment);
                    shipController.CrewNames.Add(shipConfig.Captain);
                    shipController.CrewNames.Add(shipConfig.FirstOfficer);
                    shipController.CrewNames.Add(shipConfig.Engineer);
                    NetworkServer.Spawn(ship);
                }
            }
        }

        private static int makeEquipmentMask(AuxType[] equipment) {
            var mask = 0;
            foreach(var item in equipment) {
                mask |= (int)item;
            }
            return mask;
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
            shipController.SecondaryWeaponType = config.SecondaryWeapon;
            shipController.EquipmentMask = makeEquipmentMask(config.Equipment);
            shipController.CrewNames.Add(config.Captain);
            shipController.CrewNames.Add(config.FirstOfficer);
            shipController.CrewNames.Add(config.Engineer);
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

        /// <summary>
        /// Executes the selected moves (paying attention to ownership because it's not a 
        /// server-authoratative model and we have to move them on their owner client).
        /// </summary>
        /// <param name="moves">List of moves.</param>
        public void EnactMoves(List<ShipMoveInstance> moves) {
            foreach(var move in moves) {
                if (move.Move.MoveType != MoveType.None) {
                    controller.BeginMove(move);
                    RpcMoveShip(move);
                }
            }
        }

        [ClientRpc]
        void RpcMoveShip(ShipMoveInstance move) {
            controller.MoveShip(move);
            var shipController = controller.GetShip(move.ShipID);
            shipController.EnableEngineFX(true);
        }

        /// <summary>
        /// Shows message on all clients.
        /// </summary>
        /// <param name="msg">The message to show.</param>
        /// <param name="role">Who issued the message.</param>
        [Command]
        public void CmdBroadcastMessage(string msg, Role role) {
            RpcBroadcastMessage(msg, role);
        }

        [ClientRpc]
        void RpcBroadcastMessage(string message, Role role) {
            var myShips = getLocalShips();
            var myShip = myShips[0];
            var crewMember = myShip.GetCrewMember(role);
            controller.ShowMsg(message, crewMember.Photo);
        }

        private List<ShipController> getLocalShips() {
            var localOwnerID = controller.LocalOwnerID;
            return controller.GetShipsOwnedBy(localOwnerID);
        }

        /// <summary>
        ///  Called when a ship has finished its motion.
        /// </summary>
        /// <param name="shipID">The ship ID.</param>
        [Command]
        public void CmdMoveFinished(int shipID) {
            controller.MoveFinished(shipID);
            RpcEnableEngineFX(shipID, false);
        }

        [ClientRpc]
        void RpcEnableEngineFX(int shipID, bool enable) {
            var shipController = controller.GetShip(shipID);
            shipController.EnableEngineFX(enable);
        }

        /// <summary>
        /// Sets the status of a ship's shields.
        /// </summary>
        /// <param name="shipID">The ship to modify.</param>
        /// <param name="enable">Enable / disable.</param>
        [Command]
        public void CmdSetShieldStatus(int shipID, bool enable) {
            var shipController = controller.GetShip(shipID);
            shipController.EnableShields(enable);
            var msg = string.Format("Ship <color=orange>'{0}'</color> has {1}ed shields",
                shipController.ShipData.Name, enable ? "rais" : "lower");
            RpcShowMessageToOtherPlayers(shipController.OwnerID, msg, Role.FirstOfficer);
        }

        [ClientRpc]
        void RpcShowMessageToOtherPlayers(int ownerID, string msg, Role role) {
            if (!controller.IsOwner(ownerID)) {
                // TODO: select a ship (ergo crew member) in a better way.
                var myShips = getLocalShips();
                var myShip = myShips[0];
                var crewMember = myShip.GetCrewMember(role);
                controller.ShowMsg(msg, crewMember.Photo);
            }
        }

        /// <summary>
        /// Sets the cloaking (Shinobi) status of the ship.
        /// </summary>
        /// <param name="shipID">The ID of the ship to affect.</param>
        /// <param name="enable">Control flag.</param>
        [Command]
        public void CmdSetCloakStatus(int shipID, bool enable) {
            RpcEnableCloaking(shipID, enable);
        }

        [ClientRpc]
        void RpcEnableCloaking(int shipID, bool enable) {
            var shipController = controller.GetShip(shipID);
            shipController.EnableCloaking(enable);
            var msg = string.Format("Ship <color=orange>'{0}'</color> has {1}stealthed",
                shipController.ShipData.Name, enable ? "" : "de-");
            RpcShowMessageToOtherPlayers(shipController.OwnerID, msg, Role.FirstOfficer);
        }

        /// <summary>
        /// Enables an auxiliary item on a ship.
        /// </summary>
        /// <param name="shipID">The ship to affect.</param>
        /// <param name="auxType">The type of item.</param>
        /// <param name="enable">Enable / disable flag.</param>
        [Command]
        public void CmdEnableAuxiliary(int shipID, AuxType auxType, bool enable) {
            RpcEnableAuxiliary(shipID, auxType, enable);
        }

        [ClientRpc]
        void RpcEnableAuxiliary(int shipID, AuxType auxType, bool enable) {
            var ship = controller.GetShip(shipID);
            ship.EnableAuxiliary(auxType, enable);
        }

        #region UI
        [Command]
        public void CmdDisableActionPanel() {
            RpcDisableActionPanel();
        }

        [ClientRpc]
        void RpcDisableActionPanel() {
            controller.EnableActionPanel(false);
        }

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