using UnityEngine;

namespace VoidWars {
    public abstract class ActionDetailPanelController : MonoBehaviour {
        /// <summary>
        /// Initializes the panel.
        /// </summary>
        /// <param name="item">The item the planel is controlling</param>
        /// <param name="args">Arguments to send to the panel.</param>
        public void Initialize(ActionItem item, string[] args) {
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
        protected virtual void initializeInner(ActionItem item, string[] args) {
            var controller = Util.GetGameController();
            controller.InfoPanel.NotifyContent("SetDoneButtonCaption", "Select");
        }
    }
}