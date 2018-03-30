using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    public class VoidNetworkManager : NetworkManager {
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
            base.OnServerAddPlayer(conn, playerControllerId);
            var controller = conn.playerControllers.Find(pc => pc.playerControllerId == playerControllerId);
            var playerID = (int)controller.unetView.netId.Value;
            var gameController = Util.GetGameController();
            var player = new PlayerServerRep(conn, playerID);
            gameController.AddPlayer(player);
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player) {
            base.OnServerRemovePlayer(conn, player);
            var playerID = (int)player.unetView.netId.Value;
            var gameController = Util.GetGameController();
            gameController.RemovePlayer(playerID);
        }
    }
}