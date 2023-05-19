using System;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Logging;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.DI.Tests; 

[TestRunner.Test]
public class ScannerFactoryTests : Node {
    [SetUpClass]
    public void Setup() {
        LoggerFactory.OverrideTraceLevel(TraceLevel.All);
    }

    [Attributes.Factory.Singleton] public class WrongFactory { }

    [Configuration]
    public class WrongConfig {
        [Attributes.Factory.Singleton] public Node WrongType => new Node();

    }
    [TestRunner.Test(Description = "[SingletonFactory] should implement IFactory<> (class)")]
    public void FactoryWrongType1() {
        var c = new Container();
        var di = c.CreateBuilder();
        Assert.Throws<InvalidCastException>(() => di.Scan<WrongFactory>());
    }

    [TestRunner.Test(Description = "[SingletonFactory] should implement IFactory<> (configuration)")]
    public void FactoryWrongType2() {
        var c = new Container();
        var di = c.CreateBuilder();
        Assert.Throws<InvalidCastException>(() => di.Scan<WrongConfig>());
    }

    [TestRunner.Test(Description = "Not implement IFactory<> fails at runtime")]
    public void FactoryWrongType3() {
        var c = new Container();
        var di = c.CreateBuilder();
        Assert.Throws<InvalidCastException>(() => di.RegisterFactory(typeof(MyService), Lifetime.Singleton, () => new object()));
    }

    [Configuration]
    public class TransientFactoryConfiguration {
        [Transient] public TransientD TransientD => new();
    }

    public class TransientD {
        public static int Instances = 0;

        public TransientD() {
            Instances++;
        }
    }

    [Singleton]
    public class Client1 {
        [Inject] public IGet<TransientD> TransientD { get; set; }
    }
    
    [Singleton]
    public class Client2 {
        [Inject] public IFactory<TransientD> TransientD { get; set; }
    }

    [TestRunner.Test(Description = "Test defining a Transient service by name with a Factory")]
    public void FactoryTransientFromConfiguration() {
        TransientD.Instances = 0;
        var c = new Container();
        var di = c.CreateBuilder();
        di.Scan<TransientFactoryConfiguration>();
        di.Scan<Client1>();
        di.Scan<Client2>();
        di.Build();

        Client1 client1 = c.Resolve<Client1>();

        Assert.That(TransientD.Instances, Is.EqualTo(0));
        Assert.That(client1.TransientD.Get(), Is.TypeOf<TransientD>());
        Assert.That(client1.TransientD.Get(), Is.Not.EqualTo(client1.TransientD.Get()));
        Assert.That(TransientD.Instances, Is.EqualTo(3));

        Client2 client2 = c.Resolve<Client2>();

        Assert.That(TransientD.Instances, Is.EqualTo(3));
        Assert.That(client2.TransientD.Get(), Is.TypeOf<TransientD>());
        Assert.That(client2.TransientD.Get(), Is.Not.EqualTo(client2.TransientD.Get()));
        Assert.That(TransientD.Instances, Is.EqualTo(6));
    }

    [TestRunner.Test(Description = "Register a Singleton Factory")]
    public void RegisterSingletonAndAddFactoryTst() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.RegisterServiceAndAddFactory(typeof(Node), typeof(Node), Lifetime.Singleton, () => new Node { Name = "L" }, "X");
        di.Build();
        Assert.That(c.Resolve<Node>(), Is.EqualTo(c.Resolve<Node>()));
        Assert.That(c.Resolve<Node>().Name.ToString(), Is.EqualTo("L"));
        var factory = c.Resolve<ILazy<Node>>();
        Assert.That(factory.Get(), Is.EqualTo(c.Resolve<Node>()));
    }

    [TestRunner.Test(Description = "Register a Transient Factory")]
    public void RegisterTransientAndAddFactoryTest() {
        var c = new Container();
        var di = c.CreateBuilder();
        di.RegisterServiceAndAddFactory(typeof(Node), typeof(Node), Lifetime.Transient, () => new Node { Name = "L" }, "X");
        di.Build();
        Assert.That(c.Resolve<Node>(), Is.Not.EqualTo(c.Resolve<Node>()));
        Assert.That(c.Resolve<Node>().Name.ToString(), Is.EqualTo("L"));
        var factory = c.Resolve<IFactory<Node>>();
        Assert.That(factory.Get(), Is.Not.EqualTo(factory.Get()));
    }

    [Configuration]
    public class ServiceFactoryConfiguration {
        [Attributes.Factory.Singleton] public MyServiceFactory MyService => new MyServiceFactory();
        [Attributes.Factory.Transient] public MyTransientFactory MyTransient => new MyTransientFactory();
    }

    public class MyServiceFactory : IGet<MyService> {
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
    
    public class MyTransientFactory : IGet<MyTransient> {
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
    
    [Singleton]
    public class DemoSingleton {
        [Inject] public ILazy<MyService> MyService { get; set;  }
        [Inject] public ILazy<MyService> ServiceFactory { get; set;  }
        [Inject] public IFactory<MyTransient> MyTransient { get; set;  }
        [Inject] public IFactory<MyTransient> TransientFactory { get; set;  }
        [Inject("InnerFactory:MyService")] 
        public IGet<MyService> InnerMyService { get; set;  }
        [Inject("InnerFactory:MyTransient")] 
        public IGet<MyTransient> InnerMyTransient { get; set;  }
    }

    [Transient]
    public class DemoTransient {
        [Inject] public MyService MyService { get; set;  }
        [Inject] public MyTransient MyTransient { get; set;  }
    }

    [TestRunner.Test(Description = "Custom service IFactory (in configuration), inject by name and type")]
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
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(0));

        Assert.That(MyTransientFactory.Instances, Is.EqualTo(1));
        Assert.That(MyTransientFactory.Gets, Is.EqualTo(0));

        var demoSingleton = c.Resolve<DemoSingleton>();
        Assert.That(demoSingleton.MyService, Is.EqualTo(c.Resolve<ILazy<MyService>>()));
        Assert.That(demoSingleton.ServiceFactory, Is.EqualTo(c.Resolve<ILazy<MyService>>()));
        Assert.That(demoSingleton.InnerMyService, Is.EqualTo(c.Resolve<MyServiceFactory>("InnerFactory:MyService")));
        Assert.That(demoSingleton.InnerMyTransient, Is.EqualTo(c.Resolve<MyTransientFactory>("InnerFactory:MyTransient")));

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

    [Attributes.Factory.Singleton(Name="MyService")]
    public class MyServiceFactoryClass : IGet<MyService> {
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

    [Attributes.Factory.Transient(Name="MyTransient")]
    public class MyTransientFactoryClass : IGet<MyTransient> {
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

    [Singleton]
    public class DemoSingleton3 {
        [Inject] public ILazy<MyService> MyService { get; set;  }
        [Inject] public ILazy<MyService> ServiceFactory { get; set;  }
        [Inject] public IFactory<MyTransient> MyTransient { get; set;  }
        [Inject] public IFactory<MyTransient> TransientFactory { get; set;  }
        [Inject("InnerFactory:MyService")] 
        public IGet<MyService> InnerMyService { get; set;  }
        [Inject("InnerFactory:MyTransient")] 
        public IGet<MyTransient> InnerMyTransient { get; set;  }
    }

    [Transient]
    public class DemoTransient2 {
        [Inject] public MyService MyService { get; set;  }
        [Inject] public MyTransient MyTransient { get; set;  }
    }

    [TestRunner.Test(Description = "Custom service IFactory (in class), inject by name and type", Only = true)]
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
        Assert.That(MyServiceFactoryClass.Gets, Is.EqualTo(0));

        Assert.That(MyTransientFactoryClass.Instances, Is.EqualTo(1));
        Assert.That(MyTransientFactoryClass.Gets, Is.EqualTo(0));

        var demoSingleton = c.Resolve<DemoSingleton3>();
        Assert.That(demoSingleton.MyService, Is.EqualTo(c.Resolve<ILazy<MyService>>()));
        Assert.That(demoSingleton.ServiceFactory, Is.EqualTo(c.Resolve<ILazy<MyService>>()));
        Assert.That(demoSingleton.InnerMyService, Is.EqualTo(c.Resolve<MyServiceFactoryClass>("InnerFactory:MyService")));
        Assert.That(demoSingleton.InnerMyTransient, Is.EqualTo(c.Resolve<MyTransientFactoryClass>("InnerFactory:MyTransient")));

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

    public class Element1Factory : IGet<Element>, IInjectable {
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
    
    public class Element2Factory : IGet<Element>, IInjectable {
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

    [TestRunner.Test(Description = "RegisterServiceAndAddFactory tests, by type and by name")]
    public void RegisterFactoryTests() {
        Element1Factory.Instances = 0;
        Element2Factory.Instances = 0;
        var c = new Container();
        var di = c.CreateBuilder();
        di.RegisterServiceAndAddFactory(typeof(Element), typeof(Element), Lifetime.Transient, () => new Element(0), "E");
        di.Build();
    
        Assert.That(c.Resolve<Element>().Type, Is.EqualTo(0));
        Assert.That(c.Resolve<Element>().WasInjected, Is.True);
        Assert.That(c.Resolve<IFactory<Element>>().Get().Type, Is.EqualTo(0));
        Assert.That(c.Resolve<IFactory<Element>>().Get().WasInjected, Is.True);

        Assert.That(c.Resolve<Element>("E").Type, Is.EqualTo(0));
        Assert.That(c.Resolve<Element>("E").WasInjected, Is.True);
        Assert.That(c.Resolve<IGet<Element>>("Factory:E").Get().Type, Is.EqualTo(0));
        Assert.That(c.Resolve<IGet<Element>>("Factory:E").Get().WasInjected, Is.True);
    }

    [TestRunner.Test(Description = "RegisterCustomFactory tests, by type and by name")]
    public void RegisterCustomFactoryTests() {
        Element1Factory.Instances = 0;
        Element2Factory.Instances = 0;
        var c = new Container();
        var di = c.CreateBuilder();
        di.RegisterFactory(() => new Element1Factory(), "E1");
        di.RegisterFactory(() => new Element2Factory(), "E2");
        di.Build();

        Assert.That(c.Resolve<Element>("E1").Type, Is.EqualTo(1));
        Assert.That(c.Resolve<Element>("E1").WasInjected, Is.True);
        Assert.That(c.Resolve<Element>("E2").Type, Is.EqualTo(2));
        Assert.That(c.Resolve<Element>("E2").WasInjected, Is.True);
        // no name = type 1 (the first)
        Assert.That(c.Resolve<Element>().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<Element>().WasInjected, Is.True);
        
        // Factory creates instances well injected
        Assert.That(c.Resolve<IGet<Element>>("Factory:E1").Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<IGet<Element>>("Factory:E1").Get().WasInjected, Is.True);
        Assert.That(c.Resolve<IGet<Element>>("Factory:E2").Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<IGet<Element>>("Factory:E2").Get().WasInjected, Is.True);
        Assert.That(c.Resolve<ILazy<Element>>().Get().Type, Is.EqualTo(1)); // no name = type 1 (the first)
        Assert.That(c.Resolve<ILazy<Element>>().Get().WasInjected, Is.True);// no name = type 1 (the first)
        Assert.That(c.Resolve<ILazy<Element>>(), Is.TypeOf<ProviderSingletonFactory<Element>>());
        
        // InnerFactory only by name. It doesn't inject
        Assert.Throws<ServiceNotFoundException>(() => c.Resolve<Element1Factory>());
        Assert.Throws<ServiceNotFoundException>(() => c.Resolve<Element2Factory>());
        Assert.That(c.Resolve<Element1Factory>("InnerFactory:E1").Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<Element1Factory>("InnerFactory:E1").Get().WasInjected, Is.False);
        Assert.That(c.Resolve<Element2Factory>("InnerFactory:E2").Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<Element2Factory>("InnerFactory:E2").Get().WasInjected, Is.False);
        Assert.That(c.Resolve<IGet<Element>>("InnerFactory:E1").Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<IGet<Element>>("InnerFactory:E1").Get().WasInjected, Is.False);
        Assert.That(c.Resolve<IGet<Element>>("InnerFactory:E2").Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<IGet<Element>>("InnerFactory:E2").Get().WasInjected, Is.False);
        
        Assert.That(Element1Factory.Instances, Is.EqualTo(1));
        Assert.That(Element2Factory.Instances, Is.EqualTo(1));
    }    
    
    [TestRunner.Test(Description = "RegisterCustomFactory tests, by type and by name. When the factory is exposed as IFactory<>")]
    public void RegisterCustomFactoryAsIFactoryTests() {
        Element1Factory.Instances = 0;
        Element2Factory.Instances = 0;
        var c = new Container();
        var di = c.CreateBuilder();
        // *******************************
        // This is the difference: the real factory type is hidden 
        di.RegisterFactory(typeof(IGet<Element>), Lifetime.Singleton, () => new Element1Factory(), "E1");
        di.RegisterFactory(typeof(IGet<Element>), Lifetime.Singleton, () => new Element2Factory(), "E2");
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
        Assert.That(c.Resolve<IGet<Element>>("Factory:E1").Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<IGet<Element>>("Factory:E1").Get().WasInjected, Is.True);
        Assert.That(c.Resolve<IGet<Element>>("Factory:E2").Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<IGet<Element>>("Factory:E2").Get().WasInjected, Is.True);
        Assert.That(c.Resolve<ILazy<Element>>().Get().Type, Is.EqualTo(1)); // no name = type 1 (the first)
        Assert.That(c.Resolve<ILazy<Element>>().Get().WasInjected, Is.True);// no name = type 1 (the first)
        Assert.That(c.Resolve<ILazy<Element>>(), Is.TypeOf<ProviderSingletonFactory<Element>>());
            
        // InnerFactory only by name. It doesn't inject
        Assert.Throws<ServiceNotFoundException>(() => c.Resolve<Element1Factory>());
        Assert.Throws<ServiceNotFoundException>(() => c.Resolve<Element2Factory>());
        Assert.That(c.Resolve<Element1Factory>("InnerFactory:E1").Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<Element1Factory>("InnerFactory:E1").Get().WasInjected, Is.False);
        Assert.That(c.Resolve<Element2Factory>("InnerFactory:E2").Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<Element2Factory>("InnerFactory:E2").Get().WasInjected, Is.False);
        Assert.That(c.Resolve<IGet<Element>>("InnerFactory:E1").Get().Type, Is.EqualTo(1));
        Assert.That(c.Resolve<IGet<Element>>("InnerFactory:E1").Get().WasInjected, Is.False);
        Assert.That(c.Resolve<IGet<Element>>("InnerFactory:E2").Get().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<IGet<Element>>("InnerFactory:E2").Get().WasInjected, Is.False);
        
        Assert.That(Element1Factory.Instances, Is.EqualTo(1));
        Assert.That(Element2Factory.Instances, Is.EqualTo(1));
    }

    [TestRunner.Test(Description = "RegisterCustomFactory tests, by type and by name. When the factory is exposed as IFactory<>")]
    public void RegisterCustomFactoryPrimaryTests() {
        Element1Factory.Instances = 0;
        Element2Factory.Instances = 0;
        var c = new Container();
        var di = c.CreateBuilder();
        di.RegisterFactory(() => new Element1Factory(), "E1", Lifetime.Transient);
        di.RegisterFactory(() => new Element2Factory(), "E2", Lifetime.Transient, true);
        di.Build();
        
        // no name = type 2 (because primary)
        Assert.That(c.Resolve<Element>().Type, Is.EqualTo(2));
        Assert.That(c.Resolve<Element>().WasInjected, Is.True);
        
        // Factory creates instances well injected
        Assert.That(c.Resolve<IFactory<Element>>().Get().Type, Is.EqualTo(2)); // no name = type 2 (because primary)
        Assert.That(c.Resolve<IFactory<Element>>().Get().WasInjected, Is.True);// no name = type 2 (because primary)
        Assert.That(c.Resolve<IFactory<Element>>(), Is.TypeOf<ProviderTransientFactory<Element>>());
    }
}