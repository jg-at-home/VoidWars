using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// The type of an auxiliary item. Not very OO, #oldschool
    /// </summary>
    public enum AuxType {
        None = 0,
        BussardCollector = 1,
        FlareLauncher = 2,
        DriveBoost = 4,
        ERBInducer = 8,
        HeatShield = 16,
        SelfDestruct = 32,
        Shinobi = 64,
        Scanners = 128,
        PowerCell = 256,
        CoolingElement = 512,

        // New items above here.
        NumTypes
    }

    /// <summary>
    /// How an aux item operates.
    /// </summary>
    public enum AuxMode {
        /// <summary>
        /// The device can't be switched on or off, it just does its thing.
        /// </summary>
        Continuous,

        /// <summary>
        /// The device can be enabled or disabled as an action.
        /// </summary>
        Switchable,

        /// <summary>
        /// The device is triggered but does not stay active.
        /// </summary>
        OneShot
    }

    [CreateAssetMenu(menuName = "VoidWars/Auxiliary Item")]
    public class AuxiliaryClass : ItemClass {
        [Tooltip("The type of the item")]
        public AuxType ItemType;

        [Tooltip("The mode of operation")]
        public AuxMode Mode;
    }

    /// <summary>
    /// State of an aux item.
    /// </summary>
    public enum AuxState {
        Idle,
        Operational,
        Overheated,
        Broken
    }

    /// <summary>
    /// Instance of an auxiliary item.
    /// </summary>
    public class AuxiliaryItem {
        /// <summary>
        /// The item class.
        /// </summary>
        public AuxiliaryClass Class;

        /// <summary>
        /// The item's state.
        /// </summary>
        public AuxState State;

        /// <summary>
        /// Construct an item.
        /// </summary>
        /// <param name="auxClass">TGhe item's class.</param>
        public AuxiliaryItem(AuxiliaryClass auxClass) {
            Class = auxClass;
            State = AuxState.Idle;
        }


    }
}