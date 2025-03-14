using System.Linq;
using Betauer.Core.DataMath.Data;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestFixture]
public class FastSearchTest {

    [Test]
    public void FindMinimumValueTest() {
        Assert.AreEqual(1, FastSearch.FindMinimumValue(new[] { 1 }.ToList(), i => i));
        Assert.AreEqual(1, FastSearch.FindMinimumValue(new[] { 5, 1 }.ToList(), i => i));
        Assert.AreEqual(1, FastSearch.FindMinimumValue(new[] { 5, 1, 2 }.ToList(), i => i));
        Assert.AreEqual(1, FastSearch.FindMinimumValue(new[] { 5, 1, 2, 3, 1, 7 }.ToList(), i => i));
        
        Assert.AreEqual(1, FastSearch.FindMinimumValue(new[] { 1 }.ToList(), (i, j) => i.CompareTo(j)));
        Assert.AreEqual(1, FastSearch.FindMinimumValue(new[] { 5, 1 }.ToList(), (i, j) => i.CompareTo(j)));
        Assert.AreEqual(1, FastSearch.FindMinimumValue(new[] { 5, 1, 2 }.ToList(), (i, j) => i.CompareTo(j)));
        Assert.AreEqual(1, FastSearch.FindMinimumValue(new[] { 5, 1, 2, 3, 1, 7 }.ToList(), (i, j) => i.CompareTo(j)));
        
    }

}