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
        public IEnumerator Attack(ShipController target, int weaponSlot, WeaponClass weaponClass) {
            Transform node = GetWeaponNode(weaponSlot);
            WeaponType weaponType = weaponClass.WeaponType;
            switch(weaponType) {
                case WeaponType.Laser:
                case WeaponType.UVLaser:
                    yield return Laser.Attack(node.position, weaponClass, this, target, isServer);
                    break;

                default:
                    yield return null;
                    break;
            }
        }
    }
}