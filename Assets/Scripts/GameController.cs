using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VoidWars {
    /// <summary>
    /// The state the game is in.
    /// </summary>
    public enum GameState {
        UNINITIALIZED,
        LOBBY,
        CONFIGURATION,
        WAIT_FOR_SPAWN,
        SETUP,
        IN_PLAY,
        FINISHED
    }

    /// <summary>
    /// The phase of play in the IN_PLAY state.
    /// </summary>
    public enum PlayPhase {
        IDLE,
        MOVING_SHIP,
        TAKING_ACTION,
        ATTACKING,
        // TODO: other stuff.
    }

    /// <summary>
    /// Class responsible for coordinating the game events.
    /// </summary>
    public partial class GameController : MonoBehaviour {
        public GameObject[] StartPositions;
        public Color[] FactionColors;
        public GameConfig Configuration;
        public UnityEvent<GameState> StateChangeEvent;
        public UnityEvent<PlayPhase> PhaseChangeEvent;
        public GameObject ActiveShipIndicator;
        public InfoPanelController InfoPanel;

        /// <summary>
        /// Gets the current game state.
        /// </summary>
        public GameState State {
            get { return _state; }
        }

        /// <summary>
        /// Gets the region where a player at the given start position can move their ship during setup.
        /// </summary>
        /// <param name="spawnPointIndex">The index of the ship's spawn point.</param>
        /// <returns></returns>
        public Rect GetStartPositionBoundary(int spawnPointIndex) {
            var spawnObj = StartPositions[spawnPointIndex];
            var spawnPoint = spawnObj.GetComponent<SpawnPoint>();
            var collider = spawnPoint.StartBoundary.GetComponent<BoxCollider>();
            var bounds = collider.bounds;
            var rx = bounds.center.x - bounds.size.x / 2;
            var ry = bounds.center.z - bounds.size.z / 2;
            return new Rect(rx, ry, bounds.size.x, bounds.size.z);
        }

        /// <summary>
        /// Gets the start point indices for a game configuration.
        /// </summary>
        /// <param name="faction">The faction to get the start points for.</param>
        /// <param name="numPlayers">How many human players there are.</param>
        /// <param name="numShipsPerPlayer">How many ships ach player controls.</param>
        /// <returns>An array of start point indices.</returns>
        public int [] GetStartPointIndices(Faction faction, int numPlayers, int numShipsPerPlayer) {
            if (faction == Faction.HUMANS) {
                if (numPlayers == 1) {
                    if (numShipsPerPlayer == 1) {
                        return s_p1StartPositions1;
                    }
                    else {
                        return s_p1StartPositions2;
                    }
                }
                else {
                    if (numShipsPerPlayer == 1) {
                        return s_startPositions2;
                    }
                    else {
                        return s_startPositions4;
                    }
                }
            }
            else {
                if (numPlayers == 1) {
                    if (numShipsPerPlayer == 1) {
                        return s_p2StartPositions1;
                    }
                    else {
                        return s_p2StartPositions2;
                    }
                }
                else {
                    if (numShipsPerPlayer == 1) {
                        return s_startPositions2;
                    }
                    else {
                        return s_startPositions4;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the local communicator instance so the game controller can perform network
        /// operations with the correct authority.
        /// </summary>
        /// <param name="communicator">The communicator.</param>
        public void SetCommunicator(Communicator communicator) {
            _communicator = communicator;
        }

        #region Client Code
        public void NotifyActiveShip(int ownerID, int shipID) {
            if (_communicator.ID == ownerID) {
                SetActiveShip(shipID);
            }
            else {
                SetActiveShip(-1);
            }

        }

        /// <summary>
        /// Makes the info panel visible and sets its content up.
        /// </summary>
        /// <param name="caption">The caption for the info panel.</param>
        /// <param name="prefabName">The name of the content prefab.</param>
        public void EnableInfoPanel(string caption, string prefabName) {
            InfoPanel.gameObject.SetActive(true);
            InfoPanel.SetTitle(caption);
            InfoPanel.SetContent(prefabName);
        }

        /// <summary>
        /// Disables the info panel in the UI.
        /// </summary>
        public void DisableInfoPanel() {
            InfoPanel.ClearContent();
            InfoPanel.gameObject.SetActive(false);
        }

        /// <summary>
        /// Registers a ship with the controller.
        /// </summary>
        /// <param name="ship">The ship.</param>
        public void RegisterShip(ShipController ship) {
            // Preconditions.
            Debug.Assert(ship != null);
            Debug.Assert(!_ships.Contains(ship), "Duplicate controller?");

            // Implementation.
            Debug.LogFormat("GameController.RegisterShip()");

            _ships.Add(ship);
        }

        /// <summary>
        /// Called client-side to handle activation of a ship.
        /// </summary>
        /// <param name="shipID">The ID of the ship to activate.</param>
        public void SetActiveShip(int shipID) {
            // Is there a currently active ship?
            if (_activeShip != null) {
                // Yes, deactivate it.
                _activeShip.Deactivate();
            }

            ActiveShipIndicator.transform.parent = null;
            if (shipID == -1) {
                ActiveShipIndicator.SetActive(false);
                _activeShip = null;

                InfoPanel.NotifyContent("SetInfoText", "Please wait whilst opponent sets up their next ship");
                InfoPanel.NotifyContent("EnableDoneButton", false);
            }
            else {
                var shipController = _ships.Find(s => s.ID == shipID);
                if (shipController.ControlType == ControlType.HUMAN) {
                    // Fire up the UI.
                    ActiveShipIndicator.SetActive(true);
                    var ship = shipController.gameObject;
                    ActiveShipIndicator.transform.parent = ship.transform;
                    ActiveShipIndicator.transform.localPosition = Vector3.zero;
                    var rotator = ActiveShipIndicator.GetComponent<Rotator>();
                    var color = FactionColors[(int)shipController.Faction];
                    rotator.SetColor(color);
                    InfoPanel.NotifyContent("SetInfoText", "Please set the start position and rotation of your ship");
                    InfoPanel.NotifyContent("EnableDoneButton", true);
                }

                _activeShip = shipController;
                _activeShip.Activate();
                _activeShipID = shipController.ID;
            }
        }

        /// <summary>
        /// Whatever the game state / phase, moves on to the next ship in the round. If there is none,
        /// updates the phase / state as required.
        /// </summary>
        public void NextShip() {
            Debug.Log("GameController.NextShip()");

            _communicator.CmdNextShip();
        }
        #endregion Client Code

        #region Server code
        public void NextShipServer() {
            var turnList = getTurnOrder();
            var nextIndex = _activeShipIndex + 1;
            if (nextIndex == turnList.Count) {
                // End of state / phase.
                AdvanceGame();
            }
            else {
                SetActiveShipByIndex(nextIndex, false);
            }
        }

        public void AdvanceGame() {
            Debug.Log("GameController.AdvanceGame()");
            switch(_state) {
                case GameState.SETUP:
                    // Setup is done. Time to play!
                    SetState(GameState.IN_PLAY, true);
                    break;

                default:
                    Debug.LogError("Broken game state machine");
                    break;
            }
        }

        /// <summary>
        /// Adds a player to the active list.
        /// </summary>
        /// <param name="player">The player to add.</param>
        public void AddPlayer(PlayerServerRep player) {
            Debug.LogFormat("GameController.AddPlayer({0})", player.PlayerID);
            _players.Add(player);
        }

        /// <summary>
        /// Removes a playr from the active list.
        /// </summary>
        /// <param name="playerID">The player's ID.</param>
        public void RemovePlayer(int playerID) {
            Debug.LogFormat("GameController.RemovePlayer({0})", playerID);
            var index = _players.FindIndex(p => p.PlayerID == playerID);
            if (index >= 0) {
                _players.RemoveAt(index);
            }
            else {
                Debug.LogWarning("GameController: unable to remove player with ID " + playerID);
            }
        }

        public PlayerServerRep GetPlayer(int playerID) {
            return _players.Find(p => p.PlayerID == playerID);
        }

        /// <summary>
        /// Updates the server every frame.
        /// </summary>
        public void UpdateServer() {
            switch(_state) {
                case GameState.UNINITIALIZED:
                    // Obvious hack for now.
                    SetState(GameState.LOBBY, true);
                    break;

                case GameState.LOBBY:
                    if (_players.Count == Configuration.NumberOfHumanPlayers) {
                        // Everyone has joined.
                        // TODO remove the fudges. For now, spawn all the players.
                        _communicator.SpawnShips(Configuration);
                        SetState(GameState.WAIT_FOR_SPAWN, true);
                    }
                    break;

                case GameState.WAIT_FOR_SPAWN:
                    if (_ships.Count == Configuration.NumberOfShips) {
                        // Enable the info panel UI.
                        _communicator.EnableInfoPanel("Setup", "SetupPanel");

                        // All the ships have spawned. Do some book-keeping, and move on to setup.
                        // TODO: simultaneous setup. For now, it's one player, one ship at a time.
                        buildTurnLists();                        
                        SetActiveShipByIndex(0, true);
                        SetState(GameState.SETUP, true);
                    }
                    break;

                default:
                    break;
            }
        }

        public void SetActiveShipByIndex(int index, bool force) {
            if (index != _activeShipIndex || force) {
                _activeShipIndex = index;
                var shipOrder = getTurnOrder();
                _activeShipID = shipOrder[index].ID;
                var ownerID = shipOrder[index].OwnerID;
                _communicator.NotifyActiveShip(ownerID, _activeShipID);
            }
        }

        private List<ShipController> getTurnOrder() {
            if ((_state == GameState.IN_PLAY) && (_playPhase == PlayPhase.ATTACKING)) {
                return _attackOrderShips;
            }
            else if (_state == GameState.SETUP) {
                return _setupOrderShips;
            }
            else {
                return _moveOrderShips;
            }
        }
        #endregion

        /// <summary>
        /// Sets the game state.
        /// </summary>
        /// <param name="newState">The new state.</param>
        /// <param name="notify">If true, all clients are notified of the change.</param>
        public void SetState(GameState newState, bool notify) {
            if (_state != newState) {
                Debug.LogFormat("GameController.SetState({0})", newState);
                _state = newState;
                _activeShipIndex = 0;
                if (notify) {
                    _communicator.NotifyGameStateChange(_state);
                }
                if (StateChangeEvent != null) {
                    StateChangeEvent.Invoke(_state);
                }
            }
        }

        /// <summary>
        /// Sets the play phase.
        /// </summary>
        /// <param name="newPhase">The new play phase.</param>
        /// <param name="notify">If true, all clients are notified of the change.</param>
        public void SetPlayPhase(PlayPhase newPhase, bool notify) {
            if (_playPhase != newPhase) {
                Debug.LogFormat("GameController.SetPlayPhase({0})", newPhase);
                _playPhase = newPhase;
                if (notify) {
                    _communicator.NotifyPlayPhaseChange(_playPhase);
                }
                if (PhaseChangeEvent != null) {
                    PhaseChangeEvent.Invoke(_playPhase);
                }
            }
        }

        private void buildTurnLists() {
            _moveOrderShips.Clear();
            _attackOrderShips.Clear();
            _moveOrderShips.AddRange(_ships);
            _moveOrderShips.Sort((s1, s2) => {
                // TODO: factor in captain's rank when all that stuff is in place.
                if (s1.Faction > s2.Faction) {
                    return 1;
                }

                if (s1.Faction < s2.Faction) {
                    return -1;
                }

                return 0;
            });
            _attackOrderShips.AddRange(_moveOrderShips);
            _attackOrderShips.Reverse();

            // For setup, interleave players.
            var p1Ships = _ships.FindAll(s => s.Faction == Faction.HUMANS);
            var p2Ships = _ships.FindAll(s => s.Faction == Faction.ALIENS);
            _setupOrderShips.Clear();
            for(int i = 0; i < p1Ships.Count; ++i) {
                _setupOrderShips.Add(p1Ships[i]);
                _setupOrderShips.Add(p2Ships[i]);
            }
        }

        #region Server Data
        private GameState _state = GameState.UNINITIALIZED;
        private PlayPhase _playPhase = PlayPhase.IDLE;
        private int _activeShipIndex = -1;
        private int _activeShipID = -1;
        #endregion Server Data

        private Communicator _communicator;
        private readonly List<PlayerServerRep> _players = new List<PlayerServerRep>();
        private readonly List<ShipController> _ships = new List<ShipController>();
        private readonly List<ShipController> _moveOrderShips = new List<ShipController>();
        private readonly List<ShipController> _attackOrderShips = new List<ShipController>();
        private readonly List<ShipController> _setupOrderShips = new List<ShipController>();

        private ShipController _activeShip;

        private static readonly int[] s_p1StartPositions1 = new[] { 3 };
        private static readonly int[] s_p1StartPositions2 = new[] { 0, 1 };
        private static readonly int[] s_p2StartPositions1 = new[] { 2 };
        private static readonly int[] s_p2StartPositions2 = new[] { 4, 5 };
        private static readonly int[] s_startPositions2 = new[] { 3, 2 };
        private static readonly int[] s_startPositions4 = new[] { 0, 1, 4, 5 };
    }
}