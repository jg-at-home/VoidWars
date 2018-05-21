using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace VoidWars {
    public class CurveTests {

        [Test]
        public void TestStraightLinePasses() {
            var points = new List<Vector3>();
            points.Add(new Vector3(0, 0, 0));
            points.Add(new Vector3(0, 0, 1));
            var curve = new PiecewiseLinearCurve(points);
            Vector3 p;
            Quaternion q;
            curve.GetPositionAndRotation(0f, out p, out q);
            Assert.AreEqual(p, Vector3.zero);
            curve.GetPositionAndRotation(1f, out p, out q);
            Assert.AreEqual(p, Vector3.forward);
            curve.GetPositionAndRotation(0.5f, out p, out q);
            Assert.AreEqual(p, new Vector3(0, 0, 0.5f));
        }

        [Test]
        public void TestTwoSegmentsPasses() {
            var p0 = Vector3.zero;
            var p1 = new Vector3(0, 0, 1);
            var p2 = new Vector3(1, 0, 1);
            var points = new List<Vector3> { p0, p1, p2 };
            var curve = new PiecewiseLinearCurve(points);
            Vector3 p;
            Quaternion q;
            curve.GetPositionAndRotation(0f, out p, out q);
            Assert.AreEqual(p, p0);
            curve.GetPositionAndRotation(1f, out p, out q);
            Assert.AreEqual(p, p2);
            curve.GetPositionAndRotation(0.5f, out p, out q);
            Assert.AreEqual(p, p1);
        }

    }
}