using Betauer.Core.Easing;
using NUnit.Framework;

namespace Betauer.Animation.Tests; 

[TestRunner.Test]
public partial class BezierCurveTests : NodeTest {
    [TestRunner.Test]
    public void BezierCurveTest() {
        // Two identical curves, but different instances
        var a = BezierCurve.Create(0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f);
        var b = BezierCurve.Create(0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f);

        // Different
        var c = BezierCurve.Create(0f, 1f, 2f, 3f, 4f, 5f, 6f, 0f);

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