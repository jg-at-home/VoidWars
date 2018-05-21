using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace VoidWars {
    /// <summary>
    /// Item that lasts a number of turns.
    /// </summary>
    public class TransientItem : AuxItem {
        public TransientItem(AuxiliaryClass itemClass) : base(itemClass) {
            DurationTurns = GetInt("DurationTurns");
        }

        [Stat]
        public int DurationTurns {
            get { return (int)getValue("DurationTurns"); }
            set { setValue("DurationTurns", value); }
        }

    }

    /// <summary>
    /// NPC (server-controlled) object that sticks around a number of turns.
    /// </summary>
    public abstract class TransientNPC : NPCObject {
        protected void setLifetime(int lifetime) {
            _turnCounter = lifetime;
        }

        [Server]
        public override IEnumerator PerTurnUpdate(NPCSyncToken syncToken) {
            if (updateTurnCount()) {
                HasExpired = true;
            }

            syncToken.Sync();
            yield break;
        }

        protected bool updateTurnCount() {
            if (_turnCounter > 0) {
                --_turnCounter;
                if (_turnCounter == 0) {
                    return true;
                }
            }

            return false;
        }

        private int _turnCounter;
    }

}