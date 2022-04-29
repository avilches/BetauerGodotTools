using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests.DI {
    [TestFixture]
    [Only]
    public class ScannerBasicTests : Node {
        [SetUp]
        public void Setup() {
            LoggerFactory.OverrideTraceLevel(TraceLevel.All);
        }

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
            var di = new ContainerBuilder(this);
            di.Scan<INotTagged>();
            di.Scan<MyServiceWithNotScanned>();
            try {
                di.Build();
                Assert.That(false, "It should fail!");
            } catch (InjectFieldException e) {
            }
        }

        [Test(Description = "Nullable")]
        public void Nullable() {
            var di = new ContainerBuilder(this);
            di.Scan<MyServiceWithWithNullable>();
            var c = di.Build();
            Assert.That(!c.Contains<INotTagged>());
            var x = c.Resolve<MyServiceWithWithNullable>();
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

            [Inject] internal EmptyTransient et1 { get; set; }
            [Inject] internal EmptyTransient et2 { get; set; }
        }

        [Singleton]
        public class MySingleton {
            public static int Created = 0;

            public MySingleton() {
                Created++;
            }

            [Inject] internal SingletonWith2Transients singleton1 { get; set; }
            [Inject] internal SingletonWith2Transients singleton2 { get; set; }
            [Inject] internal EmptyTransient et { get; set; }
        }

        [Test(Description = "Inject singletons in singleton + get/set properties")]
        public void SingletonInSingleton() {
            var di = new ContainerBuilder(this);
            EmptyTransient.Created = 0;
            SingletonWith2Transients.Created = 0;
            MySingleton.Created = 0;

            di.Scan<EmptyTransient>();
            // Order matters: MySingleton contains SingletonWith2Transients, so register SingletonWith2Transients first
            // will create 1 EmptyTransient. Then resolve the MySingleton will create a new context, so it will create
            // another EmptyTransient, 2 in total.
            // So, registering MySingleton first will create only one EmptyTransient, because it contains
            // SingletonWith2Transients
            di.Scan<SingletonWith2Transients>();
            di.Scan<MySingleton>();
            var c = di.Build();

            Assert.That(EmptyTransient.Created, Is.EqualTo(2));
            Assert.That(SingletonWith2Transients.Created, Is.EqualTo(1));
            Assert.That(MySingleton.Created, Is.EqualTo(1));

            var s1 = c.Resolve<SingletonWith2Transients>();
            var s2 = c.Resolve<SingletonWith2Transients>();
            var ms1 = c.Resolve<MySingleton>();
            var ms2 = c.Resolve<MySingleton>();

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
            var di = new ContainerBuilder(this);
            EmptyTransient.Created = 0;
            SingletonWith2Transients.Created = 0;
            TransientService.Created = 0;
            EmptyTransient.Created = 0;

            di.Scan<EmptyTransient>();
            di.Scan<TransientService>();
            di.Scan<SingletonWith2Transients>();
            di.Scan<EmptyTransient>();
            var c = di.Build();
            var s1 = c.Resolve<SingletonWith2Transients>();

            Assert.That(EmptyTransient.Created, Is.EqualTo(1));
            Assert.That(s1.et1, Is.EqualTo(s1.et2));

            var ts1 = c.Resolve<TransientService>();
            Assert.That(TransientService.Created, Is.EqualTo(1));
            Assert.That(EmptyTransient.Created, Is.EqualTo(2));
            Assert.That(s1.et1, Is.Not.EqualTo(ts1.et));

            var ts2 = c.Resolve<TransientService>();
            Assert.That(TransientService.Created, Is.EqualTo(2));
            Assert.That(EmptyTransient.Created, Is.EqualTo(3));
            Assert.That(ts1.et, Is.Not.EqualTo(ts2.et));

            Assert.That(SingletonWith2Transients.Created, Is.EqualTo(1));
            Assert.That(ts1, Is.Not.EqualTo(ts2));
            Assert.That(ts1.SingletonWith2Transients, Is.EqualTo(s1));
            Assert.That(ts2.SingletonWith2Transients, Is.EqualTo(s1));
        }
        
        public interface IMultipleImp {
            
        }

        [Singleton(Name = "M1")]
        public class MultipleImpl1 : IMultipleImp {}

        [Singleton(Name = "M2")]
        public class MultipleImpl2 : IMultipleImp {}

        [Transient(Name = "M3")]
        public class MultipleImpl3 : IMultipleImp {
            public static int Created = 0;

            public MultipleImpl3() {
                Created++;
            }
            
        }

        [Singleton]
        public class ServiceWithMultipleImpl1 {
            [Inject(Name = "M1")] internal IMultipleImp mul11;
            [Inject(Name = "M1")] internal IMultipleImp mul12;
            
            [Inject(Name = "M2")] internal IMultipleImp mul21;
            [Inject(Name = "M2")] internal IMultipleImp mul22;
            
            [Inject(Name = "M3")] internal IMultipleImp mul31;
            [Inject(Name = "M3")] internal IMultipleImp mul32;
        }

        [Singleton]
        public class ServiceWithMultipleImpl2 {
            [Inject(Name = "M1")] internal IMultipleImp mul11;
            [Inject(Name = "M1")] internal IMultipleImp mul12;
            
            [Inject(Name = "M2")] internal IMultipleImp mul21;
            [Inject(Name = "M2")] internal IMultipleImp mul22;
            
            [Inject(Name = "M3")] internal IMultipleImp mul31;
            [Inject(Name = "M3")] internal IMultipleImp mul32;
        }

        [Test(Description = "When an interface has multiple implementations")]
        public void InterfaceWithMultipleImplementations() {
            var di = new ContainerBuilder(this);
            di.Scan<MultipleImpl1>();
            di.Scan<MultipleImpl2>();
            di.Scan<MultipleImpl3>();
            di.Scan<ServiceWithMultipleImpl1>();
            di.Scan<ServiceWithMultipleImpl2>();
            var c = di.Build();
            var i1 = c.Resolve<MultipleImpl1>();
            var i2 = c.Resolve<MultipleImpl2>();
            Assert.That(MultipleImpl3.Created, Is.EqualTo(2));            
            var s1 = c.Resolve<ServiceWithMultipleImpl1>();
            var s2 = c.Resolve<ServiceWithMultipleImpl2>();
            Assert.That(MultipleImpl3.Created, Is.EqualTo(2));            

            Assert.That(s1.mul11, Is.EqualTo(i1));
            Assert.That(s1.mul12, Is.EqualTo(i1));
            Assert.That(s1.mul21, Is.EqualTo(i2));
            Assert.That(s1.mul22, Is.EqualTo(i2));
            Assert.That(s2.mul11, Is.EqualTo(i1));
            Assert.That(s2.mul12, Is.EqualTo(i1));
            Assert.That(s2.mul21, Is.EqualTo(i2));
            Assert.That(s2.mul22, Is.EqualTo(i2));
            Assert.That(s1.mul31, Is.EqualTo(s1.mul32));
            Assert.That(s2.mul31, Is.EqualTo(s2.mul32));
            
            Assert.That(s1.mul31, Is.Not.EqualTo(s2.mul31));
        }

        class AutoTransient1 {
        }

        class AutoTransient2 {
            [Inject] internal AutoTransient1 auto;
        }

        [Test(Description = "Inject transients in transient")]
        public void CreateIfNotFound() {
            var di = new ContainerBuilder(this);
            var c = di.Build();
            c.CreateIfNotFound = true;
            var s1 = c.Resolve<AutoTransient2>();
            var s2 = c.Resolve<AutoTransient2>();

            Assert.That(s1, Is.Not.EqualTo(s2));
            Assert.That(s1.auto, Is.Not.EqualTo(s2.auto));
            
        }

    }
}