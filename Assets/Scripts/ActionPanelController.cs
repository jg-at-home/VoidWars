using UnityEngine;
using TMPro;

namespace VoidWars {
    public class ActionPanelController : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _titleText;

        public void SetTitle(string title) {
            _titleText.text = title;
        }
    }
}