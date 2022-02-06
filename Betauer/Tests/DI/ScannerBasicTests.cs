using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.Tests.DI {
    [TestFixture]
    public class ScannerBasicTests : Node {
        public interface INotTagged {
        }

        [Singleton]
        public class MyServiceWithNotScanned {
            [Inject] internal INotTagged notFound;
        }

        [Singleton]
        public class MyServiceWithWithNullable {
            [Inject(Nullable = true)] internal INotTagged nullable;
        }

        [Test(Description = "Types not found")]
        public void NotFound() {
            var di = new Container(this);
            di.Scanner.Scan<INotTagged>();
            di.Scanner.Scan<MyServiceWithNotScanned>();
            Assert.That(!di.Contains<INotTagged>());
            Assert.That(di.Contains<MyServiceWithNotScanned>());
            try {
                di.Resolve<MyServiceWithNotScanned>();
                Assert.That(false, "It should fail!");
            } catch (InjectFieldException e) {
            }
        }

        [Test(Description = "Nullable")]
        public void Nullable() {
            var di = new Container(this);
            di.Scanner.Scan<MyServiceWithWithNullable>();
            Assert.That(!di.Contains<INotTagged>());
            var x = di.Resolve<MyServiceWithWithNullable>();
            Assert.That(x.nullable, Is.Null);
        }

        [Transient]
        public class EmptyTransient {
            public static int Created = 0;

            public EmptyTransient() {
                Created++;
            }
        }

        [Singleton]
        public class SingletonWith2Transients {
            public static int Created = 0;

            public SingletonWith2Transients() {
                Created++;
            }

            [Inject] internal EmptyTransient et1;
            [Inject] internal EmptyTransient et2;
        }

        [Singleton]
        public class MySingleton {
            public static int Created = 0;

            public MySingleton() {
                Created++;
            }

            [Inject] internal SingletonWith2Transients singleton1;
            [Inject] internal SingletonWith2Transients singleton2;
            [Inject] internal EmptyTransient et;
        }

        [Test(Description = "Inject singletons in singleton")]
        public void SingletonInSingleton() {
            var di = new Container(this);
            EmptyTransient.Created = 0;
            SingletonWith2Transients.Created = 0;
            MySingleton.Created = 0;

            di.Scanner.Scan<EmptyTransient>();
            di.Scanner.Scan<MySingleton>();
            di.Scanner.Scan<SingletonWith2Transients>();
            var s1 = di.Resolve<SingletonWith2Transients>();
            var s2 = di.Resolve<SingletonWith2Transients>();
            var ms1 = di.Resolve<MySingleton>();
            var ms2 = di.Resolve<MySingleton>();

            Assert.That(EmptyTransient.Created, Is.EqualTo(2));
            Assert.That(SingletonWith2Transients.Created, Is.EqualTo(1));
            Assert.That(MySingleton.Created, Is.EqualTo(1));

            // Singleton are all the same instance
            Assert.That(s1, Is.EqualTo(s2));
            Assert.That(ms1.singleton1, Is.EqualTo(s1));
            Assert.That(ms1.singleton2, Is.EqualTo(s1));

            Assert.That(ms2.singleton1, Is.EqualTo(s1));
            Assert.That(ms2.singleton2, Is.EqualTo(s1));

            // Transient are different between the transient in MyService and the transients in SingletonWith2Transients
            Assert.That(s1.et1, Is.EqualTo(s1.et2));
            Assert.That(s1.et1, Is.Not.EqualTo(ms1.et));
            Assert.That(s1.et2, Is.Not.EqualTo(ms1.et));
        }

        [Transient]
        public class TransientService {
            public static int Created = 0;

            public TransientService() {
                Created++;
            }

            [Inject] internal EmptyTransient et;
            [Inject] internal SingletonWith2Transients SingletonWith2Transients;
        }

        [Test(Description = "Inject transients in transient")]
        public void SingletonInTransient() {
            var di = new Container(this);
            EmptyTransient.Created = 0;
            SingletonWith2Transients.Created = 0;
            TransientService.Created = 0;
            EmptyTransient.Created = 0;

            di.Scanner.Scan<EmptyTransient>();
            di.Scanner.Scan<TransientService>();
            di.Scanner.Scan<SingletonWith2Transients>();
            di.Scanner.Scan<EmptyTransient>();
            var s1 = di.Resolve<SingletonWith2Transients>();

            Assert.That(EmptyTransient.Created, Is.EqualTo(1));
            Assert.That(s1.et1, Is.EqualTo(s1.et2));

            var ts1 = di.Resolve<TransientService>();
            Assert.That(TransientService.Created, Is.EqualTo(1));
            Assert.That(EmptyTransient.Created, Is.EqualTo(2));
            Assert.That(s1.et1, Is.Not.EqualTo(ts1.et));

            var ts2 = di.Resolve<TransientService>();
            Assert.That(TransientService.Created, Is.EqualTo(2));
            Assert.That(EmptyTransient.Created, Is.EqualTo(3));
            Assert.That(ts1.et, Is.Not.EqualTo(ts2.et));

            Assert.That(SingletonWith2Transients.Created, Is.EqualTo(1));
            Assert.That(ts1, Is.Not.EqualTo(ts2));
            Assert.That(ts1.SingletonWith2Transients, Is.EqualTo(s1));
            Assert.That(ts2.SingletonWith2Transients, Is.EqualTo(s1));
        }
    }
}