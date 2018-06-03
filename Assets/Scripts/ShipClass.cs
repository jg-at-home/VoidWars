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

        [Tooltip("The maximum size of a move in movement units.")]
        public int MaxMoveSize;

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

        [Tooltip("Maximum efficiency of shields")]
        public float MaxShieldEfficiency;

        [Tooltip("Luck factor")]
        public float Luckiness;

        [Tooltip("Damage fraction above which items will start breaking")]
        [Range(0f, 1f)]
        public float DamageThreshold;

        [Tooltip("If damage is over this level, there's a chance the propulsion will suffer damage.")]
        [Range(0f, 1f)]
        public float PropulsionDamageThreshold;

        [Tooltip("The number of turns it takes to repair the engines")]
        [Range(1, 5)]
        public int EngineRepairTurns;

        [Tooltip("Energy cost of engine repair")]
        public float EngineRepairCost;

        [Tooltip("Resistance to EMP")]
        [Range(0.5f, 0.95f)]
        public float EMPResistance;

        [Header("Sound effects")]
        public AudioClip EnginesClip;
        public AudioClip ShieldsClip;
        public AudioClip CloakClip;
        public AudioClip ScannersClip;

        [Header("Visual Effects")]
        public Material CloakEffect;
        public Material ShieldEffect;
        public GameObject EmpPrefab;
    }

    /// <summary>
    /// Buffable ship data.
    /// </summary>
    public class ShipInstance : ItemInstance {
        public ShipInstance(ShipClass shipClass) : base(shipClass) {
            _class = shipClass;
            MaxHealth = _class.MaxHealth;
            MaxSpeed = _class.MaxSpeed;
            MaxEnergy = _class.MaxEnergy;
            MaxShieldEfficiency = _class.MaxShieldEfficiency;
            RechargeRate = _class.RechargeRate;
            CoolingRate = _class.CoolingRate;
            MaxMoveSize = _class.MaxMoveSize;
            Luckiness = _class.Luckiness;
            Maneuverability = _class.Maneuverability;
            DamageThreshold = _class.DamageThreshold;
            EMPResistance = _class.EMPResistance;
            EngineRepairTurns = _class.EngineRepairTurns;
        }

        public Species Species {  get { return _class.Species; } }
        public string ModelName {  get { return _class.ModelName; } }
        public bool HasSecondaryWeaponSlot {  get { return _class.HasSecondaryWeaponSlot; } }
        public bool HasReverseThrust {  get { return _class.HasReverseThrust; } }

        [Stat(0f, float.PositiveInfinity)]
        public float MaxHealth {
            get { return getValue("MaxHealth"); }
            set { setValue("MaxHealth", value); }
        }

        [Stat(0f, float.PositiveInfinity)]
        public float MaxSpeed {
            get { return getValue("MaxSpeed"); }
            set { setValue("MaxSpeed", value); }
        }

        [Stat(0f, float.PositiveInfinity)]
        public float MaxEnergy {
            get { return getValue("MaxEnergy"); }
            set { setValue("MaxEnergy", value); }
        }

        [Stat(0f, 1f)]
        public float MaxShieldEfficiency {
            get { return getValue("MaxShieldEfficiency"); }
            set { setValue("MaxShieldEfficiency", value); }
        }

        [Stat(0f, float.PositiveInfinity)]
        public float RechargeRate  {
            get { return getValue("RechargeRate"); }
            set { setValue("RechargeRate", value); }
        }

        [Stat(0f, float.PositiveInfinity)]
        public float CoolingRate {
            get { return getValue("CoolingRate"); }
            set { setValue("CoolingRate", value); }
        }

        [Stat(0f, float.PositiveInfinity)]
        public int MaxMoveSize {
            get { return (int)getValue("MaxMoveSize"); }
            set { setValue("MaxMoveSize", (float)value); }
        }

        [Stat(0f, 1f)]
        public float Luckiness {
            get { return getValue("Luckiness"); }
            set { setValue("Luckiness", value); }
        }

        [Stat(1f, 3f)]
        public int Maneuverability {
            get { return (int)getValue("Maneuverability"); }
            set { setValue("Maneuverability", value); }
        }

        [Stat(0f, 1f)]
        public float DamageThreshold {
            get { return getValue("DamageThreshold"); }
            set { setValue("DamageThreshold", value); }
        }

        [Stat(0f, 1f)]
        public float EMPResistance {
            get { return getValue("EMPResistance"); }
            set { setValue("EMPResistance", value); }
        }

        [Stat(1f, float.PositiveInfinity)]
        public int EngineRepairTurns {
            get { return (int)getValue("EngineRepairTurns"); }
            set { setValue("EngineRepairTurns", value); }
        }

        public float MaxFuelLevel {  get { return _class.MaxFuelLevel; } }
        public int NumAuxiliarySlots {  get { return _class.NumAuxiliarySlots; } }
        public bool Stealth { get { return _class.Stealth; } }
        public int ActionsPerTurn { get { return _class.ActionsPerTurn; } }
        public float MoveDrainRate { get { return _class.MoveDrainRate; } }
        public float LifeSupportDrainRate {  get { return _class.LifeSupportDrainRate; } }
        public float ShieldDrainRate { get { return _class.ShieldDrainRate; } }
        public float PropulsionDamageThreshold {  get { return _class.PropulsionDamageThreshold; } }
        public float EngineRepairCost {  get { return _class.EngineRepairCost; } }
        public AudioClip EnginesClip { get { return _class.EnginesClip; } }
        public AudioClip ShieldsClip { get { return _class.ShieldsClip; } }
        public AudioClip CloakClip {  get { return _class.CloakClip; } }
        public AudioClip ScannersClip {  get { return _class.ScannersClip; } }
        public Material CloakEffect {  get { return _class.CloakEffect; } }
        public Material ShieldEffect {  get { return _class.ShieldEffect; } }
        public GameObject EmpPrefab {  get { return _class.EmpPrefab; } }        

        private ShipClass _class;
    }
}