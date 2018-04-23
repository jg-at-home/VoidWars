using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VoidWars {
    /// <summary>
    /// UI view of an ActionItem.
    /// </summary>
    public class ActionItemView : MonoBehaviour, IPointerClickHandler {
        /// <summary>
        /// The icon to show.
        /// </summary>
        public Image Icon;

        /// <summary>
        /// The text to display.
        /// </summary>
        public Text ItemText;

        /// <summary>
        /// Get / set the selection status.
        /// </summary>
        public bool IsSelected {
            get { return _selected; }
            set {
                if (_selected != value) {
                    _selected = value;
                    if (value) {
                        _background.color = Color.white;
                    }
                    else {
                        _background.color = s_bgColor;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the item being viewed.
        /// </summary>
        public ActionItem Item {
            get { return _item; }
        }

        /// <summary>
        /// Sets up the view.
        /// </summary>
        /// <param name="item">The item to show.</param>
        /// <param name="list">The list holding the view.</param>
        public void Setup(ActionItem item, ActionListController list) {
            Icon.sprite = item.Icon;
            ItemText.text = item.Description;
            _item = item;
            _actionList = list;
            _background = GetComponent<Image>();
            IsSelected = false;
        }

        public void OnPointerClick(PointerEventData eventData) {
            Debug.LogFormat("ActionItemView: you selected '{0}'", _item.Action);
            _actionList.OnItemClicked(_item);
        }

        private ActionItem _item;
        private ActionListController _actionList;
        private bool _selected;
        private Image _background;
        private static readonly Color s_bgColor = new Color(1f, 1f, 1f, 100f / 255f);
    }
}