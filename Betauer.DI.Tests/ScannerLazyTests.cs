using System;
using Betauer.DI.ServiceProvider;
using NUnit.Framework;

namespace Betauer.DI.Tests;

[TestFixture]
public class ScannerLazyTests {
    public class DummyClass {
    }


    [Service]
    public class NoLazyClass {
    }

    [Service(Lazy = true)]
    public class LazyClass {
    }

    [Service]
    [Lazy]
    public class LazyTagClass {
    }

    [Configuration]
    [Scan<LazyClass>]
    [Scan<LazyTagClass>()]
    [Scan<NoLazyClass>()]
    public class LazyConfiguration {
        [Service] private DummyClass noLazy => new DummyClass();
        [Service] [Lazy] private DummyClass lazyTag => new DummyClass();
        [Service(Lazy = true)] private DummyClass lazy => new DummyClass();
    }

    [Test(Description = "Check Lazy attribute")]
    public void CheckLazyAttribute() {
        var di = new Container.Builder();
        di.Scan<LazyConfiguration>();
        var c = di.Build();
        Assert.That((c.GetProvider<NoLazyClass>() as ISingletonProvider)!.Lazy, Is.False);
        Assert.That((c.GetProvider<LazyClass>() as ISingletonProvider)!.Lazy, Is.True);
        Assert.That((c.GetProvider<LazyTagClass>() as ISingletonProvider)!.Lazy, Is.True);

        Assert.That((c.GetProvider("noLazy") as ISingletonProvider)!.Lazy, Is.False);
        Assert.That((c.GetProvider("lazyTag") as ISingletonProvider)!.Lazy, Is.True);
        Assert.That((c.GetProvider("lazy") as ISingletonProvider)!.Lazy, Is.True);
    }


    [Service]
    [Lazy]
    class LazyPostInjectdA1 {
        [Inject] internal PostInjectdA2 A2 { get; set; }
        [Inject] internal Container container { get; set; }

        internal int Called = 0;

        [PostInject]
        void PostInjectMethod() {
            Assert.That(A2, Is.Not.Null);
            Called++;
        }
    }

    [Service]
    class PostInjectdA2 {
        [Inject] internal Container container { get; set; }

        internal int Called = 0;

        [PostInject]
        void PostInjectMethod() {
            Called++;
        }

        internal bool more1 = false;
        internal bool more2 = false;

        [PostInject]
        void More1() => more1 = true;

        [PostInject]
        void More2() => more2 = true;
    }

    [Test(Description = "Test if the [PostInject] methods are invoked + Lazy using a non lazy")]
    public void PostInjectMethodLazyWithNoLazyTest() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<LazyPostInjectdA1>();
        di.Scan<PostInjectdA2>();
        di.Build();

        Assert.That(c.GetProvider<LazyPostInjectdA1>() is ISingletonProvider { IsInstanceCreated: false });
        Assert.That(c.GetProvider<PostInjectdA2>() is ISingletonProvider { IsInstanceCreated: true });
        var A1 = c.Resolve<LazyPostInjectdA1>();
        Assert.That(A1.Called, Is.EqualTo(1));

        Assert.That(c.GetProvider<LazyPostInjectdA1>() is ISingletonProvider { IsInstanceCreated: true });

        var A2 = c.Resolve<PostInjectdA2>();
        Assert.That(A1.A2, Is.EqualTo(A2));
        Assert.That(A2.Called, Is.EqualTo(1));
        Assert.That(A2.more1, Is.True);
        Assert.That(A2.more2, Is.True);
    }

    [Service]
    class PostInjectdB1 {
        [Inject] internal LazyPostInjectdB2 B2 { get; set; }
        [Inject] internal Container container { get; set; }

        internal int Called = 0;

        [PostInject]
        void PostInjectMethod() {
            Assert.That(B2, Is.Not.Null);
            Assert.That(B2.B1, Is.Not.Null);
            Called++;
        }
    }

    [Service]
    [Lazy]
    class LazyPostInjectdB2 {
        [Inject] internal PostInjectdB1 B1 { get; set; }
        [Inject] internal Container container { get; set; }

        internal int Called = 0;

        [PostInject]
        void PostInjectMethod() {
            Assert.That(B1, Is.Not.Null);
            Assert.That(B1.B2, Is.Not.Null);
            Called++;
        }

        internal bool more1 = false;
        internal bool more2 = false;

        [PostInject]
        void More1() => more1 = true;

        [PostInject]
        void More2() => more2 = true;
    }

    [Test(Description = "Test if the [PostInject] methods are invoked + Non Lazy using Lazy")]
    public void PostInjectMethodTest() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<PostInjectdB1>();
        di.Scan<LazyPostInjectdB2>();
        di.Build();

        Assert.That(c.GetProvider<PostInjectdB1>() is ISingletonProvider { IsInstanceCreated: true });
        Assert.That(c.GetProvider<LazyPostInjectdB2>() is ISingletonProvider { IsInstanceCreated: true });

        var B1 = c.Resolve<PostInjectdB1>();
        var B2 = c.Resolve<LazyPostInjectdB2>();

        Assert.That(B1.B2, Is.EqualTo(B2));
        Assert.That(B1.Called, Is.EqualTo(1));
        Assert.That(B2.B1, Is.EqualTo(B1));
        Assert.That(B2.Called, Is.EqualTo(1));

        Assert.That(B2.more1, Is.True);
        Assert.That(B2.more2, Is.True);
    }

    [Service]
    [Lazy]
    class LazyPostInjectdC1 {
        [Inject] internal LazyPostInjectdC2 C2 { get; set; }
        [Inject] internal Container container { get; set; }

        internal int Called = 0;

        [PostInject]
        void PostInjectMethod() {
            Assert.That(C2, Is.Not.Null);
            Assert.That(C2.C1, Is.Not.Null);
            Called++;
        }
    }

    [Service]
    [Lazy]
    class LazyPostInjectdC2 {
        [Inject] internal LazyPostInjectdC1 C1 { get; set; }
        [Inject] internal Container container { get; set; }

        internal int Called = 0;

        [PostInject]
        void PostInjectMethod() {
            Assert.That(C1, Is.Not.Null);
            Assert.That(C1.C2, Is.Not.Null);
            Called++;
        }

        internal bool more1 = false;
        internal bool more2 = false;

        [PostInject]
        void More1() => more1 = true;

        [PostInject]
        void More2() => more2 = true;
    }

    [Test(Description = "Test if the [PostInject] methods are invoked + Lazy using Lazy")]
    public void PostInjectMethodLazyWithLazyTest() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<LazyPostInjectdC1>();
        di.Scan<LazyPostInjectdC2>();
        di.Build();

        Assert.That(c.GetProvider<LazyPostInjectdC1>() is ISingletonProvider { IsInstanceCreated: false });
        Assert.That(c.GetProvider<LazyPostInjectdC2>() is ISingletonProvider { IsInstanceCreated: false });

        var C1 = c.Resolve<LazyPostInjectdC1>();
        Assert.That(c.GetProvider<LazyPostInjectdC1>() is ISingletonProvider { IsInstanceCreated: true });
        Assert.That(c.GetProvider<LazyPostInjectdC2>() is ISingletonProvider { IsInstanceCreated: true });

        var C2 = c.Resolve<LazyPostInjectdC2>();
        Assert.That(C1.C2, Is.EqualTo(C2));
        Assert.That(C1.Called, Is.EqualTo(1));
        Assert.That(C2.C1, Is.EqualTo(C1));
        Assert.That(C2.Called, Is.EqualTo(1));

        Assert.That(C2.more1, Is.True);
        Assert.That(C2.more2, Is.True);
    }

    public class LazySingleton {
        public static int Calls = 0;

        public LazySingleton() {
            Calls++;
        }
    }

    [Configuration]
    public class LazySingletonConfiguration {
        [Service] [Lazy] public LazySingleton LazySingleton => new();
    }

    [Service]
    public class AnotherSingleton {
        [Inject] public IFactory<LazySingleton> LazySingleton { get; set; }
    }


    [Test(Description = "Test defining a Lazy service by name with a Factory")]
    public void LazySingletonFromConfiguration() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<LazySingletonConfiguration>();
        di.Scan<AnotherSingleton>();
        di.Build();

        AnotherSingleton another = c.Resolve<AnotherSingleton>();

        Assert.That(LazySingleton.Calls, Is.EqualTo(0));
        Assert.That(c.GetProvider<LazySingleton>() is ISingletonProvider { IsInstanceCreated: false });

        another.LazySingleton.Get();
        Assert.That(LazySingleton.Calls, Is.EqualTo(1));
        Assert.That(c.GetProvider<LazySingleton>() is ISingletonProvider { IsInstanceCreated: true });
    }

    [Service]
    [Lazy]
    class LazyPostInjectdD1 {
        [Inject] internal IFactory<LazyPostInjectdD2> D2 { get; set; }
    }

    [Service]
    [Lazy]
    class LazyPostInjectdD2 {
        [Inject] internal IFactory<LazyPostInjectdD1> D1 { get; set; }
    }

    [Test(Description = "Test if the [PostInject] methods are invoked + Lazy using Lazy and Factory<T>")]
    public void PostInjectMethodLazyWithLazyTypedAsLazyTest() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<LazyPostInjectdD1>();
        di.Scan<LazyPostInjectdD2>();
        di.Build();

        Assert.That(c.GetProvider<LazyPostInjectdD1>() is ISingletonProvider { IsInstanceCreated: false });
        Assert.That(c.GetProvider<LazyPostInjectdD2>() is ISingletonProvider { IsInstanceCreated: false });

        var D1 = c.Resolve<LazyPostInjectdD1>();
        Assert.That(c.GetProvider<LazyPostInjectdD1>() is ISingletonProvider { IsInstanceCreated: true });
        Assert.That(c.GetProvider<LazyPostInjectdD2>() is ISingletonProvider { IsInstanceCreated: false });

        var D2 = D1.D2.Get();
        Assert.That(c.GetProvider<LazyPostInjectdD1>() is ISingletonProvider { IsInstanceCreated: true });
        Assert.That(c.GetProvider<LazyPostInjectdD2>() is ISingletonProvider { IsInstanceCreated: true });

        Assert.That(D2, Is.EqualTo(c.Resolve<LazyPostInjectdD2>()));
        Assert.That(D2.D1.Get(), Is.EqualTo(D1));
    }
}