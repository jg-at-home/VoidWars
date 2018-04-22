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
    public class AuxiliaryClass : ItemClass {
        [Tooltip("The type of the item")]
        public AuxType ItemType;

        [Tooltip("The mode of operation")]
        public AuxMode Mode;
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