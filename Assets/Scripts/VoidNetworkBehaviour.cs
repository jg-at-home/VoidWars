using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    /// <summary>
    /// Base class for NetworkBehaviours that adds common functionality.
    /// </summary>
    public class VoidNetworkBehaviour : NetworkBehaviour {
        /// <summary>
        /// Gets the game controller.
        /// </summary>
        protected GameController controller {
            get {
                if (_controller == null) {
                    var gcObj= GameObject.FindGameObjectWithTag("GameController");
                    _controller = gcObj.GetComponent<GameController>();
                }

                return _controller;
            }
        }
        private GameController _controller;
    }
}

