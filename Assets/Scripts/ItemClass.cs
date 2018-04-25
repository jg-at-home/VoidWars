using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Base class for weapons, auxiliaries etc.
    /// </summary>
    public class ItemClass : ScriptableObject {
        [Tooltip("The name of the item")]
        public string Name;

        [Tooltip("The icon for the equipment")]
        public Sprite Icon;

        [Tooltip("Description of the item (terse)")]
        public string Description;

        [Tooltip("Description of the item (verbose)")]
        public string Detail;

        [Tooltip("Budget cost of the item")]
        public int Cost;

        [Tooltip("How much mass the item adds")]
        public int Mass;

        [Tooltip("How much power it takes to use the item.")]
        public int PowerUsage;

        [Tooltip("Maximum temperature above which the device will not function")]
        public float MaxTemperature;

        [Tooltip("Metadata for the item held as a string")]
        public string Metadata;

        [Tooltip("How much energy it takes to repair the item")]
        public float RepairCost;
    }
}