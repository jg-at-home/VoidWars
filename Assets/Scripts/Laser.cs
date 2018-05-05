using UnityEngine;
using System.Collections;

namespace VoidWars {
    public class Laser : WeaponInstance {
        public Laser(WeaponClass weaponClass) : base(weaponClass) {
        }

        public override IEnumerator Attack(ShipController ship, int slot, ShipController target, bool applyDamage) {
            var sourceNode = ship.GetWeaponNode(slot);
            var sourcePoint = sourceNode.position;
            var laserGob = Object.Instantiate(Prefab);
            var laser = laserGob.GetComponent<LaserController>();
            var targetPoint = target.gameObject.transform.position;
            var direction = (targetPoint - sourcePoint).normalized;
            RaycastHit hit;
            var ray = new Ray(sourcePoint, direction);
            var layer = 1 << LayerMask.NameToLayer("ActiveObjects");
            Physics.Raycast(ray, out hit, layer);
            var color = (WeaponType == WeaponType.Laser) ? Color.red : Color.magenta;
            laser.Run(color, sourcePoint, hit.point);
            ship.AudioPlayer.PlayOneShot(SoundEffect);
            yield return new WaitForSeconds(laser.Duration);
            laser.Stop();
            ship.AudioPlayer.Stop();

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

                // Small amount of luck
                var luckFactor = Random.Range(0.95f, 1.05f);

                // Compute thermal damage.
                var dT = luckFactor * GetFloat("ThermalDamage");

                // Total.
                var damage = MaxDamage * luckFactor * distanceScalar * temperatureScalar;

                // Push to server.               
                var gameController = Util.GetGameController();
                gameController.ApplyDamageToShip(target.ID, damage, dT);
            }
        }
    }
}