using System;

namespace VoidWars {
    public static class ShipExt {
        /// <summary>
        /// Gets the cost in energy for an action (note: not the additional drain, just the one-off hit).
        /// </summary>
        /// <param name="action">An action string.</param>
        /// <returns>The energy cost.</returns>
        public static float GetEnergyCostForAction(this ShipController ship, string action) {
            var parts = action.Split(' ');
            switch (parts[0].ToLower()) {
                case "pass":
                case "shields":
                    return 0f;

                case "repair":
                    return repairCost(ship, parts);

                case "aux":
                    return auxCost(ship, parts);
            }

            return 0f;
        }

        private static float repairCost(ShipController ship, string[] parts) {
            if (parts[1].ToLower() == "propulsion") {
                return ship.ShipData.EngineRepairCost;
            }
            else {
                var itemType = (AuxType)Enum.Parse(typeof(AuxType), parts[1]);
                var item = ship.GetAuxiliaryItem(itemType);
                return item.RepairCost;
            }
        }

        private static float auxCost(ShipController ship, string[] parts) {
            // If we're switching it off, we're not changing.
            if (!bool.Parse(parts[2])) {
                return 0f;
            }

            var auxType = auxTypeFromString(parts[1]);
            if (auxType == AuxType.None) {
                return 0f;
            }

            var aux = ship.GetAuxiliaryItem(auxType);
            if (aux.Mode != AuxMode.OneShot) {
                return 0f;
            }

            return aux.PowerUsage;
        }

        /// <summary>
        /// Gets the drain of an action. For one-off actions, this will be 0.
        /// </summary>
        /// <param name="action">The action as a string.</param>
        /// <returns>The energy drain.</returns>
        public static float GetEnergyDrainForAction(this ShipController ship, string action) {
            var parts = action.ToLower().Split(' ');

            switch (parts[0]) {
                case "pass":
                    break;

                case "shields":
                    return ship.ShipData.ShieldDrainRate;

                case "aux":
                    return auxDrain(ship, parts);
            }

            return 0f;
        }

        private static float auxDrain(ShipController ship, string[] parts) { 
            var auxType = auxTypeFromString(parts[1]);
            if (auxType == AuxType.None) {
                return 0f;
            }

            var aux = ship.GetAuxiliaryItem(auxType);
            if (aux.Mode == AuxMode.OneShot) {
                return 0f;
            }

            // If we're switching it off, we're reducing power consumption.
            if (!bool.Parse(parts[2])) {
                return -aux.PowerUsage;
            }

            return aux.PowerUsage;
        }

        private static AuxType auxTypeFromString(string type) {
            AuxType auxType = AuxType.None;
            switch (type) {
                case "shinobi":
                    auxType = AuxType.Shinobi;
                    break;

                case "scanners":
                    auxType = AuxType.Scanners;
                    break;

                case "flarelauncher":
                    auxType = AuxType.FlareLauncher;
                    break;

                case "erbinducer":
                    auxType = AuxType.ERBInducer;
                    break;

                case "chafflauncher":
                    auxType = AuxType.ChaffLauncher;
                    break;

                case "minelauncher":
                    auxType = AuxType.MineLauncher;
                    break;

                default:
                    auxType = AuxType.None;
                    break;
            }

            return auxType;
        }
    }
}