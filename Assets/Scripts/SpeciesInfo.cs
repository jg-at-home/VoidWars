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
        [Tooltip("The name of the species")]
        public string Name;

        [Tooltip("Efficacy scalar for weapons / shields etc")]
        public float TechLevel;

        [Tooltip("A measure of how aggressive or defensive a species is")]
        [Range(-1f,1f)]
        public float Mindset;

        [Tooltip("The colour used to indicate the species")]
        public Color MarkerColor;
    }
}
