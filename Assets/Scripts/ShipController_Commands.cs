using System.Collections.Generic;
using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Command and action functions for the ship controller class. CLIENT-SIDE only.
    /// </summary>
    public partial class ShipController {
        /// <summary>
        /// Executes the command string provided.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        public void ExecuteCommand(string command) {
            Debug.LogFormat("ShipController.ExecuteCommand{0})", command);

            // TODO
        }

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
                Detail = "Change the balance of power in your ships to suit your needs",
                Icon = ImageManager.GetImage("EnergyIcon"),
                EditorPrefabInfo = "RerouteEnergyPanel"
            });

            if (_energy < _maxEnergy) {
                actions.Add(new ActionItem {
                    Action = "recharge",
                    Description = "Recover some power",
                    Detail = "Regain some additional energy",
                    Icon = ImageManager.GetImage("EnergyIcon"),
                    EditorPrefabInfo = "OKPanel RechargeImage"
                });
            }

            // Shields.
            if (_shieldsOK) {
                if (_shieldsActive) {
                    // Disable shields.
                    actions.Add(new ActionItem {
                        Action = "shields false",
                        Description = "Lower shields",
                        Detail = "Drop your shields if you're feeling safe",
                        Icon = ImageManager.GetImage("ShieldsIcon"),
                        EditorPrefabInfo = "OKPanel ShieldsImage"
                    });
                }
                else if (ShieldEnergy >= _class.ShieldDrainRate) {
                    actions.Add(new ActionItem {
                        Action = "shields true",
                        Description = "Raise shields",
                        Detail = "Raise shields and protect yourself from unwanted destruction",
                        Icon = ImageManager.GetImage("ShieldsIcon"),
                        EditorPrefabInfo = "OKPanel ShieldsImage"
                    });
                }
            }

            // Auxiliary items.
            foreach(var aux in _equipment) {
                if (aux.IsFunctional) {
                    if (aux.Class.Mode == AuxMode.Switchable) {
                        if (aux.IsActive) {
                            actions.Add(new ActionItem {
                                Action = string.Format("aux {0} false", aux.Class.ItemType),
                                Description = string.Format("Disable {0}", aux.Class.Description),
                                Detail = aux.Class.Detail,
                                Icon = aux.Class.Icon,
                                EditorPrefabInfo = string.Format("OKPanel {0}Image", aux.Class.ItemType)
                            });
                        }
                        else if (_energy >= aux.Class.PowerUsage) {
                            actions.Add(new ActionItem {
                                Action = string.Format("aux {0} true", aux.Class.ItemType),
                                Description = string.Format("Enable {0}", aux.Class.Description),
                                Detail = aux.Class.Detail,
                                Icon = aux.Class.Icon,
                                EditorPrefabInfo = string.Format("OKPanel {0}Image", aux.Class.ItemType)
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
                                EditorPrefabInfo = string.Format("OKPanel {0}Image", aux.Class.ItemType)
                            });
                        }
                    }
                }
                else if (_canRepairItems && (_energy >= aux.Class.RepairCost)) {
                    actions.Add(new ActionItem {
                        Action = string.Format("repair {0}", aux.Class.ItemType),
                        Description = string.Format("Repair {0}", aux.Class.ItemType),
                        Detail = "Restore function of this device at some cost to your energy",
                        Icon = ImageManager.GetImage("RepairIcon"),
                        EditorPrefabInfo = string.Format("OKPanel RepairImage")
                    });
                }
            }

            return actions;
        }
    }
}