using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VoidWars {
    /// <summary>
    /// UI viee of an ActionItem.
    /// </summary>
    public class ActionItemView : MonoBehaviour, IPointerClickHandler {
        public Image Icon;
        public Text ItemText;

        public void Setup(ActionItem item, ActionListController list) {
            Icon.sprite = item.Icon;
            ItemText.text = item.Description;
            _item = item;
            _actionList = list;
        }

        public void OnPointerClick(PointerEventData eventData) {
            Debug.LogFormat("ActionItemView: you selected '{0}'", _item.Action);
            _actionList.OnItemClicked(_item);
        }

        private ActionItem _item;
        private ActionListController _actionList;
    }
}