using UnityEngine;
using System.Collections;

namespace VoidWars {
    public abstract class Pilot {
        public abstract void UpdateShip(GameController gameController, ShipController shipController);
    }

    public class HumanPilot : Pilot {
        public override void UpdateShip(GameController gameController, ShipController shipController) {
        }
    }

    public class AIPilot : Pilot {
        public override void UpdateShip(GameController gameController, ShipController shipController) {
        }
    }
}
