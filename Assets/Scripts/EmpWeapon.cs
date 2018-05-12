using UnityEngine;
using System.Collections;

namespace VoidWars {
    /// <summary>
    /// ElectroMagnetic Pulse weapon.
    /// </summary>
    public class EmpWeapon : WeaponInstance {
        /// <summary>
        /// Constructs an instance.
        /// </summary>
        /// <param name="weaponClass">Weapon data.</param>
        public EmpWeapon(WeaponClass weaponClass) : base(weaponClass) {
            _duration = GetFloat("Duration");
        }

        protected override IEnumerator attack(ShipController ship, int slot, ShipController target, bool applyDamage) {
            ship.AudioPlayer.PlayOneShot(SoundEffect);
            var effect = Object.Instantiate(Prefab, ship.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(_duration);
            Object.Destroy(effect);
            if (applyDamage) {
                var layerMask = 1 << LayerMask.NameToLayer("ActiveObjects");
                var radius = Range * ship.LuckRoll;
                var overlaps = Physics.OverlapSphere(ship.transform.position, radius, layerMask);
                var numTurns = GetInt("LastsForTurns");
                for (int i = 0; i < overlaps.Length; ++i) {
                    var gob = overlaps[i].gameObject;
                    if (!ReferenceEquals(gob, ship.gameObject)) {
                        gob.SendMessage("RespondToEmp", numTurns, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }

        private readonly float _duration;
    }
}