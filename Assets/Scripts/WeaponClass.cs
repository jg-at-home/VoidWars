using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Types of weapon.
    /// </summary>
    public enum WeaponType {
        None = 0,
        Laser = 1,
        RailGun = 2,
        HomingMissile = 4,
        EMP = 8,
        UVLaser = 16,
        ProximityMine = 32,

        // More above here.

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
        public int MaxDamage;

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
    }
}