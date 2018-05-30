using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

namespace VoidWars {
    public class MineLauncher : TransientItem {
        public MineLauncher(AuxiliaryClass auxClass) : base(auxClass) {
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
        public float MaxDamage {
            get { return (int)getValue("MaxDamage"); }
            set { setValue("MaxDamage", value); }
        }
    }

    public class ProximityMine : TransientNPC {
        public float DeploymentSpeed = 3f;
        public float DeploymentDistance = 3.5f;
        public GameObject ExplosionPrefab;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
        }

        [Server]
        public void Initialize(MineLauncher launcher, ShipController launchingShip, Vector3 direction) {
            setLifetime(launcher.DurationTurns);
            _launcher = launcher;
            _launchingShip = launchingShip;
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

        public override float ComputeDamage(VoidWarsObject source, float damage, float dT) {
            if (damage > 0f) {
                explode();
            }

            return base.ComputeDamage(source, damage, dT);
        }

        private void OnTriggerEnter(Collider other) {
            if (isServer) {
                if (_deployed)  {
                    var ship = other.gameObject.GetComponent<ShipController>();
                    if (ship != null) {
                        explode();
                        var damage = _launchingShip.LuckRoll * _launcher.MaxDamage;
                        controller.ApplyDamage(_launchingShip.ID, ship.ID, damage, 0f);
                    }
                }
            }
        }

        [Server]
        private void explode() {
            var explosion = Instantiate(ExplosionPrefab, _rb.position, Quaternion.identity);
            NetworkServer.Spawn(explosion);
            Destroy(explosion, 3f);
            NetworkServer.Destroy(gameObject);
        }

        private MineLauncher _launcher;
        private ShipController _launchingShip;
        private Rigidbody _rb;
        private bool _deployed;
        private Vector3 _direction;
    }
}