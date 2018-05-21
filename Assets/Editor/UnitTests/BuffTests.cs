using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace VoidWars {
    public class Foo : Buffable {
        [Stat]
        public float Bar {
            get { return getValue("Bar"); }
            set { setValue("Bar", value); }
        }
    }

    public class Foo2 : Buffable {
        [Stat]
        public float Bar1 {
            get { return getValue("Bar1"); }
            set { setValue("Bar1", value); }
        }

        [Stat]
        public float Bar2 {
            get { return getValue("Bar2"); }
            set { setValue("Bar2", value); }
        }
    }

    public class RangedFoo : Buffable {
        [Stat(min:0f, max:1f)]
        public float Bar {
            get { return getValue("Bar"); }
            set { setValue("Bar", value); }
        }
    }

    public class BuffTests {
        [Test]
        public void TestStatsAreReflected1() {
            var foo = new Foo();
            var fooStats = new List<Stat>(foo.Stats);
            Assert.AreEqual(1, fooStats.Count);
            var fooNames = new List<string>(foo.StatNames);
            Assert.AreEqual("Bar", fooNames[0]);
        }

        [Test]
        public void TestStatsAreReflected2() {
            var foo = new Foo2();
            var fooStats = new List<Stat>(foo.Stats);
            Assert.AreEqual(2, fooStats.Count);
            var fooNames = new List<string>(foo.StatNames);
            Assert.AreEqual("Bar1", fooNames[0]);
            Assert.AreEqual("Bar2", fooNames[1]);
        }

        [Test]
        public void TestAddAdditiveBuff1() {
            var foo = new Foo();
            var buff = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(buff);
            Assert.AreEqual(1f, foo.Bar);
        }

        [Test]
        public void TestRemoveAdditiveBuff1() {
            var foo = new Foo();
            var buff = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(buff);
            foo.RemoveBuff(buff);
            Assert.AreEqual(0f, foo.Bar);
        }

        [Test]
        public void TestAddMultipleAdditiveBuffs1() {
            var foo = new Foo();
            var buff1 = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(buff1);
            Assert.AreEqual(1f, foo.Bar);
            var buff2 = new Buff("Bar", BuffType.Additive, 2f, null);
            foo.AddBuff(buff2);
            Assert.AreEqual(3f, foo.Bar);
        }

        [Test]
        public void TestAddAndRemoveMultipleAdditiveBuffs1() {
            var foo = new Foo();
            var buff1 = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(buff1);
            var buff2 = new Buff("Bar", BuffType.Additive, 2f, null);
            foo.AddBuff(buff2);
            foo.RemoveBuff(buff1);
            Assert.AreEqual(2f, foo.Bar);
            foo.RemoveBuff(buff2);
            Assert.AreEqual(0f, foo.Bar);
        }

        [Test]
        public void TestAddAndRemoveMultipleAdditiveBuffs2() {
            var foo = new Foo();
            var buff1 = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(buff1);
            var buff2 = new Buff("Bar", BuffType.Additive, 2f, null);
            foo.AddBuff(buff2);
            foo.RemoveBuff(buff2);
            Assert.AreEqual(1f, foo.Bar);
            foo.RemoveBuff(buff1);
            Assert.AreEqual(0f, foo.Bar);
        }

        [Test]
        public void TestPercentageBuff1() {
            var foo = new Foo();
            var buff = new Buff("Bar", BuffType.Percentage, 100f, null);
            foo.Bar = 1f;
            foo.AddBuff(buff);
            Assert.AreEqual(2f, foo.Bar);
        }

        [Test]
        public void TestTwoPercentageBuffs1() {
            var foo = new Foo();
            foo.Bar = 1f;
            var buff1 = new Buff("Bar", BuffType.Percentage, 100f, null);
            foo.AddBuff(buff1);
            var buff2 = new Buff("Bar", BuffType.Percentage, 100f, null);
            foo.AddBuff(buff2);
            Assert.AreEqual(4f, foo.Bar);
        }

        [Test]
        public void TestRemovePercentageBuff1() {
            var foo = new Foo();
            foo.Bar = 1f;
            var buff1 = new Buff("Bar", BuffType.Percentage, 100f, null);
            foo.AddBuff(buff1);
            Assert.AreEqual(2f, foo.Bar);
            foo.RemoveBuff(buff1);
            Assert.AreEqual(1f, foo.Bar);
        }

        [Test]
        public void TestSingleCumulativePercentage() {
            var foo = new Foo();
            foo.Bar = 1f;
            var buff1 = new Buff("Bar", BuffType.CumulativePercentage, 100f, null);
            foo.AddBuff(buff1);
            Assert.AreEqual(2f, foo.Bar);
        }

        [Test]
        public void TestTwoCumulativePercentages() {
            var foo = new Foo();
            foo.Bar = 1f;
            var buff1 = new Buff("Bar", BuffType.CumulativePercentage, 100f, null);
            foo.AddBuff(buff1);
            var buff2 = new Buff("Bar", BuffType.CumulativePercentage, 100f, null);
            foo.AddBuff(buff2);
            Assert.AreEqual(3f, foo.Bar);
        }

        [Test]
        public void TestMixed1() {
            var foo = new Foo();
            var add = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(add);
            var per = new Buff("Bar", BuffType.Percentage, 100f, null);
            foo.AddBuff(per);
            Assert.AreEqual(2f, foo.Bar);
            foo.RemoveBuff(per);
            Assert.AreEqual(1f, foo.Bar);
            foo.RemoveBuff(add);
            Assert.AreEqual(0f, foo.Bar);
        }

        [Test]
        public void TestMixed2() {
            var foo = new Foo();
            var add = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(add);
            var per = new Buff("Bar", BuffType.Percentage, 100f, null);
            foo.AddBuff(per);
            Assert.AreEqual(2f, foo.Bar);
            foo.RemoveBuff(add);
            Assert.AreEqual(0f, foo.Bar);
            foo.RemoveBuff(per);
            Assert.AreEqual(0f, foo.Bar);
        }

        [Test]
        public void TestMixed3() {
            var foo = new Foo();
            var add = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(add);
            var per = new Buff("Bar", BuffType.CumulativePercentage, 100f, null);
            foo.AddBuff(per);
            Assert.AreEqual(2f, foo.Bar);
            foo.RemoveBuff(add);
            Assert.AreEqual(0f, foo.Bar);
            foo.RemoveBuff(per);
            Assert.AreEqual(0f, foo.Bar);
        }

        [Test]
        public void TestMixed4() {
            var foo = new Foo();
            var add = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(add);
            var pc1 = new Buff("Bar", BuffType.CumulativePercentage, 100f, null);
            foo.AddBuff(pc1);
            var pc2 = new Buff("Bar", BuffType.CumulativePercentage, 100f, null);
            foo.AddBuff(pc2);
            Assert.AreEqual(3f, foo.Bar);

            foo.RemoveBuff(pc2);
            Assert.AreEqual(2f, foo.Bar);

            foo.RemoveBuff(pc1);
            Assert.AreEqual(1f, foo.Bar);

            foo.RemoveBuff(add);
            Assert.AreEqual(0f, foo.Bar);
        }

        [Test]
        public void TestMixed5() {
            var foo = new Foo();
            var add = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(add);
            var pc1 = new Buff("Bar", BuffType.CumulativePercentage, 100f, null);
            foo.AddBuff(pc1);
            var pc2 = new Buff("Bar", BuffType.Percentage, 100f, null);
            foo.AddBuff(pc2);
            Assert.AreEqual(4f, foo.Bar);

            foo.RemoveBuff(pc2);
            Assert.AreEqual(2f, foo.Bar);

            foo.RemoveBuff(pc1);
            Assert.AreEqual(1f, foo.Bar);

            foo.RemoveBuff(add);
            Assert.AreEqual(0f, foo.Bar);
        }

        [Test]
        public void TestMixed6() {
            var foo = new Foo();
            var add = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(add);
            var pc1 = new Buff("Bar", BuffType.Percentage, 100f, null);
            foo.AddBuff(pc1);
            var pc2 = new Buff("Bar", BuffType.CumulativePercentage, 100f, null);
            foo.AddBuff(pc2);
            Assert.AreEqual(4f, foo.Bar);

            foo.RemoveBuff(pc2);
            Assert.AreEqual(2f, foo.Bar);

            foo.RemoveBuff(pc1);
            Assert.AreEqual(1f, foo.Bar);

            foo.RemoveBuff(add);
            Assert.AreEqual(0f, foo.Bar);
        }

        [Test]
        public void TestRangedMin() {
            var foo = new RangedFoo();
            foo.AddBuff(new Buff("Bar", BuffType.Additive, -1f));
            Assert.AreEqual(0f, foo.Bar);
        }

        [Test]
        public void TestRangedMax() {
            var foo = new RangedFoo();
            foo.AddBuff(new Buff("Bar", BuffType.Additive, 2f));
            Assert.AreEqual(1f, foo.Bar);
        }

        [Test]
        public void TestAddNullBuffFails() {
            var foo = new Foo();
            Assert.That(() => foo.AddBuff(null), Throws.Exception);
        }

        [Test]
        public void TestAddDuplicateBuffFails() {
            var foo = new Foo();
            var add = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(add);
            Assert.That(() => foo.AddBuff(add), Throws.Exception);
        }

        [Test]
        public void TestOwner1() {
            var foo = new Foo();
            var add1 = new Buff("Bar", BuffType.Additive, 1f, this);
            foo.AddBuff(add1);
            var add2 = new Buff("Bar", BuffType.Additive, 1f, null);
            foo.AddBuff(add2);
            var add3 = new Buff("Bar", BuffType.Additive, 1f, this);
            foo.AddBuff(add3);
            Assert.AreEqual(3, foo.BuffCount);
            foo.RemoveBuffsWithOwner(this);
            Assert.AreEqual(1, foo.BuffCount);
        }
    }
}
