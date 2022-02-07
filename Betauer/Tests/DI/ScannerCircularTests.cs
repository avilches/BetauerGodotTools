using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.Tests.DI {
    [TestFixture]
    public class ScannerCircularTests : Node {
        [Singleton]
        public class SingletonA {
            public static int Created = 0;

            public SingletonA() {
                Created++;
            }
            [Inject] internal SingletonB sb;
        }
        [Singleton]
        public class SingletonB {
            public static int Created = 0;

            public SingletonB() {
                Created++;
            }
            [Inject] internal SingletonC sc;
        }

        [Singleton]
        public class SingletonC {
            public static int Created = 0;

            public SingletonC() {
                Created++;
            }
            [Inject] internal SingletonA sa;
        }


        [Test(Description = "Circular dependency")]
        public void CircularSingleton() {
            var di = new ContainerBuilder(this);
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

            Assert.That(sa.sb, Is.EqualTo(sb));
            Assert.That(sa.sb.sc, Is.EqualTo(sc));
            Assert.That(sa.sb.sc.sa, Is.EqualTo(sa));

        }

        [Transient]
        public class TransientA {
            public static int Created = 0;

            public TransientA() {
                Created++;
            }
            [Inject] internal TransientB sb;
        }

        [Transient]
        public class TransientB {
            public static int Created = 0;

            public TransientB() {
                Created++;
            }
            [Inject] internal TransientC sc;
        }

        [Transient]
        public class TransientC {
            public static int Created = 0;

            public TransientC() {
                Created++;
            }
            [Inject] internal TransientA sa;
        }


        [Test(Description = "Transient dependency")]
        public void CircularTransient() {
            var di = new ContainerBuilder(this);
            TransientA.Created = 0;
            TransientB.Created = 0;
            TransientC.Created = 0;
            di.Scan<TransientA>();
            di.Scan<TransientB>();
            di.Scan<TransientC>();

            var sa = di.Build().Resolve<TransientA>();
            Assert.That(TransientA.Created, Is.EqualTo(1));
            Assert.That(TransientB.Created, Is.EqualTo(1));
            Assert.That(TransientC.Created, Is.EqualTo(1));

            var sb = di.Build().Resolve<TransientB>();
            Assert.That(TransientA.Created, Is.EqualTo(2));
            Assert.That(TransientB.Created, Is.EqualTo(2));
            Assert.That(TransientC.Created, Is.EqualTo(2));

            Assert.That(sa, Is.Not.EqualTo(sb.sc.sa));
            Assert.That(sa.sb, Is.Not.EqualTo(sb));
            Assert.That(sa.sb.sc, Is.Not.EqualTo(sb.sc));

        }

        [Transient]
        public class MultiTransientA {
            public static int Created = 0;

            public MultiTransientA() {
                Created++;
            }
            [Inject] internal MultiTransientA sa;
            [Inject] internal MultiTransientB sb;
            [Inject] internal MultiTransientC sc;
        }

        [Transient]
        public class MultiTransientB {
            public static int Created = 0;

            public MultiTransientB() {
                Created++;
            }
            [Inject] internal MultiTransientA sa;
            [Inject] internal MultiTransientB sb;
            [Inject] internal MultiTransientC sc;
        }

        [Transient]
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
            var di = new ContainerBuilder(this);
            MultiTransientA.Created = 0;
            MultiTransientB.Created = 0;
            MultiTransientC.Created = 0;
            di.Scan<MultiTransientA>();
            di.Scan<MultiTransientB>();
            di.Scan<MultiTransientC>();

            var sa = di.Build().Resolve<MultiTransientA>();
            Assert.That(MultiTransientA.Created, Is.EqualTo(1));
            Assert.That(MultiTransientB.Created, Is.EqualTo(1));
            Assert.That(MultiTransientC.Created, Is.EqualTo(1));

            Assert.That(sa, Is.EqualTo(sa.sb.sa));
            Assert.That(sa, Is.EqualTo(sa.sb.sa));
            Assert.That(sa, Is.EqualTo(sa.sb.sc.sa));
        }
    }
}