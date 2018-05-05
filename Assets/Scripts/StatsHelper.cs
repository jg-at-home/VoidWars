using UnityEngine;

namespace VoidWars { 
    public static class StatsHelper {
        /// <summary>
        /// Gets the efficiency of a thing which has a maximum temperature at the current temperature.
        /// </summary>
        /// <param name="T">The current temperature.</param>
        /// <param name="Tmax">The maximum temperature > 0.</param>
        /// <returns></returns>
        public static float EfficiencyAtTemperature(float T, float Tmax) {
            return Mathf.Exp(-T / Tmax);
        }
    }
}
