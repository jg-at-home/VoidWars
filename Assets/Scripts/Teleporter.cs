using UnityEngine;
using System.Collections;
using System;

namespace VoidWars {
    /// <summary>
    /// Equipment used to teleport a ship to one of the warp points (usually the safest).
    /// </summary>
    public class Teleporter : AuxItem {
        [Tooltip("The layer to check to see if points are occupied")]
        public LayerMask OverlapLayers;

        public Teleporter(AuxiliaryClass itemClass) : base(itemClass) {
            if (s_teleportPoints == null) {
                s_teleportPoints = GameObject.FindGameObjectsWithTag("Teleport");
                s_layerMask = 1 << LayerMask.NameToLayer("Ships");
                s_layerMask |= 1 << LayerMask.NameToLayer("ActiveObjects");
            }
        }

        public override IEnumerator Use(ShipController ship, Action onCompletion) {
            // Do the effect at the current position.
            ship.AudioPlayer.PlayOneShot(StartAudio);
            GameObject.Instantiate(EffectPrefab, ship.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);

            // Hide the ship.
            ship.Hide();

            // Select the point that maximises the distance from enemy ships and is not occupied by
            // another ship.
            var index = selectPosition(ship);
            if (index >= 0) {
                // Jump the ship there
                var bestPoint = s_teleportPoints[index].transform.position;
                bestPoint.y = ship.transform.position.y;
                ship.transform.position = bestPoint;
            }

            // Once again play the effect.
            ship.AudioPlayer.PlayOneShot(StopAudio);
            GameObject.Instantiate(EffectPrefab, ship.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);

            // Unhide the ship.
            ship.Show();
            yield return new WaitForSeconds(1f);

            onCompletion();
        }

        private int selectPosition(ShipController ship) {
            var gameController = Util.GetGameController();
            var enemyShips = gameController.GetShipsNotOwnedBy(ship.OwnerID);
            var bestPoint = -1;
            var bestDistance = 0f;
            for(int i = 0; i < s_teleportPoints.Length; ++i) {
                var point = s_teleportPoints[i].transform.position;
                var distance = 0f;
                foreach(var enemy in enemyShips) {
                    distance += Vector3.Distance(enemy.transform.position, point);
                }

                if (distance > bestDistance) {
                    if (!Physics.CheckSphere(point, 1f, s_layerMask)) {
                        bestDistance = distance;
                        bestPoint = i;
                    }
                }
            }

            return bestPoint;
        }

        private static GameObject[] s_teleportPoints;
        private static int s_layerMask;
    }
}