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

        protected override IEnumerator attack(ShipController ship, int slot, VoidWarsObject target, bool server) {
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
    public class HomingMissile : TransientNPC {
        [Tooltip("How sharp turns are")]
        public float DirectionSmoothing;

        [Tooltip("Collision radius")]
        public float Radius;

        [Tooltip("Travel speed")]
        public float Speed;

        [Tooltip("The layers the missile looks for collisions in.")]
        public LayerMask CollisionLayers;

        [SerializeField] private GameObject _explosionPrefab;
        [SerializeField] private GameObject _expiryPrefab;
        [SerializeField] private TargetIndicator _targetIndicatorPrefab;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
            _targetIndicator = GetComponentInChildren<TargetIndicator>();
        }

        [Server]
        public void Initialize(HomingMissileLauncher launcher, Transform node, int ownerID, int targetID) {
            setLifetime(launcher.DurationTurns);
            _launcher = launcher;
            _ownerID = ownerID;
            _targetID = targetID;
            HasExpired = false;
            _velocity = node.forward * Speed; ;
            _distancePerTurn = launcher.GetFloat("DistancePerTurn");
            _state = State.Idle;
            _targetInstanceId = NetworkInstanceId.Invalid;
        }

        public override void OnStartClient() {
            base.OnStartClient();

            // Set particle color. Don't use full-range RGB as it will lose the white core effect.
            var markerColor = controller.GetShipMarkerColor(_ownerID);
            var particleSystem = GetComponentInChildren<ParticleSystem>().main;
            var tint = markerColor;
            tint.a = 0.5f;
            particleSystem.startColor = tint;

            _targetIndicator = Instantiate(_targetIndicatorPrefab);
            var source = gameObject;
            var target = controller.GetObjectWithID(_targetID);
            _targetPosition = target.transform.position;
            _targetIndicator.Initialize(source.transform.position, _targetPosition, markerColor);
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
            var toDamage = new List<VoidWarsObject>();
            var colliders = Physics.OverlapSphere(transform.position, Radius, CollisionLayers);
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
                else if (collider.gameObject.CompareTag("Targetable")) {
                    collided = true;
                    toDamage.Add(collider.gameObject.GetComponent<VoidWarsObject>());
                }
                else if (collider.gameObject.CompareTag("Sun")) {
                    collided = true;
                }
                else if (collider.gameObject.CompareTag("Chaff")) {
                    collided = true;
                }
            }

            // If I hit one or more ships, I explode. Otherwise I fizzle out.
            if (collided) {
                if (toDamage.Count > 0) {
                    // I hit one or more ships.
                    var launchShip = controller.GetShip(_ownerID);
                    var damage = launchShip.LuckRoll * _launcher.MaxDamage;
                    foreach (var obj in toDamage) {
                        controller.ApplyDamage(obj.ID, damage, 0f);
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
            if (updateTurnCount()) { 
                syncToken.Sync();
                expire(true);
                yield break;
            }

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

                var myPosition = _rb.position;
                var target = computeTarget(myPosition);
                if (target != null) {
                    // Compute the next move segment and smooth direction changes so we don't veer too sharply.
                    var myDir = _velocity.normalized;
                    var targetDir = (target.transform.position - myPosition).normalized;
                    var newDir = Vector3.Lerp(myDir, targetDir, DirectionSmoothing);
                    _velocity = newDir.normalized * Speed;
                    _targetInstanceId = target.GetComponent<NetworkIdentity>().netId;
                    _targetPosition = target.transform.position;
                }
                else {
                    _targetInstanceId = NetworkInstanceId.Invalid;
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

        private GameObject computeTarget(Vector3 myPos) {
            // Check for chaff.
            var myPosition = _rb.position;
            var objectsInRange = Physics.OverlapSphere(myPosition, _launcher.Range, CollisionLayers);
            foreach (var obj in objectsInRange) {
                if (obj.gameObject.CompareTag("Chaff")) {
                    // TODO: check that chaff is in the view cone.
                    return obj.gameObject;
                }
            }

            var target = controller.GetShip(_targetID);
            if (target != null && !target.IsCloaked) {
                return target.gameObject;
            }

            return null;
        }

        private void onTargetChanged(NetworkInstanceId targetId) {
            if (targetId != _targetInstanceId) {
                _targetInstanceId = targetId;
                if (targetId != NetworkInstanceId.Invalid) {
                    _target = ClientScene.FindLocalObject(targetId);
                }
                else {
                    _target = null;
                }
            }
        }

        private void LateUpdate() {
            if (_state == State.Launch || _state == State.Flight) {
                var targetPos = _targetPosition;
                if (_target != null) {
                    targetPos = _target.transform.position;
                }

                _targetIndicator.Rebuild(_rb.position, targetPos);
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
        private Vector3 _velocity;
        [SyncVar] private int _ownerID;
        [SyncVar] private int _targetID;
        private float _distancePerTurn;
        private HomingMissileLauncher _launcher;
        private Rigidbody _rb;
        private TargetIndicator _targetIndicator;
        private GameObject _target;
        [SyncVar(hook ="onTargetChanged")] private NetworkInstanceId _targetInstanceId;
        [SyncVar] private Vector3 _targetPosition;
    }
}