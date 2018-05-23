using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Script controlling the list of actions.
    /// </summary>
    public class ActionListController : MonoBehaviour {
        [SerializeField] private ActionPanelController _parentController;
        [SerializeField] private RectTransform _contentPanel;
        [SerializeField] private ObjectPool _buttonPool;
        [SerializeField] private RectTransform _detailPanel;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private float _spinRate;

        private void Awake() {
            var control = _buttonPool.GetObject();
            _itemHeight = control.GetComponent<RectTransform>().rect.height;
            _buttonPool.ReturnObject(control);
        }

        /// <summary>
        /// Rebuilds the list of actions.
        /// </summary>
        public void Refresh() {
            Debug.Log("ActionListController.Refresh()");
            var gameController = Util.GetGameController();
            var activeShip = gameController.GetActiveShip();
            _views.Clear();
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

            SendMessageUpwards("PlayButtonClick");
            var prefabInfoParts = item.EditorPrefabInfo.Split(' ');
            var prefabName = prefabInfoParts[0];
            var prefabPath = "Prefabs/UI/" + prefabName;
            var panelPrefab = (GameObject)Resources.Load(prefabPath);
            var panel = Instantiate(panelPrefab);
            clearDetailPanel();

            panel.transform.SetParent(_detailPanel, false);
            _current = panel.GetComponent<ActionDetailPanelController>();
            _current.Initialize(_parentController, item, prefabInfoParts);
            _currentItem = item;
            var controller = Util.GetGameController();
            controller.OnActionSelected(_currentItem.Action, _current.EnergyCost);

            foreach(var view in _views) {
                view.IsSelected = (view.Item == _currentItem);
            }
        }

        private void clearDetailPanel() {
            if (_current != null) {
                _current.transform.SetParent(null, false);
                Destroy(_current.gameObject);
                _current = null;
            }
        }

        private void OnEnable() {
            _parentController.EnableSelectButton(false);
            clearDetailPanel();
            Refresh();
        }

        private void refreshButtons() {
            removeButtons();
            addButtons();
        }

        private void addButtons() {
            if (_items.Count > 0) {
                StartCoroutine(addButtonsCoro());
            }
        }

        private IEnumerator addButtonsCoro() {
            var i = 0;
            int numVisibleItems = (int)((_viewport.rect.height + _itemHeight - 1) / _itemHeight);
            for (; i < Mathf.Min(_items.Count, numVisibleItems); ++i) {
                var action = _items[i];
                var control = _buttonPool.GetObject();
                control.transform.SetParent(_contentPanel, false);
                var actionControl = control.GetComponent<ActionItemView>();
                actionControl.Setup(action, this);
                _views.Add(actionControl);
                StartCoroutine(spinItemIntoView(actionControl));
                yield return new WaitForSeconds(0.1f);
            }

            for(; i < _items.Count; ++i ) {
                var action = _items[i];
                var control = _buttonPool.GetObject();
                control.transform.SetParent(_contentPanel, false);
                var actionControl = control.GetComponent<ActionItemView>();
                actionControl.Setup(action, this);
                _views.Add(actionControl);
            }
        }

        private IEnumerator spinItemIntoView(ActionItemView item) {
            var rt = item.GetComponent<RectTransform>();
            var angle = -90f;
            while(angle < 0f) {
                rt.rotation = Quaternion.Euler(angle, 0f, 0f);
                angle += _spinRate * Time.deltaTime;
                yield return null;
            }
            rt.rotation = Quaternion.identity;
        }

        private void removeButtons() {
            while(_contentPanel.childCount > 0) {
                var item = _contentPanel.GetChild(0);
                item.SetParent(null, false);
                _buttonPool.ReturnObject(item.gameObject);
            }

            _views.Clear();
        }

        private List<ActionItem> _items;
        private readonly List<ActionItemView> _views = new List<ActionItemView>();
        private ActionDetailPanelController _current;
        private ActionItem _currentItem;
        private float _itemHeight;
    }
}