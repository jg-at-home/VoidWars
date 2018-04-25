using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Controller for temperature panel.
    /// </summary>
    public class TemperaturePanelController : SubpanelController {
        public RectTransform Needle;
        public float MinAngle = -33f;
        public float MaxAngle = 57f;

        protected override void initialize() {
            Needle.rotation = Quaternion.Euler(0f, 0f, MinAngle);
            refresh();
        }

        protected override void refresh() {
            if (activeShip != null) {
                var t = Mathf.Clamp(activeShip.HullTemperature, 0f, 100f) / 100f;
                var zRot = Mathf.Lerp(MinAngle, MaxAngle, t);
                Needle.rotation = Quaternion.Euler(0f, 0f, zRot);
            }
        }

        protected override void updateInner() {
        }
    }
}