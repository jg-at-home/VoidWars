using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    /// <summary>
    /// Base class for NetworkBehaviours that adds common functionality.
    /// </summary>
    public class VoidWarsObject : NetworkBehaviour {
        /// <summary>
        /// Gets the unique ID of this instance.
        /// </summary>
        public int ID {  get { return (int)netId.Value; } }

        /// <summary>
        /// Computes an acts on an amount of damage as a result of some action.
        /// </summary>
        /// <param name="source">The object doing the damage.</param>
        /// <param name="damage">The amount of damage to apply.</param>
        /// <param name="dT">Associated temperature change.</param>
        /// <returns>The amount of damage done (may be different from that coming in).</returns>
        public virtual float ComputeDamage(VoidWarsObject source, float damage, float dT) {
            return damage;
        }

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

