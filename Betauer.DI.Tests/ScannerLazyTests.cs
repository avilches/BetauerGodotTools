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
    class PostInjectedA2 : IInjectable{
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
    class PostInjectedB1 : IInjectable {
        [Inject] internal LazyPostInjectedB2 B2 { get; set; }
        [Inject] internal Container container { get; set; }

        internal int Called = 0;

        public void PostInject() {
            Assert.That(B2, Is.Not.Null);
            Assert.That(B2.B1, Is.Not.Null);
            Called++;
        }
    }

    [Singleton(Lazy = true)]
    class LazyPostInjectedB2 : IInjectable {
        [Inject] internal PostInjectedB1 B1 { get; set; }
        [Inject] internal Container container { get; set; }

        internal int Called = 0;

        public void PostInject() {
            Assert.That(B1, Is.Not.Null);
            Assert.That(B1.B2, Is.Not.Null);
            Called++;
        }
    }

    [TestRunner.Test(Description = "Test if the [PostInject] methods are invoked + Non Lazy using Lazy")]
    public void PostInjectMethodTest() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<PostInjectedB1>();
        di.Scan<LazyPostInjectedB2>();
        di.Build();

        Assert.That(c.GetProvider<PostInjectedB1>() is ISingletonProvider { IsInstanceCreated: true });
        Assert.That(c.GetProvider<LazyPostInjectedB2>() is ISingletonProvider { IsInstanceCreated: true });

        var B1 = c.Resolve<PostInjectedB1>();
        var B2 = c.Resolve<LazyPostInjectedB2>();

        Assert.That(B1.B2, Is.EqualTo(B2));
        Assert.That(B1.Called, Is.EqualTo(1));
        Assert.That(B2.B1, Is.EqualTo(B1));
        Assert.That(B2.Called, Is.EqualTo(1));
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
        [Inject] internal IFactory<LazyPostInjectedD2> D2 { get; set; }
    }

    [Singleton(Lazy = true)]
    class LazyPostInjectedD2 {
        [Inject] internal IFactory<LazyPostInjectedD1> D1 { get; set; }
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
    
    public class LazySingleton {
        public static int Instances = 0;

        public LazySingleton() {
            Instances++;
        }
    }

    [Configuration]
    public class LazySingletonConfiguration {
        [Attributes.Factory.Singleton] public Factory<LazySingleton> LazySingleton =>  FactoryTools.Create(() => new LazySingleton());
    }

    [Singleton]
    public class AnotherSingleton {
        [Inject] public IFactory<LazySingleton> LazySingleton { get; set; }
    }

    [TestRunner.Test(Description = "Simulate a lazy behaviour with with a Factory")]
    public void LazySingletonFromConfiguration() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<LazySingletonConfiguration>();
        di.Scan<AnotherSingleton>();
        di.Build();

        AnotherSingleton another = c.Resolve<AnotherSingleton>();

        Assert.That(LazySingleton.Instances, Is.EqualTo(0));
        Assert.That(c.GetProvider<LazySingleton>() is ISingletonProvider { IsInstanceCreated: false });

        another.LazySingleton.Get();
        Assert.That(LazySingleton.Instances, Is.EqualTo(1));
        Assert.That(c.GetProvider<LazySingleton>() is ISingletonProvider { IsInstanceCreated: true });
    }


    
}