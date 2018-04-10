using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    public class ShipController : VoidNetworkBehaviour {
        /// <summary>
        /// Front node where movement indicators attach.
        /// </summary>
        public Transform FrontNode;

        /// <summary>
        /// Rear node where indicators attach.
        /// </summary>
        public Transform RearNode;

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
        /// Primary weapon type.
        /// </summary>
        [SyncVar]
        public WeaponType PrimaryWeaponType;

        /// <summary>
        /// Secondary weapon type.
        /// </summary>
        [SyncVar]
        public WeaponType SecondaryWeaponType;

        /// <summary>
        /// Bitmask describing what auxiliary equipment is fitted to the ship.
        /// </summary>
        [SyncVar]
        public int EquipmentMask;

        /// <summary>
        /// Gets the amount of energy the ship has.
        /// </summary>
        public float Energy {
            get { return _energy; }
        }

        /// <summary>
        /// Gets the max amount of energy the ship can have.
        /// </summary>
        public float MaxEnergy {
            get { return _maxEnergy; }
        }

        /// <summary>
        /// Gets the ship's static class data.
        /// </summary>
        public ShipClass ShipClass {
            get { return _class; }
        }

        /// <summary>
        /// Gets the amount by which equipping the ship has increased mass.
        /// </summary>
        public float MassRatio {
            get {
                return( _totalMass / _class.Mass );
            }
        }

        /// <summary>
        /// Gets the maximum move size for the ship.
        /// </summary>
        public int MaxMoveSize {
            get { return _maxMoveSize; }
        }

        /// <summary>
        /// How long it takes to move a single unit.
        /// </summary>
        public float MoveDuration = 1.0f;

        /// <summary>
        /// Signature for move finished callback.
        /// </summary>
        /// <param name="shipID">ID of shi.</param>
        public delegate void OnMoveFinished(int shipID);

        /// <summary>
        /// Gets the energy available for the given consumer.
        /// </summary>
        /// <param name="consumer">The consumer.</param>
        /// <returns>The amount of available energy for that thing.</returns>
        public float GetEnergyFor(EnergyConsumer consumer) {
            return _energy * _energyBudget.Available(consumer);
        }

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

        /// <summary>
        /// Enacts a move for the ship.
        /// </summary>
        /// <param name="move"></param>
        public void EnactMove(ShipMoveInstance move, OnMoveFinished finishedHandler) {
            StartCoroutine(enactMove(move, finishedHandler));
        }

        private IEnumerator enactMove(ShipMoveInstance move, OnMoveFinished finishedHandler) {
            var points = new List<Vector3>();
            points.Add(gameObject.transform.position);

            // Locate the move template. Points are in local space so need to be transformed.
            var moveTemplate = controller.GetMoveTemplate(move.Move);
            var node = (moveTemplate.MoveType == MoveType.Reverse) ? RearNode: FrontNode;
            var renderer = moveTemplate.GetComponent<LineRenderer>();
            for (int i = 0; i < renderer.positionCount; ++i) {
                var positionLocal = renderer.GetPosition(i);
                var positionWorld = node.transform.TransformPoint(positionLocal);
                points.Add(positionWorld);
            }

            // Create a curve to follow, and follow it.
            var curve = new PiecewiseLinearCurve(points);
            var duration = MoveDuration * move.Move.Size;
            Vector3 position;
            Quaternion rotation;
            var rb = gameObject.GetComponent<Rigidbody>();
            for (var t = 0f; t < duration; t += Time.deltaTime) {
                var s = t / duration;
                curve.GetPositionAndRotation(s, out position, out rotation);
                rb.position = position;
                rb.rotation = rotation;
                yield return null;
            }

            // Snap to end values.
            rb.position = move.Position;
            rb.rotation = move.Rotation;

            // Notify we're done.
            finishedHandler(ID);
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

            // Set initial values from class.
            _energyBudget = new EnergyBudget();
            _energy = _class.MaxEnergy;
            _maxEnergy = _energy;
            _powerDrain = 0.0f;

            // Figure out the total mass from the constituent bits - weapons and equipment.
            _totalMass = _class.Mass;
            Debug.Assert(PrimaryWeaponType != WeaponType.None);
            _primaryWeapon = controller.GetWeaponClass(PrimaryWeaponType);
            _totalMass += _primaryWeapon.Mass;
            if (SecondaryWeaponType != WeaponType.None) {
                _secondaryWeapon = controller.GetWeaponClass(SecondaryWeaponType);
                _totalMass += _secondaryWeapon.Mass;
            }

            var mask = 1;
            for(int i = 0; i < sizeof(int)*8; ++i) {
                if ((EquipmentMask & mask) != 0) {
                    // Auxiliary device is equipped.
                    var auxClass = controller.ItemClasses[i];
                    _equipment.Add(auxClass);
                    _totalMass += auxClass.Mass;
                }
            }
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

        private void applyAuxiliary(AuxiliaryClass aux) {
            // Everything adds a bit of mass.
            _totalMass += aux.Mass;
            _powerDrain += aux.PowerUsage;
            switch(aux.ItemType) {
                case AuxType.PowerCell:
                    _energy += float.Parse(aux.Metadata);
                    _maxEnergy = _energy;
                    break;

                case AuxType.DriveBoost:
                    _maxMoveSize = int.Parse(aux.Metadata);
                    break;

                // TODO: other stuff.

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
        [SyncVar] private float _energy;
        [SyncVar] private float _maxEnergy;
        private Pilot _pilot;
        private ShipClass _class;
        private WeaponClass _primaryWeapon;
        private WeaponClass _secondaryWeapon;
        private readonly List<AuxiliaryClass> _equipment = new List<AuxiliaryClass>();
        private int _totalMass;
        private float _powerDrain;
        private EnergyBudget _energyBudget;
        private int _maxMoveSize = 3;
    }
}