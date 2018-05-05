using UnityEngine;
using System.Collections;

namespace VoidWars {
    public class RailGun : WeaponInstance {
        public RailGun(WeaponClass weaponClass) : base(weaponClass) {

        }

        public override IEnumerator Attack(ShipController ship, int slot, ShipController target, bool applyDamage) {
            ship.AudioPlayer.PlayOneShot(SoundEffect);
            var node = ship.GetWeaponNode(slot);
            var start = node.position;
            var projectile = Object.Instantiate(Prefab, start, Quaternion.identity);
            var rb = projectile.GetComponent<Rigidbody>();
            var controller = projectile.GetComponent<ProjectileController>();
            controller.CollisionHandler = onShipProjectileCollision;
            controller.SourceShip = ship.gameObject;

            // Depending on the accuracy of the source ship, the defensive stats of the target, and (TODO) crew buffs,
            // determine if we'll hit or not and create a suitable trajectory.
            var targetSize = target.GetComponentInChildren<Renderer>().bounds.size;
            var targetRadius = Mathf.Max(targetSize.x, targetSize.y, targetSize.z);
            var range = 2f*targetRadius * (1.1f - Accuracy);
            var delta = Statistics.RandomNormal(0f, range);
            var directionToTarget = (target.transform.position - start).normalized;
            var radial = Vector3.Cross(directionToTarget, Vector3.up);
            var targetPos = target.transform.position + radial * delta;

            // Set the projectile on its way. If it collides, do some damage. 
            var direction = (targetPos - start).normalized;
            var speed = GetFloat("Speed");
            rb.velocity = direction * speed;

            _collided = false;
            while (!_collided) {
                if (Vector3.Distance(rb.position, start) > Range) {
                    // Hasn't hit. We're done.
                    break;
                }
                yield return null;
            }

            // TODO: explosion.
            Object.Destroy(projectile);
            if (_collided && applyDamage) {
                // Compute damage.
            }
        }

        private void onShipProjectileCollision(GameObject collider) {
            Debug.Log("Projectile-ship collision!");
            _collided = true;
        }

        private bool _collided;
    }
}