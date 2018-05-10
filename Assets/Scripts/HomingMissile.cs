using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace VoidWars {
    /// <summary>
    /// Homing missile class. Actually the launcher is the weapon. The missle in a server-controiled NPC.
    /// </summary>
    public class HomingMissileLauncher : WeaponInstance {
        public HomingMissileLauncher(WeaponClass weaponClass) : base(weaponClass) {
        }

        public override IEnumerator Attack(ShipController ship, int slot, ShipController target, bool server) {
            if (server) {
                var node = ship.GetWeaponNode(slot);
                var missileGO = Object.Instantiate(Prefab, node.transform.position, Quaternion.identity);
                var missile = missileGO.GetComponent<HomingMissile>();
                missile.Initialize(this, node, ship.ID, target.ID);
                NetworkServer.Spawn(missileGO);
                yield return missile.Launch();
            }                
        }
    }

    /// <summary>
    /// NPC homing missile.
    /// </summary>
    public class HomingMissile : NPCObject {
        [Tooltip("How sharp turns are")]
        public float DirectionSmoothing;

        [Tooltip("Collision radius")]
        public float Radius;

        [Tooltip("Travel speed")]
        public float Speed;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
        }

        [Server]
        public void Initialize(HomingMissileLauncher launcher, Transform node, int ownerID, int targetID) {
            // Set particle color. Don't use full-range RGB as it will lose the white core effect.
            var particleSystem = GetComponentInChildren<ParticleSystem>().main;
            var tint = controller.GetShipMarkerColor(ownerID);
            tint.a = 0.5f;
            particleSystem.startColor = tint;

            // Set up other parameters.
            _launcher = launcher;
            _ownerID = ownerID;
            _targetID = targetID;
            HasExpired = false;
            _velocity = node.forward * Speed; ;
            _turnCounter = launcher.GetInt("DurationTurns");
            _distancePerTurn = launcher.GetFloat("DistancePerTurn");
            _state = State.Idle;

            // Add to game controller so we can be serviced.
            controller.AddNPC(this);
        }

        [Server]
        public IEnumerator Launch() {
            _state = State.Launch;
            _rb.velocity = _velocity;
            var distance = 0f;
            var start = _rb.position;
            while(distance < _distancePerTurn) {
                if (checkCollision()) {
                    yield break;
                }
                else {
                    yield return null;
                }
                distance = Vector3.Distance(start, _rb.position);
            }
            _rb.velocity = Vector3.zero;
            _state = State.Flight;
        }

        private bool checkCollision() {
            var toDamage = new List<ShipController>();
            var layerMask = 1 << LayerMask.NameToLayer("ActiveObjects");
            var colliders = Physics.OverlapSphere(transform.position, Radius, layerMask);
            bool collided = false;
            foreach(var collider in colliders) {
                // I hit something. Was it a ship?
                var ship = collider.gameObject.GetComponent<ShipController>();
                if (ship != null) {
                    // Ignore self-collision during launch.
                    if (ship.ID == _ownerID && _state == State.Launch) {
                        continue;
                    }

                    collided = true;
                    toDamage.Add(ship);
                }
                else if (collider.gameObject.CompareTag("Sun")) {
                    collided = true;
                }
            }

            // If I hit one or more ships, I explode. Otherwise I fizzle out.
            if (collided) {
                if (toDamage.Count > 0) {
                    // I hit one or more ships.
                    // TODO: compute damage.
                    var damage = 10f;
                    foreach(var ship in toDamage) {
                        controller.ApplyDamageToShip(ship.ID, damage, 0f);
                    }

                    // TODO: explosion.
                    expire(false);
                }
                else {
                    expire(true);
                }
            }

            return collided;
        }

        [Server]
        public override IEnumerator PerTurnUpdate(NPCSyncToken syncToken) {
            // Have I expired? If so, fizzle out.
            if (_turnCounter == 0) {
                syncToken.Sync();
                expire(true);
                yield break;
            }
            --_turnCounter;

            var bounds = controller.GetBoardBounds();
            var distance = 0f;
            var lastPos = _rb.position;
            _rb.velocity = _velocity;
            while (distance < _distancePerTurn) {
                // Am I out of bounds?
                if (!bounds.Contains(_rb.position)) {
                    // Yes, so kill me off.
                    syncToken.Sync();
                    expire(true);
                    yield break;
                }

                // Does my target still exist?
                var target = controller.GetShip(_targetID);
                if (target != null) {
                    // Compute the next move segment and smooth direction changes so we don't veer too sharply.
                    var myPosition = _rb.position;
                    var targetDir = (target.transform.position - myPosition).normalized;
                    var myDir = _velocity.normalized;
                    var newDir = Vector3.Lerp(myDir, targetDir, DirectionSmoothing);
                    _velocity = newDir.normalized * Speed;
                }

                _rb.velocity = _velocity;

                if (checkCollision()) {
                    syncToken.Sync();
                    yield break;
                }

                var step = (_rb.position - lastPos).magnitude;
                lastPos = _rb.position;
                distance += step;
                yield return null;
            }

            _rb.velocity = Vector3.zero;
            syncToken.Sync();
        }

        private void expire(bool showEffect) {
            Debug.Log("Kaboom!");

            _state = State.Expired;
            HasExpired = true;
            gameObject.SetActive(false);
            if (showEffect) {
                // TODO: expiry effect.
            }
        }

        private enum State {
            Idle,
            Launch,
            Flight,
            Expired
        }

        private State _state;
        private int _turnCounter;
        private Vector3 _velocity;
        private int _ownerID;
        private int _targetID;
        private float _distancePerTurn;
        private HomingMissileLauncher _launcher;
        private Rigidbody _rb;
    }
}