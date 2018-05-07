using UnityEngine;
using System.Collections.Generic;

namespace VoidWars {
    public static class GameObjectEx  {
        /// <summary>
        /// Find child objects with a given tag.
        /// </summary>
        /// <param name="gob">The root object (included in the scan)</param>
        /// <param name="tag">The tagh to search for</param>
        /// <returns>An array of tagged objects.</returns>
        public static GameObject[] FindChildrenWithTag(this GameObject gob, string tag) {
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
    }
}