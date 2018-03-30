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
        // TODO: other stuff.
    }

    /// <summary>
    /// Class responsible for coordinating the game events.
    /// </summary>
    public class GameController : MonoBehaviour {
        public GameObject[] StartPositions;
        public GameConfig Configuration;
        public UnityEvent<GameState> StateChangeEvent;
        public UnityEvent<PlayPhase> PhaseChangeEvent;

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

        public void RegisterShip(ShipController ship) {
            // Preconditions.
            Debug.Assert(ship != null);
            Debug.Assert(!_ships.Contains(ship), "Duplicate controller?");

            // Implementation.
            Debug.LogFormat("GameController.RegiserShip()");

            _ships.Add(ship);
        }

        public void UpdateServer() {
            switch(_state) {
                case GameState.UNINITIALIZED:
                    // Obvious hack for now.
                    SetState(GameState.LOBBY);
                    break;

                case GameState.LOBBY:
                    if (_players.Count == Configuration.NumberOfHumanPlayers) {
                        // Everyone has joined.
                        // TODO remove the fudges. For now, spawn all the players.
                        _spawnIndex = 0;
                        _communicator.SpawnShips(Configuration);
                        SetState(GameState.WAIT_FOR_SPAWN);
                    }
                    break;

                case GameState.WAIT_FOR_SPAWN:
                    if (_ships.Count == Configuration.NumberOfShips) {
                        // All the ships have spawned. Do some book-keeping, and move on to setup.
                        buildTurnLists();
                        SetState(GameState.SETUP);
                    }
                    break;

                default:
                    break;
            }
        }

        public void SetState(GameState newState) {
            if (_state != newState) {
                Debug.LogFormat("GameController.SetState({0})", newState);
                _state = newState;
                _communicator.NotifyGameStateChange(_state);
                if (StateChangeEvent != null) {
                    StateChangeEvent.Invoke(_state);
                }
            }
        }

        public void SetPlayPhase(PlayPhase newPhase) {
            if (_playPhase != newPhase) {
                Debug.LogFormat("GameController.SetPlayPhase({0})", newPhase);
                _playPhase = newPhase;
                _communicator.NotifyPlayPhaseChange(_playPhase);
                if (PhaseChangeEvent != null) {
                    PhaseChangeEvent.Invoke(_playPhase);
                }
            }
        }

        private void buildTurnLists() {

        }

        private GameState _state = GameState.UNINITIALIZED;
        private PlayPhase _playPhase = PlayPhase.IDLE;
        private Communicator _communicator;
        private int _activePlayerIndex;
        private int _spawnIndex;
        private readonly List<PlayerServerRep> _players = new List<PlayerServerRep>();
        private readonly List<ShipController> _ships = new List<ShipController>();
    }
}