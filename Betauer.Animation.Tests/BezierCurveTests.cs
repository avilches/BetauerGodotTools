using Betauer.Animation.Easing;
using NUnit.Framework;

namespace Betauer.Animation.Tests {
    [TestFixture]
    public class BezierCurveTests : NodeTest {
        [Test]
        public void BezierCurveTest() {
            // Two identical curves, but different instances
            var a = new BezierCurve("a", 0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, null);
            var b = new BezierCurve("b", 0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, null);

            // Different
            var c = new BezierCurve("c", 0f, 1f, 2f, 3f, 4f, 5f, 6f, 0f, null);

            Assert.That(a.Equals(b));
            Assert.That(b.Equals(a));


            Assert.That(!a.Equals(c));
            Assert.That(!c.Equals(a));
            Assert.That(!b.Equals(c));
            Assert.That(!c.Equals(b));

            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(c.GetHashCode()));
        }
    }
}