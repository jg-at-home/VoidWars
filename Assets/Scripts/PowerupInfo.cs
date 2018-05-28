using UnityEngine;

namespace VoidWars {
    public enum PowerupMode {
        Instantaneous,
        Cachable,
        TurnLimited
    }

    [CreateAssetMenu(menuName = "VoidWars/Powerup")]
    public class PowerupInfo : ScriptableObject {
        [Tooltip("The name of the powerup")]
        public string Name;

        [Tooltip("Detailed description of the powerup")]
        public string DetailText;

        [Tooltip("The action command")]
        public string CollectAction;

        [Tooltip("Frequency of occurence")]
        public int Frequency;

        [Tooltip("The mode of action")]
        public PowerupMode Mode;

        [Tooltip("If mode is turn-limited, how long the powerup can be held")]
        public int TurnLimit;

        [Tooltip("Icon for the ability")]
        public Sprite Icon;
    }
}