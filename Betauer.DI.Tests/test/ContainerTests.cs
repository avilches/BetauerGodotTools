using System;
using System.Collections.Generic;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.DI.Tests; 

public interface IInterface1 {
}

public abstract class AbstractClass {
}

public class ClassWith1Interface : IInterface1 {
}

[TestRunner.Test]
public class ContainerTests : Node {
    [TestRunner.Test(Description = "ResolveOr tests")]
    public void ResolveOrNullTests() {
        var di = new Container();
        var c = new ClassWith1Interface();
        Assert.That(di.ResolveOr<IInterface1>(() => c), Is.EqualTo(c));
        Assert.That(di.ResolveOr(() => "X"), Is.EqualTo("X"));
        Assert.That(di.ResolveOr("O", () => "X"), Is.EqualTo("X"));
    }

    [TestRunner.Test(Description = "Container in container, assert Contains and TryGetProvider")]
    public void Container() {
        var di = new Container();
        Assert.That(di.Resolve<Container>(), Is.EqualTo(di));
        Assert.That(di.Contains<Container>());

        Assert.That(di.GetProvider<Container>().Resolve(new ResolveContext(di)), Is.EqualTo(di));
        Assert.That(di.TryGetProvider<Container>(out var provider));
        Assert.That(!di.TryGetProvider<Node>(out var notFound));
        Assert.That(provider!.Resolve(new ResolveContext(di)), Is.EqualTo(di));
    }

    [TestRunner.Test(Description = "Resolve by types & alias not found")]
    public void NotFoundTests() {
        var di = new Container();

        // Not found types fail
        Assert.Throws<ServiceNotFoundException>(() => di.Resolve<IInterface1>());
        Assert.Throws<ServiceNotFoundException>(() => di.Resolve<IInterface1>("X"));
        Assert.Throws<ServiceNotFoundException>(() => di.Resolve("X"));
    }

    [TestRunner.Test(Description = "Resolve by name using a wrong type")]
    public void ResolveByNameWithWrongNameTests() {
        var di = new Container.Builder()
            .Register(Provider.Transient<ClassWith1Interface>("P")).Build();

        Assert.Throws<InvalidCastException>(() => di.Resolve<Node>("P"));
        Assert.Throws<InvalidCastException>(() => di.TryResolve<Node>("P", out var r));
    }

    [TestRunner.Test(Description = "CreateIfNotFound: if type is not found, create a new instance automatically (like transient)")]
    public void CreateTransientIfNotFoundTest() {
        var di = new Container() {
            CreateIfNotFound = true
        };

        var n1 = di.Resolve<Node>();
        var n2 = di.Resolve<Node>();
        Assert.That(n1, Is.Not.Null);
        Assert.That(n1, Is.Not.EqualTo(n2));

        // Not allowed interfaces
        Assert.Throws<MissingMethodException>(() => di.Resolve<IInterface1>());
        // Not allowed abstract classes
        Assert.Throws<MissingMethodException>(() => di.Resolve(typeof(AbstractClass)));

    }

    [TestRunner.Test(Description = "Error creating factories")]
    public void CantCreateFactoryFromInterfaceTest() {
        var di = new Container();
        Assert.Throws<MissingMethodException>(() => di.CreateBuilder().Register(Provider.Singleton<ClassWith1Interface, IInterface1>()).Build());
        Assert.Throws<MissingMethodException>(() => di.CreateBuilder().Register(Provider.Singleton<ClassWith1Interface, IInterface1>("P")).Build());
        Assert.Throws<MissingMethodException>(() => di.CreateBuilder().Register(Provider.Transient<ClassWith1Interface, IInterface1>()).Build());
        Assert.Throws<MissingMethodException>(() => di.CreateBuilder().Register(Provider.Transient<ClassWith1Interface, IInterface1>("P")).Build());
        Assert.Throws<MissingMethodException>(() => di.CreateBuilder().Register(Provider.Service<ClassWith1Interface, IInterface1>(Lifetime.Singleton)).Build());
        Assert.Throws<MissingMethodException>(() => di.CreateBuilder().Register(Provider.Service<ClassWith1Interface, IInterface1>(Lifetime.Singleton, "P")).Build());
        Assert.Throws<MissingMethodException>(() => di.CreateBuilder().Register(Provider.Service<ClassWith1Interface, IInterface1>(Lifetime.Transient)).Build());
        Assert.Throws<MissingMethodException>(() => di.CreateBuilder().Register(Provider.Service<ClassWith1Interface, IInterface1>(Lifetime.Transient, "P")).Build());
        Assert.Throws<MissingMethodException>(() => di.CreateBuilder().Register(Provider.Create<IInterface1, IInterface1>(Lifetime.Singleton)).Build());
        Assert.Throws<MissingMethodException>(() => di.CreateBuilder().Register(Provider.Create<IInterface1, IInterface1>(Lifetime.Singleton, null, "P")).Build());
        Assert.Throws<MissingMethodException>(() => di.CreateBuilder().Register(Provider.Create<IInterface1, IInterface1>(Lifetime.Transient)).Build());
        Assert.Throws<MissingMethodException>(() => di.CreateBuilder().Register(Provider.Create<IInterface1, IInterface1>(Lifetime.Transient, null, "P")).Build());
            
    }

    [TestRunner.Test(Description = "Resolve, TryResolve, Contains, TryGetProvider and GetProvider: type")]
    public void TypeTest() {
        Func<Container>[] x = {
            () => new Container.Builder().Register(Provider.Static(new ClassWith1Interface())).Build(),
            () => new Container.Builder().Register(Provider.Singleton<ClassWith1Interface>()).Build(),
            () => new Container.Builder().Register(Provider.Singleton(() => new ClassWith1Interface())).Build(),
            () => new Container.Builder().Register(Provider.Transient<ClassWith1Interface>()).Build(),
            () => new Container.Builder().Register(Provider.Transient(() => new ClassWith1Interface())).Build(),
            () => new Container.Builder().Register(Provider.Service<ClassWith1Interface>(Lifetime.Singleton)).Build(),
            () => new Container.Builder().Register(Provider.Service<ClassWith1Interface>(Lifetime.Transient)).Build(),
            () => new Container.Builder().Register(Provider.Service(() => new ClassWith1Interface(), Lifetime.Singleton)).Build(),
            () => new Container.Builder().Register(Provider.Service(() => new ClassWith1Interface(), Lifetime.Transient)).Build(),
        };
        foreach (var func in x) {
            Console.WriteLine($"Test #{x}");
            var c = func();
            Assert.That(c.Contains<ClassWith1Interface>());
            Assert.That(!c.Contains<IInterface1>());

            Assert.That(c.GetProvider<ClassWith1Interface>().ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.Throws<ServiceNotFoundException>(() => c.GetProvider<IInterface1>());

            Assert.That(c.TryGetProvider<ClassWith1Interface>(out var provider), Is.True);
            Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(c.TryGetProvider<IInterface1>(out provider), Is.False);
            Assert.That(provider, Is.Null);

            Assert.That(c.Resolve<ClassWith1Interface>(), Is.TypeOf<ClassWith1Interface>());
            Assert.Throws<ServiceNotFoundException>(() => c.Resolve<IInterface1>());
                
            Assert.That(c.TryResolve<ClassWith1Interface>(out var r), Is.True);
            Assert.That(r, Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.TryResolve<IInterface1>(out var i), Is.False);
            Assert.That(i, Is.Null);
        }
    }
        
    [TestRunner.Test(Description = "Resolve, TryResolve, Contains, TryGetProvider and GetProvider: othertype")]
    public void OtherTypeTest() {
        Func<Container>[] x = {
            () => new Container.Builder().Register(Provider.Static<IInterface1, ClassWith1Interface>(new ClassWith1Interface())).Build(),
            () => new Container.Builder().Register(Provider.Singleton<IInterface1, ClassWith1Interface>()).Build(),
            () => new Container.Builder().Register(Provider.Singleton<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface())).Build(),
            () => new Container.Builder().Register(Provider.Transient<IInterface1, ClassWith1Interface>()).Build(),
            () => new Container.Builder().Register(Provider.Transient<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface())).Build(),
            () => new Container.Builder().Register(Provider.Service<IInterface1, ClassWith1Interface>(Lifetime.Singleton)).Build(),
            () => new Container.Builder().Register(Provider.Service<IInterface1, ClassWith1Interface>(Lifetime.Transient)).Build(),
            () => new Container.Builder().Register(Provider.Service<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface(), Lifetime.Singleton)).Build(),
            () => new Container.Builder().Register(Provider.Service<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface(), Lifetime.Transient)).Build(),
        };
        foreach (var func in x) {
            Console.WriteLine($"Test #{x}");
            var c = func();
            Assert.That(!c.Contains<ClassWith1Interface>());
            Assert.That(c.Contains<IInterface1>());

            Assert.Throws<ServiceNotFoundException>(() => c.GetProvider<ClassWith1Interface>());
            Assert.That(c.GetProvider<IInterface1>().ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));

            Assert.That(c.TryGetProvider<ClassWith1Interface>(out var provider), Is.False);
            Assert.That(provider, Is.Null);
            Assert.That(c.TryGetProvider<IInterface1>(out provider), Is.True);
            Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));

            Assert.Throws<ServiceNotFoundException>(() => c.Resolve<ClassWith1Interface>());
            Assert.That(c.Resolve<IInterface1>(), Is.TypeOf<ClassWith1Interface>());
                
            Assert.That(c.TryResolve<ClassWith1Interface>(out var r), Is.False);
            Assert.That(r, Is.Null);
            Assert.That(c.TryResolve<IInterface1>(out var i), Is.True);
            Assert.That(i, Is.TypeOf<ClassWith1Interface>());

        }
    }
        
    [TestRunner.Test(Description = "Resolve, TryResolve, Contains, TryGetProvider and GetProvider: name")]
    public void NameTest() {
        Func<Container>[] x = {
            () => new Container.Builder().Register(Provider.Static(new ClassWith1Interface(), "P")).Build(),
            () => new Container.Builder().Register(Provider.Singleton<ClassWith1Interface>("P")).Build(),
            () => new Container.Builder().Register(Provider.Singleton(() => new ClassWith1Interface(), "P")).Build(),
            () => new Container.Builder().Register(Provider.Transient<ClassWith1Interface>("P")).Build(),
            () => new Container.Builder().Register(Provider.Transient(() => new ClassWith1Interface(), "P")).Build(),
            () => new Container.Builder().Register(Provider.Service<ClassWith1Interface>(Lifetime.Singleton, "P")).Build(),
            () => new Container.Builder().Register(Provider.Service<ClassWith1Interface>(Lifetime.Transient, "P")).Build(),
            () => new Container.Builder().Register(Provider.Service(() => new ClassWith1Interface(), Lifetime.Singleton, "P")).Build(),
            () => new Container.Builder().Register(Provider.Service(() => new ClassWith1Interface(), Lifetime.Transient, "P")).Build(),
        };
        foreach (var func in x) {
            Console.WriteLine($"Test #{x}");
            var c = func();
            // By name, any compatible type is ok
            Assert.That(c.Contains("P"));
            Assert.That(c.Contains<ClassWith1Interface>());
            Assert.That(!c.Contains<IInterface1>());

            Assert.That(c.GetProvider("P").ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(c.GetProvider<ClassWith1Interface>().ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.Throws<ServiceNotFoundException>(() => c.GetProvider<IInterface1>());
                
            Assert.That(c.TryGetProvider("P", out var provider), Is.True);
            Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(c.TryGetProvider("P", out provider), Is.True);
            Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(c.TryGetProvider<ClassWith1Interface>(out provider), Is.True);
            Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(c.TryGetProvider<IInterface1>(out provider), Is.False);
            Assert.That(provider, Is.Null);
                
            Assert.That(c.Resolve<ClassWith1Interface>("P"), Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.Resolve<IInterface1>("P"), Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.Resolve<object>("P"), Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.Resolve("P"), Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.Resolve<ClassWith1Interface>(), Is.TypeOf<ClassWith1Interface>());
            Assert.Throws<ServiceNotFoundException>(() => c.Resolve<IInterface1>());

            Assert.That(c.TryResolve<ClassWith1Interface>("P", out var r), Is.True);
            Assert.That(r, Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.TryResolve<IInterface1>("P", out var i), Is.True);
            Assert.That(i, Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.TryResolve<object>("P", out var o), Is.True);
            Assert.That(o, Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.TryResolve("P", out r), Is.True);
            Assert.That(r, Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.TryResolve<ClassWith1Interface>(out r), Is.True);
            Assert.That(r, Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.TryResolve<IInterface1>(out i), Is.False);
            Assert.That(i, Is.Null);
        }
    }

    [TestRunner.Test(Description = "Resolve, TryResolve, Contains, TryGetProvider and GetProvider: name other type")]
    public void NameOtherTypeTest() {
        Func<Container>[] x = {
            () => new Container.Builder().Register(Provider.Static<IInterface1, ClassWith1Interface>(new ClassWith1Interface(), "P")).Build(),
            () => new Container.Builder().Register(Provider.Singleton<IInterface1, ClassWith1Interface>("P")).Build(),
            () => new Container.Builder().Register(Provider.Singleton<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface(),"P")).Build(),
            () => new Container.Builder().Register(Provider.Transient<IInterface1, ClassWith1Interface>("P")).Build(),
            () => new Container.Builder().Register(Provider.Transient<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface(),"P")).Build(),
            () => new Container.Builder().Register(Provider.Service<IInterface1, ClassWith1Interface>(Lifetime.Singleton, "P")).Build(),
            () => new Container.Builder().Register(Provider.Service<IInterface1, ClassWith1Interface>(Lifetime.Transient, "P")).Build(),
            () => new Container.Builder().Register(Provider.Service<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface(), Lifetime.Singleton, "P")).Build(),
            () => new Container.Builder().Register(Provider.Service<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface(), Lifetime.Transient, "P")).Build(),
        };
        foreach (var func in x) {
            Console.WriteLine($"Test #{x}");
            var c = func();
            Assert.That(c.Contains("P"));
            Assert.That(!c.Contains<ClassWith1Interface>());
            Assert.That(c.Contains<IInterface1>());

            Assert.That(c.GetProvider("P").ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.Throws<ServiceNotFoundException>(() => c.GetProvider<ClassWith1Interface>());
            Assert.That(c.GetProvider<IInterface1>().ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));

            Assert.That(c.TryGetProvider<ClassWith1Interface>(out var provider), Is.False);
            Assert.That(provider, Is.Null);
            Assert.That(c.TryGetProvider<IInterface1>(out provider), Is.True);
            Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));

            Assert.That(c.Resolve<ClassWith1Interface>("P"), Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.Resolve<IInterface1>("P"), Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.Resolve<object>("P"), Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.Resolve("P"), Is.TypeOf<ClassWith1Interface>());
            Assert.Throws<ServiceNotFoundException>(() => c.Resolve<ClassWith1Interface>());
            Assert.That(c.Resolve<IInterface1>(), Is.TypeOf<ClassWith1Interface>());

            Assert.That(c.TryResolve<ClassWith1Interface>("P", out var r), Is.True);
            Assert.That(r, Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.TryResolve<IInterface1>("P", out var i), Is.True);
            Assert.That(i, Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.TryResolve<object>("P", out var o), Is.True);
            Assert.That(o, Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.TryResolve("P", out r), Is.True);
            Assert.That(r, Is.TypeOf<ClassWith1Interface>());
            Assert.That(c.TryResolve<ClassWith1Interface>(out r), Is.False);
            Assert.That(r, Is.Null);
            Assert.That(c.TryResolve<IInterface1>(out i), Is.True);
            Assert.That(i, Is.TypeOf<ClassWith1Interface>());
        }
    }

    [TestRunner.Test(Description = "GetAllInstances - no transients")]
    public void GetAllInstancesTransitionTests() {
        var b = new Container.Builder();
        b.Register(Provider.Transient<ClassWith1Interface>());
        b.Register(Provider.Transient<ClassWith1Interface>("A"));
        var c = b.Build();
        Assert.That(c.GetAllInstances<ClassWith1Interface>(), Is.Empty);
    }

    [TestRunner.Test(Description = "GetAllInstances - singleton")]
    public void GetAllInstancesSingletonTests() {
        var b = new Container.Builder();
        b.Register(Provider.Singleton<ClassWith1Interface>("A"));
        var c = b.Build();
        Assert.That(c.GetAllInstances<ClassWith1Interface>().Count, Is.EqualTo(1));
        Assert.That(c.GetAllInstances<IInterface1>().Count, Is.EqualTo(1));
        Assert.That(c.GetAllInstances<object>().Count, Is.EqualTo(2)); // +1 (Include the container)
    }

    [TestRunner.Test(Description = "GetAllInstances - singleton")]
    public void GetAllInstancesMultipleSingletonTests() {
        var b = new Container.Builder();
        b.Register(Provider.Transient<Node2D>()); // ignored
        b.Register(Provider.Singleton<Node>("n1"));
        b.Register(Provider.Singleton<Node2D>("n2"));
        b.Register(Provider.Singleton<ClassWith1Interface>("A"));
        b.Register(Provider.Static<IInterface1, ClassWith1Interface>(new ClassWith1Interface(), "B"));
        b.Register(Provider.Singleton<ClassWith1Interface>());
        var c = b.Build();
        Assert.That(c.GetAllInstances<ClassWith1Interface>().Count, Is.EqualTo(3));
        Assert.That(c.GetAllInstances<IInterface1>().Count, Is.EqualTo(3));
        Assert.That(c.GetAllInstances<Node>().Count, Is.EqualTo(2));
        Assert.That(c.GetAllInstances<Node2D>().Count, Is.EqualTo(1));
        Assert.That(c.GetAllInstances<object>().Count, Is.EqualTo(6)); // +1 (Include the container)
    }

    [TestRunner.Test(Description = "Static tests")]
    public void StaticTests() {
        var n1 = new ClassWith1Interface();
        var n2 = new ClassWith1Interface();
        var n3 = new ClassWith1Interface();
        var n4 = new ClassWith1Interface();
        var b = new Container.Builder();
        b.Register(Provider.Static(n1));
        b.Register(Provider.Static<IInterface1>(n2));
        b.Register(Provider.Static(n3, "3"));
        b.Register(Provider.Static(n4, "4"));
        var c = b.Build();
            
        Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n1));
        Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n1));
        Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(n2));
        Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(n2));
        Assert.That(c.Resolve<ClassWith1Interface>("3"), Is.EqualTo(n3));
        Assert.That(c.Resolve<IInterface1>("3"), Is.EqualTo(n3));
        Assert.That(c.Resolve<ClassWith1Interface>("4"), Is.EqualTo(n4));
        Assert.That(c.Resolve<IInterface1>("4"), Is.EqualTo(n4));
    }

    [TestRunner.Test(Description = "Singleton tests")]
    public void SingletonTests() {
        var b = new Container.Builder();
        b.Register(Provider.Singleton<ClassWith1Interface>());
        b.Register(Provider.Singleton<IInterface1>(() => new ClassWith1Interface()));
        b.Register(Provider.Singleton<ClassWith1Interface>("3"));
        b.Register(Provider.Singleton<IInterface1>(() => new ClassWith1Interface(), "4"));
        var c = b.Build();

        var n1 = c.Resolve<ClassWith1Interface>();
        var n2 = c.Resolve<IInterface1>();
        var n3 = c.Resolve<ClassWith1Interface>("3");
        var n4 = c.Resolve<ClassWith1Interface>("4");

        Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n1));
        Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n1));
        Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(n2));
        Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(n2));
        Assert.That(c.Resolve<ClassWith1Interface>("3"), Is.EqualTo(n3));
        Assert.That(c.Resolve<IInterface1>("3"), Is.EqualTo(n3));
        Assert.That(c.Resolve<ClassWith1Interface>("4"), Is.EqualTo(n4));
        Assert.That(c.Resolve<IInterface1>("4"), Is.EqualTo(n4));
    }
        
    [TestRunner.Test(Description = "Transient tests")]
    public void TransientTests() {
        var b = new Container.Builder();
        b.Register(Provider.Transient<ClassWith1Interface>());
        b.Register(Provider.Transient<IInterface1>(() => new ClassWith1Interface()));
        b.Register(Provider.Transient<ClassWith1Interface>("3"));
        b.Register(Provider.Transient<IInterface1>(() => new ClassWith1Interface(), "4"));
        var c = b.Build();

        var n11 = c.Resolve<ClassWith1Interface>();
        var n12 = c.Resolve<ClassWith1Interface>();
        Assert.That(n11, Is.Not.EqualTo(n12));
        var n21 = c.Resolve<IInterface1>();
        var n22 = c.Resolve<IInterface1>();
        Assert.That(n21, Is.Not.EqualTo(n22));
        var n31 = c.Resolve<ClassWith1Interface>("3");
        var n32 = c.Resolve<ClassWith1Interface>("3");
        Assert.That(n31, Is.Not.EqualTo(n32));
    }
        
    [TestRunner.Test(Description = "Not allow duplicates by type")]
    public void DuplicatedTypeTest() {
        var b1 = new Container.Builder();
        b1.Register(Provider.Singleton<ClassWith1Interface>());
        b1.Register(Provider.Transient<ClassWith1Interface>());
        Assert.Throws<DuplicateServiceException>(() => b1.Build());

        var b2 = new Container.Builder();
        b2.Register(Provider.Static(new ClassWith1Interface()));
        b2.Register(Provider.Static(new ClassWith1Interface()));
        Assert.Throws<DuplicateServiceException>(() => b2.Build());

    }

    [TestRunner.Test(Description = "Not allow duplicates by name")]
    public void DuplicatedNameTest() {
        var b1 = new Container.Builder();
        b1.Register(Provider.Singleton(() => new ClassWith1Interface(), "A"));
        b1.Register(Provider.Transient<IInterface1, ClassWith1Interface>("A"));
        Assert.Throws<DuplicateServiceException>(() => b1.Build());

        var b2 = new Container.Builder();
        b2.Register(Provider.Singleton<ClassWith1Interface>("A"));
        b2.Register(Provider.Static<IInterface1>(new ClassWith1Interface(), "A"));
        Assert.Throws<DuplicateServiceException>(() => b2.Build());

    }

    [TestRunner.Test(Description = "GetProvider and TryGetProvider with fallbacks")]
    public void GetProviderWithFallbackTest() {
        var b = new Container.Builder();
        var n1 = new ClassWith1Interface();
        b.Register(Provider.Static(n1, "1"));
        var c = b.Build();
        Assert.That(c.GetProvider<ClassWith1Interface>(), Is.EqualTo(c.GetProvider("1")));
        var foundByName = c.TryGetProvider<ClassWith1Interface>(out var pName);
        var foundFallback = c.TryGetProvider("1", out var fName);
        Assert.That(foundByName);
        Assert.That(foundFallback);
        Assert.That(pName, Is.EqualTo(fName));
    }

    [TestRunner.Test(Description = "Overwrite the fallback with type")]
    public void CreateFallbackTest() {
        var b = new Container.Builder();
        var n1 = new ClassWith1Interface();
        var n2 = new ClassWith1Interface();
        var typed = new ClassWith1Interface();
        b.Register(Provider.Static(n1, "1"));
        b.Register(Provider.Static(n2, "2"));
        // This one should overwrite the fallback
        b.Register(Provider.Static(typed));
        var c = b.Build();
            
        Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(typed));
        Assert.That(c.Resolve<ClassWith1Interface>("1"), Is.EqualTo(n1));
        Assert.That(c.Resolve<ClassWith1Interface>("2"), Is.EqualTo(n2));
    }

    [TestRunner.Test(Description = "Fallback to the first element registered by type when register by name and resolve by type")]
    public void FallbackFirstByTypeTest() {
        var b = new Container.Builder();
        var n1 = new ClassWith1Interface();
        var n2 = new ClassWith1Interface();
        b.Register(Provider.Static(n1, "1"));
        b.Register(Provider.Static(n2, "2"));
        var c = b.Build();
        Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n1));
    }

    [TestRunner.Test(Description = "Fallback to the primary element only if it's not registered by")]
    public void NoFallbackPrimaryByTypeTest() {
        var b = new Container.Builder();
        var n0 = new ClassWith1Interface();
        var n1 = new ClassWith1Interface();
        var n2 = new ClassWith1Interface();
        var n3 = new ClassWith1Interface();
        var n4 = new ClassWith1Interface();
        b.Register(Provider.Static(n0));
        b.Register(Provider.Static(n1, "1"));
        b.Register(Provider.Static(n2, "2", true));
        b.Register(Provider.Static(n3, "3", true));
        b.Register(Provider.Static(n4, "4"));
        var c = b.Build();
        Assert.That(c.Resolve<ClassWith1Interface>("1"), Is.EqualTo(n1));
        Assert.That(c.Resolve<ClassWith1Interface>("2"), Is.EqualTo(n2));
        Assert.That(c.Resolve<ClassWith1Interface>("3"), Is.EqualTo(n3));
        Assert.That(c.Resolve<ClassWith1Interface>("4"), Is.EqualTo(n4));

        Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n0));
    }

    [TestRunner.Test(Description = "Fallback to the primary element registered by type when register by name and resolve by type")]
    public void FallbackPrimaryByTypeTest() {
        var b = new Container.Builder();
        var n1 = new ClassWith1Interface();
        var n2 = new ClassWith1Interface();
        var n3 = new ClassWith1Interface();
        var n4 = new ClassWith1Interface();
        b.Register(Provider.Static(n1, "1"));
        b.Register(Provider.Static(n2, "2", true));
        b.Register(Provider.Static(n3, "3", true));
        b.Register(Provider.Static(n4, "4"));
        var c = b.Build();
        Assert.That(c.Resolve<ClassWith1Interface>("1"), Is.EqualTo(n1));
        Assert.That(c.Resolve<ClassWith1Interface>("2"), Is.EqualTo(n2));
        Assert.That(c.Resolve<ClassWith1Interface>("3"), Is.EqualTo(n3));
        Assert.That(c.Resolve<ClassWith1Interface>("4"), Is.EqualTo(n4));

        Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n3));
    }

    [TestRunner.Test(Description = "Lazy tests")]
    public void LazyTest() {
        var b = new Container.Builder();
        b.Register(Provider.Singleton<ClassWith1Interface>("NoLazy1"));
        b.Register(Provider.Singleton<ClassWith1Interface>("NoLazy2"));
        b.Register(Provider.Singleton<ClassWith1Interface>("Lazy1", false, true));
        b.Register(Provider.Singleton<ClassWith1Interface>("Lazy2", false, true));
        var c = b.Build();
            
        var noLazy1Provider = c.GetProvider("NoLazy1") as SingletonFactoryProvider;
        var noLazy2Provider = c.GetProvider("NoLazy2") as SingletonFactoryProvider;
        var lazy1Provider = c.GetProvider("Lazy1") as SingletonFactoryProvider;
        var lazy2Provider = c.GetProvider("Lazy2") as SingletonFactoryProvider;
            
        Assert.That(noLazy1Provider.Lazy, Is.False);
        Assert.That(noLazy1Provider.IsInstanceCreated, Is.True);
        Assert.That(noLazy1Provider.Instance, Is.TypeOf<ClassWith1Interface>());
            
        Assert.That(noLazy2Provider.Lazy, Is.False);
        Assert.That(noLazy2Provider.IsInstanceCreated, Is.True);
        Assert.That(noLazy2Provider.Instance, Is.TypeOf<ClassWith1Interface>());

        Assert.That(lazy1Provider.Lazy, Is.True);
        Assert.That(lazy1Provider.IsInstanceCreated, Is.False);
        Assert.That(lazy1Provider.Instance, Is.Null);
        Assert.That(lazy2Provider.Lazy, Is.True);
        Assert.That(lazy2Provider.IsInstanceCreated, Is.False);
        Assert.That(lazy2Provider.Instance, Is.Null);

        c.Resolve("Lazy1");
        Assert.That(lazy1Provider.Lazy, Is.True);
        Assert.That(lazy1Provider.IsInstanceCreated, Is.True);
        Assert.That(lazy1Provider.Instance, Is.TypeOf<ClassWith1Interface>());
        Assert.That(lazy2Provider.Lazy, Is.True);

        c.Resolve("Lazy2");
        Assert.That(lazy2Provider.Lazy, Is.True);
        Assert.That(lazy2Provider.IsInstanceCreated, Is.True);
        Assert.That(lazy2Provider.Instance, Is.TypeOf<ClassWith1Interface>());

    }

    [TestRunner.Test(Description = "Test OnCreated")]
    public void OnCreateTests() {
        var singletons = new List<object>();
        var transients = new List<object>();
        var calls = 0;
        var c = new Container();
        c.OnCreated += (lifetime, instance) => {
            if (lifetime == Lifetime.Singleton) {
                singletons.Add(instance);
            } else {
                transients.Add(instance);
            }
        };
        c.OnCreated += (lifetime, instance) => {
            calls++;
        };
        var b = new Container.Builder(c);
        b.Register(Provider.Static(new Node2D()));
        b.Register(Provider.Singleton<ClassWith1Interface>());
        b.Register(Provider.Transient<Node>());
        b.Build();
        c.Resolve<Node2D>();
        Assert.That(singletons.Count, Is.EqualTo(2)); // +1 because of Container
        Assert.That(singletons[1], Is.TypeOf<ClassWith1Interface>());
        c.Resolve<Node>();
        Assert.That(transients.Count, Is.EqualTo(1));
        Assert.That(transients[0], Is.TypeOf<Node>());
        c.Resolve<Node>();
        Assert.That(transients.Count, Is.EqualTo(2));
        Assert.That(transients[0], Is.TypeOf<Node>());
        Assert.That(transients[1], Is.TypeOf<Node>());
        Assert.That(transients[1], Is.Not.EqualTo(transients[0]));
            
        Assert.That(calls, Is.EqualTo(singletons.Count + transients.Count));
    }
}