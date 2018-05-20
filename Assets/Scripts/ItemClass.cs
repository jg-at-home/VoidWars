using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoidWars {
    public enum MetaType {
        String,
        Int,
        Float
    }

    [Serializable]
    public struct MetaItem {
        public string Name;
        public MetaType Type;
        public string Value;
    }

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
        public float PowerUsage;

        [Tooltip("Maximum temperature above which the device will not function")]
        public float MaxTemperature;

        [Tooltip("How much energy it takes to repair the item")]
        public float RepairCost;

        [Tooltip("How many turns it takes to repair an item")]
        public int RepairTurns;

        [Tooltip("Probability the item will break under significant stress")]
        [Range(0f, 1f)]
        public float BreakProbability;

        [Tooltip("Leaf data")]
        public MetaItem[] MetaItems;
    }

    public class ItemInstance : Buffable {
        public ItemInstance(ItemClass itemClass) {
            _class = itemClass;
            PowerUsage = _class.PowerUsage;
            RepairTurns = _class.RepairTurns;
            buildMetaData(_class.MetaItems);
        }

        public string Name { get { return _class.Name; } }
        public Sprite Icon { get { return _class.Icon; } }
        public string Description { get { return _class.Description; } }
        public string Detail { get { return _class.Detail; } }
        public int Cost { get { return _class.Cost; } }
        public int Mass { get { return _class.Mass; } }
        public float MaxTemperature { get { return _class.MaxTemperature; } }
        public float RepairCost { get { return _class.RepairCost; } }

        [Stat(0f, float.PositiveInfinity)]
        public float PowerUsage {
            get { return getValue("PowerUsage"); }
            set { setValue("PowerUsage", value); }
        }

        [Stat(1f, 10f)]
        public int RepairTurns {
            get { return (int)getValue("RepairTurns"); }
            set { setValue("RepairTurns", value); }
        }

        public float GetFloat(string field) {
            return (float)_metaData[field];
        }

        public int GetInt(string field) {
            return (int)_metaData[field];
        }

        public string GetString(string field) {
            return (string)_metaData[field];
        }

        private void buildMetaData(MetaItem[] items) {
            foreach(var item in items) {
                switch(item.Type) {
                    case MetaType.String:
                        _metaData[item.Name] = item.Value;
                        break;

                    case MetaType.Int:
                        _metaData[item.Name] = int.Parse(item.Value);
                        break;

                    case MetaType.Float:
                        _metaData[item.Name] = float.Parse(item.Value);
                        break;

                    default:
                        Debug.Assert(false);
                        break;
                }
            }
        }

        private readonly ItemClass _class;
        private readonly Dictionary<string, object> _metaData = new Dictionary<string, object>();
    }
}