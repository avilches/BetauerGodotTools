using Betauer.DI.ServiceProvider;
using Betauer.Tools.Logging;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.DI.Tests; 

[TestFixture]
public class ScannerFactoryTests : Node {
    [SetUp]
    public void Setup() {
        LoggerFactory.OverrideTraceLevel(TraceLevel.All);
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

    [Configuration]
    public class TransientFactoryConfiguration {
        [Service(Lifetime.Transient)] public TransientD TransientD => new();
    }

    public class TransientD {
        public static int Instances = 0;

        public TransientD() {
            Instances++;
        }
    }

    [Service]
    public class Client {
        [Inject] public IFactory<TransientD> TransientD { get; set; }
    }

    [Test(Description = "Test defining a Transient service by name with a Factory")]
    public void FactoryTransientFromConfiguration() {
        TransientD.Instances = 0;
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<TransientFactoryConfiguration>();
        di.Scan<Client>();
        di.Build();

        Client client = c.Resolve<Client>();

        Assert.That(TransientD.Instances, Is.EqualTo(0));
        Assert.That(client.TransientD.Get(), Is.TypeOf<TransientD>());
        Assert.That(client.TransientD.Get(), Is.Not.EqualTo(client.TransientD.Get()));
        Assert.That(TransientD.Instances, Is.EqualTo(3));
    }


    
    [Configuration]
    public class ServiceFactoryConfiguration {
        [Factory] public IFactory<MyService> MyServiceFactory => new MyServiceFactory();
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

        public static void Reset() {
            Instances = 0;
            Gets = 0;
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

    [Test(Description = "Custom service IFactory (in configuration), inject by name and type")]
    public void ServiceFactoryTests() {
        MyServiceFactory.Reset();
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
            
        Assert.That(c.Resolve("MyServiceFactory"), Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(x1.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        Assert.That(x2.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        Assert.That(x3.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
    }


    [Configuration]
    public class ServiceFactoryLazyConfiguration {
        [Factory]
        [Lazy]
        public IFactory<MyService> MyServiceFactory => new MyServiceFactory();
    }

    [Service]
    public class DemoFactory2 {
        [Inject] public IFactory<MyService> MyServiceFactory { get; set;  }
    }

    [Test(Description = "Custom service IFactory + Lazy (in configuration) is only created when it's used (not when it's injected)")]
    public void ServiceLazyFactoryTests() {
        MyServiceFactory.Reset();
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<ServiceFactoryLazyConfiguration>();
        di.Scan<MyServiceDependency>();
        di.Scan<DemoFactory2>();
        di.Build();

        Assert.That(MyServiceFactory.Instances, Is.EqualTo(0));
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(0));

        Assert.That(c.Resolve("MyServiceFactory"), Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        var factory = c.Resolve<DemoFactory2>();
        // It's not created yet because the injected IFactory<MyService> are just wrappers
        Assert.That(MyServiceFactory.Instances, Is.EqualTo(0));
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(0));

        Assert.That(factory.MyServiceFactory.Get(), Is.TypeOf<MyService>());
        Assert.That(MyServiceFactory.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(1));

    }
    // TODO test [Factory] with no IFactory throws. Test [Factory] and [Service]

    [Factory(Name="MyServiceFactory")]
    public class MyServiceFactoryClass : IFactory<MyService> {
        public static int Instances = 0;
        public static int Gets = 0;

        public MyServiceFactoryClass() {
            Instances++;
        }

        public MyService Get() {
            Gets++;
            return new MyService();
        }

        public static void Reset() {
            Instances = 0;
            Gets = 0;
        }
    }

    [Service]
    public class DemoFactoryClass {
        [Inject] public IFactory<MyService> MyServiceFactory { get; set;  }
        [Inject] public IFactory<MyService> ServiceFactory { get; set;  }
        [Inject] public MyService MyService { get; set;  }
    }

    [Test(Description = "Custom service IFactory (in class), inject by name and type")]
    public void ServiceFactoryClassTests() {
        MyServiceFactoryClass.Reset();
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<MyServiceFactoryClass>();
        di.Scan<MyServiceDependency>();
        di.Scan<DemoFactoryClass>();
        di.Build();

        Assert.That(MyServiceFactoryClass.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactoryClass.Gets, Is.EqualTo(1));

        var factory = c.Resolve<DemoFactoryClass>();
        Assert.That(MyServiceFactoryClass.Gets, Is.EqualTo(1));

        Assert.That(factory.MyServiceFactory, Is.EqualTo(factory.ServiceFactory));

        var x1 = factory.MyServiceFactory.Get();
        var x2 = factory.ServiceFactory.Get();
        var x3 = factory.MyService;
        Assert.That(MyServiceFactoryClass.Gets, Is.EqualTo(3));
            
        Assert.That(c.Resolve("MyServiceFactory"), Is.EqualTo(c.Resolve<IFactory<MyService>>()));

        Assert.That(x1.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        Assert.That(x2.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        Assert.That(x3.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
    }

    
    [Factory(Name="MyServiceFactory")]
    [Lazy]
    public class MyServiceFactoryLazyClass : IFactory<MyService> {
        public static int Instances = 0;
        public static int Gets = 0;

        public MyServiceFactoryLazyClass() {
            Instances++;
        }

        public MyService Get() {
            Gets++;
            return new MyService();
        }

        public static void Reset() {
            Instances = 0;
            Gets = 0;
        }
    }

    [Service]
    public class DemoFactory3 {
        [Inject] public IFactory<MyService> MyServiceFactory { get; set;  }
    }


    [Test(Description = "Custom service IFactory + Lazy (in class) is only created when it's used (not when it's injected)")]
    public void ServiceLazyFactoryClassTests() {
        MyServiceFactoryLazyClass.Reset();
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<MyServiceFactoryLazyClass>();
        di.Scan<MyServiceDependency>();
        di.Scan<DemoFactory3>();
        di.Build();

        Assert.That(MyServiceFactoryLazyClass.Instances, Is.EqualTo(0));
        Assert.That(MyServiceFactoryLazyClass.Gets, Is.EqualTo(0));

        Assert.That(c.Resolve("MyServiceFactory"), Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        var factory = c.Resolve<DemoFactory3>();
        // It's not created yet because the injected IFactory<MyService> are just wrappers
        Assert.That(MyServiceFactoryLazyClass.Instances, Is.EqualTo(0));
        Assert.That(MyServiceFactoryLazyClass.Gets, Is.EqualTo(0));

        Assert.That(factory.MyServiceFactory.Get(), Is.TypeOf<MyService>());
        Assert.That(MyServiceFactoryLazyClass.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactoryLazyClass.Gets, Is.EqualTo(1));
    }

}