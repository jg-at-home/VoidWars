using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    public class Communicator : NetworkBehaviour {
        public int ID {
            get { return (int)netId.Value; }
        }

        public override void OnStartLocalPlayer() {
            base.OnStartLocalPlayer();
            controller.SetCommunicator(this);
        }

        public void SpawnShips(GameConfig config) {
            // TODO
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

        protected GameController controller {
            get {
                if (_controller == null) {
                    var controllerObj = GameObject.FindGameObjectWithTag("GameController");
                    _controller = controllerObj.GetComponent<GameController>();
                }
                return _controller;
            }
        }

        private void Update() {
            if (isServer) {
                if (_controller != null) {
                    _controller.UpdateServer();
                }
            }
        }

        private GameController _controller;
    }
}