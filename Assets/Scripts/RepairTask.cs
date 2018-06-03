namespace VoidWars {
    public class RepairTask : Task {
        /// <summary>
        /// The item being repaired.
        /// </summary>
        public readonly AuxType ItemType;

        public RepairTask(int repairTurns, TaskFunc func, AuxType itemType) : base(repairTurns, func) {
            ItemType = itemType;
        }

        public override void OnTurnStart(ShipController ship) {
            base.OnTurnStart(ship);
            ship.RpcSetItemRepairTurns(ItemType, TurnsLeft);
        }
    }

    public class RepairEnginesTask : Task {
        public RepairEnginesTask(int repairTurns, TaskFunc func, TaskFunc perTurnFunc) : base(repairTurns, func) {
            _perTurnFunc = perTurnFunc;
        }

        public override void OnTurnStart(ShipController ship) {
            base.OnTurnStart(ship);
            _perTurnFunc(this);
        }

        private readonly TaskFunc _perTurnFunc;
    }

}