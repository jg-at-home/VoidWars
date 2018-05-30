using UnityEngine;
using System.Collections;

namespace VoidWars {
    /// <summary>
    /// Projectile weapon. Looks and sounds oddly familiar in some respects.
    /// </summary>
    public class RailGun : WeaponInstance {
        /// <summary>
        /// Constructs an instance.
        /// </summary>
        /// <param name="weaponClass">Weapon data.</param>
        public RailGun(WeaponClass weaponClass) : base(weaponClass) {
            if (s_explosionPrefab == null) {
                s_explosionPrefab = (GameObject)Resources.Load("Prefabs/FX/" + GetString("ExplosionPrefab"));
            }
        }

        protected override IEnumerator attack(ShipController ship, int slot, VoidWarsObject target, bool applyDamage) {
            ship.AudioPlayer.PlayOneShot(SoundEffect);
            var node = ship.GetWeaponNode(slot);
            var start = node.position;
            var projectile = Object.Instantiate(Prefab, start, Quaternion.identity);
            var rb = projectile.GetComponent<Rigidbody>();
            var controller = projectile.GetComponent<ProjectileController>();
            controller.CollisionHandler = onProjectileCollision;
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
            while (!_collided && (Vector3.Distance(rb.position, start) <= Range)) {
                yield return null;
            }

            Object.Destroy(projectile);
            if (_collided && applyDamage) {
                var explosion = Object.Instantiate(s_explosionPrefab, _collisionPoint, Quaternion.identity);
                Object.Destroy(explosion, 3f);

                // Compute damage.                
                var damage = ship.LuckRoll * MaxDamage;

                // Push to clients.
                var gameController = Util.GetGameController();
                gameController.ApplyDamage(ship.ID, target.ID, damage, 0f);
            }
        }

        private void onProjectileCollision(GameObject collider, Vector3 position) {
            _collided = true;
            _collisionPoint = position;
        }

        private bool _collided;
        private Vector3 _collisionPoint;
        private static GameObject s_explosionPrefab;
    }
}