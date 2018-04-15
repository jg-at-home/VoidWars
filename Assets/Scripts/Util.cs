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

        /// <summary>
        /// Finbd child objects with a given tag.
        /// </summary>
        /// <param name="gob">The root object (included in the scan)</param>
        /// <param name="tag">The tagh to search for</param>
        /// <returns>An array of tagged objects.</returns>
        public static GameObject[] FindChildrenWithTag(GameObject gob, string tag) {
            var list = new List<GameObject>();

            if (tag == gob.tag) {
                list.Add(gob);
            }

            foreach (Transform tran in gob.transform) {
                // recursively check children
                list.AddRange(FindChildrenWithTag(tran.gameObject, tag)); 
            }

            return list.ToArray();
        }

        private static GameController s_controller;
    }
}