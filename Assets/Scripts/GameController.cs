using System.Collections;
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
        SETUP,
        WAIT_FOR_SPAWN,
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
    public class GameController : MonoBehaviour {
        public GameObject[] StartPositions;
        public Color[] FactionColors;
        public GameConfig Configuration;
        public UnityEvent<GameState> StateChangeEvent;
        public UnityEvent<PlayPhase> PhaseChangeEvent;
        public GameObject ActiveShipIndicator;

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

        public void SetCommunicator(Communicator communicator) {
            _communicator = communicator;
        }

        public void AddPlayer(PlayerServerRep player) {
            Debug.LogFormat("GameController.AddPlayer({0})", player.PlayerID);
            _players.Add(player);
        }

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

        #region Client Code
        public void RegisterShip(ShipController ship) {
            // Preconditions.
            Debug.Assert(ship != null);
            Debug.Assert(!_ships.Contains(ship), "Duplicate controller?");

            // Implementation.
            Debug.LogFormat("GameController.RegisterShip()");

            _ships.Add(ship);
        }

        public void SetActiveShip(int shipID) {
            ActiveShipIndicator.transform.parent = null;
            if (shipID == -1) {
                ActiveShipIndicator.SetActive(false);
            }
            else {
                ActiveShipIndicator.SetActive(true);
                var shipController = _ships.Find(s => s.ID == shipID);
                var ship = shipController.gameObject;
                ActiveShipIndicator.transform.parent = ship.transform;
                ActiveShipIndicator.transform.localPosition = Vector3.zero;
                var rotator = ActiveShipIndicator.GetComponent<Rotator>();
                var color = FactionColors[(int)shipController.Faction];
                rotator.SetColor(color);
            }
        }
        #endregion Client Code

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
                        // All the ships have spawned. Do some book-keeping, and move on to setup.
                        buildTurnLists();
                        setActiveShip(0, true);
                        SetState(GameState.SETUP, true);
                    }
                    break;

                default:
                    break;
            }
        }

        private void setActiveShip(int index, bool force) {
            if (index != _activeShipIndex || force) {
                _activeShipIndex = index;
                var shipOrder = getTurnOrder();
                _activeShipID = shipOrder[index].ID;
                _communicator.NotifyActiveShip(_activeShipID);
            }
        }

        private List<ShipController> getTurnOrder() {
            if ((_state == GameState.IN_PLAY) && (_playPhase == PlayPhase.ATTACKING)) {
                return _attackOrderShips;
            }
            else {
                return _moveOrderedShips;
            }
        }

        public void SetState(GameState newState, bool notify) {
            if (_state != newState) {
                Debug.LogFormat("GameController.SetState({0})", newState);
                _state = newState;
                if (notify) {
                    _communicator.NotifyGameStateChange(_state);
                }
                if (StateChangeEvent != null) {
                    StateChangeEvent.Invoke(_state);
                }
            }
        }

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
            _moveOrderedShips.Clear();
            _attackOrderShips.Clear();
            _moveOrderedShips.AddRange(_ships);
            _moveOrderedShips.Sort((s1, s2) => {
                // TODO: factor in captain's rank when all that stuff is in place.
                if (s1.Faction > s2.Faction) {
                    return 1;
                }

                if (s1.Faction < s2.Faction) {
                    return -1;
                }

                return 0;
            });
            _attackOrderShips.AddRange(_moveOrderedShips);
            _attackOrderShips.Reverse();
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
        private readonly List<ShipController> _moveOrderedShips = new List<ShipController>();
        private readonly List<ShipController> _attackOrderShips = new List<ShipController>();

        private static readonly int[] s_p1StartPositions1 = new[] { 3 };
        private static readonly int[] s_p1StartPositions2 = new[] { 0, 1 };
        private static readonly int[] s_p2StartPositions1 = new[] { 2 };
        private static readonly int[] s_p2StartPositions2 = new[] { 4, 5 };
        private static readonly int[] s_startPositions2 = new[] { 3, 2 };
        private static readonly int[] s_startPositions4 = new[] { 0, 1, 4, 5 };


    }
}