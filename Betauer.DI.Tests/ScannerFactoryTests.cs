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
        Assert.Throws<InvalidCastException>(() => di.RegisterCustomFactory(typeof(MyService), () => new object()));
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
        [Factory] public MyServiceFactory MyService => new MyServiceFactory();
        [Factory(Lifetime = Lifetime.Transient)] public MyTransientFactory MyTransient => new MyTransientFactory();
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
    
    public class MyTransientFactory : IFactory<MyTransient> {
        public static int Instances = 0;
        public static int Gets = 0;

        public MyTransientFactory() {
            Instances++;
        }

        public MyTransient Get() {
            Gets++;
            return new MyTransient();
        }

        public static void Reset() {
            Instances = 0;
            Gets = 0;
        }
    }



    public class MyService : IInjectable {
        public bool WasInjected = false;
        public void PostInject() {
            WasInjected = true;
        }
    }
    
    public class MyTransient : IInjectable {
        public bool WasInjected = false;
        public void PostInject() {
            WasInjected = true;
        }
    }
    
    [Service]
    public class DemoSingleton {
        [Inject] public IFactory<MyService> MyService { get; set;  }
        [Inject] public IFactory<MyService> ServiceFactory { get; set;  }
        [Inject] public IFactory<MyTransient> MyTransient { get; set;  }
        [Inject] public IFactory<MyTransient> TransientFactory { get; set;  }
        [Inject("InnerFactory:MyService")] 
        public IFactory<MyService> InnerMyService { get; set;  }
        [Inject("InnerFactory:MyTransient")] 
        public IFactory<MyTransient> InnerMyTransient { get; set;  }
    }

    [Service(Lifetime.Transient)]
    public class DemoTransient {
        [Inject] public MyService MyService { get; set;  }
        [Inject] public MyTransient MyTransient { get; set;  }
    }

    [Test(Description = "Custom service IFactory (in configuration), inject by name and type")]
    public void ServiceFactoryTests() {
        MyServiceFactory.Reset();
        MyTransientFactory.Reset();
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<ServiceFactoryConfiguration>();
        di.Scan<DemoSingleton>();
        di.Scan<DemoTransient>();
        di.Build();

        Assert.That(MyServiceFactory.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(1));

        Assert.That(MyTransientFactory.Instances, Is.EqualTo(1));
        Assert.That(MyTransientFactory.Gets, Is.EqualTo(0));

        var demoSingleton = c.Resolve<DemoSingleton>();
        Assert.That(demoSingleton.MyService, Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(demoSingleton.ServiceFactory, Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(demoSingleton.InnerMyService, Is.EqualTo(c.Resolve<MyServiceFactory>()));
        Assert.That(demoSingleton.InnerMyTransient, Is.EqualTo(c.Resolve<MyTransientFactory>()));

        var d = c.Resolve<DemoTransient>();

        var x1 = demoSingleton.MyService.Get();
        var x2 = demoSingleton.ServiceFactory.Get();
        var x3 = c.Resolve<MyService>();

        Assert.That(x1.WasInjected, Is.True);
        Assert.That(x2.WasInjected, Is.True);
        Assert.That(x3.WasInjected, Is.True);
        Assert.That(d.MyService.WasInjected, Is.True);
        
        var t1 = demoSingleton.MyTransient.Get();
        var t2 = demoSingleton.TransientFactory.Get();
        var t3 = c.Resolve<MyTransient>();

        Assert.That(t1.WasInjected, Is.True);
        Assert.That(t2.WasInjected, Is.True);
        Assert.That(t3.WasInjected, Is.True);
        Assert.That(d.MyTransient.WasInjected, Is.True);
        
        Assert.That(MyServiceFactory.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(1));
        Assert.That(MyTransientFactory.Instances, Is.EqualTo(1));
        Assert.That(MyTransientFactory.Gets, Is.EqualTo(4));
    }

    [Configuration]
    public class ServiceFactoryLazyConfiguration {
        [Factory]
        [Lazy]
        public MyServiceFactory MyService => new MyServiceFactory();
    }

    [Service]
    public class DemoSingleton2 {
        [Inject] public IFactory<MyService> MyService { get; set;  }
        [Inject] public IFactory<MyService> ServiceFactory { get; set;  }
    }

    [Test(Description = "Custom service IFactory + Lazy (in configuration) is only created when it's used (not when it's injected)")]
    public void ServiceLazyFactoryTests() {
        MyServiceFactory.Reset();
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<ServiceFactoryLazyConfiguration>();
        di.Scan<DemoSingleton2>();
        di.Build();

        var demoSingleton = c.Resolve<DemoSingleton2>();
        Assert.That(demoSingleton.MyService, Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(demoSingleton.ServiceFactory, Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(c.Resolve("Factory:MyService"), Is.EqualTo(c.Resolve<IFactory<MyService>>()));

        // It's not created yet because the injected IFactory<MyService> are just wrappers
        Assert.That(MyServiceFactory.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(0));
        Assert.That(demoSingleton.MyService.Get(), Is.TypeOf<MyService>());
        Assert.That(MyServiceFactory.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(1));

    }

    [Factory(Name="MyService")]
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

    [Factory(Name="MyTransient", Lifetime = Lifetime.Transient)]
    public class MyTransientFactoryClass : IFactory<MyTransient> {
        public static int Instances = 0;
        public static int Gets = 0;

        public MyTransientFactoryClass() {
            Instances++;
        }

        public MyTransient Get() {
            Gets++;
            return new MyTransient();
        }

        public static void Reset() {
            Instances = 0;
            Gets = 0;
        }
    }

    [Service]
    public class DemoSingleton3 {
        [Inject] public IFactory<MyService> MyService { get; set;  }
        [Inject] public IFactory<MyService> ServiceFactory { get; set;  }
        [Inject] public IFactory<MyTransient> MyTransient { get; set;  }
        [Inject] public IFactory<MyTransient> TransientFactory { get; set;  }
        [Inject("InnerFactory:MyService")] 
        public IFactory<MyService> InnerMyService { get; set;  }
        [Inject("InnerFactory:MyTransient")] 
        public IFactory<MyTransient> InnerMyTransient { get; set;  }
    }

    [Service(Lifetime.Transient)]
    public class DemoTransient2 {
        [Inject] public MyService MyService { get; set;  }
        [Inject] public MyTransient MyTransient { get; set;  }
    }

    [Test(Description = "Custom service IFactory (in class), inject by name and type")]
    public void ServiceFactoryClassTests() {
        MyServiceFactoryClass.Reset();
        MyTransientFactoryClass.Reset();
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<MyServiceFactoryClass>();
        di.Scan<MyTransientFactoryClass>();
        di.Scan<DemoSingleton3>();
        di.Scan<DemoTransient2>();
        di.Build();

        Assert.That(MyServiceFactoryClass.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactoryClass.Gets, Is.EqualTo(1));

        Assert.That(MyTransientFactoryClass.Instances, Is.EqualTo(1));
        Assert.That(MyTransientFactoryClass.Gets, Is.EqualTo(0));

        var demoSingleton = c.Resolve<DemoSingleton3>();
        Assert.That(demoSingleton.MyService, Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(demoSingleton.ServiceFactory, Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(demoSingleton.InnerMyService, Is.EqualTo(c.Resolve<MyServiceFactoryClass>()));
        Assert.That(demoSingleton.InnerMyTransient, Is.EqualTo(c.Resolve<MyTransientFactoryClass>()));

        var d = c.Resolve<DemoTransient2>();

        var x1 = demoSingleton.MyService.Get();
        var x2 = demoSingleton.ServiceFactory.Get();
        var x3 = c.Resolve<MyService>();

        Assert.That(x1.WasInjected, Is.True);
        Assert.That(x2.WasInjected, Is.True);
        Assert.That(x3.WasInjected, Is.True);
        Assert.That(d.MyService.WasInjected, Is.True);
        
        var t1 = demoSingleton.MyTransient.Get();
        var t2 = demoSingleton.TransientFactory.Get();
        var t3 = c.Resolve<MyTransient>();

        Assert.That(t1.WasInjected, Is.True);
        Assert.That(t2.WasInjected, Is.True);
        Assert.That(t3.WasInjected, Is.True);
        Assert.That(d.MyTransient.WasInjected, Is.True);
        
        Assert.That(MyServiceFactoryClass.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactoryClass.Gets, Is.EqualTo(1));
        Assert.That(MyTransientFactoryClass.Instances, Is.EqualTo(1));
        Assert.That(MyTransientFactoryClass.Gets, Is.EqualTo(4));
    }

    
    [Factory(Name="MyService")]
    [Lazy]
    public class MyServiceFactoryLazyClass : IFactory<MyService>, IInjectable {
        public bool WasInjected = false;
        public static int Instances = 0;
        public static int Gets = 0;

        public MyServiceFactoryLazyClass() {
            Instances++;
        }

        public MyService Get() {
            Assert.That(WasInjected, Is.True);
            Gets++;
            return new MyService();
        }

        public static void Reset() {
            Instances = 0;
            Gets = 0;
        }
        public void PostInject() {
            WasInjected = true;
        }
    }

    [Service]
    public class DemoSingleton4 {
        [Inject] public IFactory<MyService> MyService { get; set;  }
        [Inject] public IFactory<MyService> Service { get; set;  }
    }


    [Test(Description = "Custom service IFactory + Lazy (in class) is only created when it's used (not when it's injected)")]
    public void ServiceLazyFactoryClassTests() {
        MyServiceFactoryLazyClass.Reset();
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<MyServiceFactoryLazyClass>();
        di.Scan<DemoSingleton4>();
        di.Build();

        var demoSingleton = c.Resolve<DemoSingleton4>();
        Assert.That(demoSingleton.MyService, Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(demoSingleton.Service, Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        Assert.That(c.Resolve("Factory:MyService"), Is.EqualTo(c.Resolve<IFactory<MyService>>()));
        
        // It's not created yet because the injected IFactory<MyService> are just wrappers
        Assert.That(MyServiceFactoryLazyClass.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactoryLazyClass.Gets, Is.EqualTo(0));
        Assert.That(demoSingleton.MyService.Get(), Is.TypeOf<MyService>());
        Assert.That(MyServiceFactoryLazyClass.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactoryLazyClass.Gets, Is.EqualTo(1));
    }
    
    /*
     * The same IFactory<T> with different names
     */

    public class Element : IInjectable {
        public bool WasInjected = false;
        public int Type;

        public Element(int type) {
            Type = type;
        }
        public void PostInject() {
            WasInjected = true;
        }
    }

    public class Element1Factory : IFactory<Element>, IInjectable {
        public bool WasInjected = false;
        public static int Instances = 0;
        
        public Element1Factory() {
            Instances++;
        }
        
        public Element Get() {
            Assert.That(WasInjected, Is.True);
            return new Element(1);
        }
        public void PostInject() {
            WasInjected = true;
        }
    }
    
    public class Element2Factory : IFactory<Element>, IInjectable {
        public bool WasInjected = false;
        public static int Instances = 0;

        public Element2Factory() {
            Instances++;
        }

        public Element Get() {
            Assert.That(WasInjected, Is.True);
            return new Element(2);
        }
        public void PostInject() {
            WasInjected = true;
        }
    }

    [Test(Description = "RegisterServiceAndAddFactory tests, by type and by name")]
    public void RegisterFactoryTests() {
        Element1Factory.Instances = 0;
        Element2Factory.Instances = 0;
        var c = new Container();
        var di = c.CreateBuilder();
        di.RegisterServiceAndAddFactory(typeof(Element), typeof(Element), () => new Element(0), Lifetime.Transient, "E");
        di.Build();
    
        Assert.That(c.Resolve<Element>().Type, Is.EqualTo(0));
        Assert.That(c.Resolve<Element>().WasInjected, Is.True);
        Assert.That(c.Resolve<IFactory<Element>>().Get().Type, Is.EqualTo(0));
        Assert.That(c.Resolve<IFactory<Element>>().Get().WasInjected, Is.True);

        Assert.That(c.Resolve<Element>("E").Type, Is.EqualTo(0));
        Assert.That(c.Resolve<Element>("E").WasInjected, Is.True);
        Assert.That(c.Resolve<IFactory<Element>>("Factory:E").Get().Type, Is.EqualTo(0));
        Assert.That(c.Resolve<IFactory<Element>>("Factory:E").Get().WasInjected, Is.True);
    }

    [Test(Description = "RegisterCustomFactory tests, by type and by name")]
    public void RegisterCustomFactoryTests() {
        Element1Factory.Instances = 0;
        Element2Factory.Instances = 0;
        var c = new Container();
        var di = c.CreateBuilder();
        di.RegisterCustomFactory(() => new Element1Factory(), "E1");
        di.RegisterCustomFactory(() => new Element2Factory(), "E2");
        di.Build();

        Assert.That(c.Resolve<Element>("E1").Type, Is.EqualTo(1));
        Assert.That(c.Resolve<Element>("E1").WasInjected, Is.True);
        Assert.That(c.Resolve<Element>("E2").Type, Is.EqualTo(2));
        Assert.That(c.Resolve<Element>("E2").WasInjected, Is.True);
        // no name = type 1 (the first)
        Assert.That(c.Resolve<Element>().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<Element>().WasInjected, Is.True);
        
        // Factory creates instances well injected
        Assert.That(c.Resolve<IFactory<Element>>("Factory:E1").Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<IFactory<Element>>("Factory:E1").Get().WasInjected, Is.True);
        Assert.That(c.Resolve<IFactory<Element>>("Factory:E2").Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<IFactory<Element>>("Factory:E2").Get().WasInjected, Is.True);
        Assert.That(c.Resolve<IFactory<Element>>().Get().Type, Is.EqualTo(1)); // no name = type 1 (the first)
        Assert.That(c.Resolve<IFactory<Element>>().Get().WasInjected, Is.True);// no name = type 1 (the first)
        
        // InnerFactory doesn't inject
        Assert.That(c.Resolve<Element1Factory>().Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<Element1Factory>().Get().WasInjected, Is.False);
        Assert.That(c.Resolve<Element2Factory>().Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<Element2Factory>().Get().WasInjected, Is.False);
        Assert.That(c.Resolve<Element1Factory>("InnerFactory:E1").Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<Element1Factory>("InnerFactory:E1").Get().WasInjected, Is.False);
        Assert.That(c.Resolve<Element2Factory>("InnerFactory:E2").Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<Element2Factory>("InnerFactory:E2").Get().WasInjected, Is.False);
        Assert.That(c.Resolve<IFactory<Element>>("InnerFactory:E1").Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<IFactory<Element>>("InnerFactory:E1").Get().WasInjected, Is.False);
        Assert.That(c.Resolve<IFactory<Element>>("InnerFactory:E2").Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<IFactory<Element>>("InnerFactory:E2").Get().WasInjected, Is.False);
        
        Assert.That(Element1Factory.Instances, Is.EqualTo(1));
        Assert.That(Element2Factory.Instances, Is.EqualTo(1));
    }    
    
    [Test(Description = "RegisterCustomFactory tests, by type and by name. When the factory is exposed as IFactory<>")]
    public void RegisterCustomFactoryAsIFactoryTests() {
        Element1Factory.Instances = 0;
        Element2Factory.Instances = 0;
        var c = new Container();
        var di = c.CreateBuilder();
        // *******************************
        // This is the difference: the real factory type is hidden 
        di.RegisterCustomFactory(typeof(IFactory<Element>), () => new Element1Factory(), "E1");
        di.RegisterCustomFactory(typeof(IFactory<Element>), () => new Element2Factory(), "E2");
        // *******************************
        di.Build();

        Assert.That(c.Resolve<Element>("E1").Type, Is.EqualTo(1));
        Assert.That(c.Resolve<Element>("E1").WasInjected, Is.True);
        Assert.That(c.Resolve<Element>("E2").Type, Is.EqualTo(2));
        Assert.That(c.Resolve<Element>("E2").WasInjected, Is.True);
        // no name = type 1 (the first)
        Assert.That(c.Resolve<Element>().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<Element>().WasInjected, Is.True);
        
        // Factory creates instances well injected
        Assert.That(c.Resolve<IFactory<Element>>("Factory:E1").Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<IFactory<Element>>("Factory:E1").Get().WasInjected, Is.True);
        Assert.That(c.Resolve<IFactory<Element>>("Factory:E2").Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<IFactory<Element>>("Factory:E2").Get().WasInjected, Is.True);
        
        // *******************************
        // This is the difference: the IFactory<Element> is bound to the inner factory, so it's not injected
        Assert.That(c.Resolve<IFactory<Element>>().Get().Type, Is.EqualTo(1)); // no name = type 1 (the first)
        Assert.That(c.Resolve<IFactory<Element>>().Get().WasInjected, Is.False);// no name = type 1 (the first)
        // *******************************
        
        // InnerFactory doesn't inject
        // *******************************
        // This is the difference: the Element1Factory and Element2Factory are not available
        Assert.Throws<ServiceNotFoundException>(() => c.Resolve<Element1Factory>());
        Assert.Throws<ServiceNotFoundException>(() => c.Resolve<Element2Factory>());
        // *******************************
        Assert.That(c.Resolve<Element1Factory>("InnerFactory:E1").Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<Element1Factory>("InnerFactory:E1").Get().WasInjected, Is.False);
        Assert.That(c.Resolve<Element2Factory>("InnerFactory:E2").Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<Element2Factory>("InnerFactory:E2").Get().WasInjected, Is.False);
        Assert.That(c.Resolve<IFactory<Element>>("InnerFactory:E1").Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<IFactory<Element>>("InnerFactory:E1").Get().WasInjected, Is.False);
        Assert.That(c.Resolve<IFactory<Element>>("InnerFactory:E2").Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<IFactory<Element>>("InnerFactory:E2").Get().WasInjected, Is.False);
        
        Assert.That(Element1Factory.Instances, Is.EqualTo(1));
        Assert.That(Element2Factory.Instances, Is.EqualTo(1));
    }

    [Test(Description = "RegisterCustomFactory tests, by type and by name. When the factory is exposed as IFactory<>")]
    public void RegisterCustomFactoryPrimaryTests() {
        Element1Factory.Instances = 0;
        Element2Factory.Instances = 0;
        var c = new Container();
        var di = c.CreateBuilder();
        di.RegisterCustomFactory(() => new Element1Factory(), "E1", Lifetime.Transient);
        di.RegisterCustomFactory(() => new Element2Factory(), "E2", Lifetime.Transient, true);
        di.Build();
        
        // no name = type 2 (because primary)
        Assert.That(c.Resolve<Element>().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<Element>().WasInjected, Is.True);
        
        // Factory creates instances well injected
        Assert.That(c.Resolve<IFactory<Element>>().Get().Type, Is.EqualTo(2)); // no name = type 2 (because primary)
        Assert.That(c.Resolve<IFactory<Element>>().Get().WasInjected, Is.True);// no name = type 2 (because primary)
    }
}