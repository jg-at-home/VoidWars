using UnityEngine;
using System.Collections;

namespace VoidWars { 
    /// <summary>
    /// Helper class for laser attacks. Yes, it ought to be more OO. It's in the backlog.
    /// </summary>
    public static class Weapons {
        private const float EmpDuration = 2f;

        /// <summary>
        /// Performs a laser attack.
        /// </summary>
        /// <param name="sourcePoint">The emission point.</param>
        /// <param name="weaponClass">The weapon being used.</param>
        /// <param name="source">The ship firing.</param>
        /// <param name="target">The ship getting burnt.</param>
        /// <returns>Enumerator</returns>
        public static IEnumerator LaserAttack(Vector3 sourcePoint, WeaponClass weaponClass, ShipController source, ShipController target, bool applyDamage) {
            var gameController = Util.GetGameController();
            var laserGob = Object.Instantiate(gameController.LaserPrefab);
            var laser = laserGob.GetComponent<LaserController>();
            var targetPoint = target.gameObject.transform.position;
            var direction = (targetPoint - sourcePoint).normalized;
            RaycastHit hit;
            var ray = new Ray(sourcePoint, direction);
            var layer = 1 << LayerMask.NameToLayer("ActiveObjects");
            Physics.Raycast(ray, out hit, layer);
            var color = (weaponClass.WeaponType == WeaponType.Laser) ? Color.red : Color.magenta;
            laser.Run(color, sourcePoint, hit.point);
            source.AudioPlayer.PlayOneShot(weaponClass.SoundEffect);
            yield return new WaitForSeconds(laser.Duration);            
            laser.Stop();
            source.AudioPlayer.Stop();

            if (applyDamage) {
                /* Compute the amount of damage to do and push that to the server. */
                // TODO: remove magic numbers.
                // Hull temperature affects max power.
                var temperatureScalar = Stats.EfficiencyAtTemperature(source.HullTemperature, weaponClass.MaxTemperature);

                // Effect of distance.
                var distance = Mathf.Max(Vector3.Distance(sourcePoint, targetPoint), 0.1f);
                float distanceScalar;
                if (distance < weaponClass.Range) {
                    // Between 1 and 0.8
                    distanceScalar = 1f - (0.2f * distance / weaponClass.Range);
                }
                else {
                    // Sharp falloff outside of range.
                    distanceScalar = 0.8f * weaponClass.Range / distance;
                }

                // Small amount of luck
                var luckFactor = Random.Range(0.95f, 1.05f);

                // Compute thermal damage.
                const float ThermalDamage = 10f;
                var dT = luckFactor * ThermalDamage;

                // Total.
                var damage = weaponClass.MaxDamage * luckFactor * distanceScalar * temperatureScalar;

                // Push to server.               
                gameController.ApplyDamageToShip(target.ID, damage, dT);
            }
        }

        /// <summary>
        /// EMP attack.
        /// </summary>
        /// <param name="source">The ship generating the pulse.</param>
        /// <param name="empClass">Weapon data.</param>
        /// <param name="applyDamage">If true, apply damage.</param>
        /// <returns></returns>
        public static IEnumerator EmpAttack(ShipController source, WeaponClass empClass, bool applyDamage) {
            source.AudioPlayer.PlayOneShot(empClass.SoundEffect);
            var effect = Object.Instantiate(source.ShipClass.EmpPrefab, source.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(EmpDuration);
            Object.Destroy(effect);
            if (applyDamage) {
                var gameController = Util.GetGameController();
                var layerMask = 1 << LayerMask.NameToLayer("ActiveObjects");
                var overlaps = Physics.OverlapSphere(source.transform.position, empClass.Range, layerMask);
                // TODO: randomise a bit.
                var empData = int.Parse(empClass.MetaData); 
                for (int i = 0; i < overlaps.Length; ++i) {
                    var gob = overlaps[i].gameObject;
                    if (!ReferenceEquals(gob, source.gameObject)) {
                        gob.SendMessage("RespondToEmp", empData, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }
    }
}