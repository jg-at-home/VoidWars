﻿using System.Collections.Generic;

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
    }
}