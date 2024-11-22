using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.DI.Tests; 

[TestFixture]
public class ScannerCircularTests : Node {
    [Singleton]
    public class Singleton {
        public static int Created = 0;
        public Singleton() => Created++;
        [Inject] public Singleton s { get; set; }
    }

    [Test(Description = "Singleton includes itself: OK")]
    public void SingletonItSelf() {
        var c = new Container();
        c.Build(di => {
            Singleton.Created = 0;
            di.Scan<Singleton>();
        });
        var s = c.Resolve<Singleton>();

        Assert.That(Singleton.Created, Is.EqualTo(1));
        Assert.That(s, Is.EqualTo(s.s));
    }

    [Transient]
    public class Transient {
        public static int Created = 0;
        public Transient() => Created++;
        [Inject] public Transient t { get; set; }
    }

    [Test(Description = "Transient includes itself: ERROR")]
    public void TransientItSelf() {
        var c = new Container();
        c.Build(di => {
            Transient.Created = 0;
            di.Scan<Transient>();
        });
        Assert.Throws<CircularDependencyException>(() => c.Resolve<Transient>());

        Assert.That(Transient.Created, Is.EqualTo(2));
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
        var c = new Container();
        c.Build(di => {
            SingletonX.Created = 0;

            di.Register(Provider.Singleton<ISingletonX, SingletonX>());
        });
        var tx = c.Resolve<ISingletonX>();

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
        var c = new Container();
        c.Build(di => {
            TransientX.Created = 0;

            di.Register(Provider.Transient<ITransientX, TransientX>());
        });
        Assert.Throws<CircularDependencyException>(() => c.Resolve<ITransientX>());
        Assert.That(TransientX.Created, Is.EqualTo(2));
    }

    [Singleton]
    public class SingletonA {
        public static int Created = 0;
        public SingletonA() => Created++;
        [Inject] internal SingletonB sb { get; set; }
        [Inject] internal SingletonA sa { get; set; }
    }

    [Singleton]
    public class SingletonB {
        public static int Created = 0;
        public SingletonB() => Created++;
        [Inject] internal SingletonC sc { get; set; }
    }

    [Singleton]
    public class SingletonC {
        public static int Created = 0;
        public SingletonC() => Created++;
        [Inject] internal SingletonA sa { get; set; }
    }


    [Test(Description = "Circular dependency between singleton: OK")]
    public void CircularSingleton() {
        var c = new Container();
        c.Build(di => {
            SingletonA.Created = 0;
            SingletonB.Created = 0;
            SingletonC.Created = 0;
            di.Scan<SingletonA>();
            di.Scan<SingletonB>();
            di.Scan<SingletonC>();
        });

        var sa = c.Resolve<SingletonA>();
        Assert.That(SingletonA.Created, Is.EqualTo(1));
        Assert.That(SingletonB.Created, Is.EqualTo(1));
        Assert.That(SingletonC.Created, Is.EqualTo(1));

        var sb = c.Resolve<SingletonB>();
        var sc = c.Resolve<SingletonC>();

        Assert.That(sa, Is.Not.EqualTo(sb));
        Assert.That(sb, Is.Not.EqualTo(sc));
        Assert.That(sc, Is.Not.EqualTo(sa));

        Assert.That(sa, Is.EqualTo(sa.sa));
        Assert.That(sa.sb, Is.EqualTo(sb));
        Assert.That(sa.sb.sc, Is.EqualTo(sc));
        Assert.That(sa.sb.sc.sa, Is.EqualTo(sa));
    }

    [Transient]
    public class TransientA {
        public static int Created = 0;
        public TransientA() => Created++;
        [Inject] internal TransientB sb { get; set; }
    }

    [Transient]
    public class TransientB {
        public static int Created = 0;
        public TransientB() => Created++;
        [Inject] internal TransientC sc { get; set; }
    }

    [Transient]
    public class TransientC {
        public static int Created = 0;
        public TransientC() => Created++;
        [Inject] internal TransientA sa { get; set; }
    }


    [Test(Description = "Circular dependency between transients: ERROR")]
    public void CircularTransient() {
        var c = new Container();
        c.Build(di => {
            TransientA.Created = 0;
            TransientB.Created = 0;
            TransientC.Created = 0;
            di.Scan<TransientA>();
            di.Scan<TransientB>();
            di.Scan<TransientC>();
        });

        Assert.Throws<CircularDependencyException>(() => c.Resolve<TransientA>());
        Assert.That(TransientA.Created, Is.EqualTo(2));
        Assert.That(TransientB.Created, Is.EqualTo(1));
        Assert.That(TransientC.Created, Is.EqualTo(1));
    }

    [Scan<ImportedService>()]
    [Configuration]
    internal class AddToScanByImport {
        [Singleton] private B B => new B();
    }

    [Scan<ImportSelf>()]
    [Configuration]
    internal class ImportSelf {
    }

    [Scan<AddToScanByImport>()]
    [Configuration]
    internal class ImportedService {
        [Singleton] private A A => new A();
    }

    internal class A { }
    internal class B { }

    [Test]
    public void MemberExposing() {
        var c = new Container();
        c.Build(di => {
            di.Scan<ImportSelf>();
            di.Scan<AddToScanByImport>();
        });

        Assert.That(c.Resolve<A>("A"), Is.TypeOf<A>());
        Assert.That(c.Resolve<B>("B"), Is.TypeOf<B>());
            
    }

        
}