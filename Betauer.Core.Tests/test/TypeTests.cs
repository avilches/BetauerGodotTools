using System;
using System.Collections;
using System.Collections.Generic;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class TypeTests {
    [Test]
    public void ImplementsInterfaceTests() {
        // False
        Assert.That(typeof(List<>).ImplementsInterface(typeof(IList<string>)), Is.False);
        Assert.That(typeof(Dictionary<,>).ImplementsInterface(typeof(IDictionary<string, int>)), Is.False);
        Assert.That(typeof(IEnumerable).ImplementsInterface(typeof(IList)), Is.False);
        Assert.That(typeof(IEnumerable).ImplementsInterface(typeof(IList<string>)), Is.False);

        // True
        Assert.That(typeof(IList).ImplementsInterface(typeof(IEnumerable)), Is.True);
        Assert.That(typeof(List<string>).ImplementsInterface(typeof(IEnumerable)), Is.True);
        Assert.That(typeof(List<string>).ImplementsInterface(typeof(IEnumerable<>)), Is.True); // IsAssignableTo will fail
        Assert.That(typeof(List<string>).ImplementsInterface(typeof(IList<string>)), Is.True);
        Assert.That(typeof(List<string>).ImplementsInterface(typeof(IList<>)), Is.True); // IsAssignableTo will fail
        Assert.That(typeof(List<>).ImplementsInterface(typeof(IList<>)), Is.True); // IsAssignableTo will fail
        Assert.That(typeof(IList<string>).ImplementsInterface(typeof(IList<string>)), Is.True);
        Assert.That(typeof(IList<string>).ImplementsInterface(typeof(IList<>)), Is.True); // IsAssignableTo will fail
        Assert.That(typeof(IList<>).ImplementsInterface(typeof(IList<>)), Is.True); // IsAssignableTo will fail
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IEnumerable)), Is.True);
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IEnumerable<>)), Is.True); // IsAssignableTo will fail
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IDictionary<,>)), Is.True); // IsAssignableTo will fail
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IDictionary<string, int>)), Is.True);
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IReadOnlyDictionary<string, int>)), Is.True);
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IReadOnlyCollection<KeyValuePair<string, int>>)), Is.True);
        Assert.That(typeof(Dictionary<string, int>).ImplementsInterface(typeof(ICollection<KeyValuePair<string, int>>)), Is.True);
        Assert.That(typeof(Dictionary<,>).ImplementsInterface(typeof(IEnumerable)), Is.True);
        Assert.That(typeof(IDictionary<string, int>).ImplementsInterface(typeof(IEnumerable)), Is.True);
        Assert.That(typeof(IDictionary<string, int>).ImplementsInterface(typeof(IEnumerable<>)), Is.True);
        Assert.That(typeof(IDictionary<string, int>).ImplementsInterface(typeof(ICollection<KeyValuePair<string, int>>)), Is.True);
    }

    [Test]
    public void FindGenericsFromInterfaceDefinitionTests() {
        // Invalid parameter
        Assert.Throws<ArgumentException>(() => typeof(List<>).FindGenericsFromInterfaceDefinition(typeof(List<string>))); // a class
        Assert.Throws<ArgumentException>(() => typeof(List<>).FindGenericsFromInterfaceDefinition(typeof(IList<string>))); // an interface, but it's not a generic type definition
        Assert.Throws<ArgumentException>(() => typeof(IEnumerable).FindGenericsFromInterfaceDefinition(typeof(IList)));    // an interface, but it's not a generic type definition

        // Valid parameter, not found
        Assert.Throws<InvalidOperationException>(() => typeof(List).FindGenericsFromInterfaceDefinition(typeof(IList<>))); // Is not implemented

        Assert.That(typeof(List<string>).FindGenericsFromInterfaceDefinition(typeof(IList<>)), Is.EqualTo(new[] { typeof(string) }));
        Assert.That(typeof(Dictionary<string, int>).FindGenericsFromInterfaceDefinition(typeof(IDictionary<,>)),
            Is.EqualTo(new[] { typeof(string), typeof(int) }));
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

    public class ChildChild : Child {
    }


    [Test]
    public void IsGenericSubclassOfTest() {
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(Child)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(Parent<>)), Is.True); // IsSubClassOf will fail
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(Parent<object>)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(GrandParent<,>)), Is.True);  // IsSubClassOf will fail
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(GrandParent<object, string>)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(GrandGrandParent<>)), Is.True); // IsSubClassOf will fail
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(GrandGrandParent<string>)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(GPP)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(object)), Is.True);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(Parent<int>)), Is.False);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(List<>)), Is.False);
        Assert.That(typeof(ChildChild).IsGenericSubclassOf(typeof(string)), Is.False);

        Assert.That(typeof(object).IsGenericSubclassOf(typeof(string)), Is.False);
        Assert.That(typeof(object).IsGenericSubclassOf(typeof(object)), Is.False);
        Assert.That(typeof(string).IsGenericSubclassOf(typeof(string)), Is.False);
        Assert.That(typeof(string).IsGenericSubclassOf(typeof(object)), Is.True);
    }

    [Test]
    public void FindGenericsFromBaseTypeDefinitionTests() {
        // Invalid parameter
        Assert.Throws<ArgumentException>(() => typeof(ChildChild).FindGenericsFromBaseTypeDefinition(typeof(IList<string>)));  // an interface, but it's not a generic type definition
        Assert.Throws<ArgumentException>(() => typeof(ChildChild).FindGenericsFromBaseTypeDefinition(typeof(IList<>)));        // an interface 
        Assert.Throws<ArgumentException>(() => typeof(ChildChild).FindGenericsFromBaseTypeDefinition(typeof(GrandGrandParent<string>)));  // a class, but it's not a generic type definition

        // Valid parameter, not found
        Assert.Throws<InvalidOperationException>(() => typeof(ChildChild).FindGenericsFromBaseTypeDefinition(typeof(List<>)));
        
        Assert.That(typeof(ChildChild).FindGenericsFromBaseTypeDefinition(typeof(Parent<>)), Is.EqualTo(new[] { typeof(object) }));
        Assert.That(typeof(ChildChild).FindGenericsFromBaseTypeDefinition(typeof(GrandParent<,>)), Is.EqualTo(new[] { typeof(object), typeof(string) }));
        Assert.That(typeof(ChildChild).FindGenericsFromBaseTypeDefinition(typeof(GrandGrandParent<>)), Is.EqualTo(new[] { typeof(string) }));
    }
}