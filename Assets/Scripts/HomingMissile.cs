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
            DurationTurns = GetInt("DurationTurns");
        }

        protected override IEnumerator attack(ShipController ship, int slot, ShipController target, bool server) {
            ship.AudioPlayer.PlayOneShot(SoundEffect);
            if (server) {
                var node = ship.GetWeaponNode(slot);
                var missileGO = Object.Instantiate(Prefab, node.transform.position, Quaternion.identity);
                var missile = missileGO.GetComponent<HomingMissile>();
                missile.Initialize(this, node, ship.ID, target.ID);
                NetworkServer.Spawn(missileGO);
                yield return missile.Launch();
            }                
        }

        [Stat]
        public int DurationTurns {
            get { return (int)getValue("DurationTurns"); }
            set { setValue("DurationTurns", value); }
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

        [SerializeField] private GameObject _explosionPrefab;
        [SerializeField] private GameObject _expiryPrefab;
        [SerializeField] private TargetIndicator _targetIndicatorPrefab;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
            _targetIndicator = GetComponentInChildren<TargetIndicator>();
        }

        [Server]
        public void Initialize(HomingMissileLauncher launcher, Transform node, int ownerID, int targetID) {
            // Set up other parameters.
            _launcher = launcher;
            _ownerID = ownerID;
            _targetID = targetID;
            HasExpired = false;
            _velocity = node.forward * Speed; ;
            _turnCounter = launcher.DurationTurns;
            _distancePerTurn = launcher.GetFloat("DistancePerTurn");
            _state = State.Idle;

            // Add to game controller so we can be serviced.
            controller.AddNPC(this);
        }

        public override void OnStartClient() {
            // Set particle color. Don't use full-range RGB as it will lose the white core effect.
            var markerColor = controller.GetShipMarkerColor(_ownerID);
            var particleSystem = GetComponentInChildren<ParticleSystem>().main;
            var tint = markerColor;
            tint.a = 0.5f;
            particleSystem.startColor = tint;

            _targetIndicator = Instantiate(_targetIndicatorPrefab);
            var source = gameObject;
            var target = controller.GetShip(_targetID);
            _targetIndicator.Initialize(source.gameObject, target.gameObject, markerColor);
        }

        [Server]
        public IEnumerator Launch() {
            // TODO: launch sounds.
            _state = State.Launch;
            var distance = 0f;
            var start = _rb.position;
            _rb.velocity = _velocity;
            while (distance < _distancePerTurn) {
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
            RpcSetLineBrightness(0.25f);
        }

        [ClientRpc]
        void RpcSetLineBrightness(float value) {
            _targetIndicator.SetBrightness(value);
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
                    var launchShip = controller.GetShip(_ownerID);
                    var damage = launchShip.LuckRoll * _launcher.MaxDamage;
                    foreach (var ship in toDamage) {
                        controller.ApplyDamageToShip(ship.ID, damage, 0f);
                    }

                    // Create an explosion.
                    RpcCreateExplosion(_rb.position);
                    expire(false);
                }
                else {
                    expire(true);
                }
            }

            return collided;
        }

        [ClientRpc]
        void RpcCreateExplosion(Vector3 position) {
            var explosion = Instantiate(_explosionPrefab, position, Quaternion.identity);
            Destroy(explosion, 3f);
            Destroy(_targetIndicator.gameObject);
        }

        [ClientRpc]
        void RpcCreateExpiryEffect(Vector3 position) {
            var effect = Instantiate(_expiryPrefab, position, Quaternion.identity);
            Destroy(effect, 3f);
            Destroy(_targetIndicator.gameObject);
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

            RpcSetLineBrightness(1f);
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
                if (target != null && !target.IsCloaked) {
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

            RpcSetLineBrightness(0.25f);
            _rb.velocity = Vector3.zero;
            syncToken.Sync();
        }

        private void LateUpdate() {
            if (_state == State.Launch || _state == State.Flight) {
                var target = controller.GetShip(_targetID);
                if (target != null) {
                    _targetIndicator.Rebuild(_rb.position, target.transform.position);
                }
            }
        }

        private void expire(bool showEffect) {
            if (showEffect) {
                RpcCreateExpiryEffect(_rb.position);
            }
            _state = State.Expired;
            HasExpired = true;
            gameObject.SetActive(false);
        }

        private enum State {
            Idle,
            Launch,
            Flight,
            Expired
        }

        [SyncVar] private State _state;
        private int _turnCounter;
        private Vector3 _velocity;
        [SyncVar] private int _ownerID;
        [SyncVar] private int _targetID;
        private float _distancePerTurn;
        private HomingMissileLauncher _launcher;
        private Rigidbody _rb;
        private TargetIndicator _targetIndicator;
    }
}