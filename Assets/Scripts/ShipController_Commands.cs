using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    /// <summary>
    /// Command and action functions for the ship controller class. CLIENT-SIDE only.
    /// </summary>
    public partial class ShipController {
        /// <summary>
        /// Gets a list of all the actions the ship can perform.
        /// </summary>
        /// <returns>The list of actions.</returns>
        public List<ActionItem> GetAvailableActions() {
            var actions = new List<ActionItem>();

            // Energy related
            actions.Add(new ActionItem {
                Action = "energy",
                Description = "Reroute system power",
                Detail = "Change the balance of power in your ship to suit your needs",
                Icon = ImageManager.GetImage("EnergyIcon"),
                EditorPrefabInfo = "RerouteEnergyPanel"
            });

            // Shields.
            if (_shieldPercent > 0f) {
                if (_shieldState == AuxState.Operational) {
                    // Disable shields.
                    actions.Add(new ActionItem {
                        Action = "shields false",
                        Description = "Lower shields",
                        Detail = "Drop your shields if you're feeling safe",
                        Icon = ImageManager.GetImage("ShieldsIcon"),
                        EditorPrefabInfo = "ImageDetailPanel ShieldsImage"
                    });
                }
                else if ((_shieldState == AuxState.Idle) && (ShieldEnergy >= _data.ShieldDrainRate)) {
                    actions.Add(new ActionItem {
                        Action = "shields true",
                        Description = "Raise shields",
                        Detail = "Raise shields and protect yourself from unwanted destruction",
                        Icon = ImageManager.GetImage("ShieldsIcon"),
                        EditorPrefabInfo = "ImageDetailPanel ShieldsImage"
                    });
                }
            }

            // Propulsion.
            if ((_propulsionState == AuxState.Broken) && _canRepairItems) {
                actions.Add(new ActionItem {
                    Action = "repair propulsion",
                    Description = "Repair broken engines",
                    Detail = string.Format("Repairs your critically damaged engines in {0} turns", _data.EngineRepairTurns),
                    Icon = ImageManager.GetImage("RepairIcon"),
                    EditorPrefabInfo = "ImageDetailPanel BrokenEnginesImage"
                });
            }

            // Auxiliary items.
            for(var i = 0; i < _equipment.Count; ++i) {
                var auxItem = _equipment[i];
                var auxState = auxItem.State;
                if (auxItem.Mode == AuxMode.Switchable) {
                    if (auxState == AuxState.Operational) {
                        actions.Add(new ActionItem {
                            Action = string.Format("aux {0} false", auxItem.ItemType),
                            Description = string.Format("Disable {0}", auxItem.Description),
                            Detail = auxItem.Detail,
                            Icon = auxItem.Icon,
                            EditorPrefabInfo = string.Format("ImageDetailPanel {0}Image", auxItem.ItemType)
                        });
                    }
                    else if ((auxState == AuxState.Idle) && (_energy >= auxItem.PowerUsage)) {
                        actions.Add(new ActionItem {
                            Action = string.Format("aux {0} true", auxItem.ItemType),
                            Description = string.Format("Enable {0}", auxItem.Description),
                            Detail = auxItem.Detail,
                            Icon = auxItem.Icon,
                            EditorPrefabInfo = string.Format("ImageDetailPanel {0}Image", auxItem.ItemType)
                        });
                    }
                }
                else if (auxItem.Mode == AuxMode.OneShot) {
                    if (_energy >= auxItem.PowerUsage) {
                        actions.Add(new ActionItem {
                            Action = string.Format("aux {0} true", auxItem.ItemType),
                            Description = string.Format("Use {0}", auxItem.Description),
                            Detail = auxItem.Detail,
                            Icon = auxItem.Icon,
                            EditorPrefabInfo = string.Format("ImageDetailPanel {0}Image", auxItem.ItemType)
                        });
                    }
                }
                else if (auxState == AuxState.Broken) {
                    if (_canRepairItems && (_energy >= auxItem.RepairCost)) {
                        actions.Add(new ActionItem {
                            Action = string.Format("repair {0}", auxItem.ItemType),
                            Description = string.Format("Repair {0}", auxItem.ItemType),
                            Detail = "Restore function of this device at some cost to your energy",
                            Icon = ImageManager.GetImage("RepairIcon"),
                            EditorPrefabInfo = "ImageDetailPanel RepairImage"
                        });
                    }
                }
            }

            // Abilities.
            foreach(var ability in _powerupAbilitiesClient) {
                var info = controller.GetPowerupInfo(ability);
                actions.Add(new ActionItem {
                    Action = info.CollectAction,
                    Description = info.Name,
                    Detail = info.DetailText,
                    Icon = info.Icon,
                    EditorPrefabInfo = "AbilityPanel"
                });
            }

            // You can always pass!
            actions.Add(new ActionItem {
                Action = "pass",
                Description = "Do nothing",
                Detail = "Skip this action",
                Icon = ImageManager.GetImage("PassIcon"),
                EditorPrefabInfo = "ImageDetailPanel PassImage"
            });
            return actions;
        }

        /// <summary>
        /// Executes an ability command and removes it from the ship.
        /// </summary>
        /// <param name="name">Ability name that spawned the command. Will be null for immediate effects.</param>
        /// <param name="args">Command arguments.</param>
        [Command]
        public void CmdExecuteAbility(string name, string args) {
            ExecuteCommand(args);
            var index = _powerupAbilitiesServer.FindIndex(a => a.PowerupID == name);
            _powerupAbilitiesServer.RemoveAt(index);
            _powerupAbilitiesClient.RemoveAt(index);
        }

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="args">Command args.</param>
        [Server]
        public void ExecuteCommand(string args) {
            Debug.LogFormat("Server: Ship.ExecuteCommand({0})", args);
            var parts = args.Split(' ');
            switch (parts[1]) {
                case "health": 
                    _health = changeParameter(_health, parts[2], 1f, MaxHealth, 'H');
                    break;

                case "energy":
                    _energy = changeParameter(_energy, parts[2], 1f, MaxEnergy, 'E');
                    break;

                case "shields":
                    _shieldPercent = changeParameter(_shieldPercent, parts[2], 1f, 100f, 'S');
                    break;

                case "MaxDamage":
                    // Applied as buffs.
                    maybeAdjustWeapon(_primaryWeapon, parts);
                    maybeAdjustWeapon(_secondaryWeapon, parts);
                    break;

                case "penetrate-shields":
                    SetStatusFlag(StatusFlag.PenetrateShields);
                    break;
            }
        }

        private void maybeAdjustWeapon(WeaponInstance weapon, string [] parts) {
            if (weapon != null) {
                var param = parts[1].Trim();
                var deltaStr = parts[2].Trim();
                BuffType buffType;
                float value;
                if (deltaStr[deltaStr.Length - 1] == '%') {
                    buffType = BuffType.Percentage;
                    value = float.Parse(deltaStr.Substring(0, deltaStr.Length - 1));
                }
                else {
                    buffType = BuffType.Additive;
                    value = float.Parse(deltaStr);
                }
                var buff = new Buff(param, buffType, value);
                weapon.AddBuff(buff);

                if (parts.Length == 4) {
                    // Has a duration in terms of turns.
                    var duration = int.Parse(parts[3]);
                    var removeBuffTask = new Task(duration, () => weapon.RemoveBuff(buff));
                    _tasks.Add(removeBuffTask);
                }
            }
        }

        private float changeParameter(float oldValue, string deltaStr, float maxValue, float minValue, char symbol) {
            var delta = getDelta(deltaStr, oldValue);
            var newValue = Mathf.Clamp(oldValue + delta, minValue, maxValue);
            var color = delta < 0f ? Color.red : Color.green;
            var msg = string.Format("{0}:{1:+#;-#}", symbol, (int)(newValue - oldValue));
            RpcShowPopupIndicator(msg, color);
            return newValue;
        }

        private static float getDelta(string amount, float current) {
            float delta;
            if (amount[amount.Length - 1] == '%') {
                amount = amount.Substring(0, amount.Length - 1);
                var percent = float.Parse(amount);
                delta = (current * percent / 100f);
            }
            else {
                delta = float.Parse(amount);
            }

            return delta;
        }

        [ClientRpc]
        void RpcShowPopupIndicator(string text, Color color) {
            controller.ShowPopupIndicator(ID, text, color);
        }
    }
}