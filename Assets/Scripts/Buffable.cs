using System.Collections.Generic;
using System;
using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Two simple types of buff.
    /// </summary>
    public enum BuffType {
        /// <summary>
        /// Do nothing.
        /// </summary>
        None,

        /// <summary>
        /// Add a constant value to the stat.
        /// </summary>
        Additive,

        /// <summary>
        /// Adds on a percentage of the current value.
        /// </summary>
        Percentage,

        /// <summary>
        /// Also adds on a percentage, but when there are several together, they are
        /// accumulated before applying. So two 100% buffs will result in a 3 times
        /// increase, not 4 as with the percentage.
        /// </summary>
        CumulativePercentage,

        /// <summary>
        /// Fixes the stat at a single value.
        /// </summary>
        Fix,
    }

    /// <summary>
    /// The things a buff can change.
    /// </summary>
    [Flags]
    public enum BuffTarget {
        Ship = 1,
        Auxiliary = 2,
        Weapon = 4
    }

    /// <summary>
    /// Editor buff descriptor.
    /// </summary>
    [Serializable]
    public struct BuffInfo {
        [Tooltip("Name of the Buff")]
        public string Name;

        [Tooltip("What type of thing the buff affects")]
        [EnumFlag] public BuffTarget Target;

        [Tooltip("The specific item to affect, or * to affect all")]
        public string TargetName;

        [Tooltip("The name of the stat the buff modifies")]
        public string Property;

        [Tooltip("The type of modification")]
        public BuffType BuffType;

        [Tooltip("The amount of modification")]
        public float Value;
    }

    /// <summary>
    /// Class that boosts a stat value in some way.
    /// </summary>
    public class Buff {
        /// <summary>
        /// Construct a buff.
        /// </summary>
        /// <param name="property">The property the buff will modify.</param>
        /// <param name="type">The type of buff.</param>
        /// <param name="value">The associated value.</param>
        /// <param name="owner">Optional owner of the buff.</param>
        public Buff(string property, BuffType type, float value, object owner=null) {
            Property = property;
            BuffType = type;
            Value = value;
            Owner = owner;
        }

        public readonly object Owner;
        public readonly string Property;
        public readonly BuffType BuffType;
        public readonly float Value;
    }

    /// <summary>
    /// Buffable stat class.
    /// </summary>
    public class Stat {
        /// <summary>
        /// Construct a stat.
        /// </summary>
        /// <param name="minValue">Minimum allowable value.</param>
        /// <param name="maxValue">Maximum allowable value.</param>
        internal Stat(float minValue, float maxValue) {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        /// <summary>
        /// Gets the currewnt value.
        /// </summary>
        public float Value {
            get {
                if (_dirty) {
                    evaluate();
                }
                return _valueCache;
            }
        }

        /// <summary>
        /// Sets the base stat value which buffs are applied to.
        /// </summary>
        /// <param name="value">The base value.</param>
        public void SetBaseValue(float value) {
            _baseValue = value;
            _dirty = true;
        }

        /// <summary>
        /// Adds a buff to the stat. No nulls, no duplicates.
        /// </summary>
        /// <param name="buff">The buff to add.</param>
        public void AddBuff(Buff buff) {
            if (buff == null)
                throw new ArgumentNullException("Buff is null");
            if (_buffs.Contains(buff))
                throw new ArgumentException("Duplicate buff");

            _buffs.Add(buff);
            _dirty = true;
        }

        /// <summary>
        /// Removes a buff from the stat.
        /// </summary>
        /// <param name="buff">The buff to remove.</param>
        public void RemoveBuff(Buff buff) {
            if (_buffs.Remove(buff)) {
                _dirty = true;
            }
        }

        /// <summary>
        /// Removes all buffs with the given owner.
        /// </summary>
        /// <param name="owner">The buff owner.</param>
        /// <returns>The number of buffs removed.</returns>
        public int RemoveBuffsWithOwner(object owner) {
            var count = 0;
            for(int i = _buffs.Count-1; i >= 0; --i) {
                if (_buffs[i].Owner == owner) {
                    _buffs.RemoveAt(i);
                    _dirty = true;
                    ++count;
                }
            }
            return count;
        }

        private void evaluate() {
            _valueCache = _baseValue;
            var prevType = BuffType.None;
            var cumulativePercent = 0f;
            foreach(var buff in _buffs) {
                if (buff.BuffType == BuffType.CumulativePercentage) {
                    cumulativePercent += buff.Value;
                }
                else {
                    if (prevType == BuffType.CumulativePercentage) {
                        // We've finished accumulating. Apply this before the next step.
                        _valueCache *= (1.0f + cumulativePercent / 100f);
                        cumulativePercent = 0f;
                    }

                    switch(buff.BuffType) {
                        case BuffType.Additive:
                            _valueCache += buff.Value;
                            break;

                        case BuffType.Percentage:
                            _valueCache *= (1f + (buff.Value / 100f));
                            break;

                        case BuffType.Fix:
                            _valueCache = buff.Value;
                            return;

                        default:
                            break;
                    }
                }
                prevType = buff.BuffType;
            }

            // If we're accumulating at the end, apply.
            if (cumulativePercent != 0f) {
                _valueCache *= (1.0f + cumulativePercent / 100f);
            }

            // Clamp to range.
            _valueCache = Mathf.Clamp(_valueCache, _minValue, _maxValue);
        }

        private float _baseValue;
        private float _valueCache;
        private readonly List<Buff> _buffs = new List<Buff>();
        private bool _dirty = true;
        private float _minValue;
        private float _maxValue;
    }

    /// <summary>
    /// Attribute to apply to a buffable property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class StatAttribute : Attribute {
        /// <summary>
        /// Construct a stat attribute.
        /// </summary>
        /// <param name="min">Stat min value.</param>
        /// <param name="max">Stat max value.</param>
        public StatAttribute(float min = float.NegativeInfinity, float max = float.PositiveInfinity) {
            Debug.Assert(max > min);
            MinValue = min;
            MaxValue = max;
        }

        internal readonly float MinValue;
        internal readonly float MaxValue;
    }

    /// <summary>
    /// Base class for buffable entities.
    /// </summary>
    public class Buffable {
        /// <summary>
        /// Construct a buffable entity.
        /// </summary>
        public Buffable() {
            var props = GetType().GetProperties();
            foreach(var prop in props) {
                var attrs = prop.GetCustomAttributes(true);
                foreach(var attr in attrs) {
                    var stat = attr as StatAttribute;
                    if (stat != null) {
                        _values[prop.Name] = new Stat(stat.MinValue, stat.MaxValue);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the stats associated with the entity.
        /// </summary>
        public IEnumerable<Stat> Stats {
            get { return _values.Values; }
        }

        /// <summary>
        /// Gets the names of the buffable stats.
        /// </summary>
        public IEnumerable<string> StatNames {
            get { return _values.Keys; }
        }

        /// <summary>
        /// Gets the total number of buffs.
        /// </summary>
        public int BuffCount {
            get { return _count; }
        }

        /// <summary>
        /// Adds a buff to the entity. Will only be added if it binds to a property.
        /// </summary>
        /// <param name="buff">The buff to add.</param>
        public void AddBuff(Buff buff) {
            Stat stat;
            if (_values.TryGetValue(buff.Property, out stat)) {
                stat.AddBuff(buff);
                ++_count;
            }
        }

        /// <summary>
        /// Removes a buff from the entity.
        /// </summary>
        /// <param name="buff">The buff to remove.</param>
        public void RemoveBuff(Buff buff) {
            Stat stat;
            if (_values.TryGetValue(buff.Property, out stat)) {
                stat.RemoveBuff(buff);
                --_count;
            }
        }

        /// <summary>
        /// Removes all buffs from an owner.
        /// </summary>
        /// <param name="owner">The buff owner.</param>
        public void RemoveBuffsWithOwner(object owner) {
            foreach(var stat in _values.Values) {
                _count -= stat.RemoveBuffsWithOwner(owner);
            }
        }

        protected float getValue(string prop) {
            var stat = _values[prop];
            return stat.Value;
        }

        protected void setValue(string prop, float value) {
            var stat = _values[prop];
            stat.SetBaseValue(value);
        }

        private readonly Dictionary<string, Stat> _values = new Dictionary<string, Stat>();
        private int _count;
    }
}