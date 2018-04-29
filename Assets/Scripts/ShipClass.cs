using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Static ship descriptor data.
    /// </summary>
    [CreateAssetMenu(menuName = "VoidWars/Ship Class")]
    public class ShipClass : ItemClass {
        [Tooltip("Who the ship class belongs to")]
        public Species Species;

        [Tooltip("The name of the associated model")]
        public string ModelName;

        [Tooltip("If true, supports a secondary weapon")]
        public bool HasSecondaryWeaponSlot;

        [Tooltip("If true, can move backwards")]
        public bool HasReverseThrust;

        [Tooltip("Maximum health value.")]
        public float MaxHealth;

        [Tooltip("The maximum amount of fuel the ship can hold")]
        public float MaxFuelLevel;

        [Tooltip("How many additional components can be added.")]
        public int NumAuxiliarySlots;

        [Tooltip("If true, ship supports stealth mode")]
        public bool Stealth;

        [Tooltip("The maximum (forward) speed of the ship. ")]
        public float MaxSpeed;

        [Tooltip("The number of actions the ship can take per turn")]
        public int ActionsPerTurn;

        /// <summary>
        /// 1: Forward, gentle turns
        /// 2: Forward, gentle turns, sharp turns
        /// 3: Forward, gentle turns, sharp turns, come about
        /// </summary>
        [Tooltip("How maneuverable the ship is: 1-3")]
        [Range(1, 3)]
        public int Maneuverability;

        [Header("Energy")]
        [Tooltip("Starting / max energy of the ship")]
        public float MaxEnergy;

        [Tooltip("Move drain rate")]
        public float MoveDrainRate;

        [Tooltip("Life support drain rate")]
        public float LifeSupportDrainRate;

        [Tooltip("Shield drain rate (when active)")]
        public float ShieldDrainRate;

        [Tooltip("Recharge rate")]
        public float RechargeRate;

        [Tooltip("Cooling rate")]
        public float CoolingRate;

        [Header("Sound effects")]
        public AudioClip EnginesClip;
        public AudioClip ShieldsClip;

        [Header("Visual Effects")]
        /// <summary>
        /// Effects.
        /// </summary>
        public Material CloakEffect;
        public Material ShieldEffect;
    }
}