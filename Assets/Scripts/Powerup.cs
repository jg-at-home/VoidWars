using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    /// <summary>
    /// An ability conferred to a ship via a powerup. Server-side record.
    /// </summary>
    public class PowerupAbilityServer {
        public readonly string PowerupID;

        public bool HasExpired { get; private set; }

        public PowerupAbilityServer(string powerupID, PowerupMode mode, int turnLimit) {
            PowerupID = powerupID;
            _mode = mode;
            _turnCounter = turnLimit;
            HasExpired = false;
        }

        public void PerTurnUpdate() {
            if (_mode == PowerupMode.TurnLimited) {
                if (_turnCounter == 0) {
                    HasExpired = true;
                }
                else {
                    --_turnCounter;
                }
            }
        }

        private int _turnCounter;
        private PowerupMode _mode;
    }

    /// <summary>
    /// A powerup object.
    /// </summary>
    public class Powerup : VoidWarsObject {
        [SyncVar, HideInInspector]
        public string PowerupID;

        [Tooltip("Prefab for VFX / sound on collection")]
        public GameObject CollectEffectPrefab;

        public override void OnStartClient() {
            base.OnStartClient();

            _info = controller.GetPowerupInfo(PowerupID);
        }

        private void OnTriggerEnter(Collider other) {
            if (isServer) {
                var ship = other.gameObject.GetComponent<ShipController>();
                if (ship != null) {
                    Debug.Log("Pickup collected: " + _info.CollectAction);

                    // Run collection effect on clients.
                    RpcCollectionEffect(transform.position);

                    // Do what we need to.
                    onCollection(ship);

                    // I'm done. Hacky delay to wait for RPCs to complete first.
                    Invoke("kill", 0.1f);
                }
            }
        }

        private void kill() {
            NetworkServer.Destroy(gameObject);
        }

        [ClientRpc]
        void RpcCollectionEffect(Vector3 position) {
            Instantiate(CollectEffectPrefab, position, Quaternion.identity);
        }

        private void onCollection(ShipController ship) {
            switch(_info.Mode) {
                case PowerupMode.Instantaneous:
                    // Has an immediate effect and then vanishes.
                    ship.ExecuteCommand(_info.CollectAction);
                    break;

                case PowerupMode.Cachable:
                case PowerupMode.TurnLimited: {
                        // The collecting ship can keep hold of the conferred ability.
                        ship.AddPowerupAbility(_info);
                        var msg = string.Format("Acquired powerup <color=orange>{0}</color>", ToString());
                        ship.ShowMessage(msg, Role.Captain);
                    }
                    break;

            }
        }

        public override string ToString() {
            if (_info.Mode == PowerupMode.TurnLimited) {
                return string.Format("{0} ({1})", _info.Name, _info.TurnLimit);
            }
            else {
                return _info.Name;
            }
        }

        private PowerupInfo _info;
    }
}