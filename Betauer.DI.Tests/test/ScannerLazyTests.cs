using System;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.DI.Tests;

[TestRunner.Test]
public class ScannerLazyTests {
    public class DummyClass {
    }


    [Singleton]
    public class NoLazyClass {
    }

    [Singleton(Lazy = true)]
    public class LazyClass {
    }

    [Configuration]
    [Scan<LazyClass>]
    [Scan<NoLazyClass>()]
    public class LazyConfiguration {
        [Singleton] private DummyClass noLazy => new DummyClass();
        [Singleton(Lazy = true)] private DummyClass lazy => new DummyClass();
    }

    [TestRunner.Test(Description = "Check Lazy attribute")]
    public void CheckLazyAttribute() {
        var di = new Container.Builder();
        di.Scan<LazyConfiguration>();
        var c = di.Build();
        Assert.That((c.GetProvider<NoLazyClass>() as ISingletonProvider)!.Lazy, Is.False);
        Assert.That((c.GetProvider<LazyClass>() as ISingletonProvider)!.Lazy, Is.True);

        Assert.That((c.GetProvider("noLazy") as ISingletonProvider)!.Lazy, Is.False);
        Assert.That((c.GetProvider("lazy") as ISingletonProvider)!.Lazy, Is.True);
    }


    [Singleton(Lazy = true)]
    class LazyPostInjectedA1 : IInjectable {
        [Inject] internal PostInjectedA2 A2 { get; set; }
        [Inject] internal Container container { get; set; }

        internal int Called = 0;

        public void PostInject() {
            Assert.That(A2, Is.Not.Null);
            Called++;
        }
    }

    [Singleton]
    class PostInjectedA2 : IInjectable {
        [Inject] internal Container container { get; set; }

        internal int Called = 0;

        public void PostInject() {
            Called++;
        }
    }

    [TestRunner.Test(Description = "Test if the [PostInject] methods are invoked + Lazy using a non lazy")]
    public void PostInjectMethodLazyWithNoLazyTest() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<LazyPostInjectedA1>();
        di.Scan<PostInjectedA2>();
        di.Build();

        Assert.That(c.GetProvider<LazyPostInjectedA1>() is ISingletonProvider { IsInstanceCreated: false });
        Assert.That(c.GetProvider<PostInjectedA2>() is ISingletonProvider { IsInstanceCreated: true });
        var A1 = c.Resolve<LazyPostInjectedA1>();
        Assert.That(A1.Called, Is.EqualTo(1));

        Assert.That(c.GetProvider<LazyPostInjectedA1>() is ISingletonProvider { IsInstanceCreated: true });

        var A2 = c.Resolve<PostInjectedA2>();
        Assert.That(A1.A2, Is.EqualTo(A2));
        Assert.That(A2.Called, Is.EqualTo(1));
    }

    [Singleton]
    class PostInjectedB1 {
        [Inject] internal LazyPostInjectedB2 B2 { get; set; }
    }

    [Singleton(Lazy = true)]
    class LazyPostInjectedB2 {
    }

    [TestRunner.Test(Description = "Using Lazy singleton as non Lazy fails during initialization")]
    public void PostInjectMethodTest() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<PostInjectedB1>();
        di.Scan<LazyPostInjectedB2>();
        var e = Assert.Throws<InvalidOperationException>(() => di.Build());

        Assert.That(e.Message, Contains.Substring("Container initialization failed. These Lazy Singletons are initialized when they shouldn't."));
    }

    [Singleton(Lazy = true)]
    class LazyPostInjectedC1 : IInjectable {
        [Inject] internal LazyPostInjectedC2 C2 { get; set; }
        [Inject] internal Container container { get; set; }

        internal int Called = 0;

        public void PostInject() {
            Assert.That(C2, Is.Not.Null);
            Assert.That(C2.C1, Is.Not.Null);
            Called++;
        }
    }

    [Singleton(Lazy = true)]
    class LazyPostInjectedC2 : IInjectable {
        [Inject] internal LazyPostInjectedC1 C1 { get; set; }
        [Inject] internal Container container { get; set; }

        internal int Called = 0;

        public void PostInject() {
            Assert.That(C1, Is.Not.Null);
            Assert.That(C1.C2, Is.Not.Null);
            Called++;
        }
    }

    [TestRunner.Test(Description = "Test if the [PostInject] methods are invoked + Lazy using Lazy")]
    public void PostInjectMethodLazyWithLazyTest() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<LazyPostInjectedC1>();
        di.Scan<LazyPostInjectedC2>();
        di.Build();

        Assert.That(c.GetProvider<LazyPostInjectedC1>() is ISingletonProvider { IsInstanceCreated: false });
        Assert.That(c.GetProvider<LazyPostInjectedC2>() is ISingletonProvider { IsInstanceCreated: false });

        var C1 = c.Resolve<LazyPostInjectedC1>();
        Assert.That(c.GetProvider<LazyPostInjectedC1>() is ISingletonProvider { IsInstanceCreated: true });
        Assert.That(c.GetProvider<LazyPostInjectedC2>() is ISingletonProvider { IsInstanceCreated: true });

        var C2 = c.Resolve<LazyPostInjectedC2>();
        Assert.That(C1.C2, Is.EqualTo(C2));
        Assert.That(C1.Called, Is.EqualTo(1));
        Assert.That(C2.C1, Is.EqualTo(C1));
        Assert.That(C2.Called, Is.EqualTo(1));
    }

    [Singleton(Lazy = true)]
    class LazyPostInjectedD1 {
        [Inject] internal ILazy<LazyPostInjectedD2> D2 { get; set; }
    }

    [Singleton(Lazy = true)]
    class LazyPostInjectedD2 {
        [Inject] internal ILazy<LazyPostInjectedD1> D1 { get; set; }
    }

    [TestRunner.Test(Description = "Test if the [PostInject] methods are invoked + Lazy using Lazy and Factory<T>")]
    public void PostInjectMethodLazyWithLazyTypedAsLazyTest() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<LazyPostInjectedD1>();
        di.Scan<LazyPostInjectedD2>();
        di.Build();

        Assert.That(c.GetProvider<LazyPostInjectedD1>() is ISingletonProvider { IsInstanceCreated: false });
        Assert.That(c.GetProvider<LazyPostInjectedD2>() is ISingletonProvider { IsInstanceCreated: false });

        var D1 = c.Resolve<LazyPostInjectedD1>();
        Assert.That(c.GetProvider<LazyPostInjectedD1>() is ISingletonProvider { IsInstanceCreated: true });
        Assert.That(c.GetProvider<LazyPostInjectedD2>() is ISingletonProvider { IsInstanceCreated: false });

        var D2 = D1.D2.Get();
        Assert.That(c.GetProvider<LazyPostInjectedD1>() is ISingletonProvider { IsInstanceCreated: true });
        Assert.That(c.GetProvider<LazyPostInjectedD2>() is ISingletonProvider { IsInstanceCreated: true });

        Assert.That(D2, Is.EqualTo(c.Resolve<LazyPostInjectedD2>()));
        Assert.That(D2.D1.Get(), Is.EqualTo(D1));
    }

    public class Singleton {
        public static int Instances = 0;

        public Singleton() {
            Instances++;
        }
    }

    public class SingletonFactory : IFactory<Singleton> {
        public Singleton Create() => new Singleton();
    }


    [Configuration]
    public class LazySingletonConfiguration {
        [Attributes.Factory.Singleton(Lazy = true)] public IFactory<Singleton> LazySingleton => new SingletonFactory();
    }

    [Singleton]
    public class UsingLazySingleton {
        [Inject] public ILazy<Singleton> LazySingleton { get; set; }
    }

    [TestRunner.Test(Description = "Simulate a lazy behaviour with a Factory")]
    public void LazySingletonFromConfigurationTest() {
        Singleton.Instances = 0;
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<LazySingletonConfiguration>();
        di.Scan<UsingLazySingleton>();
        di.Build();

        UsingLazySingleton usingLazy = c.Resolve<UsingLazySingleton>();

        Assert.That(Singleton.Instances, Is.EqualTo(0));
        Assert.That(c.GetProvider("LazySingleton") is ISingletonProvider { IsInstanceCreated: false });

        usingLazy.LazySingleton.Get();
        Assert.That(Singleton.Instances, Is.EqualTo(1));
        Assert.That(c.GetProvider("LazySingleton") is ISingletonProvider { IsInstanceCreated: true });
    }

    [Configuration]
    public class SingletonConfiguration {
        [Attributes.Factory.Singleton(Lazy = false)] public IFactory<Singleton> Singleton => new SingletonFactory();
    }
    
    [Singleton]
    public class UsingSingleton {
        [Inject] public ILazy<Singleton> Singleton { get; set; }
    }

    [TestRunner.Test(Description = "Simulate a no lazy behaviour with a Factory")]
    public void SingletonFromConfigurationTest() {
        Singleton.Instances = 0;
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<SingletonConfiguration>();
        di.Scan<UsingSingleton>();
        di.Build();

        UsingSingleton usingSingleton = c.Resolve<UsingSingleton>();

        Assert.That(Singleton.Instances, Is.EqualTo(1));
        Assert.That(c.GetProvider("Singleton") is ISingletonProvider { IsInstanceCreated: true });

        usingSingleton.Singleton.Get();
        Assert.That(Singleton.Instances, Is.EqualTo(1));
        Assert.That(c.GetProvider("Singleton") is ISingletonProvider { IsInstanceCreated: true });
    }
}