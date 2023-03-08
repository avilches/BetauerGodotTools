using Betauer.DI.ServiceProvider;
using Betauer.Tools.Logging;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.DI.Tests {
    [TestFixture]
    [Only]
    public class ScannerFactoryTests : Node {
        [SetUp]
        public void Setup() {
            LoggerFactory.OverrideTraceLevel(TraceLevel.All);
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

            [PostInject] void More1() => more1 = true;
            [PostInject] void More2() => more2 = true;
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

            [PostInject] void More1() => more1 = true;
            [PostInject] void More2() => more2 = true;

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

            [PostInject] void More1() => more1 = true;
            [PostInject] void More2() => more2 = true;

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
            [Service]
            [Lazy] 
            public LazySingleton LazySingleton => new();
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
        
        
        [Configuration]
        public class ServiceFactoryConfiguration {
            [Service] public IFactory<MyService> MyServiceFactory => new MyServiceFactory();
        }

        public class MyServiceFactory : IFactory<MyService> {
            public static int Instances = 0;
            public static int Gets = 0;

            public MyServiceFactory() {
                Instances++;
            }

            public MyService Get() {
                Gets++;
                return new MyService();
            }
        }

        public class MyService {
            [Inject] public MyServiceDependency Dependency { get; set; }
        }
        
        [Service]
        public class MyServiceDependency {}

        [Service]
        public class DemoFactory {
            [Inject] public IFactory<MyService> MyServiceFactory { get; set;  }
            [Inject] public IFactory<MyService> ServiceFactory { get; set;  }
            [Inject] public MyService MyService { get; set;  }
        }

        [Test(Description = "Custom service IFactory by name and type")]
        public void ServiceFactoryTests() {
            var c = new Container();
            var di = c.CreateBuilder();
            di.Scan<ServiceFactoryConfiguration>();
            di.Scan<MyServiceDependency>();
            di.Scan<DemoFactory>();
            di.Build();

            Assert.That(MyServiceFactory.Instances, Is.EqualTo(1));
            Assert.That(MyServiceFactory.Gets, Is.EqualTo(1));

            var factory = c.Resolve<DemoFactory>();
            Assert.That(MyServiceFactory.Gets, Is.EqualTo(1));

            Assert.That(factory.MyServiceFactory, Is.EqualTo(factory.ServiceFactory));

            var x1 = factory.MyServiceFactory.Get();
            var x2 = factory.ServiceFactory.Get();
            var x3 = factory.MyService;
            Assert.That(MyServiceFactory.Gets, Is.EqualTo(3));
            
            Assert.That(x1.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
            Assert.That(x2.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
            Assert.That(x3.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        }


        [Configuration]
        public class ServiceFactoryLazyConfiguration {
            [Service]
            [Lazy]
            public IFactory<MyService> MyServiceFactory => new MyServiceFactory();
        }

        [Service]
        public class DemoFactory2 {
            [Inject] public IFactory<MyService> MyServiceFactory { get; set;  }
        }

        [Test(Description = "Custom service IFactory Lazy is only created when it's used (not when it's injected)")]
        [Only]
        public void ServiceLazyFactoryTests() {
            var c = new Container();
            var di = c.CreateBuilder();
            di.Scan<ServiceFactoryLazyConfiguration>();
            di.Scan<MyServiceDependency>();
            di.Scan<DemoFactory2>();
            di.Build();

            Assert.That(MyServiceFactory.Instances, Is.EqualTo(0));
            Assert.That(MyServiceFactory.Gets, Is.EqualTo(0));

            c.Resolve("MyServiceFactory");
            c.Resolve<IFactory<MyService>>();
            var factory = c.Resolve<DemoFactory2>();
            // It's not created yet because the injected IFactory<MyService> are just wrappers
            Assert.That(MyServiceFactory.Instances, Is.EqualTo(0));
            Assert.That(MyServiceFactory.Gets, Is.EqualTo(0));

            factory.MyServiceFactory.Get();
            Assert.That(MyServiceFactory.Instances, Is.EqualTo(1));
            Assert.That(MyServiceFactory.Gets, Is.EqualTo(1));

        }
        
    }
}