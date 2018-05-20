using System.Collections;
using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Used to synchronize multiple NPC actions.
    /// </summary>
    public class NPCSyncToken {
        public NPCSyncToken(int count) {
            _count = count;
        }

        public void Sync() {
            --_count;
            Debug.Assert(_count >= 0);
        }

        public bool IsSynced {
            get { return _count == 0; }
        }

        private int _count;
    }

    /// <summary>
    /// Base class for an NPC object synced over the network.
    /// </summary>
    public abstract class NPCObject : VoidNetworkBehaviour {
        /// <summary>
        /// Does a single turn update of the NPC.
        /// </summary>
        /// <param name="syncToken">Sync token.</param>
        /// <returns>Enumerator</returns>
        public abstract IEnumerator PerTurnUpdate(NPCSyncToken syncToken);

        /// <summary>
        /// Has the NPC expired?
        /// </summary>
        public bool HasExpired { get; protected set; }

        public override void OnStartServer() {
            controller.AddNPC(this);
        }
    }
}