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
            _targets = new int[(int)EnergyConsumer.NumTargets];
            _weights = new float[_targets.Length];
            Reset();
        }

        public float Available(EnergyConsumer target) {
            return _weights[(int)target];
        }

        public void Reset() {
            for (int i = 0; i < _targets.Length; ++i) {
                _targets[i] = 1;
            }
            buildWeights();
        }

        public void ModifyTarget(EnergyConsumer target, int step) {
            _targets[(int)target] += step;
            buildWeights();
        }

        private void buildWeights() {
            var sum = 0f;
            foreach (var target in _targets) {
                sum += target;
            }

            var scale = 1f / sum;
            for (int i = 0; i < _weights.Length; ++i) {
                _weights[i] = _targets[i] * scale;
            }
        }

        private readonly int[] _targets;
        private readonly float[] _weights;
    }
}