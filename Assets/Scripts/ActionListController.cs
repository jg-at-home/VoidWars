using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VoidWars {
    public class ActionListController : MonoBehaviour {
        public RectTransform ContentPanel;
        public ObjectPool ButtonPool;
        public RectTransform DetailPanel;

        public void SelectCurrentAction() {
            Debug.Log("ActionListController.SelectCurrentAction()");
            Debug.Assert(_current != null);

            _current.SelectAction();
        }

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

            panel.transform.SetParent(DetailPanel, false);
            _current = panel.GetComponent<ActionDetailPanelController>();
            _current.Initialize(item, prefabInfoParts);
            _currentItem = item;
        }

        private void OnEnable() {
            var gameController = Util.GetGameController();
            var activeShip = gameController.GetActiveShip();
            _items = activeShip.GetAvailableActions();
            refresh();
        }

        private void Start() {
        }

        private void Update() {
        }

        private void refresh() {
            removeButtons();
            addButtons();
        }

        private void addButtons() {
            for(var i = 0; i < _items.Count; ++i) {
                var action = _items[i];
                var control = ButtonPool.GetObject();
                control.transform.SetParent(ContentPanel, false);
                var actionControl = control.GetComponent<ActionItemView>();
                actionControl.Setup(action, this);
            }
        }

        private void removeButtons() {
            while(ContentPanel.childCount > 0) {
                var item = ContentPanel.transform.GetChild(0).gameObject;
                ButtonPool.ReturnObject(item);
            }
        }

        private List<ActionItem> _items;
        private ActionDetailPanelController _current;
        private ActionItem _currentItem;
    }
}