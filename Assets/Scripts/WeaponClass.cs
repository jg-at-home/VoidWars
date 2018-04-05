using UnityEngine;

namespace VoidWars {
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

    public enum WeaponCardinality {
        Primary,
        Secondary,
        Any
    }

    [CreateAssetMenu(menuName ="VoidWars/Weapon")]
    public class WeaponClass : ScriptableObject {
        [Tooltip("The name of the weapon")]
        public string Name;

        [Tooltip("Description of the weapon")]
        public string Description;

        [Tooltip("The type of the weapon")]
        public WeaponType WeaponType;

        [Tooltip("Whether it's a primary, secondary or either.")]
        public WeaponCardinality Cardinality;

        [Tooltip("The max damage the weapon can do")]
        public int MaxDamage;

        [Tooltip("How much power it takes to use the weapon.")]
        public int PowerUsage;

        [Tooltip("How much mass the weapon adds")]
        public int Mass;

        [Tooltip("Nominal cost of the item")]
        public int Cost;

        [Tooltip("How far away the weapon is effective")]
        public float Range;

        [Tooltip("What angle the weapon can operate over as a primary")]
        public float PrimaryAngle;

        [Tooltip("What angle the weapon can operate over as a secondary")]
        public float SecondaryAngle;
    }
}