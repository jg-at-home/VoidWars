using UnityEngine;

namespace VoidWars {
    public enum EnergyConsumer {
        Weapons,
        LifeSupport,
        Shields,
        Propulsion,

        // More above here.
        NumTargets
    }

    /// <summary>
    /// Helper class for dealing with budgeted energy.
    /// </summary>
    public class EnergyBudget {
        public EnergyBudget() {
            _weights = new float[(int)EnergyConsumer.NumTargets];
            Reset();
        }

        /// <summary>
        /// Gets the fraction of energy available in the given category.
        /// </summary>
        /// <param name="target">The target category.</param>
        /// <returns>The fraction available in [0,1]</returns>
        public float Available(EnergyConsumer target) {
            return _weights[(int)target];
        }

        /// <summary>
        /// Sets the fraction of energy available in each category.
        /// </summary>
        /// <param name="fraction">The fraction available in [0,1]</param>
        public void SetFractions(float [] fractions) {
            Debug.Assert(fractions.Length == (int)EnergyConsumer.NumTargets);
            for (int i = 0; i < fractions.Length; ++i) {
                _weights[i] = fractions[i];
            }
            normalize();
        }

        /// <summary>
        /// Resets everything to equal weighting.
        /// </summary>
        public void Reset() {
            for (int i = 0; i < _weights.Length; ++i) {
                _weights[i] = 1f / _weights.Length;
            }
        }

        private void normalize() {
            var sum = 0f;
            foreach (var weight in _weights) {
                sum += weight;
            }

            var scale = 1f / sum;
            for (int i = 0; i < _weights.Length; ++i) {
                _weights[i] *= scale;
            }
        }

        private readonly float[] _weights;
    }
}