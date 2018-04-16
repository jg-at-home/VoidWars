using System.Collections.Generic;
using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Script controlling the list of actions.
    /// </summary>
    public class ActionListController : MonoBehaviour {
        [SerializeField] private RectTransform _contentPanel;
        [SerializeField] private ObjectPool _buttonPool;
        [SerializeField] private RectTransform _detailPanel;

        /// <summary>
        /// Rebuilds the list of actions.
        /// </summary>
        public void Refresh() {
            var gameController = Util.GetGameController();
            var activeShip = gameController.GetActiveShip();
            _items = activeShip.GetAvailableActions();
            refreshButtons();
        }

        /// <summary>
        /// Selects the current action for execution.
        /// </summary>
        public void SelectCurrentAction() {
            Debug.Log("ActionListController.SelectCurrentAction()");
            Debug.Assert(_current != null);

            _current.SelectAction();
        }

        /// <summary>
        /// Handler for click event.
        /// </summary>
        /// <param name="item">The selected item.</param>
        public void OnItemClicked(ActionItem item) {
            if (_currentItem == item) {
                return;
            }

            var prefabInfoParts = item.EditorPrefabInfo.Split(' ');
            var prefabName = prefabInfoParts[0];
            var prefabPath = "Prefabs/UI/" + prefabName;
            var panelPrefab = (GameObject)Resources.Load(prefabPath);
            var panel = Instantiate(panelPrefab);
            if (_current != null) {
                _current.transform.SetParent(null, false);
                // TODO: cache?
                Destroy(_current.gameObject);
            }

            panel.transform.SetParent(_detailPanel, false);
            _current = panel.GetComponent<ActionDetailPanelController>();
            _current.Initialize(item, prefabInfoParts);
            _currentItem = item;
        }

        private void OnEnable() {
            Refresh();
        }

        private void refreshButtons() {
            removeButtons();
            addButtons();
        }

        private void addButtons() {
            for(var i = 0; i < _items.Count; ++i) {
                var action = _items[i];
                var control = _buttonPool.GetObject();
                control.transform.SetParent(_contentPanel, false);
                var actionControl = control.GetComponent<ActionItemView>();
                actionControl.Setup(action, this);
            }
        }

        private void removeButtons() {
            while(_contentPanel.childCount > 0) {
                var item = _contentPanel.transform.GetChild(0).gameObject;
                _buttonPool.ReturnObject(item);
            }
        }

        private List<ActionItem> _items;
        private ActionDetailPanelController _current;
        private ActionItem _currentItem;
    }
}