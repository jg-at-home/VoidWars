using UnityEngine;
using System.Collections.Generic;

namespace VoidWars {
    public static class ImageManager {
        public static Sprite GetImage(string name) {
            Sprite sprite;
            if (!s_cache.TryGetValue(name, out sprite)) {
                sprite = Resources.Load<Sprite>("Images/" + name);
                s_cache[name] = sprite;
            }

            return sprite;
        }
        private static readonly Dictionary<string, Sprite> s_cache = new Dictionary<string, Sprite>();
    }
}