using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Base class for manipulators.
    /// </summary>
    public class Manipulator : MonoBehaviour {
        /// <summary>
        /// Called when the manipulator is activated.
        /// </summary>
        public virtual void OnActivation() {

        }

        /// <summary>
        /// Called when the manipulator is deactivated.
        /// </summary>
        public virtual void OnDeactivation() {

        }
    }
}