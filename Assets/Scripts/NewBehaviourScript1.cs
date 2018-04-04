using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// The species in the game.
    /// </summary>
    public enum Species {
        Human,
        Etokai,
        Dohahza
    }

    [CreateAssetMenu(menuName ="VoidWars/Species")]
    public class SpeciesInfo : ScriptableObject {
        [Tooltip("The name of the spacies")]
        public string Name;

        [Tooltip("Efficacy scalar for weapons / shields etc")]
        public float TechLevel;
    }
}
