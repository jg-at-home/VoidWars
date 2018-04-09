﻿namespace VoidWars {
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

    public class ShipMoveInstance {
        public ShipMove Move;
        public int ShipID;
    }
}