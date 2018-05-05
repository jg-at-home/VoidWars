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
        EMP,
        UVLaser,
        ProximityMine,

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

        [Tooltip("The noise to make when invoked")]
        public AudioClip SoundEffect;

        [Tooltip("Metadata for the weapon type encoded as a string")]
        public string MetaData;

        [Tooltip("Prefab for the weapon or projectile")]
        public GameObject Prefab;
    }

    public class WeaponInstance : ItemInstance{
        public WeaponInstance(WeaponClass weaponClass) : base(weaponClass) { 
            _class = weaponClass;
            MaxDamage = _class.MaxDamage;
        }

        [Stat]
        public float MaxDamage {
            get { return getValue("MaxDamage"); }
            set { setValue("MaxDamage", value); }
        }

        [Stat]
        public float Range {
            get { return getValue("Range"); }
            set { setValue("MaxDamage", value); }
        }

        [Stat]
        public float PrimaryAngle {
            get { return getValue("PrimaryAngle"); }
            set { setValue("PrimaryAngle", value); }
        }

        [Stat]
        public float SecondaryAngle {
            get { return getValue("SecondaryAngle"); }
            set { setValue("SecondaryAngle", value); }
        }

        public bool RequiresLineOfSight {  get { return _class.RequiresLineOfSight; } }
        public AudioClip SoundEffect {  get { return _class.SoundEffect; } }
        public string MetaData { get { return _class.MetaData; } }
        public GameObject Prefab {  get { return _class.Prefab; } }

        private readonly WeaponClass _class;
    }
}