using System.Collections;
using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Types of weapon.
    /// </summary>
    public enum WeaponType {
        None,
        Laser,
        RailGun,
        HomingMissile,
        UVLaser,

        // More above here.
        NumTypes
    }

    /// <summary>
    /// Which slot a weapon can attach to.
    /// </summary>
    public enum WeaponCardinality {
        Primary,
        Secondary,
        Any
    }

    /// <summary>
    /// Weapon descriptor.
    /// </summary>
    [CreateAssetMenu(menuName ="VoidWars/Weapon")]
    public class WeaponClass : ItemClass {
        [Tooltip("The type of the weapon")]
        public WeaponType WeaponType;

        [Tooltip("Whether it's a primary, secondary or either.")]
        public WeaponCardinality Cardinality;

        [Tooltip("The max damage the weapon can do")]
        public float MaxDamage;

        [Tooltip("How far away the weapon is effective")]
        public float Range;

        [Tooltip("What angle the weapon can operate over as a primary")]
        public float PrimaryAngle;

        [Tooltip("What angle the weapon can operate over as a secondary")]
        public float SecondaryAngle;

        [Tooltip("If true, weapon requires direct line-of-sight to function")]
        public bool RequiresLineOfSight;

        [Tooltip("How accurate the weapon is. YMMV as to what that means")]
        [Range(0f, 1f)]
        public float Accuracy;

        [Tooltip("How many turns before you can use the weapon again")]
        public int TurnsBetweenUses;

        [Tooltip("The noise to make when invoked")]
        public AudioClip SoundEffect;

        [Tooltip("Prefab for the weapon or projectile")]
        public GameObject Prefab;
    }

    public abstract class WeaponInstance : ItemInstance {
        protected WeaponInstance(WeaponClass weaponClass) : base(weaponClass) { 
            _class = weaponClass;
            MaxDamage = _class.MaxDamage;
            Range = _class.Range;
            PrimaryAngle = _class.PrimaryAngle;
            SecondaryAngle = _class.SecondaryAngle;
            Accuracy = _class.Accuracy;
        }

        [Stat]
        public float MaxDamage {
            get { return getValue("MaxDamage"); }
            set { setValue("MaxDamage", value); }
        }

        [Stat]
        public float Range {
            get { return getValue("Range"); }
            set { setValue("Range", value); }
        }

        [Stat(0f, 180f)]
        public float PrimaryAngle {
            get { return getValue("PrimaryAngle"); }
            set { setValue("PrimaryAngle", value); }
        }

        [Stat(0f, 180f)]
        public float SecondaryAngle {
            get { return getValue("SecondaryAngle"); }
            set { setValue("SecondaryAngle", value); }
        }

        [Stat(0f, 1f)]
        public float Accuracy {
            get { return getValue("Accuracy"); }
            set { setValue("Accuracy", value); }
        }

        public bool RequiresLineOfSight {  get { return _class.RequiresLineOfSight; } }
        public AudioClip SoundEffect {  get { return _class.SoundEffect; } }
        public GameObject Prefab {  get { return _class.Prefab; } }
        public WeaponType WeaponType { get { return _class.WeaponType; } }
        public bool IsAvailable {  get { return _turnCounter == 0; } }

        /// <summary>
        /// Called at start of round.
        /// </summary>
        public void BeginRound() {
            if (_turnCounter > 0) {
                --_turnCounter;
            }
        }

        /// <summary>
        /// Fires the weapon.
        /// </summary>
        /// <param name="ship">The firing ship.</param>
        /// <param name="slot">The weapon slot.</param>
        /// <param name="target">The target ship.</param>
        /// <param name="applyDamage">If true, compute damage.</param>
        /// <returns>Enumerator.</returns>
        public IEnumerator Fire(ShipController ship, int slot, VoidWarsObject target, bool applyDamage) {
            _turnCounter = _class.TurnsBetweenUses;
            yield return attack(ship, slot, target, applyDamage);
        }

        protected abstract IEnumerator attack(ShipController ship, int slot, VoidWarsObject target, bool applyDamage);

        private readonly WeaponClass _class;
        private int _turnCounter;
    }
}