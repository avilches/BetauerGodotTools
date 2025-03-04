using System;
using Betauer.Core.DataMath.Collision.Spatial2D;
using Betauer.Core.PCG.PoissonDiskSampling;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class UniformPoissonDiskTest {

    [Test]
    public void UniformPoissonTest() {
        var random = new Random(0);
        var u = new UniformPoissonSampler2D(400, 400, null, 100);
        for (int i = 10; i < 50; i++) {
            var spatial = new SpatialGrid(i);
            var points = u.Generate(i, random);
            points.ForEach(point => {
                spatial.Add(new Point(point));
            });
            spatial.ForEach<Point>(point => {
                Assert.That(spatial.IntersectCircle(point.Position.X, point.Position.Y, i/2f, point), Is.False);
            });
        }
    } 
}