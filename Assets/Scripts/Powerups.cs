using UnityEngine;

namespace VoidWars {
    public static class Powerups {
        public static void RunPayload(ShipController ship, string action) {
            Debug.LogFormat("Powerups.RunPayload({0}, {1})", ship.name, action);

            // Format is 'target parameter [data...]
            var parts = action.ToLower().Split(' ');
            switch(parts[0]) {
                case "ship":
                    runShipAction(ship, parts);
                    break;

                default:
                    Debug.LogAssertionFormat("Invalid target: {0}", parts[0]);
                    break;
            }
        }

        private static void runShipAction(ShipController ship, string [] parts) {
            var controller = Util.GetGameController();
            switch(parts[1]) {
                case "health_percent": {
                        float percent = float.Parse(parts[2]);
                        float oldHealth = ship.Health;
                        float delta = oldHealth * percent / 100f;
                        float newHealth = Mathf.Clamp(oldHealth + delta, 0f, ship.MaxHealth);
                        if (newHealth != oldHealth) {
                            ship.Health = newHealth;
                            controller.ShowHealthChange(ship.ID, newHealth - oldHealth);
                        }
                    }
                    break;
                
                   
            }
        }
    }
}