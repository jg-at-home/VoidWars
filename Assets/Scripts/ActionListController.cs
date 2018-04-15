using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VoidWars {
    public class ActionListController : MonoBehaviour {
        public RectTransform ContentPanel;
        public ObjectPool ButtonPool;
        public RectTransform DetailPanel;

        public void OnItemClicked(ActionItem item) {
            var prefabInfoParts = item.EditorPrefabInfo.Split(' ');
            var prefabName = prefabInfoParts[0];
            var prefabPath = Path.Combine("/Prefabs/UI", prefabName);
            var panel = (ActionDetailPanelController)Resources.Load(prefabPath);
            if (_current != null) {
                _current.transform.SetParent(null, false);
                // TODO: cache?
                Destroy(_current);
            }

            _current = panel;
            _current.transform.SetParent(DetailPanel, false);
            _current.Setup(item, prefabInfoParts);
        }

        private void OnEnable() {
            //var gameController = Util.GetGameController();
            //var activeShip = gameController.GetActiveShip();
            //_items = activeShip.GetAvailableActions();
            //refresh();
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
    }
}