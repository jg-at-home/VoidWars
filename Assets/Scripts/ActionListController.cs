using System.Collections.Generic;
using UnityEngine;

namespace VoidWars {
    public class ActionListController : MonoBehaviour {
        public RectTransform ContentPanel;
        public ObjectPool ButtonPool;

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
    }
}