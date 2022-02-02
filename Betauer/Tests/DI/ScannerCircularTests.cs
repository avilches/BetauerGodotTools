using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.Tests.DI {
    [TestFixture]
    public class ScannerCircularTests : Node {
        [Service]
        public class SingletonA {
            public static int Created = 0;

            public SingletonA() {
                Created++;
            }
            [Inject] internal SingletonB sb;
        }
        [Service]
        public class SingletonB {
            public static int Created = 0;

            public SingletonB() {
                Created++;
            }
            [Inject] internal SingletonC sc;
        }

        [Service]
        public class SingletonC {
            public static int Created = 0;

            public SingletonC() {
                Created++;
            }
            [Inject] internal SingletonA sa;
        }


        [Test(Description = "Circular dependency")]
        public void CircularSingleton() {
            var di = new Container(this);
            SingletonA.Created = 0;
            SingletonB.Created = 0;
            SingletonC.Created = 0;
            di.Scanner.Scan<SingletonA>();
            di.Scanner.Scan<SingletonB>();
            di.Scanner.Scan<SingletonC>();

            var sa = di.Resolve<SingletonA>();
            Assert.That(SingletonA.Created, Is.EqualTo(1));
            Assert.That(SingletonB.Created, Is.EqualTo(1));
            Assert.That(SingletonC.Created, Is.EqualTo(1));

            var sb = di.Resolve<SingletonB>();
            var sc = di.Resolve<SingletonC>();

            Assert.That(sa, Is.Not.EqualTo(sb));
            Assert.That(sb, Is.Not.EqualTo(sc));
            Assert.That(sc, Is.Not.EqualTo(sa));

            Assert.That(sa.sb, Is.EqualTo(sb));
            Assert.That(sa.sb.sc, Is.EqualTo(sc));
            Assert.That(sa.sb.sc.sa, Is.EqualTo(sa));

        }

        [Service(Scope = Scope.Prototype)]
        public class TransientA {
            public static int Created = 0;

            public TransientA() {
                Created++;
            }
            [Inject] internal TransientB sb;
        }

        [Service(Scope = Scope.Prototype)]
        public class TransientB {
            public static int Created = 0;

            public TransientB() {
                Created++;
            }
            [Inject] internal TransientC sc;
        }

        [Service(Scope = Scope.Prototype)]
        public class TransientC {
            public static int Created = 0;

            public TransientC() {
                Created++;
            }
            [Inject] internal TransientA sa;
        }


        [Test(Description = "Transient dependency")]
        public void CircularTransient() {
            var di = new Container(this);
            TransientA.Created = 0;
            TransientB.Created = 0;
            TransientC.Created = 0;
            di.Scanner.Scan<TransientA>();
            di.Scanner.Scan<TransientB>();
            di.Scanner.Scan<TransientC>();

            var sa = di.Resolve<TransientA>();
            Assert.That(TransientA.Created, Is.EqualTo(1));
            Assert.That(TransientB.Created, Is.EqualTo(1));
            Assert.That(TransientC.Created, Is.EqualTo(1));

            var sb = di.Resolve<TransientB>();
            Assert.That(TransientA.Created, Is.EqualTo(2));
            Assert.That(TransientB.Created, Is.EqualTo(2));
            Assert.That(TransientC.Created, Is.EqualTo(2));

            Assert.That(sa, Is.Not.EqualTo(sb.sc.sa));
            Assert.That(sa.sb, Is.Not.EqualTo(sb));
            Assert.That(sa.sb.sc, Is.Not.EqualTo(sb.sc));

        }

        [Service(Scope = Scope.Prototype)]
        public class MultiTransientA {
            public static int Created = 0;

            public MultiTransientA() {
                Created++;
            }
            [Inject] internal MultiTransientA sa;
            [Inject] internal MultiTransientB sb;
            [Inject] internal MultiTransientC sc;
        }

        [Service(Scope = Scope.Prototype)]
        public class MultiTransientB {
            public static int Created = 0;

            public MultiTransientB() {
                Created++;
            }
            [Inject] internal MultiTransientA sa;
            [Inject] internal MultiTransientB sb;
            [Inject] internal MultiTransientC sc;
        }

        [Service(Scope = Scope.Prototype)]
        public class MultiTransientC {
            public static int Created = 0;

            public MultiTransientC() {
                Created++;
            }
            [Inject] internal MultiTransientA sa;
            [Inject] internal MultiTransientB sb;
            [Inject] internal MultiTransientC sc;
        }


        [Test(Description = "Multiple Transient dependency is not allowed")]
        public void CircularMultipleTransient() {
            var di = new Container(this);
            MultiTransientA.Created = 0;
            MultiTransientB.Created = 0;
            MultiTransientC.Created = 0;
            di.Scanner.Scan<MultiTransientA>();
            di.Scanner.Scan<MultiTransientB>();
            di.Scanner.Scan<MultiTransientC>();

            var sa = di.Resolve<MultiTransientA>();
            Assert.That(MultiTransientA.Created, Is.EqualTo(1));
            Assert.That(MultiTransientB.Created, Is.EqualTo(1));
            Assert.That(MultiTransientC.Created, Is.EqualTo(1));

            Assert.That(sa, Is.EqualTo(sa.sb.sa));
            Assert.That(sa, Is.EqualTo(sa.sb.sa));
            Assert.That(sa, Is.EqualTo(sa.sb.sc.sa));
        }
    }
}