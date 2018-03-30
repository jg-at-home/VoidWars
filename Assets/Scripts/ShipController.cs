using UnityEngine;
using UnityEngine.Networking;

namespace VoidWars {
    public class ShipController : NetworkBehaviour {
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
        public ControlType ControlType {
            get { return _controlType; }
            set { _controlType = value; }
        }

        /// <summary>
        /// Get / set the faction the ship belongs to.
        /// </summary>
        public Faction Faction {
            get { return _faction; }
            set { _faction = value; }
        }

        /// <summary>
        /// Make this ship the active one.
        /// </summary>
        public void Activate() {
            Debug.Assert(_controlState == ControlState.IDLE);
            _controlState = ControlState.ACTIVE;
        }

        /// <summary>
        /// Deactivate this ship.
        /// </summary>
        public void Deactivate() {
            Debug.Assert(_controlState == ControlState.ACTIVE);
            _controlState = ControlState.IDLE;
        }

        private void Awake() {
            _controlState = ControlState.UNINITIALIZED;
        }

        public override void OnStartClient() {
            base.OnStartClient();

            // SyncVars should be good now.
            _controlState = ControlState.READY;
            _gameController = Util.GetGameController();
            _gameController.RegisterShip(this);
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
                    _pilot.UpdateShip(_gameController, this);
                    break;
            }
        }

        private void createPilot() {
            switch(_controlType) {
                case ControlType.HUMAN:
                    _pilot = new HumanPilot();
                    break;

                case ControlType.AI:
                    _pilot = new AIPilot();
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

        [SyncVar] private ControlType _controlType;
        [SyncVar] private ControlState _controlState;
        [SyncVar] private Faction _faction;
        private Pilot _pilot;
        private GameController _gameController;
    }
}