﻿using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Launches a flare. Will briefly highlight any cloaked, I mean stealthed, ships in the vicinity.
    /// </summary>
    public class FlareLauncher : AuxItem {
        public FlareLauncher(AuxiliaryClass itemClass) : base(itemClass) {
        }

        public override void Use(ShipController ship) {
            // Play sound and effect on all clients.
            ship.AudioPlayer.PlayOneShot(StartAudio);
            Object.Instantiate(EffectPrefab, ship.transform.position, Quaternion.identity);

            // On the client of the ship's owner, do the temporary uncloak of enemy ships.
            var controller = Util.GetGameController();
            if (!controller.IsOwner(ship.OwnerID)) {
                return;
            }

            var flashTime = GetFloat("FlashTime");
            var range = GetFloat("Range");
            var enemyShips = controller.GetShipsNotOwnedBy(ship.OwnerID);
            foreach(var enemy in enemyShips) {
                if (enemy.IsCloaked && 
                    (Vector3.Distance(ship.transform.position, enemy.transform.position) < range)) {
                    enemy.FlashCloak(flashTime);
                }
            }
        }
    }
}