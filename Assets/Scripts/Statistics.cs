using UnityEngine;
using System.Collections;
using System;

namespace VoidWars {
    public static class Statistics {
        /// <summary>
        /// Box-Muller formula for generating samples from a normal distribution.
        /// </summary>
        /// <param name="mean">Mean.</param>
        /// <param name="stdDev">Stanbdard deviation.</param>
        /// <returns></returns>
        public static float RandomNormal(float mean, float stdDev) {
            var u1 = 1f - (float)s_random.NextDouble();
            var u2 = 1f - (float)s_random.NextDouble();
            var randStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);
            return mean + stdDev * randStdNormal;
        }

        public static void Reseed(int seed) {
            s_random = new System.Random(seed);
        }

        private static System.Random s_random = new System.Random();
    }
}