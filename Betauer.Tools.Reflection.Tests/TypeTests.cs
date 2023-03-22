using System;
using System.Collections;
using System.Collections.Generic;
using Betauer.Tools.Logging;
using NUnit.Framework;

namespace Betauer.Tools.Reflection.Tests; 

[TestFixture]
public class TypeTests {
    [Test]
    public void TraceLevelDefault() {
        // False
        Assert.That(typeof(List<>).ImplementsInterface(typeof(IList<string>)), Is.False);
        Assert.That(typeof(Dictionary<,>).ImplementsInterface(typeof(IDictionary<string,int>)), Is.False);
        Assert.That(typeof(IEnumerable).ImplementsInterface(typeof(IList)), Is.False);
        Assert.That(typeof(IEnumerable).ImplementsInterface(typeof(IList<string>)), Is.False);
            
        // True
        Assert.That(typeof(IList).ImplementsInterface(typeof(IEnumerable)), Is.True);
        Assert.That(typeof(List<string>).ImplementsInterface(typeof(IEnumerable)), Is.True);
        Assert.That(typeof(List<string>).ImplementsInterface(typeof(IEnumerable<>)), Is.True);
        Assert.That(typeof(List<string>).ImplementsInterface(typeof(IList<string>)), Is.True);
        Assert.That(typeof(List<string>).ImplementsInterface(typeof(IList<>)), Is.True);
        Assert.That(typeof(List<>).ImplementsInterface(typeof(IList<>)), Is.True);
        Assert.That(typeof(IList<string>).ImplementsInterface(typeof(IList<string>)), Is.True);
        Assert.That(typeof(IList<string>).ImplementsInterface(typeof(IList<>)), Is.True);
        Assert.That(typeof(IList<>).ImplementsInterface(typeof(IList<>)), Is.True);
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IEnumerable)), Is.True);
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IEnumerable<>)), Is.True);
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IDictionary<,>)), Is.True);
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IDictionary<string,int>)), Is.True);
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IReadOnlyDictionary<string,int>)), Is.True);
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IReadOnlyCollection<KeyValuePair<string,int>>)), Is.True);
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(ICollection<KeyValuePair<string,int>>)), Is.True);
        Assert.That(typeof(Dictionary<,>).ImplementsInterface(typeof(IEnumerable)), Is.True);
        Assert.That(typeof(IDictionary<string, int>).ImplementsInterface(typeof(IEnumerable)), Is.True);
        Assert.That(typeof(IDictionary<string, int>).ImplementsInterface(typeof(IEnumerable<>)), Is.True);
        Assert.That(typeof(IDictionary<string, int>).ImplementsInterface(typeof(ICollection<KeyValuePair<string,int>>)), Is.True);
    }
}