using UnityEngine;

namespace VoidWars {
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

    [CreateAssetMenu(menuName = "VoidWars/Auxiliary Item")]
    public class AuxiliaryClass : ScriptableObject {
        [Tooltip("The name of the item")]
        public string Name;

        [Tooltip("Description of the item")]
        public string Description;

        [Tooltip("The type of the item")]
        public AuxType ItemType;

        [Tooltip("How much power it takes to use the item.")]
        public int PowerUsage;

        [Tooltip("How much mass the item adds")]
        public int Mass;

        [Tooltip("Budget cost of the item")]
        public int Cost;

        [Tooltip("Metadata for the item held as a string")]
        public string Metadata;
    }
}