using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

namespace VoidWars {
    public class ChaffLauncher : TransientItem  {
        public ChaffLauncher(AuxiliaryClass itemClass) : base(itemClass) {
        }

        public override IEnumerator Use(ShipController ship, Action onCompletion) {
            var position = ship.RearNode.transform.position;
            var chaffGO = GameObject.Instantiate(Class.EffectPrefab, position, Quaternion.identity);
            var chaff = chaffGO.GetComponent<Chaff>();
            chaff.Initialize(this, ship.ID, ship.RearNode.transform.forward);
            NetworkServer.Spawn(chaffGO);
            yield return chaff.Launch();
            onCompletion();
        }
    }

    public class Chaff : TransientNPC {
        public GameObject ChaffCloud;
        public GameObject ChaffRocket;
        public float RocketSpeed = 1f;
        public float DeploymentDistance = 3.5f;
        public float CloudRadius = 5f;
        public float ExpansionSpeed = 3.5f;

          public void Initialize(ChaffLauncher launcher, int shipID, Vector3 direction) {
            setLifetime(launcher.DurationTurns);
            _direction = direction;
            _shipID = shipID;
        }

        [Server]
        public IEnumerator Launch() {
            // Run for the deployment distance.
            var startPosition = _rb.position;
            _rb.velocity = RocketSpeed * _direction;
            while(Vector3.Distance(_rb.position, startPosition) < DeploymentDistance) {
                yield return null;
            }

            // Deactivate the rocket and start the cloud, growing it from small to large.
            _rb.velocity = Vector3.zero;
            RpcDeploy();
        }

        [ClientRpc]
        void RpcDeploy() {
            ChaffCloud.SetActive(true);
            ChaffRocket.SetActive(false);
            StartCoroutine(expand(0.1f, CloudRadius, ExpansionSpeed));
        }

        private IEnumerator expand(float startScale, float endScale, float time) {
            var scale = startScale;
            while(scale < endScale) {
                ChaffCloud.transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
                scale += Time.deltaTime;
            }
        }

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
        }

        public override void OnStartClient() {
            var ship = controller.GetShip(_shipID);
            var auxClass = (AuxiliaryClass)controller.GetItemClass(AuxType.ChaffLauncher);
            ship.AudioPlayer.PlayOneShot(auxClass.StartSoundClip);
            ChaffCloud.SetActive(false);
            ChaffRocket.SetActive(true);
        }

        private Rigidbody _rb;
        private Vector3 _direction;
        private int _shipID;
    }
}