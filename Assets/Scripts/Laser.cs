using UnityEngine;
using System.Collections;

namespace VoidWars {
    /// <summary>
    /// Laser weapon. 
    /// </summary>
    public class Laser : WeaponInstance {
        /// <summary>
        /// Constructs an instance.
        /// </summary>
        /// <param name="weaponClass">Weapon data.</param>
        public Laser(WeaponClass weaponClass) : base(weaponClass) {
            var aoLayerMask = 1 << LayerMask.NameToLayer("ActiveObjects");
            var shipLayerMask = 1 << LayerMask.NameToLayer("Ships");
            _layerMask = aoLayerMask | shipLayerMask;
        }

        protected override IEnumerator attack(ShipController ship, int slot, VoidWarsObject target, bool applyDamage) {
            var sourceNode = ship.GetWeaponNode(slot);
            var sourcePoint = sourceNode.position;
            var laserGob = Object.Instantiate(Prefab);
            var laser = laserGob.GetComponent<LaserController>();
            var targetPoint = target.gameObject.transform.position;
            var direction = (targetPoint - sourcePoint).normalized;
            RaycastHit hit;
            var ray = new Ray(sourcePoint, direction);
            Physics.Raycast(ray, out hit, _layerMask);
            var color = (WeaponType == WeaponType.Laser) ? Color.red : Color.magenta;
            laser.Run(color, sourcePoint, hit.point);
            ship.AudioPlayer.PlayOneShot(SoundEffect);
            yield return new WaitForSeconds(laser.Duration);
            laser.Stop();
            ship.AudioPlayer.Stop();
            GameObject.Destroy(laser.gameObject);

            if (applyDamage) {
                /* Compute the amount of damage to do and push that to the server. */
                // TODO: remove magic numbers.
                // Hull temperature affects max power.
                var temperatureScalar = StatsHelper.EfficiencyAtTemperature(ship.HullTemperature, MaxTemperature);

                // Effect of distance.
                var distance = Mathf.Max(Vector3.Distance(sourcePoint, targetPoint), 0.1f);
                float distanceScalar;
                if (distance < Range) {
                    // Between 1 and 0.8
                    distanceScalar = 1f - (0.2f * distance / Range);
                }
                else {
                    // Sharp falloff outside of range.
                    distanceScalar = 0.8f * Range / distance;
                }

                // Compute thermal damage.
                var dT = ship.LuckRoll * GetFloat("ThermalDamage");

                // Total.
                var damage = MaxDamage * ship.LuckRoll * distanceScalar * temperatureScalar;

                // Push to server.
                var gameController = Util.GetGameController();
                gameController.ApplyDamage(ship.ID, target.ID, damage, dT);
            }
        }

        private int _layerMask;
    }
}