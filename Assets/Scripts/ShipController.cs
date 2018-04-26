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
        /// Gets the ship's health level.
        /// </summary>
        public float Health {
            get { return _health; }
        }

        /// <summary>
        /// Gets the ship's hull temperature in arbitrary [0,100] range.
        /// </summary>
        public float HullTemperature {
            get { return _hullTemperature; }
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
            get { return (_shieldEnergy >= _class.ShieldDrainRate) && (_shieldPercent > 0); }
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
        /// Gets the number of auxiliary items equipped.
        /// </summary>
        /// <returns>The aux count.</returns>
        public int GetAuxiliaryCount() {
            return _equipment.Count;
        }

        /// <summary>
        /// Gets an aux by index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The requested aux.</returns>
        public AuxiliaryItem GetAuxiliaryItem(int index) {
            return _equipment[index];
        }

        /// <summary>
        /// Gets the weapon type for the slot (0=primary, 1 = secondary)
        /// </summary>
        /// <param name="slot">Slot index.</param>
        /// <returns>Weapon type.</returns>
        public WeaponType GetWeaponType(int slot) {
            Debug.Assert(slot == 0 || slot == 1);

            return (slot == 0) ? PrimaryWeaponType : SecondaryWeaponType;
        }

        /// <summary>
        /// Gets the weapon node for the slot (0=primary, 1 = secondary)
        /// </summary>
        /// <param name="slot">Slot index.</param>
        /// <returns>Weapon node.</returns>
        public Transform GetWeaponNode(int slot) {
            return (slot == 0) ? FrontNode : RearNode;
        }

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
        /// Called server-side to change the shield status.
        /// </summary>
        /// <param name="enable">Enable / disable the shields.</param>
        public void EnableShields(bool enable) {
            Debug.LogFormat("ShipController.EnableShields({0})", enable);

            if (_shieldsActive != enable) {
                _shieldsActive = enable;
                if (enable) {
                    _powerDrain += _class.ShieldDrainRate;
                }
                else {
                    _powerDrain -= _class.ShieldDrainRate;
                }
            }            
        }

        /// <summary>
        /// Are the shields active?
        /// </summary>
        public bool ShieldsActive {
            get { return _shieldsActive; }
        }

        private void onShieldStatusChanged(bool status) {
            Debug.LogFormat("ShipController.onShieldStatusChanged({0})", status);

            var shields = gameObject.GetComponent<ForceField3Y3>();
            if (status) {
                shields.SetEffectOn();
            }
            else {
                shields.SetEffectOff();
            }
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
        /// Computes and applies damage to the ship.
        /// </summary>
        /// <param name="damage">The amount of damage to apply.</param>
        /// <param name="dT">The temperature effect/</param>
        /// <returns>The amount of damage done</returns>
        public float ComputeDamage(float damage, float dT) {
            Debug.LogFormat("Ship ID {0} took {1} damage", ID, damage);

            // How much the shields reduce damage by at 100% when energy is nominally distributed (25%)
            const float MaxShieldEfficiency = 0.8f;
            if (ShieldsActive) {
                var shieldFrac = _shieldPercent / 100f;
                var shieldEfficiency = MaxShieldEfficiency * (_energyBudget.Available(EnergyConsumer.Shields)/0.25f);
                var shieldReduction = Mathf.Clamp01(shieldFrac * shieldEfficiency);
                damage *= (1f - shieldReduction);
                dT *= (1f - shieldReduction);
                
                _shieldPercent -= damage;
                if (_shieldPercent <= 0) {
                    Debug.LogFormat("Ship #{0}'s shields have -failed-", ID);
                    // TODO: notify player shields have failed.
                    _shieldsActive = false;
                    damage = -_shieldPercent;
                    _shieldPercent = 0;
                }
            }

            // Apply any heat to the ship.
            _hullTemperature = Mathf.Clamp(_hullTemperature + dT, 0f, 100f);

            // Whatever damage is left after the shields goes into the hull.
            _health -= damage;
            if (_health <= 0) {
                Debug.LogFormat("Ship {0} is DEAD!!!!", ID);
                // TODO: explodify ship, remove from game, etc.
            }

            return damage;
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

            // Hull temperature.
            updateHullTemperature();

            // Other systems.
            updateSystemStatuses();
        }

        private void updateSystemStatuses() {
            // Check systems.
            if (_shieldPercent > 0f) {
                _shieldEnergy = GetEnergyBudgetFor(EnergyConsumer.Shields);
                if (_shieldsOK) {
                    if (_shieldEnergy < _class.ShieldDrainRate) {
                        // Shields have failed.
                        _shieldsOK = false;
                        var gameController = Util.GetGameController();
                        gameController.OnShieldsFailed(this);
                    }
                }
                else {
                    if (_shieldEnergy >= _class.ShieldDrainRate) {
                        _shieldsOK = true;
                        Debug.Log("Shields online");
                    }
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

        private void updateHullTemperature() {
            var temperature = _hullTemperature;
            var suns = GameObject.FindGameObjectsWithTag("Sun");
            if (suns.Length > 0) {
                var shieldFraction = _energyBudget.Available(EnergyConsumer.Shields);
                var e = 0.8f * shieldFraction + 0.55f;
                var a = ShieldsActive ? (1f - e * _shieldPercent / 100f) : 1f;
                const float K = 175f;
                const float R = 3f;
                foreach (var sun in suns) {
                    var r = Vector3.Distance(sun.transform.position, gameObject.transform.position);
                    var dT = a * K / Mathf.Pow(r / R + 1f, 2);
                    temperature += dT;
                }
            }

            temperature -= _coolingRate;
            _hullTemperature = Mathf.Clamp(temperature, 0f, 100f);              
        }

        private void onHullTemperatureChanged(float temperature) {
            Debug.LogFormat("Ship #{0}: T_hull = {1}", ID, temperature);
            // Disable items above their max T (and re-enable those under it).
            foreach (var auxItem in _equipment) {
                if (auxItem.State == AuxState.Operational) {
                    if (temperature >= auxItem.Class.MaxTemperature) {
                        // TODO: notification
                        auxItem.State = AuxState.Overheated;
                        unapplyAuxiliary(auxItem.Class);
                    }
                }
                else if (auxItem.State == AuxState.Overheated) {
                    if (temperature < auxItem.Class.MaxTemperature) {
                        // TODO: notification
                        auxItem.State = AuxState.Operational;
                        applyAuxiliary(auxItem.Class);
                    }
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
        /// Enable / disable the engine FX.
        /// </summary>
        /// <param name="enable">Status flag.</param>
        public void EnableEngineFX(bool enable) {
            foreach(var engine in _engineFX) {
                engine.SetActive(enable);
            }
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
            _engineFX = Util.FindChildrenWithTag(gameObject, "Engine");
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
            _health = _class.MaxHealth;
            _coolingRate = _class.CoolingRate;

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
                    var auxItem = new AuxiliaryItem(auxClass);
                    _equipment.Add(auxItem);
                    _totalMass += auxClass.Mass;
                    if (auxClass.Mode == AuxMode.Continuous) {
                        auxItem.State = AuxState.Operational;
                        applyAuxiliary(auxClass);
                    }
                }
            }

            // Can set parameters affected by aux items now.
            _energy = _maxEnergy;

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
            // Everything has an effect on power.
            _powerDrain += aux.PowerUsage;

            // Specific effects here.
            switch(aux.ItemType) {
                case AuxType.PowerCell:
                    _maxEnergy += float.Parse(aux.Metadata);
                    break;

                case AuxType.DriveBoost:
                    _maxMoveSize = int.Parse(aux.Metadata);
                    break;

                case AuxType.CoolingElement:
                    _coolingRate += float.Parse(aux.Metadata);
                    break;
                // TODO: other stuff.
            }
        }

        private void unapplyAuxiliary(AuxiliaryClass aux) {
            // Everything has an effect on power.
            _powerDrain -= aux.PowerUsage;

            // Specific effects here.
            switch (aux.ItemType) {
                case AuxType.PowerCell:
                    _maxEnergy -= float.Parse(aux.Metadata);
                    if (_energy > _maxEnergy) {
                        _energy = _maxEnergy;
                    }
                    break;

                case AuxType.DriveBoost:
                    _maxMoveSize -= int.Parse(aux.Metadata);
                    break;

                case AuxType.CoolingElement:
                    _coolingRate -= float.Parse(aux.Metadata);
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
        [SyncVar(hook="onShieldStatusChanged")] private bool _shieldsActive;
        [SyncVar] private bool _cloakActive;
        [SyncVar] private float _lifeSupportLevel;
        [SyncVar] private float _propulsionLevel;
        [SyncVar] private float _shieldEnergy;
        [SyncVar] private float _weaponsLevel;
        [SyncVar] private float _shieldPercent;
        [SyncVar(hook="onHullTemperatureChanged")] private float _hullTemperature;
        [SyncVar] private float _health;
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
        private float _coolingRate;
        private EnergyBudget _energyBudget;
        private int _maxMoveSize = 3;
        private bool _canRepairItems;
        private int _actionsThisTurn = 1;
        private GameObject[] _engineFX;
    }
}