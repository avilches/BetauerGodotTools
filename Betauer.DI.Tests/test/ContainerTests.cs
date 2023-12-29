using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.DI.Tests; 

public interface IInterface1 {
}

public abstract class AbstractClass {
}

public class Class1WithInterface : IInterface1 {
}

public class Class2WithInterface : IInterface1 {
}

[TestRunner.Test]
public class ContainerTests : Node {

    public readonly Type[] NotSupportedTypes = new Type[] { typeof(IInterface1), typeof(AbstractClass), typeof(List<>), typeof(void) };
    
    [TestRunner.Test(Description = "Default ctor types not supported")]
    // [TestRunner.Ignore("Fails in debugger because the NotSupportedException")]
    public void DefaultCtorTypes() {
        NotSupportedTypes.ForEach(type => {
            Assert.Throws<NotSupportedException>(() => new SingletonProvider(type, type, null, null, null, false, null));
            Assert.Throws<NotSupportedException>(() => new TransientProvider(type, type));
            Assert.Throws<NotSupportedException>(() => new TemporalProvider(type, type));
            Assert.Throws<NotSupportedException>(() => Provider.CreateCtor(type, Lifetime.Singleton));
            Assert.Throws<NotSupportedException>(() => Provider.CreateCtor(type, Lifetime.Transient));
        });
        Assert.Throws<NotSupportedException>(() => Provider.CreateCtor<IInterface1>(Lifetime.Singleton));
        Assert.Throws<NotSupportedException>(() => Provider.CreateCtor<IInterface1>(Lifetime.Transient));
        Assert.Throws<NotSupportedException>(() => Provider.CreateCtor<AbstractClass>(Lifetime.Singleton));
        Assert.Throws<NotSupportedException>(() => Provider.CreateCtor<AbstractClass>(Lifetime.Transient));

        
    }

    [TestRunner.Test(Description = "ResolveOr tests")]
    public void ResolveOrNullTests() {
        var di = new Container();
        var c = new Class1WithInterface();
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

    [TestRunner.Test(Description = "Resolve not found")]
    public void NotFoundTests() {
        var di = new Container();

        // Not found types fail
        Assert.Throws<ServiceNotFoundException>(() => di.Resolve<IInterface1>());
        Assert.Throws<ServiceNotFoundException>(() => di.Resolve(typeof(IInterface1)));
        Assert.Throws<ServiceNotFoundException>(() => di.Resolve<IInterface1>("X"));
        Assert.Throws<ServiceNotFoundException>(() => di.Resolve("X"));
        
        Assert.That(di.TryResolve<IInterface1>(out var _), Is.False);
        Assert.That(di.TryResolve(typeof(IInterface1), out var _), Is.False);
        Assert.That(di.TryResolve<IInterface1>("X", out var _), Is.False);
        Assert.That(di.TryResolve("X", out var _, typeof(IInterface1)), Is.False);

        Assert.That(di.Contains<IInterface1>(), Is.False);
        Assert.That(di.Contains(typeof(IInterface1)), Is.False);
        Assert.That(di.Contains<IInterface1>("X"), Is.False);
        Assert.That(di.Contains("X", typeof(IInterface1)), Is.False);
    }

    [TestRunner.Test(Description = "Resolve by name using a wrong type fails because type validation")]
    public void ResolveByNameWithWrongNameTests() {
        var c = new Container();
        c.Build(di => { di.Register(Provider.Transient<Class1WithInterface>("P")); });

        Assert.That(c.Resolve("P"), Is.TypeOf<Class1WithInterface>());
        Assert.That(c.Resolve("P", typeof(Class1WithInterface)), Is.TypeOf<Class1WithInterface>());
        Assert.That(c.Resolve<Class1WithInterface>("P"), Is.TypeOf<Class1WithInterface>());
        Assert.That(c.Resolve<object>("P"), Is.TypeOf<Class1WithInterface>());
        
        Assert.Throws<ServiceNotFoundException>(() => c.Resolve("P", typeof(Node)));
        Assert.Throws<ServiceNotFoundException>(() => c.Resolve<Node>("P"));
        Assert.That(c.TryResolve<Node>("P", out var _), Is.False);
        Assert.That(c.TryResolve("P", out var _, typeof(Node)), Is.False);
    }

    [TestRunner.Test(Description = "CreateIfNotFound when Resolve(): if type is not found, create a new instance automatically (like transient)")]
    public void CreateTransientIfNotFoundTest() {
        var di = new Container() {
            CreateIfNotFound = true
        };

        var n1 = di.Resolve<Node>();
        var n2 = di.Resolve<Node>();
        Assert.That(n1, Is.Not.Null);
        Assert.That(n2, Is.Not.Null);
        Assert.That(n1, Is.Not.EqualTo(n2));

        NotSupportedTypes.ForEach(type => {
            Assert.Throws<NotSupportedException>(() => di.Resolve(type));
        });
    }

    [Transient]
    public class CreateIfNotFoundClass {
        [Inject] public Node N1 { get; set; }
        [Inject] public Node N2 { get; set; }
    }
    
    [TestRunner.Test(Description = "CreateIfNotFound when injecting: if type is not found, create a new instance automatically (like transient)")]
    public void CreateTransientIfNotFoundTestScan() {
        var c = new Container() {
            CreateIfNotFound = true
        };
        c.Build(build => build.Scan<CreateIfNotFoundClass>());

        var test = c.Resolve<CreateIfNotFoundClass>();
        Assert.That(test.N1, Is.Not.Null);
        Assert.That(test.N2, Is.Not.Null);
        Assert.That(test, Is.Not.EqualTo(test.N2));
    }

    [TestRunner.Test(Description = "Error creating providers when public type is not assignable from real type")]
    public void ErrorCreatingProviderTest() {
        var c = new Container();
        Assert.Throws<InvalidCastException>(() => c.Build(di => di.Register(Provider.Singleton<Class1WithInterface, IInterface1>())));
        Assert.Throws<InvalidCastException>(() => c.Build(di => di.Register(Provider.Singleton<Class1WithInterface, IInterface1>())));
        Assert.Throws<InvalidCastException>(() => c.Build(di => di.Register(Provider.Transient<Class1WithInterface, IInterface1>())));
        Assert.Throws<InvalidCastException>(() => c.Build(di => di.Register(Provider.Temporal<Class1WithInterface, IInterface1>())));
        Assert.Throws<InvalidCastException>(() => c.Build(di => di.Register(Provider.Static<Class1WithInterface, IInterface1>(new Class1WithInterface()))));
        Assert.Throws<InvalidCastException>(() => c.Build(di => di.Register(Provider.Temporal<Class1WithInterface, IInterface1>())));
    }

    [TestRunner.Test(Description = "Resolve, TryResolve, Contains, TryGetProvider and GetProvider: type")]
    public void TypeTest() {
        Action<Container>[] x = {
            (c) => c.Build(di => di.Register(Provider.Static(new Class1WithInterface()))),
            (c) => c.Build(di => di.Register(Provider.Singleton<Class1WithInterface>())),
            (c) => c.Build(di => di.Register(Provider.Singleton(() => new Class1WithInterface()))),
            (c) => c.Build(di => di.Register(Provider.Transient<Class1WithInterface>())),
            (c) => c.Build(di => di.Register(Provider.Transient(() => new Class1WithInterface()))),
        };
        foreach (var func in x) {
            Console.WriteLine($"Test #{x}");
            var c = new Container();
            func(c);
            Assert.That(c.Contains<Class1WithInterface>());
            Assert.That(!c.Contains<IInterface1>());

            Assert.That(c.GetProvider<Class1WithInterface>().RealType, Is.EqualTo(typeof(Class1WithInterface)));
            Assert.Throws<ServiceNotFoundException>(() => c.GetProvider<IInterface1>());

            Assert.That(c.TryGetProvider<Class1WithInterface>(out var provider), Is.True);
            Assert.That(provider.RealType, Is.EqualTo(typeof(Class1WithInterface)));
            Assert.That(c.TryGetProvider<IInterface1>(out provider), Is.False);
            Assert.That(provider, Is.Null);

            Assert.That(c.Resolve<Class1WithInterface>(), Is.TypeOf<Class1WithInterface>());
            Assert.Throws<ServiceNotFoundException>(() => c.Resolve<IInterface1>());
                
            Assert.That(c.TryResolve<Class1WithInterface>(out var r), Is.True);
            Assert.That(r, Is.TypeOf<Class1WithInterface>());
            Assert.That(c.TryResolve<IInterface1>(out var i), Is.False);
            Assert.That(i, Is.Null);
        }
    }
        
    [TestRunner.Test(Description = "Resolve, TryResolve, Contains, TryGetProvider and GetProvider: othertype")]
    public void OtherTypeTest() {
        Action<Container>[] x = {
            (c) => c.Build(di => di.Register(Provider.Static<IInterface1, Class1WithInterface>(new Class1WithInterface()))),
            (c) => c.Build(di => di.Register(Provider.Singleton<IInterface1, Class1WithInterface>())),
            (c) => c.Build(di => di.Register(Provider.Singleton<IInterface1, Class1WithInterface>(() => new Class1WithInterface()))),
            (c) => c.Build(di => di.Register(Provider.Transient<IInterface1, Class1WithInterface>())),
            (c) => c.Build(di => di.Register(Provider.Transient<IInterface1, Class1WithInterface>(() => new Class1WithInterface()))),
        };
        foreach (var func in x) {
            Console.WriteLine($"Test #{x}");
            var c = new Container();
            func(c);
            Assert.That(!c.Contains<Class1WithInterface>());
            Assert.That(c.Contains<IInterface1>());

            Assert.Throws<ServiceNotFoundException>(() => c.GetProvider<Class1WithInterface>());
            Assert.That(c.GetProvider<IInterface1>().RealType, Is.EqualTo(typeof(Class1WithInterface)));

            Assert.That(c.TryGetProvider<Class1WithInterface>(out var provider), Is.False);
            Assert.That(provider, Is.Null);
            Assert.That(c.TryGetProvider<IInterface1>(out provider), Is.True);
            Assert.That(provider.RealType, Is.EqualTo(typeof(Class1WithInterface)));

            Assert.Throws<ServiceNotFoundException>(() => c.Resolve<Class1WithInterface>());
            Assert.That(c.Resolve<IInterface1>(), Is.TypeOf<Class1WithInterface>());
                
            Assert.That(c.TryResolve<Class1WithInterface>(out var r), Is.False);
            Assert.That(r, Is.Null);
            Assert.That(c.TryResolve<IInterface1>(out var i), Is.True);
            Assert.That(i, Is.TypeOf<Class1WithInterface>());

        }
    }
        
    [TestRunner.Test(Description = "Resolve, TryResolve, Contains, TryGetProvider and GetProvider: name")]
    public void NameTest() {
        Action<Container>[] x = {
            (c) => c.Build(di => di.Register(Provider.Static(new Class1WithInterface(), "P"))),
            (c) => c.Build(di => di.Register(Provider.Singleton<Class1WithInterface>("P"))),
            (c) => c.Build(di => di.Register(Provider.Singleton(() => new Class1WithInterface(), "P"))),
            (c) => c.Build(di => di.Register(Provider.Transient<Class1WithInterface>("P"))),
            (c) => c.Build(di => di.Register(Provider.Transient(() => new Class1WithInterface(), "P"))),
        };
        foreach (var func in x) {
            Console.WriteLine($"Test #{x}");
            var c = new Container();
            func(c);
            // By name, any compatible type is ok
            Assert.That(c.Contains("P"));
            Assert.That(!c.Contains<Class1WithInterface>());
            Assert.That(!c.Contains<IInterface1>());

            Assert.That(c.GetProvider("P").RealType, Is.EqualTo(typeof(Class1WithInterface)));
            Assert.Throws<ServiceNotFoundException>(() => c.GetProvider<Class1WithInterface>());
            Assert.Throws<ServiceNotFoundException>(() => c.GetProvider<IInterface1>());
                
            Assert.That(c.TryGetProvider("P", out var provider), Is.True);
            Assert.That(provider.RealType, Is.EqualTo(typeof(Class1WithInterface)));
            Assert.That(c.TryGetProvider("P", out provider), Is.True);
            Assert.That(provider.RealType, Is.EqualTo(typeof(Class1WithInterface)));
            Assert.That(c.TryGetProvider<Class1WithInterface>(out provider), Is.False);
            Assert.That(provider, Is.Null);
            Assert.That(c.TryGetProvider<IInterface1>(out provider), Is.False);
            Assert.That(provider, Is.Null);
                
            Assert.That(c.Resolve<Class1WithInterface>("P"), Is.TypeOf<Class1WithInterface>());
            Assert.That(c.Resolve<IInterface1>("P"), Is.TypeOf<Class1WithInterface>());
            Assert.That(c.Resolve<object>("P"), Is.TypeOf<Class1WithInterface>());
            Assert.That(c.Resolve("P"), Is.TypeOf<Class1WithInterface>());
            Assert.Throws<ServiceNotFoundException>(() => c.Resolve<Class1WithInterface>());
            Assert.Throws<ServiceNotFoundException>(() => c.Resolve<IInterface1>());

            Assert.That(c.TryResolve<Class1WithInterface>("P", out var r), Is.True);
            Assert.That(r, Is.TypeOf<Class1WithInterface>());
            Assert.That(c.TryResolve<IInterface1>("P", out var i), Is.True);
            Assert.That(i, Is.TypeOf<Class1WithInterface>());
            Assert.That(c.TryResolve<object>("P", out var o), Is.True);
            Assert.That(o, Is.TypeOf<Class1WithInterface>());
            Assert.That(c.TryResolve("P", out r), Is.True);
            Assert.That(r, Is.TypeOf<Class1WithInterface>());
            Assert.That(c.TryResolve<IInterface1>(out i), Is.False);
            Assert.That(i, Is.Null);
            Assert.That(c.TryResolve<IInterface1>(out i), Is.False);
            Assert.That(i, Is.Null);
        }
    }

    [TestRunner.Test(Description = "Resolve, TryResolve, Contains, TryGetProvider and GetProvider: name other type")]
    public void NameOtherTypeTest() {
        Action<Container>[] x = {
            (c) => c.Build(di => di.Register(Provider.Static<IInterface1, Class1WithInterface>(new Class1WithInterface(), "P"))),
            (c) => c.Build(di => di.Register(Provider.Singleton<IInterface1, Class1WithInterface>("P"))),
            (c) => c.Build(di => di.Register(Provider.Singleton<IInterface1, Class1WithInterface>(() => new Class1WithInterface(),"P"))),
            (c) => c.Build(di => di.Register(Provider.Transient<IInterface1, Class1WithInterface>("P"))),
            (c) => c.Build(di => di.Register(Provider.Transient<IInterface1, Class1WithInterface>(() => new Class1WithInterface(),"P"))),
        };
        foreach (var func in x) {
            Console.WriteLine($"Test #{x}");
            var c = new Container();
            func(c);
            Assert.That(c.Contains("P"));
            Assert.That(!c.Contains<Class1WithInterface>());
            Assert.That(!c.Contains<IInterface1>());

            Assert.That(c.GetProvider("P").RealType, Is.EqualTo(typeof(Class1WithInterface)));
            Assert.Throws<ServiceNotFoundException>(() => c.GetProvider<Class1WithInterface>());
            Assert.Throws<ServiceNotFoundException>(() => c.GetProvider<IInterface1>());

            Assert.That(c.TryGetProvider<Class1WithInterface>(out var provider), Is.False);
            Assert.That(provider, Is.Null);
            Assert.That(c.TryGetProvider<IInterface1>(out provider), Is.False);
            Assert.That(provider, Is.Null);

            Assert.That(c.Resolve<Class1WithInterface>("P"), Is.TypeOf<Class1WithInterface>());
            Assert.That(c.Resolve<IInterface1>("P"), Is.TypeOf<Class1WithInterface>());
            Assert.That(c.Resolve<object>("P"), Is.TypeOf<Class1WithInterface>());
            Assert.That(c.Resolve("P"), Is.TypeOf<Class1WithInterface>());
            Assert.Throws<ServiceNotFoundException>(() => c.Resolve<Class1WithInterface>());
            Assert.Throws<ServiceNotFoundException>(() => c.Resolve<IInterface1>());

            Assert.That(c.TryResolve<Class1WithInterface>("P", out var r), Is.True);
            Assert.That(r, Is.TypeOf<Class1WithInterface>());
            Assert.That(c.TryResolve<IInterface1>("P", out var i), Is.True);
            Assert.That(i, Is.TypeOf<Class1WithInterface>());
            Assert.That(c.TryResolve<object>("P", out var o), Is.True);
            Assert.That(o, Is.TypeOf<Class1WithInterface>());
            Assert.That(c.TryResolve("P", out r), Is.True);
            Assert.That(r, Is.TypeOf<Class1WithInterface>());
            Assert.That(c.TryResolve<Class1WithInterface>(out r), Is.False);
            Assert.That(r, Is.Null);
            Assert.That(c.TryResolve<IInterface1>(out i), Is.False);
            Assert.That(i, Is.Null);
        }
    }

    [TestRunner.Test(Description = "GetAllInstances - transients")]
    public void GetAllInstancesTransitionTests() {
        var c = new Container();
        c.Build(b => {
            b.Register(Provider.Transient<Class1WithInterface>());
            b.Register(Provider.Transient<Class1WithInterface>("A"));
        });
        Assert.That(c.ResolveAll<Class1WithInterface>(), Has.Count.EqualTo(2));
    }

    [TestRunner.Test(Description = "GetAllInstances - singleton")]
    public void GetAllInstancesSingletonTests() {
        var c = new Container();
        c.Build(b => {
            b.Register(Provider.Singleton<Class1WithInterface>("A"));
            b.Register(Provider.Singleton<IInterface1, Class1WithInterface>("B"));
            b.Register(Provider.Singleton<IInterface1, Class2WithInterface>("C"));
        });
        Assert.That(c.ResolveAll<Class1WithInterface>().Count, Is.EqualTo(2));
        Assert.That(c.ResolveAll<Class2WithInterface>().Count, Is.EqualTo(1));
        Assert.That(c.ResolveAll<IInterface1>().Count, Is.EqualTo(3));
        Assert.That(c.ResolveAll<object>().Count, Is.EqualTo(4)); // +1 (Include the container)
    }

    [TestRunner.Test(Description = "GetAllInstances - singleton")]
    public void GetAllInstancesMultipleSingletonTests() {
        var c = new Container();
        c.Build(b => {
            b.Register(Provider.Transient<Node2D>()); // ignored
            b.Register(Provider.Singleton<Node>("n1"));
            b.Register(Provider.Singleton<Node2D>("n2"));
            b.Register(Provider.Singleton<Class1WithInterface>("A"));
            b.Register(Provider.Static<IInterface1, Class1WithInterface>(new Class1WithInterface(), "B"));
            b.Register(Provider.Singleton<Class1WithInterface>());
        });
        Assert.That(c.ResolveAll<Class1WithInterface>().Count, Is.EqualTo(3));
        Assert.That(c.ResolveAll<IInterface1>().Count, Is.EqualTo(3));
        Assert.That(c.ResolveAll<Node>().Count, Is.EqualTo(3));
        Assert.That(c.ResolveAll<Node2D>().Count, Is.EqualTo(2));
        Assert.That(c.ResolveAll<object>().Count, Is.EqualTo(7)); // +1 (Include the container)
    }

    [TestRunner.Test(Description = "Static tests")]
    public void StaticTests() {
        var n1 = new Class1WithInterface();
        var n2 = new Class1WithInterface();
        var n3 = new Class1WithInterface();
        var n4 = new Class1WithInterface();
        var c = new Container();
        c.Build(b => {
            b.Register(Provider.Static(n1));
            b.Register(Provider.Static<IInterface1>(n2));
            b.Register(Provider.Static(n3, "3"));
            b.Register(Provider.Static(n4, "4"));
        });
            
        Assert.That(c.Resolve<Class1WithInterface>(), Is.EqualTo(n1));
        Assert.That(c.Resolve<Class1WithInterface>(), Is.EqualTo(n1));
        Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(n2));
        Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(n2));
        Assert.That(c.Resolve<Class1WithInterface>("3"), Is.EqualTo(n3));
        Assert.That(c.Resolve<IInterface1>("3"), Is.EqualTo(n3));
        Assert.That(c.Resolve<Class1WithInterface>("4"), Is.EqualTo(n4));
        Assert.That(c.Resolve<IInterface1>("4"), Is.EqualTo(n4));
    }

    [TestRunner.Test(Description = "Singleton tests")]
    public void SingletonTests() {
        var c = new Container();
        c.Build(b => {
            b.Register(Provider.Singleton<Class1WithInterface>());
            b.Register(Provider.Singleton<IInterface1, Class1WithInterface>(() => new Class1WithInterface()));
            b.Register(Provider.Singleton<Class1WithInterface>("3"));
            b.Register(Provider.Singleton<IInterface1, Class1WithInterface>(() => new Class1WithInterface(), "4"));
        });

        var n1 = c.Resolve<Class1WithInterface>();
        var n2 = c.Resolve<IInterface1>();
        var n3 = c.Resolve<Class1WithInterface>("3");
        var n4 = c.Resolve<IInterface1>("4");

        Assert.That(c.Resolve<Class1WithInterface>(), Is.EqualTo(n1));
        Assert.That(c.Resolve<Class1WithInterface>(), Is.EqualTo(n1));
        Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(n2));
        Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(n2));
        Assert.That(c.Resolve<Class1WithInterface>("3"), Is.EqualTo(n3));
        Assert.That(c.Resolve<IInterface1>("3"), Is.EqualTo(n3));
        Assert.That(c.Resolve<Class1WithInterface>("4"), Is.EqualTo(n4));
        Assert.That(c.Resolve<IInterface1>("4"), Is.EqualTo(n4));
    }
        
    [TestRunner.Test(Description = "Transient tests")]
    public void TransientTests() {
        var c = new Container();
        c.Build(b => {
            b.Register(Provider.Transient<Class1WithInterface>());
            b.Register(Provider.Transient<IInterface1>(() => new Class1WithInterface()));
            b.Register(Provider.Transient<Class1WithInterface>("3"));
            b.Register(Provider.Transient<IInterface1>(() => new Class1WithInterface(), "4"));
        });

        var n11 = c.Resolve<Class1WithInterface>();
        var n12 = c.Resolve<Class1WithInterface>();
        Assert.That(n11, Is.Not.EqualTo(n12));
        var n21 = c.Resolve<IInterface1>();
        var n22 = c.Resolve<IInterface1>();
        Assert.That(n21, Is.Not.EqualTo(n22));
        var n31 = c.Resolve<Class1WithInterface>("3");
        var n32 = c.Resolve<Class1WithInterface>("3");
        Assert.That(n31, Is.Not.EqualTo(n32));
    }
        
    [TestRunner.Test(Description = "Not allow duplicates by type")]
    public void DuplicatedTypeTest() {
        Assert.Throws<DuplicateServiceException>(() => {
            var c = new Container();
            c.Build(b1 => {
                b1.Register(Provider.Singleton<Class1WithInterface>());
                b1.Register(Provider.Transient<Class1WithInterface>());
            });
        });

        Assert.Throws<DuplicateServiceException>(() => {
            var c2 = new Container();
            c2.Build(b2 => {
                b2.Register(Provider.Static(new Class1WithInterface()));
                b2.Register(Provider.Static(new Class1WithInterface()));
            });
        });
    }

    [TestRunner.Test(Description = "Not allow duplicates by name")]
    public void DuplicatedNameTest() {
        Assert.Throws<DuplicateServiceException>(() => {
            var c = new Container();
            c.Build(b1 => {
                b1.Register(Provider.Singleton(() => new Class1WithInterface(), "A"));
                b1.Register(Provider.Transient<IInterface1, Class1WithInterface>("A"));
            });
        });

        Assert.Throws<DuplicateServiceException>(() => {
            var c2 = new Container();
            c2.Build(b2 => {
                b2.Register(Provider.Singleton<Class1WithInterface>("A"));
                b2.Register(Provider.Static<IInterface1>(new Class1WithInterface(), "A"));
            });
        });
    }
    
    [TestRunner.Test(Description = "Lazy tests")]
    public void LazyTest() {
        var c = new Container();
        c.Build(b => {
            b.Register(Provider.Singleton<Class1WithInterface>("NoLazy1"));
            b.Register(Provider.Singleton<Class1WithInterface>("NoLazy2"));
            b.Register(Provider.Singleton<Class1WithInterface>("Lazy1", true));
            b.Register(Provider.Singleton<Class1WithInterface>("Lazy2", true));
        });

        var noLazy1Provider = c.GetProvider("NoLazy1") as SingletonProvider;
        var noLazy2Provider = c.GetProvider("NoLazy2") as SingletonProvider;
        var lazy1Provider = c.GetProvider("Lazy1") as SingletonProvider;
        var lazy2Provider = c.GetProvider("Lazy2") as SingletonProvider;
            
        Assert.That(noLazy1Provider.Lazy, Is.False);
        Assert.That(noLazy1Provider.IsInstanceCreated, Is.True);
        Assert.That(noLazy1Provider.Instance, Is.TypeOf<Class1WithInterface>());
            
        Assert.That(noLazy2Provider.Lazy, Is.False);
        Assert.That(noLazy2Provider.IsInstanceCreated, Is.True);
        Assert.That(noLazy2Provider.Instance, Is.TypeOf<Class1WithInterface>());

        Assert.That(lazy1Provider.Lazy, Is.True);
        Assert.That(lazy1Provider.IsInstanceCreated, Is.False);
        Assert.That(lazy1Provider.Instance, Is.Null);
        Assert.That(lazy2Provider.Lazy, Is.True);
        Assert.That(lazy2Provider.IsInstanceCreated, Is.False);
        Assert.That(lazy2Provider.Instance, Is.Null);

        c.Resolve("Lazy1");
        Assert.That(lazy1Provider.Lazy, Is.True);
        Assert.That(lazy1Provider.IsInstanceCreated, Is.True);
        Assert.That(lazy1Provider.Instance, Is.TypeOf<Class1WithInterface>());
        Assert.That(lazy2Provider.Lazy, Is.True);

        c.Resolve("Lazy2");
        Assert.That(lazy2Provider.Lazy, Is.True);
        Assert.That(lazy2Provider.IsInstanceCreated, Is.True);
        Assert.That(lazy2Provider.Instance, Is.TypeOf<Class1WithInterface>());

    }

    [TestRunner.Test(Description = "Test OnInstanceCreated singleton")]
    public void OnInstanceCreatedSingletonTest() {
        var calls = 0;
        var c = new Container();
        c.OnInstanceCreated += (instanceCreatedEvent) => {
            Assert.That(instanceCreatedEvent.Lifetime, Is.EqualTo(Lifetime.Singleton));
            Assert.That(instanceCreatedEvent.Name, Is.Null);
            Assert.That(instanceCreatedEvent.GetMetadata("k"), Is.EqualTo("singleton"));
            Assert.That(instanceCreatedEvent.GetMetadata<string>("k"), Is.EqualTo("singleton"));
            Assert.That(instanceCreatedEvent.GetFlag("flag1"), Is.True);
            Assert.That(instanceCreatedEvent.GetFlag("flag2"), Is.False);
            Assert.That(instanceCreatedEvent.GetFlag("flag2", true), Is.True);
            calls++;
        };
        c.OnInstanceCreated += (instanceCreatedEvent) => {
            calls++;
        };
        c.Build(b => {
            b.Register(Provider.Static(new Node2D()));
            b.Register(Provider.Singleton<Class1WithInterface>(null, false, new Dictionary<string, object>() {{"flag1", true}, {"k", "singleton"}}));
            b.Register(Provider.Singleton<Button>(null, true, new Dictionary<string, object>() {{"flag1", true}, {"k", "singleton"}}));
        });
        Assert.That(calls, Is.EqualTo(2)); // the non lazy singleton triggers OnInstanceCreated
        
        c.Resolve<Node2D>(); // Static values don't trigger OnInstanceCreated
        Assert.That(calls, Is.EqualTo(2));
        
        c.Resolve<Button>();
        Assert.That(calls, Is.EqualTo(4));
    }

    [TestRunner.Test(Description = "Test OnInstanceCreated transients")]
    public void OnInstanceCreatedTransientTest() {
        var calls = 0;
        var c = new Container();
        c.OnInstanceCreated += (instanceCreatedEvent) => {
            Assert.That(instanceCreatedEvent.Lifetime, Is.EqualTo(Lifetime.Transient));
            Assert.That(instanceCreatedEvent.Name, Is.Null);
            Assert.That(instanceCreatedEvent.GetMetadata("k"), Is.EqualTo("transient"));
            Assert.That(instanceCreatedEvent.GetMetadata<string>("k"), Is.EqualTo("transient"));
            Assert.That(instanceCreatedEvent.GetFlag("flag1"), Is.True);
            Assert.That(instanceCreatedEvent.GetFlag("flag2"), Is.False);
            Assert.That(instanceCreatedEvent.GetFlag("flag2", true), Is.True);
            calls++;
        };
        c.OnInstanceCreated += (instanceCreatedEvent) => {
            calls++;
        };
        c.Build(b => {
            b.Register(Provider.Transient<Node>(null, new Dictionary<string, object>() {{"flag1", true}, {"k", "transient"}}));
        });
        Assert.That(calls, Is.EqualTo(0));
        c.Resolve<Node>();
        c.Resolve<Node>();
        c.Resolve<Node>();
        Assert.That(calls, Is.EqualTo(6));
    }

    public class StaticPostInject : IInjectable {
        public static int Calls = 0;
        public void PostInject() {
            Calls++;
        }
    }

    [TestRunner.Test(Description = "Test PostInject static")]
    public void PostInjectStatic() {
        StaticPostInject.Calls = 0;
        var c = new Container();
        c.Build(b => {
            b.Register(Provider.Static(new StaticPostInject()));
        });
        Assert.That(StaticPostInject.Calls, Is.EqualTo(1));
    }
}
