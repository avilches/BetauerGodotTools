using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Betauer.Tools.FastReflection.Tests; 

[TestRunner.Test(Only = true)]
public class FastReflectionTests {

    [AttributeUsage(AttributeTargets.All | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MyAttribute : Attribute {
        
    }
    public class GetterClass {
        [MyAttribute] 
        public int P { get; set; } = 1;
        public int Px { get; set; } = 100; // No attribute, ignored

        [MyAttribute] 
        public int PC => 3;
        public int PCx => 100; // No attribute, ignored

        [MyAttribute] 
        public int PIgnored { set { } } // No getter, ignored

        [MyAttribute] 
        public int F = 5;
        public int Fx = 100; // No attribute, ignored

        [MyAttribute]
        public int M() => 7; 
        public int Mx() => 100; // No attribute, ignored 
    }
    
    
    [TestRunner.Test]
    public void GetterTests() {
        var getters = typeof(GetterClass).GetGetters<MyAttribute>(MemberTypes.All, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(getters.Count, Is.EqualTo(4));
        var instance = new GetterClass();
        var sum = getters.Select(g => {
            var value = g.GetValue(instance);
            Assert.That(value, Is.TypeOf<int>());
            return (int)value;
        }).Sum();
        Assert.That(sum, Is.EqualTo(instance.P + instance.PC + instance.F + instance.M()));
    }
    
    public class SetterClass {
        [MyAttribute] 
        public int P { get; set; } = -1;
        public int Px { get; set; } = -1; // No attribute, ignored

        [MyAttribute] 
        public int PIgnored => -1; // Computed has no setter, ignored

        [MyAttribute] 
        public int F = -1;
        public int Fx = -1; // No attribute, ignored

        public int m = -1;
        [MyAttribute]
        public void M(int x) => m = x;
        public void Mx(int x) => throw new Exception(); // No attribute, ignored
    }
    
    [TestRunner.Test]
    public void SetterTests() {
        var setters = typeof(SetterClass).GetSetters<MyAttribute>(MemberTypes.All, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(setters.Count, Is.EqualTo(3));
        var instance = new SetterClass();
        setters.ForEach(g => {
            g.SetValue(instance, 3);
        });
        Assert.That(instance.P, Is.EqualTo(3));
        Assert.That(instance.Px, Is.EqualTo(-1));
        Assert.That(instance.F, Is.EqualTo(3));
        Assert.That(instance.Fx, Is.EqualTo(-1));
        Assert.That(instance.m, Is.EqualTo(3));
    }
}