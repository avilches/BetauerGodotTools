using System;
using System.Collections;
using System.Collections.Generic;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestRunner.Test]
public class TypeTests {
    [TestRunner.Test]
    public void ImplementsInterfaceTests() {
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

    public class GPP {
    }

    public class GrandGrandParent<B> : GPP {
    }
    
    public class GrandParent<T, B> : GrandGrandParent<B> {
    }
    
    public class Parent<T> : GrandParent<T, string> {
    }
    
    public class Child : Parent<object> {
    }
    
    public class ChildChild : Child {}


    [TestRunner.Test]
    public void IsGenericSubclassOfTests() {
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(Child)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(Parent<>)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(Parent<object>)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(GrandParent<,>)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(GrandParent<object, string>)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(GrandGrandParent<>)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(GrandGrandParent<string>)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(GPP)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(object)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(Parent<int>)), Is.False);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(List<>)), Is.False);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(string)), Is.False);
    }
    [TestRunner.Test]
    public void FindGenericsFromBaseTypeTests() {
        Assert.That(typeof(ChildChild).FindGenericsFromBaseType(typeof(Parent<>)), Is.EqualTo(new Type[]{ typeof(object)}));
        Assert.That(typeof(ChildChild).FindGenericsFromBaseType(typeof(GrandParent<,>)), Is.EqualTo(new Type[]{ typeof(object), typeof(string)}));
        Assert.That(typeof(ChildChild).FindGenericsFromBaseType(typeof(GrandGrandParent<>)), Is.EqualTo(new Type[]{ typeof(string)}));
    }
}