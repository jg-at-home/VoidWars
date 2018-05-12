using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VoidWars {
    public static class Util {
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

        private static GameController s_controller;
    }
}