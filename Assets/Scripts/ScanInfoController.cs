using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VoidWars {
    public class ScanInfoController : MonoBehaviour {
        [SerializeField] TextMeshProUGUI _shipNameText;
        [SerializeField] Image _shieldsBar;
        [SerializeField] Image _energyBar;
        [SerializeField] Image _healthBar;
        [SerializeField] private Vector3 _offset;

        private void Start() {
            _shipController = GetComponentInParent<ShipController>();
            _shipNameText.text = _shipController.ShipData.Name;
        }

        private void Update() {
            _energyBar.fillAmount = _shipController.Energy / _shipController.MaxEnergy;
            _healthBar.fillAmount = _shipController.Health / _shipController.MaxHealth;
            _shieldsBar.fillAmount = _shipController.ShieldPercent / 100f;
            var color = _shieldsBar.color;
            if (_shipController.ShieldsActive) {
                color.a = 1f;
            }
            else {
                color.a = 0.5f;
            }
            _shieldsBar.color = color;
        }

        private void LateUpdate() {
            // Don't rotate the panel along with the ship.
            transform.position = _shipController.gameObject.transform.position + _offset;
            transform.rotation = Quaternion.identity;
        }
        private ShipController _shipController;
    }
}