using System;
using Betauer.DI.Exceptions;
using Betauer.DI.Factory;
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

    [Factory] public class WrongFactory { }

    [Configuration]
    public class WrongConfig {
        [Factory] public List WrongType => new List();

    }
    [Test(Description = "[Factory] should implement IFactory<> (class)")]
    public void FactoryWrongType1() {
        var c = new Container();
        var di = c.CreateBuilder();
        Assert.Throws<InvalidAttributeException>(() => di.Scan<WrongFactory>());
    }

    [Test(Description = "[Factory] should implement IFactory<> (configuration)")]
    public void FactoryWrongType2() {
        var c = new Container();
        var di = c.CreateBuilder();
        Assert.Throws<InvalidAttributeException>(() => di.Scan<WrongConfig>());
    }

    [Test(Description = "Not implement IFactory<> fails at runtime")]
    public void FactoryWrongType3() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.RegisterCustomFactory(typeof(MyService), () => new object());
        di.Build();
        Assert.Throws<InvalidCastException>(() => c.Resolve<MyService>());
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

    [Test(Description = "Register a Singleton Factory")]
    public void RegisterSingletonAndAddFactoryTst() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.RegisterServiceAndAddFactory(typeof(Node), typeof(Node), () => new Node { Name = "L" }, Lifetime.Singleton, "X");
        di.Build();
        Assert.That(c.Resolve<Node>(), Is.EqualTo(c.Resolve<Node>()));
        Assert.That(c.Resolve<Node>().Name.ToString(), Is.EqualTo("L"));
        var factory = c.Resolve<IFactory<Node>>();
        Assert.That(factory.Get(), Is.EqualTo(c.Resolve<Node>()));
    }

    [Test(Description = "Register a Transient Factory")]
    public void RegisterTransientAndAddFactoryTst() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.RegisterServiceAndAddFactory(typeof(Node), typeof(Node), () => new Node { Name = "L" }, Lifetime.Transient, "X");
        di.Build();
        Assert.That(c.Resolve<Node>(), Is.Not.EqualTo(c.Resolve<Node>()));
        Assert.That(c.Resolve<Node>().Name.ToString(), Is.EqualTo("L"));
        var factory = c.Resolve<IFactory<Node>>();
        Assert.That(factory.Get(), Is.Not.EqualTo(factory.Get()));
    }

    [Configuration]
    public class ServiceFactoryConfiguration {
        [Factory] public IFactory<MyService> MyServiceFactoryX => new MyServiceFactory();
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
    public class DemoSingleton {
        [Inject] public IFactory<MyService> MyServiceFactory { get; set;  }
        [Inject] public IFactory<MyService> ServiceFactory { get; set;  }
    }

    [Service(Lifetime.Transient)]
    public class DemoTransient {
        [Inject] public MyService MyService { get; set;  }
    }

    [Test(Description = "Custom service IFactory (in configuration), inject by name and type")]
    public void ServiceFactoryTests() {
        MyServiceFactory.Reset();
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<ServiceFactoryConfiguration>();
        di.Scan<MyServiceDependency>();
        di.Scan<DemoSingleton>();
        di.Scan<DemoTransient>();
        di.Build();

        Assert.That(MyServiceFactory.Instances, Is.EqualTo(1));

        var demoSingleton = c.Resolve<DemoSingleton>();
        Assert.That(demoSingleton.MyServiceFactory, Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(demoSingleton.ServiceFactory, Is.EqualTo(c.Resolve<IFactory<MyService>>()));

        Assert.That(MyServiceFactory.Gets, Is.EqualTo(0));
        var x1 = demoSingleton.MyServiceFactory.Get();
        var x2 = demoSingleton.ServiceFactory.Get();
        var x3 = c.Resolve<MyService>();
        var d = c.Resolve<DemoTransient>();
            
        Assert.That(c.Resolve("MyServiceFactoryX"), Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(x1.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        Assert.That(x2.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        Assert.That(x3.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        Assert.That(d.MyService.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(4));
        Assert.That(MyServiceFactory.Instances, Is.EqualTo(1));
    }

    [Configuration]
    public class ServiceFactoryLazyConfiguration {
        [Factory]
        [Lazy]
        public IFactory<MyService> MyServiceFactoryX => new MyServiceFactory();
    }

    [Service]
    public class DemoSingleton2 {
        [Inject] public IFactory<MyService> MyServiceFactory { get; set;  }
        [Inject] public IFactory<MyService> ServiceFactory { get; set;  }
    }

    [Test(Description = "Custom service IFactory + Lazy (in configuration) is only created when it's used (not when it's injected)")]
    public void ServiceLazyFactoryTests() {
        MyServiceFactory.Reset();
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<ServiceFactoryLazyConfiguration>();
        di.Scan<MyServiceDependency>();
        di.Scan<DemoSingleton2>();
        di.Build();

        var demoSingleton = c.Resolve<DemoSingleton2>();
        Assert.That(demoSingleton.MyServiceFactory, Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(demoSingleton.ServiceFactory, Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(c.Resolve("MyServiceFactoryX"), Is.EqualTo(c.Resolve<IFactory<MyService>>()));

        // It's not created yet because the injected IFactory<MyService> are just wrappers


        Assert.That(MyServiceFactory.Instances, Is.EqualTo(0));
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(0));
        Assert.That(demoSingleton.MyServiceFactory.Get(), Is.TypeOf<MyService>());
        Assert.That(MyServiceFactory.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(1));

    }
    [Factory(Name="MyServiceFactoryX")]
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
    public class DemoSingleton3 {
        [Inject] public IFactory<MyService> MyServiceFactoryX { get; set;  }
        [Inject] public IFactory<MyService> ServiceFactory { get; set;  }
    }

    [Service(Lifetime.Transient)]
    public class DemoTransient2 {
        [Inject] public MyService MyService { get; set;  }
    }

    [Test(Description = "Custom service IFactory (in class), inject by name and type")]
    public void ServiceFactoryClassTests() {
        MyServiceFactoryClass.Reset();
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<MyServiceFactoryClass>();
        di.Scan<MyServiceDependency>();
        di.Scan<DemoSingleton3>();
        di.Scan<DemoTransient2>();
        di.Build();


        Assert.That(MyServiceFactoryClass.Instances, Is.EqualTo(1));

        var demoSingleton = c.Resolve<DemoSingleton3>();
        Assert.That(demoSingleton.MyServiceFactoryX, Is.EqualTo(demoSingleton.ServiceFactory));
        Assert.That(demoSingleton.ServiceFactory, Is.EqualTo(demoSingleton.ServiceFactory));

        Assert.That(MyServiceFactoryClass.Gets, Is.EqualTo(0));
        var x1 = demoSingleton.MyServiceFactoryX.Get();
        var x2 = demoSingleton.ServiceFactory.Get();
        var x3 = c.Resolve<MyService>();
        var d = c.Resolve<DemoTransient2>();

        Assert.That(c.Resolve("MyServiceFactoryX"), Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(x1.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        Assert.That(x2.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        Assert.That(x3.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        Assert.That(d.MyService.Dependency, Is.EqualTo(c.Resolve<MyServiceDependency>()));
        
        Assert.That(MyServiceFactoryClass.Gets, Is.EqualTo(4));
        Assert.That(MyServiceFactoryClass.Instances, Is.EqualTo(1));
    }

    
    [Factory(Name="MyServiceFactoryX")]
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
    public class DemoSingleton4 {
        [Inject] public IFactory<MyService> MyServiceFactory { get; set;  }
        [Inject] public IFactory<MyService> ServiceFactory { get; set;  }
    }


    [Test(Description = "Custom service IFactory + Lazy (in class) is only created when it's used (not when it's injected)")]
    public void ServiceLazyFactoryClassTests() {
        MyServiceFactoryLazyClass.Reset();
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<MyServiceFactoryLazyClass>();
        di.Scan<MyServiceDependency>();
        di.Scan<DemoSingleton4>();
        di.Build();

        var demoSingleton = c.Resolve<DemoSingleton4>();
        Assert.That(demoSingleton.MyServiceFactory, Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(demoSingleton.ServiceFactory, Is.EqualTo(c.Resolve<IFactory<MyService>>()));

        Assert.That(c.Resolve("MyServiceFactoryX"), Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        
        // It's not created yet because the injected IFactory<MyService> are just wrappers
        Assert.That(MyServiceFactoryLazyClass.Instances, Is.EqualTo(0));
        Assert.That(MyServiceFactoryLazyClass.Gets, Is.EqualTo(0));
        Assert.That(demoSingleton.MyServiceFactory.Get(), Is.TypeOf<MyService>());
        Assert.That(MyServiceFactoryLazyClass.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactoryLazyClass.Gets, Is.EqualTo(1));
    }
}