using UnityEngine;

namespace VoidWars {
    public abstract class ActionDetailPanelController : MonoBehaviour {
        public abstract void Initialize(ActionItem item, string[] args);
        public abstract void SelectAction();
    }
}