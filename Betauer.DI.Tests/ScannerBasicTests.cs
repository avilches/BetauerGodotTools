using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.DI.Tests {
    [TestFixture]
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
            Assert.Throws<InjectFieldException>(() => di.Build());
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
            var i1 = c.Resolve<IMultipleImp>("M1");
            var i2 = c.Resolve<IMultipleImp>("M2");
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


        [Singleton]
        public class Hold {
            public string Name;

            public Hold(string name) {
                Name = name;
            }
        }

        [Singleton]
        public class SingletonHolder {

            // Property
            [Singleton]
            private Hold SingletonHold1 => new Hold("1");
            
            // Property with Name
            [Singleton(Name = "SingletonHold2")]
            private Hold _hold2 => new Hold("2");

            // Method
            [Singleton]
            public Hold SingletonHold3() {
                return new Hold("3");
            }

            // Method with name
            [Singleton(Name = "SingletonHold4")]
            public Hold Hold4() {
                return new Hold("4");
            }

        }

        [Singleton]
        public class SingletonInjected {
            [Inject] internal Hold SingletonHold1;
            [Inject(Name = "SingletonHold1")] internal Hold h1;
            
            [Inject] internal Hold SingletonHold2;
            [Inject(Name = "SingletonHold2")] internal Hold h2;
            
            [Inject] internal Hold SingletonHold3;
            [Inject(Name = "SingletonHold3")] internal Hold h3;
            
            [Inject] internal Hold SingletonHold4;
            [Inject(Name = "SingletonHold4")] internal Hold h4;
        }
        
        [Test(Description = "Inject singletons by name exported in a singleton")]
        public void ExportSingletonFrom() {
            var di = new ContainerBuilder(this);
            di.Scan<SingletonInjected>();
            di.Scan<SingletonHolder>();
            var c = di.Build();

            LoggerFactory.SetTraceLevel(typeof(Container), TraceLevel.Info);
            LoggerFactory.SetTraceLevel(typeof(ContainerBuilder), TraceLevel.Info);
            SingletonInjected s1 = c.Resolve<SingletonInjected>();
            SingletonInjected s2 = c.Resolve<SingletonInjected>();
            Assert.That(s1, Is.EqualTo(s2));
            
            Assert.That(s1.h1.Name, Is.EqualTo("1"));
            Assert.That(s1.SingletonHold1.Name, Is.EqualTo("1"));
            Assert.That(s1.SingletonHold1, Is.EqualTo(s1.h1));
            
            Assert.That(s1.h2.Name, Is.EqualTo("2"));
            Assert.That(s1.SingletonHold2.Name, Is.EqualTo("2"));
            Assert.That(s1.SingletonHold2, Is.EqualTo(s1.h2));
            
            Assert.That(s1.h3.Name, Is.EqualTo("3"));
            Assert.That(s1.SingletonHold3.Name, Is.EqualTo("3"));
            Assert.That(s1.SingletonHold3, Is.EqualTo(s1.h3));
            
            Assert.That(s1.h4.Name, Is.EqualTo("4"));
            Assert.That(s1.SingletonHold4.Name, Is.EqualTo("4"));
            Assert.That(s1.SingletonHold4, Is.EqualTo(s1.h4));
        }
        
        [Singleton]
        public class TransientHolder {

            // Property
            [Transient]
            private Hold Hold1 => new Hold("1");
            
            // Property with Name
            [Transient(Name = "Hold2")]
            private Hold _hold2 => new Hold("2");

            // Method
            [Transient]
            public Hold Hold3() {
                return new Hold("3");
            }

            // Method with name
            [Transient(Name = "Hold4")]
            public Hold Hold4() {
                return new Hold("4");
            }

        }

        [Transient]
        public class TransientInjected {
            [Inject] internal Hold Hold1;
            [Inject(Name = "Hold1")] internal Hold h1;
            
            [Inject] internal Hold Hold2;
            [Inject(Name = "Hold2")] internal Hold h2;
            
            [Inject] internal Hold Hold3;
            [Inject(Name = "Hold3")] internal Hold h3;
            
            [Inject] internal Hold Hold4;
            [Inject(Name = "Hold4")] internal Hold h4;
        }
        
        [Test(Description = "Inject Transients by name exported in a Transient")]
        public void ExportTransientFrom() {
            var di = new ContainerBuilder(this);
            di.Scan<TransientInjected>();
            di.Scan<TransientHolder>();
            var c = di.Build();

            LoggerFactory.SetTraceLevel(typeof(Container), TraceLevel.Info);
            LoggerFactory.SetTraceLevel(typeof(ContainerBuilder), TraceLevel.Info);
            TransientInjected s1 = c.Resolve<TransientInjected>();
            TransientInjected s2 = c.Resolve<TransientInjected>();
            Assert.That(s1, Is.Not.EqualTo(s2));
            
            Assert.That(s1.h1.Name, Is.EqualTo("1"));
            Assert.That(s1.Hold1.Name, Is.EqualTo("1"));
            Assert.That(s1.Hold1, Is.EqualTo(s1.h1)); // Same context, so the 2 transients are the same
            Assert.That(s2.Hold1, Is.Not.EqualTo(s1.h1)); // Different context, different transient instances
            
            Assert.That(s1.h2.Name, Is.EqualTo("2"));
            Assert.That(s1.Hold2.Name, Is.EqualTo("2"));
            Assert.That(s1.Hold2, Is.EqualTo(s1.h2));
            Assert.That(s2.Hold2, Is.Not.EqualTo(s1.h2));
            
            Assert.That(s1.h3.Name, Is.EqualTo("3"));
            Assert.That(s1.Hold3.Name, Is.EqualTo("3"));
            Assert.That(s1.Hold3, Is.EqualTo(s1.h3));
            Assert.That(s2.Hold3, Is.Not.EqualTo(s1.h3));
            
            Assert.That(s1.h4.Name, Is.EqualTo("4"));
            Assert.That(s1.Hold4.Name, Is.EqualTo("4"));
            Assert.That(s1.Hold4, Is.EqualTo(s1.h4));
            Assert.That(s2.Hold4, Is.Not.EqualTo(s1.h4));
            
            
        }

        [Configuration]
        public class ConfigurationService {
            public static int Created = 0;
            public ConfigurationService() {
                Created++;
            }

            [Transient] private Hold TransientHold1 => new Hold("1");
            [Singleton] private Hold SingletonHold1() => new Hold("2");
        }

        [Singleton]
        public class ConfigurationServiceSingleton {
            [Transient] private Hold TransientHold2 => new Hold("2");
            [Singleton] private Hold SingletonHold2() => new Hold("2");
        }

        [Configuration]
        public class ConfigurationServiceStatic {
            public static int Created = 0;
            public ConfigurationServiceStatic() {
                Created++;
            }

            [Transient] private static Hold TransientStatic => new Hold("3");
            [Singleton] private static Hold SingletonStatic() => new Hold("3");
        }

        [Test(Description = "Use configuration to export members")]
        public void ExportFromConfiguration() {
            var di = new ContainerBuilder(this);
            di.Scan<ConfigurationService>();
            di.Scan<ConfigurationServiceSingleton>();
            di.Scan<ConfigurationServiceStatic>();
            var c = di.Build();
            
            Assert.That(ConfigurationService.Created, Is.EqualTo(1));
            Assert.That(ConfigurationServiceStatic.Created, Is.EqualTo(0));
            
            Assert.That(c.Contains<ConfigurationService>(), Is.False);
            Assert.That(c.Contains<ConfigurationServiceSingleton>(), Is.True);
            Assert.That(c.Contains<ConfigurationServiceStatic>(), Is.False);
            
            Hold t1 = c.Resolve<Hold>("TransientHold1");
            Hold t2 = c.Resolve<Hold>("TransientHold1");
            Hold t3 = c.Resolve<Hold>("TransientHold2");
            Hold t4 = c.Resolve<Hold>("TransientStatic");
            
            Assert.That(t1.Name, Is.EqualTo("1"));
            Assert.That(t2.Name, Is.EqualTo("1"));
            Assert.That(t3.Name, Is.EqualTo("2"));
            // Transients are all different
            Assert.That(t1, Is.Not.EqualTo(t2));
            Assert.That(t2, Is.Not.EqualTo(t3));
            Assert.That(t3, Is.Not.EqualTo(t4));
            Assert.That(t4, Is.Not.EqualTo(t1));

            // Singleton are the same
            Hold s11 = c.Resolve<Hold>("SingletonHold1");
            Hold s12 = c.Resolve<Hold>("SingletonHold1");
            Assert.That(s11, Is.EqualTo(s12));

            Hold s21 = c.Resolve<Hold>("SingletonHold2");
            Hold s22 = c.Resolve<Hold>("SingletonHold2");
            Assert.That(s21, Is.EqualTo(s22));

            Hold s31 = c.Resolve<Hold>("SingletonStatic");
            Hold s32 = c.Resolve<Hold>("SingletonStatic");
            Assert.That(s31, Is.EqualTo(s32));
            
        }
        
        
        [Singleton]
        class PostCreatedA {
            [Inject] internal PostCreatedB B;
            [Inject] internal Container container;

            internal int Called = 0;
            [PostCreate]
            void PostCreateMethod() {
                Assert.That(B, Is.Not.Null);
                Assert.That(B.A, Is.Not.Null);
                Called++;
            }
        }

        [Singleton]
        class PostCreatedB {
            [Inject] internal PostCreatedA A;
            [Inject] internal Container container;

            internal int Called = 0;
            [PostCreate]
            void PostCreateMethod() {
                Assert.That(A, Is.Not.Null);
                Assert.That(A.B, Is.Not.Null);
                Called++;
            }
        }

        [Test(Description = "Test if the [PostCreate] methods are invoked")]
        public void PostCreateMethodTest() {
            var c = new Container(this);
            var di = c.CreateBuilder();
            di.Scan<PostCreatedA>();
            di.Scan<PostCreatedB>();
            di.Build();

            var A = c.Resolve<PostCreatedA>();
            var B = c.Resolve<PostCreatedB>();
            
            Assert.That(A.B, Is.EqualTo(B));
            Assert.That(A.Called, Is.EqualTo(1));
            Assert.That(B.A, Is.EqualTo(A));
            Assert.That(B.Called, Is.EqualTo(1));

       }
    }
}