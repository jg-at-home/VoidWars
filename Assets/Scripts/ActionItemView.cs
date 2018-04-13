using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VoidWars {
    public class ActionItemView : PooledObject, IPointerClickHandler {
        public Image Icon;
        public Text ItemText;

        public void Setup(ActionItem item, ActionListController list) {
            Icon.sprite = item.Icon;
            ItemText.text = item.Description;
            _item = item;
            _actionList = list;
        }

        public void OnPointerClick(PointerEventData eventData) {
            Debug.LogFormat("ActionItemView: you selected '{0}'" + _item.Action);
            // TODO: handle click.
        }

        private ActionItem _item;
        private ActionListController _actionList;
    }
}