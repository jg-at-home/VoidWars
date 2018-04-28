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
            if (_shieldsOK && (_shieldPercent > 0f)) {
                if (_shieldsActive) {
                    // Disable shields.
                    actions.Add(new ActionItem {
                        Action = "shields false",
                        Description = "Lower shields",
                        Detail = "Drop your shields if you're feeling safe",
                        Icon = ImageManager.GetImage("ShieldsIcon"),
                        EditorPrefabInfo = "ImageDetailPanel ShieldsImage"
                    });
                }
                else if (ShieldEnergy >= _class.ShieldDrainRate) {
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
            foreach(var aux in _equipment) {
                if (aux.Class.Mode == AuxMode.Switchable) {
                    if (aux.State == AuxState.Operational) {
                        actions.Add(new ActionItem {
                            Action = string.Format("aux {0} false", aux.Class.ItemType),
                            Description = string.Format("Disable {0}", aux.Class.Description),
                            Detail = aux.Class.Detail,
                            Icon = aux.Class.Icon,
                            EditorPrefabInfo = string.Format("ImageDetailPanel {0}Image", aux.Class.ItemType)
                        });
                    }
                    else if ((aux.State == AuxState.Idle) && (_energy >= aux.Class.PowerUsage)) {
                        actions.Add(new ActionItem {
                            Action = string.Format("aux {0} true", aux.Class.ItemType),
                            Description = string.Format("Enable {0}", aux.Class.Description),
                            Detail = aux.Class.Detail,
                            Icon = aux.Class.Icon,
                            EditorPrefabInfo = string.Format("ImageDetailPanel {0}Image", aux.Class.ItemType)
                        });
                    }
                }
                else if (aux.Class.Mode == AuxMode.OneShot) {
                    if (_energy >= aux.Class.PowerUsage) {
                        actions.Add(new ActionItem {
                            Action = string.Format("aux {0} true", aux.Class.ItemType),
                            Description = string.Format("Use {0}", aux.Class.Description),
                            Detail = aux.Class.Detail,
                            Icon = aux.Class.Icon,
                            EditorPrefabInfo = string.Format("ImageDetailPanel {0}Image", aux.Class.ItemType)
                        });
                    }
                }
                else if (aux.State == AuxState.Broken) {
                    if (_canRepairItems && (_energy >= aux.Class.RepairCost)) {
                        actions.Add(new ActionItem {
                            Action = string.Format("repair {0}", aux.Class.ItemType),
                            Description = string.Format("Repair {0}", aux.Class.ItemType),
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