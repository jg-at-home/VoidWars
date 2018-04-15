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
    public class AuxiliaryClass : ScriptableObject {
        [Tooltip("The name of the item")]
        public string Name;

        [Tooltip("The icon for the equipment")]
        public Sprite Icon;

        [Tooltip("Description of the item (terse)")]
        public string Description;

        [Tooltip("Description of the item (verbose)")]
        public string Detail;

        [Tooltip("The type of the item")]
        public AuxType ItemType;

        [Tooltip("How much power it takes to use the item.")]
        public int PowerUsage;

        [Tooltip("How much mass the item adds")]
        public int Mass;

        [Tooltip("Budget cost of the item")]
        public int Cost;

        [Tooltip("The mode of operation")]
        public AuxMode Mode;

        [Tooltip("How much energy it takes to repair the item")]
        public float RepairCost;

        [Tooltip("Metadata for the item held as a string")]
        public string Metadata;
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
        /// Construct an item.
        /// </summary>
        /// <param name="auxClass">TGhe item's class.</param>
        public AuxiliaryItem(AuxiliaryClass auxClass) {
            Class = auxClass;
        }

        /// <summary>
        /// Enable / disable the item.
        /// </summary>
        /// <param name="enable">Enable flag.</param>
        public void Enable(bool enable) {
            if (_functional) {
                _enabled = enable;
            }
        }

        /// <summary>
        /// Is the item active.
        /// </summary>
        public bool IsActive {
            get { return _enabled; }
        }

        /// <summary>
        /// Is the item working?
        /// </summary>
        public bool IsFunctional {
            get { return _functional; }
        }

        /// <summary>
        /// Destroy the item.
        /// </summary>
        public void Destroy() {
            _functional = false;
        }

        private bool _enabled;
        private bool _functional = true;
    }
}