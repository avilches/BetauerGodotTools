using System;
using System.Collections.Generic;
using Betauer.Core.PCG.Graph;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class CustomUnionFindTest {
    [Test]
    public void Test() {
        var uf = new CustomUnionFind();

        Assert.That(uf.GetNumSets(), Is.EqualTo(0));
        uf.AddRegion(8);
        
        Assert.That(uf.GetNumSets(), Is.EqualTo(1));
        uf.AddRegion(1);
        uf.AddRegion(2);
        uf.AddRegion(3);
        uf.AddRegion(4);

        Assert.That(uf.GetNumSets(), Is.EqualTo(5));

        var connections = new List<(int, int)> { (1, 8), (1, 2), (3, 4) };
        foreach (var (a, b) in connections) {
            uf.Union(a, b);
        }

        Assert.That(uf.Find(8), Is.EqualTo(1));
        Assert.That(uf.Find(1), Is.EqualTo(1));
        Assert.That(uf.Find(2), Is.EqualTo(1));
        Assert.That(uf.Find(3), Is.EqualTo(3));
        Assert.That(uf.Find(4), Is.EqualTo(3));

        Assert.IsFalse(uf.AreAllConnected());

        Assert.That(uf.GetFinalRegions().Keys.Count, Is.EqualTo(2));
        CollectionAssert.AreEquivalent(uf.GetFinalRegions()[1], new[] { 8, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetFinalRegions()[3], new[] { 3, 4 });
        
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(8), new[] { 8, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(1), new[] { 8, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(2), new[] { 8, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(3), new[] { 3, 4 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(4), new[] { 3, 4 });

        // Redundant or duplicated unions are ignored
        uf.Union(8, 1);
        uf.Union(1, 2);
        uf.Union(1, 8);
        uf.Union(2, 1);
        uf.Union(2, 8);
        uf.Union(8, 2);

        Assert.That(uf.Find(8), Is.EqualTo(1));
        Assert.That(uf.Find(1), Is.EqualTo(1));
        Assert.That(uf.Find(2), Is.EqualTo(1));
        Assert.That(uf.Find(3), Is.EqualTo(3));
        Assert.That(uf.Find(4), Is.EqualTo(3));

        Assert.IsFalse(uf.AreAllConnected());

        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(8), new[] { 8, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(1), new[] { 8, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(2), new[] { 8, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(3), new[] { 3, 4 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(4), new[] { 3, 4 });

        uf.Union(2, 3);

        Assert.IsTrue(uf.AreAllConnected());

        Assert.That(uf.Find(8), Is.EqualTo(1));
        Assert.That(uf.Find(1), Is.EqualTo(1));
        Assert.That(uf.Find(2), Is.EqualTo(1));
        Assert.That(uf.Find(3), Is.EqualTo(1));
        Assert.That(uf.Find(4), Is.EqualTo(1));

        Assert.That(uf.GetFinalRegions().Keys.Count, Is.EqualTo(1));
        CollectionAssert.AreEquivalent(uf.GetFinalRegions()[1], new[] { 8, 1, 2, 3, 4 });
        
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(8), new[] { 8, 1, 2, 3, 4 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(1), new[] { 8, 1, 2, 3, 4 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(2), new[] { 8, 1, 2, 3, 4 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(3), new[] { 8, 1, 2, 3, 4 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(4), new[] { 8, 1, 2, 3, 4 });
    }
}