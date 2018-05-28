using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VoidWars {
    public static class Util {
        /// <summary>
        /// Shuffles a list of things using Fisher-Yates.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="input">Unshuffled list.</param>
        /// <returns>The shuffled list.</returns>
        public static IList<T> Shuffled<T>(this IList<T> input) {
            var result = new List<T>(input);
            var n = result.Count;
            while(n > 1) {
                --n;
                var k = Random.Range(0, n + 1);
                var tmp = result[k];
                result[k] = result[n];
                result[n] = tmp;
            }

            return result;
        }

        /// <summary>
        /// Gets the singleton game controller.
        /// </summary>
        /// <returns>The game controller instance.</returns>
        public static GameController GetGameController() {
            if (s_controller == null) {
                var controllerObj = GameObject.FindGameObjectWithTag("GameController");
                s_controller = controllerObj.GetComponent<GameController>();
            }
            return s_controller;
        }

        /// <summary>
        /// Convert a PascalCasedString to a space one (Pascal Cased String).
        /// </summary>
        /// <param name="pascal">Input string.</param>
        /// <returns>Spaced output.</returns>
        public static string PascalToSpaced(string pascal) {
            var prevLower = false;
            var sb = new StringBuilder();
            foreach(var c in pascal) {
                if (char.IsUpper(c) && prevLower) { 
                    sb.Append(' ');
                }

                sb.Append(c);
                prevLower = char.IsLower(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Compute a weighted random selection from an array of items.
        /// </summary>
        /// <param name="weights">Item weights.</param>
        /// <returns>Random weighted selection.</returns>
        public static int RandomWeightedSelection(float[] weights) {
            if (weights.Length == 1) {
                return 0;
            }

            // Normalize the weights to add to 1.
            var total = 0f;
            foreach(var weight in weights) {
                total += weight;
            }

            // Build accumulator table.
            var cumulative = new float[weights.Length];
            var accum = 0f;
            for(int i = 0; i < weights.Length; ++i) {
                accum += (weights[i] / total);
                cumulative[i] = accum;
            }

            var r01 = Random.Range(0f, 1f);
            for(int i = 0; i < cumulative.Length; ++i) {
                if (r01 < cumulative[i]) {
                    return i;
                }
            }

            return weights.Length-1;
        }

        private static GameController s_controller;
    }
}