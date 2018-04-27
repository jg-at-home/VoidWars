using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;

namespace VoidWars {
    public class GraphSim : MonoBehaviour {
        public UILineRenderer LineRenderer;
        public TextMeshProUGUI Caption;
        public float Value = 0.0f;
        public float Range = 2.0f;
        public Color Color;
        private int NumPoints = 136;
        private Vector2[] _points;
        private float _range;

        public void FlatLine(bool flag) {
            if (flag) {
                LineRenderer.color = Color;
                _range = Range / 2;
            }
            else {
                LineRenderer.color = Color;
                _range = Range;
            }
        }

        private void Start() {
            _points = new Vector2[NumPoints];
            for (int i = 0; i < NumPoints; ++i) {
                _points[i] = new Vector2(i, Value);
            }
            LineRenderer.Points = _points;
            LineRenderer.color = Color;
            Caption.color = new Color(Color.r, Color.g, Color.b, 1f);
            _range = Range;
        }

        private void Update() {
            for (int i = 0; i < NumPoints - 1; ++i) {
                _points[i].y = _points[i + 1].y;
            }

            float newValue = Value + Random.Range(-_range, _range);
            _points[NumPoints - 1].y = newValue;
            LineRenderer.Points = _points;
            LineRenderer.SetVerticesDirty();
        }
    }
}