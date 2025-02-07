using System;
using System.Collections.Generic;
using Betauer.Core.PCG.Graph;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class UnionFindTest {
    
    [Test]
        public void Test() {
        var uf = new UnionFind(5);

        var connections = new List<(int, int)> { (0, 1), (1, 2), (3, 4) };
        foreach (var (a, b) in connections) {
            uf.Union(a, b);
        }

        Assert.That(uf.Find(0), Is.EqualTo(0));
        Assert.That(uf.Find(1), Is.EqualTo(0));
        Assert.That(uf.Find(2), Is.EqualTo(0));
        Assert.That(uf.Find(3), Is.EqualTo(3));
        Assert.That(uf.Find(4), Is.EqualTo(3));

        Assert.IsFalse(uf.AreAllConnected());

        Assert.That(uf.GetFinalRegions().Count, Is.EqualTo(2));
        CollectionAssert.AreEquivalent(uf.GetFinalRegions()[0], new[] { 0, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetFinalRegions()[1], new[] { 3, 4 });
        
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(0), new[] { 0, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(1), new[] { 0, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(2), new[] { 0, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(3), new[] { 3, 4 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(4), new[] { 3, 4 });

        // Redundant or duplicated unions are ignored
        uf.Union(0, 1);
        uf.Union(1, 2);
        uf.Union(1, 0);
        uf.Union(2, 1);
        uf.Union(2, 0);
        uf.Union(0, 2);

        Assert.That(uf.Find(0), Is.EqualTo(0));
        Assert.That(uf.Find(1), Is.EqualTo(0));
        Assert.That(uf.Find(2), Is.EqualTo(0));
        Assert.That(uf.Find(3), Is.EqualTo(3));
        Assert.That(uf.Find(4), Is.EqualTo(3));

        Assert.IsFalse(uf.AreAllConnected());

        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(0), new[] { 0, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(1), new[] { 0, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(2), new[] { 0, 1, 2 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(3), new[] { 3, 4 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(4), new[] { 3, 4 });

        uf.Union(2, 3);

        Assert.IsTrue(uf.AreAllConnected());

        Assert.That(uf.Find(0), Is.EqualTo(0));
        Assert.That(uf.Find(1), Is.EqualTo(0));
        Assert.That(uf.Find(2), Is.EqualTo(0));
        Assert.That(uf.Find(3), Is.EqualTo(0));
        Assert.That(uf.Find(4), Is.EqualTo(0));

        Assert.That(uf.GetFinalRegions().Count, Is.EqualTo(1));
        CollectionAssert.AreEquivalent(uf.GetFinalRegions()[0], new[] { 0, 1, 2, 3, 4 });
        
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(0), new[] { 0, 1, 2, 3, 4 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(1), new[] { 0, 1, 2, 3, 4 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(2), new[] { 0, 1, 2, 3, 4 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(3), new[] { 0, 1, 2, 3, 4 });
        CollectionAssert.AreEquivalent(uf.GetConnectedRegions(4), new[] { 0, 1, 2, 3, 4 });
    }

}