using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace VoidWars {
    /// <summary>
    /// Class representing the player on the server side.
    /// </summary>
    public class PlayerServerRep {
        public PlayerServerRep(NetworkConnection connection, int playerID) {
            Connection = connection;
            PlayerID = playerID;
        }

        public readonly NetworkConnection Connection;
        public readonly int PlayerID;
    }
}