using UnityEngine;
using System.Collections;
using System;

namespace VoidWars {
    /// <summary>
    /// ElectroMagnetic Pulse weapon.
    /// </summary>
    public class EMPGenerator : TransientItem {
        /// <summary>
        /// Constructs an instance.
        /// </summary>
        /// <param name="weaponClass">Weapon data.</param>
        public EMPGenerator(AuxiliaryClass itemClass) : base(itemClass) {
            Range = GetFloat("Range");
            _effectDuration = GetFloat("EffectDuration");
            var aoLayerMask = 1 << LayerMask.NameToLayer("ActiveObjects");
            var shipLayerMask = 1 << LayerMask.NameToLayer("Ships");
            _layerMask = aoLayerMask | shipLayerMask;
        }

        [Stat]
        public float Range {
            get { return getValue("Range"); }
            set { setValue("Range", value); }
        }

        public void ApplyEffects(ShipController ship) {
            var radius = Range * ship.LuckRoll;
            var overlaps = Physics.OverlapSphere(ship.transform.position, radius, _layerMask);
            for (int i = 0; i < overlaps.Length; ++i) {
                var gob = overlaps[i].gameObject;
                if (!ReferenceEquals(gob, ship.gameObject)) {
                    // TODO: dice roll.
                    gob.SendMessage("RespondToEmp", DurationTurns, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        public override IEnumerator Use(ShipController ship, Action onCompletion) {
            ship.AudioPlayer.PlayOneShot(StartAudio);
            var effect = GameObject.Instantiate(EffectPrefab, ship.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(_effectDuration);
            GameObject.Destroy(effect);
            onCompletion();
        }

        private float _effectDuration;
        private int _layerMask;
    }
}