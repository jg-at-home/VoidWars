using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    public partial class ShipController : VoidNetworkBehaviour {
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
        /// Gets the number of actions the ship can perform this turn.
        /// </summary>
        public int ActionsThisTurn { get { return _actionsThisTurn; } }

        /// <summary>
        /// Gets the ship's energy budget.
        /// </summary>
        public EnergyBudget EnergyBudget {  get { return _energyBudget; } }

        /// <summary>
        /// Gets the amount of energy available for life support.
        /// </summary>
        public float LifeSupportEnergy {
            get { return _lifeSupportLevel; }
        }

        /// <summary>
        /// Gets the amount of energy available for weapons.
        /// </summary>
        public float WeaponsEnergy {
            get { return _weaponsLevel; }
        }

        /// <summary>
        /// Gets the amount of energy available for shields.
        /// </summary>
        public float ShieldEnergy {
            get { return _shieldEnergy; }
        }

        /// <summary>
        /// Gets the amount of energy available for propulsion.
        /// </summary>
        public float PropulsionEnergy {
            get { return _propulsionLevel; }
        }

        /// <summary>
        /// Life support working?
        /// </summary>
        public bool IsLifeSupportOK {
            get { return _lifeSupportLevel >= _class.LifeSupportDrainRate; }
        }

        /// <summary>
        /// Drive working?
        /// </summary>
        public bool IsPropulsionOK {
            get { return _propulsionLevel >= MassRatio * _class.MoveDrainRate; }
        }

        /// <summary>
        /// Primary weapons working?
        /// </summary>
        public bool IsPrimaryWeaponOK {
            get { return _weaponsLevel >= _primaryWeapon.PowerUsage; }
        }

        /// <summary>
        /// Secondary weapons working (if fitted).
        /// </summary>
        public bool IsSecondaryWeaponOK {
            get {
                if (_secondaryWeapon == null) {
                    return true;
                }
                else {
                    return _weaponsLevel >= _secondaryWeapon.PowerUsage;
                }
            }
        }

        /// <summary>
        /// Shields working?
        /// </summary>
        public bool IsShieldsOK {
            get { return _shieldEnergy >= _class.ShieldDrainRate; }
        }

        /// <summary>
        /// Gets the percentage efficacy of the shields.
        /// </summary>
        public float ShieldPercent {
            get { return _shieldPercent; }
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
        public float GetEnergyBudgetFor(EnergyConsumer consumer) {
            return _energy * _energyBudget.Available(consumer);
        }

        /// <summary>
        /// Increases the number of actions the ship can make on this turn.
        /// </summary>
        public void IncreaseActionsThisTurn() {
            Debug.Assert(_actionsThisTurn == 1);

            ++_actionsThisTurn;
        }

        /// <summary>
        /// Resets the action count for this turn to the default.
        /// </summary>
        public void ResetActionsThisTurn() {
            _actionsThisTurn = 1;
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
        /// Called by the server to begin a round.
        /// </summary>
        public void BeginRound(int round) {
            Debug.Log("ShipController.BeginRound()");

            // Drain some power, recharge some power.
            if (round > 0) {
                _energy = Mathf.Clamp(_energy + (_class.RechargeRate - _powerDrain), 0f, _maxEnergy);
            }

            updateSystemStatuses();
        }

        private void updateSystemStatuses() {
            // Check systems.
            _shieldEnergy = GetEnergyBudgetFor(EnergyConsumer.Shields);
            if (_shieldsOK) {
                if (_shieldEnergy < _class.ShieldDrainRate) {
                    // Shields have failed.
                    Debug.Log("Shields failed.");
                    _shieldsOK = false;
                    // TODO: turn shields off.
                }
            }
            else {
                if (_shieldEnergy >= _class.ShieldDrainRate) {
                    _shieldsOK = true;
                    Debug.Log("Shields online");
                }
            }

           _lifeSupportLevel = GetEnergyBudgetFor(EnergyConsumer.LifeSupport);
            if (_lifeSupportOK) {
                if (_lifeSupportLevel < _class.LifeSupportDrainRate) {
                    // Life support failed.
                    Debug.LogWarning("Life support failed!");
                    _roundsWithoutLifeSupport = 0;
                    _lifeSupportOK = false;
                }
            }
            else {
                if (_lifeSupportLevel >= _class.LifeSupportDrainRate) {
                    _lifeSupportOK = true;
                    Debug.Log("Life support online");
                }
                else {
                    ++_roundsWithoutLifeSupport;
                    Debug.LogWarningFormat("!!! Now {0} rounds without life support", _roundsWithoutLifeSupport);
                    // TODO: if >= limit, kill ship.
                }
            }

            _propulsionLevel = GetEnergyBudgetFor(EnergyConsumer.Propulsion);
            var minEnergyForMove = MassRatio * _class.MoveDrainRate;
            if (_propulsionOK) {
                if (_propulsionLevel < minEnergyForMove) {
                    _propulsionOK = false;
                    Debug.LogWarning("Propulsion failed");
                }
            }
            else if (_propulsionLevel >= minEnergyForMove) {
                Debug.Log("Propulsion online");
                _propulsionOK = true;
            }

            _weaponsLevel = GetEnergyBudgetFor(EnergyConsumer.Weapons);
            if (_primaryWeaponsOK) {
                if (_weaponsLevel < _primaryWeapon.PowerUsage) {
                    _primaryWeaponsOK = false;
                    Debug.LogWarning("Primary weapons unavailable");
                }
            }
            else if (_weaponsLevel >= _primaryWeapon.PowerUsage) {
                Debug.Log("Primary weapons online");
                _primaryWeaponsOK = true;
            }
            if (_secondaryWeapon != null) {
                if (_secondaryWeaponsOK) {
                    if (_weaponsLevel < _secondaryWeapon.PowerUsage) {
                        _secondaryWeaponsOK = false;
                        Debug.LogWarning("Secondary weapons unavailable");
                    }
                }
                else if (_weaponsLevel >= _secondaryWeapon.PowerUsage) {
                    Debug.Log("Secondary weapons online");
                    _secondaryWeaponsOK = true;
                }
            }
        }

        /// <summary>
        /// Called server-side when a ship is about to make a move.
        /// </summary>
        /// <param name="move">The move to make.</param>
        public void BeginMove(ShipMove move) {
            _energy -= GetEnergyForMove(move);
            Debug.Assert(_energy >= 0f, "Energy negative?");
            if (_energy < 0f) {
                _energy = 0f;
            }

            updateSystemStatuses();
        }

        /// <summary>
        /// Gets the energy required for a move. 
        /// </summary>
        /// <param name="move">The move to make.</param>
        /// <returns>The energy required.</returns>
        public float GetEnergyForMove(ShipMove move) {
            return MassRatio * _class.MoveDrainRate * move.EnergyScale;
        }

        /// <summary>
        /// Enacts a move for the ship. Called client-side.
        /// </summary>
        /// <param name="move">The move to execute.</param>
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
            _powerDrain = _class.LifeSupportDrainRate;
            _shieldPercent = 100.0f;

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
                    _equipment.Add(new AuxiliaryItem(auxClass));
                    _totalMass += auxClass.Mass;
                }
            }

            updateSystemStatuses();
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

            // Everything has an effect on power.
            _powerDrain += aux.PowerUsage;

            // Specific effects here.
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
        [SyncVar] private bool _shieldsActive;
        [SyncVar] private bool _cloakActive;
        [SyncVar] private float _lifeSupportLevel;
        [SyncVar] private float _propulsionLevel;
        [SyncVar] private float _shieldEnergy;
        [SyncVar] private float _weaponsLevel;
        [SyncVar] private float _shieldPercent;
        private bool _lifeSupportOK = true;
        private bool _primaryWeaponsOK = true;
        private bool _secondaryWeaponsOK = true;
        private bool _propulsionOK = true;
        private bool _shieldsOK = true;
        private int _roundsWithoutLifeSupport;
        private Pilot _pilot;
        private ShipClass _class;
        private WeaponClass _primaryWeapon;
        private WeaponClass _secondaryWeapon;
        private readonly List<AuxiliaryItem> _equipment = new List<AuxiliaryItem>();
        private int _totalMass;
        private float _powerDrain;
        private EnergyBudget _energyBudget;
        private int _maxMoveSize = 3;
        private bool _canRepairItems;
        private int _actionsThisTurn = 1;
    }
}