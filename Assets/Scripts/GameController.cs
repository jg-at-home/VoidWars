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

    public class GameController : MonoBehaviour {
        public GameConfig Configuration;
        public UnityEvent<GameState> StateChangeEvent;
        public UnityEvent<PlayPhase> PhaseChangeEvent;

        public void SetCommunicator(Communicator communicator) {

        }

        public void AddPlayer(int playerID) {
            _players.Add(playerID);
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
                        _communicator.SpawnShips(Configuration);
                        SetState(GameState.SETUP);
                    }
                    break;
            }
        }

        public void SetState(GameState newState) {
            if (_state != newState) {
                _state = newState;
                _communicator.NotifyGameStateChange(_state);
                if (StateChangeEvent != null) {
                    StateChangeEvent.Invoke(_state);
                }
            }
        }

        public void SetPlayPhase(PlayPhase newPhase) {
            if (_playPhase != newPhase) {
                _playPhase = newPhase;
                _communicator.NotifyPlayPhaseChange(_playPhase);
                if (PhaseChangeEvent != null) {
                    PhaseChangeEvent.Invoke(_playPhase);
                }
            }
        }

        private GameState _state = GameState.UNINITIALIZED;
        private PlayPhase _playPhase = PlayPhase.IDLE;
        private Communicator _communicator;
        private int _activePlayerIndex;
        private readonly List<int> _players = new List<int>();
    }
}