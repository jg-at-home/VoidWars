﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    public class ShipController : VoidNetworkBehaviour {
        /// <summary>
        /// Value indicating that an AI rather than a human controls the ship
        /// </summary>
        public const int AI_OWNER = -1;

        /// <summary>
        /// Gets the unique ID of the ship.
        /// </summary>
        public int ID {
            get {
                return (int)netId.Value;
            }
        }

        /// <summary>
        /// Get / set the type of control on this ship (human or AI).
        /// </summary>
        [SyncVar]
        public ControlType ControlType;

        /// <summary>
        /// Get / set the faction the ship belongs to.
        /// </summary>
        [SyncVar]
        public Faction Faction; 

        /// <summary>
        /// Get / set the index of the start point for the ship.
        /// </summary>
        [SyncVar]
        public int StartPointIndex;

        /// <summary>
        /// Get / set the ID of the player who owns the ship.
        /// </summary>
        [SyncVar]
        public int OwnerID; 

        /// <summary>
        /// Gets the class ID of the ship. The game controller can look up the specifics
        /// using that ID.
        /// </summary>
        [SyncVar]
        public string ClassID;

        /// <summary>
        /// Bitmask describing which weapons are equipped on the ship.
        /// </summary>
        [SyncVar]
        public int WeaponMask;

        /// <summary>
        /// Make this ship the active one.
        /// </summary>
        public void Activate() {
            Debug.LogFormat("ShipController.Activate({0})", ID);

            Debug.Assert(_controlState == ControlState.IDLE);

            _controlState = ControlState.ACTIVE;
            if (_pilot != null) {
                _pilot.OnShipActivation(controller, this);
            }
        }

        /// <summary>
        /// Deactivate this ship.
        /// </summary>
        public void Deactivate() {
            Debug.LogFormat("ShipController.Deactivate({0})", ID);

            Debug.Assert(_controlState == ControlState.ACTIVE);
            _controlState = ControlState.IDLE;
            if (_pilot != null) {
                _pilot.OnShipDeactivation(controller, this);
            }
        }

        private void Awake() {
            _controlState = ControlState.UNINITIALIZED;
        }

        public override void OnStartClient() {
            base.OnStartClient();

            // SyncVars should be good now.
            createPilot();
            controller.RegisterShip(this);
            _controlState = ControlState.IDLE;
            _class = controller.GetShipClassByName(ClassID);
        }

        private void Start() {

        }

        private void Update() {
            if (hasAuthority) {
                updateInner();
            }
        }

        private void updateInner() {
            switch (_controlState) {
                case ControlState.UNINITIALIZED:
                case ControlState.DESTROYED:
                case ControlState.IDLE:
                    break;

                case ControlState.READY:
                    // Create the pilot subclass that will manipulate this guy.
                    createPilot();
                    _controlState = ControlState.IDLE;
                    break;

                case ControlState.ACTIVE:
                    if (_pilot != null) {
                        _pilot.UpdateShip(controller, this);
                    }
                    break;
            }
        }

        private void createPilot() {
            switch(ControlType) {
                case ControlType.HUMAN:
                    Debug.LogFormat("ShipController: creating human pilot for ship {0}", ID);
                    _pilot = new HumanPilot();
                    break;

                case ControlType.AI:
                    Debug.LogFormat("ShipController: creating AI pilot for ship {0}", ID);
                    _pilot = new AIPilot();
                    break;

                case ControlType.NONE:
                    Debug.LogFormat("ShipController: ship {0} is uncontrolled", ID);
                    break;

                default:
                    Debug.Assert(false, "This shouldn't happen");
                    break;
            }
        }

        private enum ControlState {
            UNINITIALIZED,
            READY,
            IDLE,
            ACTIVE,
            DESTROYED
        }

        [SyncVar] private ControlState _controlState;
        private Pilot _pilot;
        private ShipClass _class;
        private WeaponClass _primaryWeapon;
        private WeaponClass _secondaryWeapon;
        private readonly List<AuxiliaryClass> _equipment = new List<AuxiliaryClass>();
    }
}