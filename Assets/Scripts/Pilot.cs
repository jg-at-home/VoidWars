using UnityEngine;
using System.Collections;

namespace VoidWars {
    /// <summary>
    /// Abstraction of an agent that control a ship.
    /// </summary>
    public abstract class Pilot {
        /// <summary>
        /// Updates the ship.
        /// </summary>
        /// <param name="gameController">The game controller.</param>
        /// <param name="shipController">The ship controller.</param>
        public virtual void UpdateShip(GameController gameController, ShipController shipController) {
        }

        /// <summary>
        /// Called when the ship becomes active.
        /// </summary>
        /// <param name="gameController">The game controller.</param>
        /// <param name="shipController">The ship controller.</param>
        public abstract void OnShipActivation(GameController gameController, ShipController shipController);

        /// <summary>
        /// Called when the ship becomes inactive.
        /// </summary>
        /// <param name="gameController">The game controller.</param>
        /// <param name="shipController">The ship controller.</param>
        public virtual void OnShipDeactivation(GameController gameController, ShipController shipController) {
        }
    }
}
