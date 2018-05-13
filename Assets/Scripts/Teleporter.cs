using UnityEngine;
using System.Collections;
using System;

namespace VoidWars {
    /// <summary>
    /// Equipment used to teleport a ship to one of the warp points (usually the safest).
    /// </summary>
    public class Teleporter : AuxItem {
        public Teleporter(AuxiliaryClass itemClass) : base(itemClass) {
        }

        public void SetDestination(Vector3 destination) {
            _destination = destination;
        }

        public override IEnumerator Use(ShipController ship, Action onCompletion) {
            // Do the effect at the current position.
            ship.AudioPlayer.PlayOneShot(StartAudio);
            GameObject.Instantiate(EffectPrefab, ship.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);

            // Hide the ship.
            ship.Hide();

            // Move to the destination.
            ship.transform.position = _destination;

            // Once again play the effect.
            ship.AudioPlayer.PlayOneShot(StopAudio);
            GameObject.Instantiate(EffectPrefab, ship.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);

            // Unhide the ship.
            ship.Show();
            yield return new WaitForSeconds(1f);

            onCompletion();
        }

        private Vector3 _destination;
    }
}