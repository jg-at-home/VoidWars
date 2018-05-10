using System.Collections.Generic;
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

        private static GameController s_controller;
    }
}