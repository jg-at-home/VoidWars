using UnityEngine;

namespace VoidWars {
    public enum MoveType {
        None,

        // Scale by 1, 2 or 3 units.
        Forward,
        GentleTurnLeft,
        GentleTurnRight,

        // Medium maneuverability => Smaller turn radii.
        SharpTurnLeft, 
        SharpTurnRight,

        // High maneuverability (3)
        TurnAbout,

        // Only for ships equipped with reverse thrust
        Reverse,
    }

    /// <summary>
    /// Describes a ship manuever.
    /// </summary>
    public struct ShipMove {
        /// <summary>
        /// What we're going to do.
        /// </summary>
        public MoveType MoveType;

        /// <summary>
        /// The size of the motion.
        /// </summary>
        public int Size;

        public ShipMove(MoveType type, int size=1) {
            MoveType = type;
            Size = size;
        }

        /// <summary>
        /// Gets a scalar that reflects the energy demands of a specific move type.
        /// </summary>
        public float EnergyScale {
            get {
                switch (MoveType) {
                    case MoveType.Forward:
                    case MoveType.Reverse:
                        return Size;

                    case MoveType.GentleTurnLeft:
                    case MoveType.GentleTurnRight:
                        return Size * Mathf.PI / 4f;

                    case MoveType.SharpTurnLeft:
                    case MoveType.SharpTurnRight:
                        return Size * Mathf.PI / 2f;

                    case MoveType.TurnAbout:
                        return Size * 3f;

                    default:
                        Debug.Assert(false, "Huh?");
                        return Size;
                }
            }
        }

        public override string ToString() {
            string name = MoveType.ToString();
            switch(MoveType) {
                case MoveType.Forward:
                case MoveType.Reverse:
                    return string.Format("{0}: {1}", name, Size);

                case MoveType.None:
                    return "<None>";

                case MoveType.GentleTurnLeft:
                case MoveType.GentleTurnRight:
                    return string.Format("{0}: {1}", name.Substring(10), Size);

                case MoveType.SharpTurnLeft:
                case MoveType.SharpTurnRight:
                    return string.Format("{0}-90: {1}", name.Substring(10), Size);

                case MoveType.TurnAbout:
                    return "Turn About";

                default:
                    return "???";
            }
        }
    }

    /// <summary>
    /// Describes a move for a specific ship.
    /// </summary>
    public struct ShipMoveInstance {
        /// <summary>
        /// The move to make.
        /// </summary>
        public ShipMove Move;

        /// <summary>
        /// The ship that's making it.
        /// </summary>
        public int ShipID;

        /// <summary>
        /// The target position to move to.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The target rotation.
        /// </summary>
        public Quaternion Rotation;
    }
}