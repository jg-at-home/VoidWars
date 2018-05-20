using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

namespace VoidWars {
    public class MineLauncher : AuxItem {
        public MineLauncher(AuxiliaryClass auxClass) : base(auxClass) {
            DurationTurns = GetInt("DurationTurns");
            MaxDamage = GetFloat("MaxDamage");
        }

        public override IEnumerator Use(ShipController ship, Action onCompletion) {
            var tx = ship.RearNode.transform;
            var mineGO = GameObject.Instantiate(Class.EffectPrefab, ship.RearNode.position, Quaternion.identity);
            var mine = mineGO.GetComponent<ProximityMine>();
            mine.Initialize(this, ship, tx.forward);
            NetworkServer.Spawn(mineGO);
            yield return mine.Launch();
            onCompletion();
        }

        [Stat]
        public int DurationTurns {
            get { return (int)getValue("DurationTurns"); }
            set { setValue("DurationTurns", value); }
        }

        [Stat]
        public float MaxDamage {
            get { return (int)getValue("MaxDamage"); }
            set { setValue("MaxDamage", value); }
        }
    }

    public class ProximityMine : NPCObject {
        public float DeploymentSpeed = 3f;
        public float DeploymentDistance = 3.5f;
        public GameObject ExplosionPrefab;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
        }

        [Server]
        public void Initialize(MineLauncher launcher, ShipController launchingShip, Vector3 direction) {
            _launcher = launcher;
            _launchingShip = launchingShip;
            _turnCounter = launcher.DurationTurns;
            _deployed = false;
            _direction = direction;
        }

        [Server]
        public IEnumerator Launch() {
            // Run for the deployment distance.
            var startPosition = _rb.position;
            _rb.velocity = DeploymentSpeed * _direction;
            while (Vector3.Distance(_rb.position, startPosition) < DeploymentDistance) {
                yield return null;
            }

            _rb.velocity = Vector3.zero;
            _deployed = true;
        }

        [Server]
        public override IEnumerator PerTurnUpdate(NPCSyncToken syncToken) {
            if (_turnCounter > 0) {
                --_turnCounter;
                if (_turnCounter == 0) {
                    HasExpired = true;
                }
            }

            syncToken.Sync();
            yield break;
        }

        private void OnTriggerEnter(Collider other) {
            if (isServer) {
                if (_deployed)  {
                    var ship = other.gameObject.GetComponent<ShipController>();
                    if (ship != null) {
                        var explosion = Instantiate(ExplosionPrefab, _rb.position, Quaternion.identity);
                        NetworkServer.Spawn(explosion);
                        Destroy(explosion, 3f);
                        Destroy(gameObject);
                        var damage = _launchingShip.LuckRoll * _launcher.MaxDamage;
                        controller.ApplyDamageToShip(ship.ID, damage, 0f);
                    }
                }
            }
        }

        private MineLauncher _launcher;
        private ShipController _launchingShip;
        private int _turnCounter;
        private Rigidbody _rb;
        private bool _deployed;
        private Vector3 _direction;
    }
}