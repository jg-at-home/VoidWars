using UnityEngine;
namespace VoidWars {
    /// <summary>
    /// Delegate for a task.
    /// </summary>
    public delegate void TaskFunc(Task task);

    /// <summary>
    /// A job that gets postponed for a number of turns.
    /// </summary>
    public class Task {
        /// <summary>
        /// Construct a task.
        /// </summary>
        /// <param name="numTurns">The number of turns to wait.</param>
        /// <param name="func">The function to call when the waiting is over.></param>
        public Task(int numTurns, TaskFunc func) {
            _turnsLeft = numTurns;
            _func = func;
        }

        /// <summary>
        /// Number of turns left for the task to run.
        /// </summary>
        public int TurnsLeft {  get { return _turnsLeft; } }

        /// <summary>
        /// Called at the start of each turn.
        /// </summary>
        /// <param name="ship">The ship running the task.</param>
        public virtual void OnTurnStart(ShipController ship) {
            Debug.Assert(_turnsLeft > 0);
            --_turnsLeft;
            if (_turnsLeft == 0) {
                _func(this);
            }
        }

        /// <summary>
        /// Has the task finished?
        /// </summary>
        public bool HasExpired {
            get { return _turnsLeft <= 0; }
        }

        private int _turnsLeft;
        private TaskFunc _func;
    }
}