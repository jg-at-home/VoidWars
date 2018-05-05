using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    /// <summary>
    /// The type of an auxiliary item. Not very OO, #oldschool
    /// </summary>
    public enum AuxType {
        None = 0,
        BussardCollector = 1,
        ERBInducer = 2,
        FlareLauncher = 4,
        DriveBoost = 8,
        Scanners = 16,
        SelfDestruct = 32,
        Shinobi = 64,
        PowerCell = 128,
        CoolingElement = 256,

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
        Broken,
        Disabled
    }

    /// <summary>
    /// Descriptor for an auxiliary item.
    /// </summary>
    public class AuxItem : ItemInstance {
        /// <summary>
        /// The item's state.
        /// </summary>
        public AuxState State;

        /// <summary>
        /// Construct an item.
        /// </summary>
        /// <param name="itemClass">The item's class</param>
        public AuxItem(AuxiliaryClass itemClass) : base(itemClass) {
            _class = itemClass;
            State = AuxState.Idle;
        }

        public AuxMode Mode {  get { return _class.Mode; } }
        public AuxType ItemType {  get { return _class.ItemType; } }

        private readonly AuxiliaryClass _class;
    }
}