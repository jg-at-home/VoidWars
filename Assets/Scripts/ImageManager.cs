using UnityEngine;
using System.Collections.Generic;

namespace VoidWars {
    public static class ImageManager {
        public static Sprite GetImage(string name) {
            return getImageInner(name, true);
        }

        private static Sprite getImageInner(string name, bool returnDefaultOnError) {
            Sprite sprite;
            if (!s_cache.TryGetValue(name, out sprite)) {
                sprite = Resources.Load<Sprite>("Images/" + name);
                if (sprite == null) {
                    if (returnDefaultOnError) {
                        // Doesn't exist - return default image
                        return getImageInner("WorkInProgress", false);
                    }
                    else {
                        Debug.LogErrorFormat("ImageManager: could not load image {0} or default", name);
                        return null;
                    }
                }

                s_cache[name] = sprite;
            }

            return sprite;
        }

        private static readonly Dictionary<string, Sprite> s_cache = new Dictionary<string, Sprite>();
    }
}