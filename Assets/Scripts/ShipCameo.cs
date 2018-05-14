using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VoidWars {
    public class ShipCameo : MonoBehaviour {
        [SerializeField] private RawImage _image;
        [SerializeField] private TextMeshProUGUI _caption;

        public void Initialize(RenderTexture rt, string name) {
            _caption.text = name;
            _renderTexture = rt;
            _image.texture = rt;
        }

        private void OnDestroy() {
            _renderTexture.Release();    
        }

        private RenderTexture _renderTexture;
    }
}