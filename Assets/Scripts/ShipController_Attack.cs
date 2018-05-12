using System.Collections;
using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Ship Controller class weapon and attack code.
    /// </summary>
    public partial class ShipController {
        /// <summary>
        /// Coroutine for performning an attack.
        /// </summary>
        /// <param name="target">The target ship</param>
        /// <param name="weaponSlot">The weapon slot.</param>
        /// <param name="weaponClass">The weapon class.</param>
        /// <param name="applyDamage">If true, apply the damage.</param>
        /// <returns>Enumerator</returns>
        public IEnumerator Attack(ShipController target, int weaponSlot, WeaponInstance weapon) {
            // Use up some juice.
            _energy -= weapon.PowerUsage;
            Debug.Assert(_energy >= 0f);

            // Do the attack.
            yield return weapon.Fire(this, weaponSlot, target, isServer);
        }
    }
}