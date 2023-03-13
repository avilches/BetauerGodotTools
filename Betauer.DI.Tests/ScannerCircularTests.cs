using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.DI.Tests; 

[TestFixture]
public class ScannerCircularTests : Node {
    [Service]
    public class Singleton {
        public static int Created = 0;
        public Singleton() => Created++;
        [Inject] public Singleton s { get; set; }
    }

    [Test(Description = "Singleton includes itself: OK")]
    public void SingletonItSelf() {
        var di = new Container.Builder();
        Singleton.Created = 0;
        di.Scan<Singleton>();
        var s = di.Build().Resolve<Singleton>();

        Assert.That(Singleton.Created, Is.EqualTo(1));
        Assert.That(s, Is.EqualTo(s.s));
    }

    [Service(Lifetime.Transient)]
    public class Transient {
        public static int Created = 0;
        public Transient() => Created++;
        [Inject] public Transient t { get; set; }
    }

    [Test(Description = "Transient includes itself: ERROR")]
    public void TransientItSelf() {
        var di = new Container.Builder();
        Transient.Created = 0;
        di.Scan<Transient>();
        var container = di.Build();
        Assert.Throws<CircularDependencyException>(() => container.Resolve<Transient>());

        Assert.That(Transient.Created, Is.EqualTo(1));
    }

    public interface ISingletonX {
        public ISingletonX s { get; }
    }

    public class SingletonX : ISingletonX {
        public static int Created = 0;

        public SingletonX() {
            Created++;
        }

        [Inject] public ISingletonX s { get; set; }
    }

    [Test(Description = "Singleton includes itself using interface as type: OK")]
    public void CircularSingletonTypes() {
        var di = new Container.Builder();
        SingletonX.Created = 0;

        di.Register(Provider.Singleton<ISingletonX, SingletonX>());
        var tx = di.Build().Resolve<ISingletonX>();

        Assert.That(SingletonX.Created, Is.EqualTo(1));
        Assert.That(tx, Is.EqualTo(tx.s));
    }

    public interface ITransientX {
        public ITransientX t { get; }
    }

    public class TransientX : ITransientX {
        public static int Created = 0;

        public TransientX() {
            Created++;
        }

        [Inject] public ITransientX t { get; set; }
    }

    [Test(Description = "Transient includes itself using interface as type: ERROR")]
    public void CircularTransientTypes() {
        var di = new Container.Builder();
        TransientX.Created = 0;

        di.Register(Provider.Transient<ITransientX, TransientX>());
        var c = di.Build();
        Assert.Throws<CircularDependencyException>(() => c.Resolve<ITransientX>());
        Assert.That(TransientX.Created, Is.EqualTo(1));
    }

    [Service]
    public class SingletonA {
        public static int Created = 0;
        public SingletonA() => Created++;
        [Inject] internal SingletonB sb { get; set; }
        [Inject] internal SingletonA sa { get; set; }
    }

    [Service]
    public class SingletonB {
        public static int Created = 0;
        public SingletonB() => Created++;
        [Inject] internal SingletonC sc { get; set; }
    }

    [Service]
    public class SingletonC {
        public static int Created = 0;
        public SingletonC() => Created++;
        [Inject] internal SingletonA sa { get; set; }
    }


    [Test(Description = "Circular dependency between singleton: OK")]
    public void CircularSingleton() {
        var di = new Container.Builder();
        SingletonA.Created = 0;
        SingletonB.Created = 0;
        SingletonC.Created = 0;
        di.Scan<SingletonA>();
        di.Scan<SingletonB>();
        di.Scan<SingletonC>();

        var sa = di.Build().Resolve<SingletonA>();
        Assert.That(SingletonA.Created, Is.EqualTo(1));
        Assert.That(SingletonB.Created, Is.EqualTo(1));
        Assert.That(SingletonC.Created, Is.EqualTo(1));

        var sb = di.Build().Resolve<SingletonB>();
        var sc = di.Build().Resolve<SingletonC>();

        Assert.That(sa, Is.Not.EqualTo(sb));
        Assert.That(sb, Is.Not.EqualTo(sc));
        Assert.That(sc, Is.Not.EqualTo(sa));

        Assert.That(sa, Is.EqualTo(sa.sa));
        Assert.That(sa.sb, Is.EqualTo(sb));
        Assert.That(sa.sb.sc, Is.EqualTo(sc));
        Assert.That(sa.sb.sc.sa, Is.EqualTo(sa));
    }

    [Service(Lifetime.Transient)]
    public class TransientA {
        public static int Created = 0;
        public TransientA() => Created++;
        [Inject] internal TransientB sb { get; set; }
    }

    [Service(Lifetime.Transient)]
    public class TransientB {
        public static int Created = 0;
        public TransientB() => Created++;
        [Inject] internal TransientC sc { get; set; }
    }

    [Service(Lifetime.Transient)]
    public class TransientC {
        public static int Created = 0;
        public TransientC() => Created++;
        [Inject] internal TransientA sa { get; set; }
    }


    [Test(Description = "Circular dependency between transients: ERROR")]
    public void CircularTransient() {
        var di = new Container.Builder();
        TransientA.Created = 0;
        TransientB.Created = 0;
        TransientC.Created = 0;
        di.Scan<TransientA>();
        di.Scan<TransientB>();
        di.Scan<TransientC>();

        var c = di.Build();
        Assert.Throws<CircularDependencyException>(() => c.Resolve<TransientA>());
        Assert.That(TransientA.Created, Is.EqualTo(1));
        Assert.That(TransientB.Created, Is.EqualTo(1));
        Assert.That(TransientC.Created, Is.EqualTo(1));
    }

    [Scan<ImportedService>()]
    [Configuration]
    internal class AddToScanByImport {
        [Service] private B B => new B();
    }

    [Scan<ImportSelf>()]
    [Configuration]
    internal class ImportSelf {
    }

    [Scan<AddToScanByImport>()]
    [Configuration]
    internal class ImportedService {
        [Service] private A A => new A();
    }

    internal class A { }
    internal class B { }

    [Test]
    public void MemberExposing() {
        var di = new Container.Builder();
        di.Scan<ImportSelf>();
        di.Scan<AddToScanByImport>();
        var c = di.Build();

        Assert.That(c.Resolve<A>(), Is.TypeOf<A>());
        Assert.That(c.Resolve<B>(), Is.TypeOf<B>());
            
    }

        
}