using UnityEngine;
using TMPro;

namespace VoidWars {
    public class ActionPanelController : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private ActionListController _actionList;

        public void SelectCurrentAction() {
            _actionList.SelectCurrentAction();
        }

        public void SetTitle(string title) {
            _titleText.text = title;
        }
    }
}