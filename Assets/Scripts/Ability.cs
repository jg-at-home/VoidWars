using System;
using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Types of ability (see below).
    /// </summary>
    public enum AbilityType {
        RepairItem,
    }

    /// <summary>
    /// A class that confers an additional action to the buffable.
    /// </summary>
    [Serializable]
    public struct AbilityInfo {
        public AbilityType Type;
        public string Description;
        public Sprite Icon;
        public MetaItem[] Metadata;
    }

    /// <summary>
    /// Helper class for applying abilities to a ship.
    /// </summary>
    public static class Abilities {
        public static void Apply(ShipController ship, AbilityInfo ability) {
            switch(ability.Type) {
                case AbilityType.RepairItem:
                    ship.EnableRepairs();
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }
        }
    }
}