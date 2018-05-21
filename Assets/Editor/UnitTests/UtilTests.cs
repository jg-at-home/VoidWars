using NUnit.Framework;

namespace VoidWars {
    public class UtilTests {
        [Test]
        public void TestWeightedSelect1() {
            var weights = new[] { 0.1f };
            var index = Util.RandomWeightedSelection(weights);
            Assert.AreEqual(0, index);
        }

        [Test]
        public void TestWeightedSelect2() {
            var weights = new[] { 0.1f, 0.2f };
            var counts = new[] { 0, 0 };
            for (int i = 0; i < 1000; ++i) {
                var index = Util.RandomWeightedSelection(weights);
                counts[index]++;
            }
            Assert.IsTrue(counts[0] < counts[1]);
        }

        [Test]
        public void TestWeightedSelect3() {
            var weights = new[] { 0.1f, 0.2f, 0.3f };
            var counts = new[] { 0, 0, 0 };
            for (int i = 0; i < 1000; ++i) {
                var index = Util.RandomWeightedSelection(weights);
                counts[index]++;
            }
            Assert.IsTrue(counts[0] < counts[1]);
            Assert.IsTrue(counts[1] < counts[2]);
        }

    }
}