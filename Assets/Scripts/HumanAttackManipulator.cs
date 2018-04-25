using UnityEngine;

namespace VoidWars {
    public class HumanAttackManipulator : Manipulator {

        private void Start() {
            _shipController = gameObject.GetComponent<ShipController>();
            _weaponIndex = 0;

            _gameController = Util.GetGameController();
            _gameController.SetActiveWeapon(_weaponIndex);
        }

        private void Update() {
            // Left / right -> switch between primary and secondary weapons.
            if (_shipController.SecondaryWeaponType != WeaponType.None) {
                if (Input.GetKeyDown(KeyCode.LeftArrow) ||
                    Input.GetKeyDown(KeyCode.RightArrow)) {
                    _weaponIndex = 1 - _weaponIndex;
                    _gameController.SetActiveWeapon(_weaponIndex);
                }
            }

            if (_gameController.AttackTargetCount > 0) {
                if (Input.GetMouseButtonDown(0)) {
                    // Left mouse button pressed, check to see if we hit owt.
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100.0f)) {
                        var rootObj = hit.collider.transform.root.gameObject;
                        Debug.Log("You clicked on " + rootObj.name);
                        // TODO: allow us to hit other things, too!
                        var ship = rootObj.GetComponent<ShipController>();
                        if (ship != null) {
                            _gameController.SelectTarget(rootObj);
                        }
                    }
                }
            }
        }

        private GameController _gameController;
        private ShipController _shipController;
        private int _weaponIndex;
    }
}