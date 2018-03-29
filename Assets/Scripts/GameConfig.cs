using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    [Tooltip("Who controls the ship")]
    public int ControllerID;
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
}

