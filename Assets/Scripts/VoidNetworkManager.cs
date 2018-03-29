using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    public class VoidNetworkManager : NetworkManager {
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
            var controller = conn.playerControllers.Find(pc => pc.playerControllerId == playerControllerId);
            var playerID = (int)controller.unetView.netId.Value;
            var gameControllerObj = GameObject.FindGameObjectWithTag("GameController");
            var gameController = gameControllerObj.GetComponent<GameController>();
            gameController.AddPlayer(playerID);
            base.OnServerAddPlayer(conn, playerControllerId);
        }
    }
}