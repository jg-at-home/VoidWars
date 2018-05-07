using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// What a crew member does.
    /// </summary>
    public enum Role {
        Captain,
        FirstOfficer,
        Engineer
    }

    [CreateAssetMenu(menuName = "VoidWars/Crew Member")]
    public class CrewMember : ScriptableObject {
        [Tooltip("The member's species")]
        public Species Species;

        [Tooltip("The member's name")]
        public string Name;

        [Tooltip("Role")]
        public Role Role;

        [Tooltip("Photo")]
        public Sprite Photo;

        [Tooltip("Buffs for the crew member")]
        public BuffInfo[] Buffs;

        [Tooltip("Crew member abilities")]
        public AbilityInfo[] Abilities;
    }
}