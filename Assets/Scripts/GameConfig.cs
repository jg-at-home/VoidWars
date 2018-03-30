using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Describes how a ship is controlled.
    /// </summary>
    public enum ControlType {
        /// <summary>
        /// A human controls this ship.
        /// </summary>
        HUMAN,

        /// <summary>
        /// An AI controls this ship.
        /// </summary>
        AI
    }

    [Serializable]
    public class PlayerConfig {
        [Tooltip("The ships this player will control")]
        public List<string> ShipPrefabs;

        [Tooltip("What controls the ship")]
        public ControlType ControlType;

        [HideInInspector]
        public int ControllerID;

        [Tooltip("The player's faction")]
        public Faction Faction;
    }

    [Serializable]
    public class GameConfig {
        [Tooltip("Configs for each of the players")]
        public List<PlayerConfig> PlayerConfigs;

        /// <summary>
        /// Gets the number of human players taking part.
        /// </summary>
        public int NumberOfHumanPlayers {
            get {
                return PlayerConfigs.Count(pc => pc.ControlType == ControlType.HUMAN);
            }
        }

        /// <summary>
        /// Gets the number of ships expected at the start of the game.
        /// </summary>
        public int NumberOfShips {
            get {
                var count = 0;
                foreach (var pc in PlayerConfigs) {
                    count += pc.ShipPrefabs.Count;
                }
                return count;
            }
        }
    }
}
