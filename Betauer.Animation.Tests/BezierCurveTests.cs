using Betauer.Animation.Easing;
using NUnit.Framework;

namespace Betauer.Animation.Tests {
    [TestFixture]
    public class BezierCurveTests : NodeTest {
        [Test]
        public void BezierCurveTest() {
            // Two identical curves, but different instances
            var a = new BezierCurve(0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f);
            var b = new BezierCurve(0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f);

            // Different
            var c = new BezierCurve(0f, 1f, 2f, 3f, 4f, 5f, 6f, 0f);

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