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

        [Tooltip("How many turns it takes to repair an item")]
        public int RepairTurns;
    }

    public class ItemInstance : Buffable {
        public ItemInstance(ItemClass itemClass) {
            _class = itemClass;
        }

        public string Name { get { return _class.Name; } }
        public Sprite Icon {  get { return _class.Icon; } }
        public string Description {  get { return _class.Description; } }
        public string Detail {  get { return _class.Detail; } }
        public int Cost {  get { return _class.Cost; } }
        public int Mass {  get { return _class.Mass; } }
        public float MaxTemperature {  get { return _class.MaxTemperature; } }
        public string Metadata {  get { return _class.Metadata; } }
        public float RepairCost {  get { return _class.RepairCost; } }

        [Stat]
        public int PowerUsage {
            get { return (int)getValue("PowerUsage"); }
            set { setValue("PowerUsage", value); }
        }

        [Stat]
        public int RepairTurns {
            get { return (int)getValue("RepairTurns"); }
            set { setValue("RepairTurns", value); }
        }

        private ItemClass _class;
    }
}