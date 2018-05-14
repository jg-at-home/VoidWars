using UnityEngine;

namespace VoidWars {
    public abstract class ActionDetailPanelController : MonoBehaviour {
        /// <summary>
        /// Initializes the panel.
        /// </summary>
        /// <param name="actionPanel">The parent action panel.</param>
        /// <param name="item">The item the planel is controlling</param>
        /// <param name="args">Arguments to send to the panel.</param>
        public void Initialize(ActionPanelController actionPanel, ActionItem item, string[] args) {
            _actionPanel = actionPanel;
            Item = item;
            initializeInner(item, args);
        }

        /// <summary>
        /// Gets the underlying item for the panel to control.
        /// </summary>
        public ActionItem Item { get; private set; }

        /// <summary>
        /// Selects the associated action as the one to perform.
        /// </summary>
        public abstract void SelectAction();

        /// <summary>
        /// Internal implementation.
        /// </summary>
        /// <param name="item">The item the planel is controlling</param>
        /// <param name="args">Arguments to send to the panel.</param>
        protected abstract void initializeInner(ActionItem item, string[] args);

        /// <summary>
        /// Enable the select button on the parent panel.
        /// </summary>
        /// <param name="enable">Status flag.</param>
        protected void enableSelectButton(bool enable) {
            _actionPanel.EnableSelectButton(enable);
        }

        private ActionPanelController _actionPanel;
    }
}