using System.Collections.Generic;
using UnityEngine;

namespace VoidWars {
    /// <summary>
    /// Curve made up of line segments.
    /// </summary>
    public class PiecewiseLinearCurve {
        /// <summary>
        /// Construct the curve from a set of points.
        /// </summary>
        /// <param name="points"></param>
        public PiecewiseLinearCurve(List<Vector3> points) {
            _segments = new Segment[points.Count-1];
            var pathLength = 0f;
            for (int i = 0; i < _segments.Length; ++i) {
                _segments[i] = new Segment(points[i], points[i+1]);
                pathLength += _segments[i].Length;
            }

            var accLength = 0f;
            foreach (var segment in _segments) {
                segment.tStart = accLength / pathLength;
                accLength += segment.Length;
                segment.tEnd = Mathf.Min(accLength / pathLength, 1f);
            }
        }

        /// <summary>
        /// Gets a point at parameter t on the curve where 0=start and 1=end.
        /// </summary>
        /// <param name="t">Curve parameter in [0,1]</param>
        /// <returns>Point on the curve.</returns>
        public Vector3 GetPoint(float t) {
            if (t <= 0f) {
                return _segments[0].StartPos;
            }

            if (t >= 1f) {
                return _segments[_segments.Length - 1].EndPos;
            }

            foreach(var segment in _segments) {
                if (t >= segment.tStart && t < segment.tEnd) {
                    return segment.GetPoint(t);
                }
            }

            Debug.Assert(false, "Should never get here");
            return Vector3.zero;
        }

        public void GetPositionAndRotation(float t, out Vector3 position, out Quaternion rotation) {
            if (t <= 0f) {
                position = _segments[0].StartPos;
                rotation = _segments[0].TangentRotation;
                return;
            }

            if (t >= 1f) {
                var segment = _segments[_segments.Length - 1];
                position = segment.EndPos;
                rotation = segment.TangentRotation;
                return;
            }

            foreach (var segment in _segments) {
                if (t >= segment.tStart && t < segment.tEnd) {
                    position = segment.GetPoint(t);
                    rotation = segment.TangentRotation;
                    return;
                }
            }

            Debug.Assert(false, "Should never get here");
            position = Vector3.zero;
            rotation = Quaternion.identity;
        }

        private class Segment {
            public Segment(Vector3 start, Vector3 end) {
                StartPos = start;
                EndPos = end;
                Length = Vector3.Distance(end, start);
                TangentRotation = Quaternion.LookRotation(end - start);
            }

            public Vector3 GetPoint(float t) {
                // t is global in [0,1]. Convert to local.
                var s = (t - tStart) / (tEnd - tStart);
                return Vector3.Lerp(StartPos, EndPos, s);
            }

            public float tStart;
            public float tEnd;
            public readonly Vector3 StartPos;
            public readonly Vector3 EndPos;
            public readonly float Length;
            public readonly Quaternion TangentRotation;
        }

        private Segment[] _segments;
    }
}