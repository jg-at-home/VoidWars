﻿
using UnityEngine.UI;

namespace VoidWars {
    public class AttackInfoPanelController : ContentPanelController {
        public Text WeaponText;

        private void OnEnable() {
            // Done = Skip the attack, so always available.
            EnableDoneButton(true);
            SetDoneButtonCaption("Skip");
        }

        public void OnTargetsRecomputed() {
            refreshInner(true);
        }

        private void refreshInner(bool active) {
            string text;
            if (active) {
                // It's the local ship.
                var gameController = Util.GetGameController();
                var shipController = gameController.GetActiveShip();
                if (shipController.IsCloaked) {
                    text = "You cannot attack whilst in stealth mode";
                    WeaponText.text = "---";
                }
                else if (shipController.PrimaryWeaponType == WeaponType.None &&
                    shipController.SecondaryWeaponType == WeaponType.None) {
                    text = "You have no active weapons; attack is going to be tricky, no?";
                    WeaponText.text = "---";
                }
                else {
                    var index = gameController.GetActiveWeaponIndex();
                    var weaponType = shipController.GetWeaponType(index);
                    var targets = gameController.AttackTargetCount;
                    var weapon = shipController.GetWeapon(index);
                    if (targets == 0) {
                        text = "There are no targets within range. Change weapons, or press 'Done' to finish";
                    }
                    else if (!weapon.IsAvailable) {
                        text = "Weapon not available this turn";
                    }
                    else {
                        var weaponClass = gameController.GetWeaponClass(weaponType);
                        if (shipController.GetEnergyBudgetFor(EnergyConsumer.Weapons) >= weaponClass.PowerUsage) {
                            text = "Click on a target to attack, or press 'Skip' to finish";
                        }
                        else {
                            text = "Insufficient energy to use this weapon";
                        }
                    }

                    WeaponText.text = Util.PascalToSpaced(weaponType.ToString());
                }
            }
            else {
                // It's the other guy's ship.
                text = "Please wait whilst opponent plots your demise";
            }
            SetInfoText(text);
        }

        public void OnActiveShipChanged(bool active) {
            refreshInner(active);
        }
    }
}