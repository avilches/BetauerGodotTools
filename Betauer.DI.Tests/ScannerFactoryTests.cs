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

    [Test(Description = "Custom service IFactory by name and type")]
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

    [Test(Description = "Custom service IFactory + Lazy is only created when it's used (not when it's injected)")]
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