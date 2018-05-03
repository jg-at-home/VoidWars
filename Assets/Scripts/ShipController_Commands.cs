using System.Collections.Generic;

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

            if (_energy < _maxEnergy) {
                actions.Add(new ActionItem {
                    Action = "recharge",
                    Description = "Recover some power",
                    Detail = "Regain some additional energy",
                    Icon = ImageManager.GetImage("EnergyIcon"),
                    EditorPrefabInfo = "ImageDetailPanel RechargeImage"
                });
            }

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
                else if ((_shieldState == AuxState.Idle) && (ShieldEnergy >= _class.ShieldDrainRate)) {
                    actions.Add(new ActionItem {
                        Action = "shields true",
                        Description = "Raise shields",
                        Detail = "Raise shields and protect yourself from unwanted destruction",
                        Icon = ImageManager.GetImage("ShieldsIcon"),
                        EditorPrefabInfo = "ImageDetailPanel ShieldsImage"
                    });
                }
            }

            // Auxiliary items.
            for(var i = 0; i < _equipment.Count; ++i) {
                var auxClass = _equipment[i].Class;
                var auxState = _equipment[i].State;
                if (auxClass.Mode == AuxMode.Switchable) {
                    if (auxState == AuxState.Operational) {
                        actions.Add(new ActionItem {
                            Action = string.Format("aux {0} false", auxClass.ItemType),
                            Description = string.Format("Disable {0}", auxClass.Description),
                            Detail = auxClass.Detail,
                            Icon = auxClass.Icon,
                            EditorPrefabInfo = string.Format("ImageDetailPanel {0}Image", auxClass.ItemType)
                        });
                    }
                    else if ((auxState == AuxState.Idle) && (_energy >= auxClass.PowerUsage)) {
                        actions.Add(new ActionItem {
                            Action = string.Format("aux {0} true", auxClass.ItemType),
                            Description = string.Format("Enable {0}", auxClass.Description),
                            Detail = auxClass.Detail,
                            Icon = auxClass.Icon,
                            EditorPrefabInfo = string.Format("ImageDetailPanel {0}Image", auxClass.ItemType)
                        });
                    }
                }
                else if (auxClass.Mode == AuxMode.OneShot) {
                    if (_energy >= auxClass.PowerUsage) {
                        actions.Add(new ActionItem {
                            Action = string.Format("aux {0} true", auxClass.ItemType),
                            Description = string.Format("Use {0}", auxClass.Description),
                            Detail = auxClass.Detail,
                            Icon = auxClass.Icon,
                            EditorPrefabInfo = string.Format("ImageDetailPanel {0}Image", auxClass.ItemType)
                        });
                    }
                }
                else if (auxState == AuxState.Broken) {
                    if (_canRepairItems && (_energy >= auxClass.RepairCost)) {
                        actions.Add(new ActionItem {
                            Action = string.Format("repair {0}", auxClass.ItemType),
                            Description = string.Format("Repair {0}", auxClass.ItemType),
                            Detail = "Restore function of this device at some cost to your energy",
                            Icon = ImageManager.GetImage("RepairIcon"),
                            EditorPrefabInfo = "ImageDetailPanel RepairImage"
                        });
                    }
                }
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