using System;
using Betauer.DI.Attributes;
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
    [TestRunner.Test(Description = "[SingletonFactory] should implement ITransient<> (class)")]
    public void FactoryWrongType1() {
        var c = new Container();
        c.Build(di => {
            Assert.Throws<InvalidOperationException>(() => di.Scan<WrongFactory>());
        });
    }

    [TestRunner.Test(Description = "[SingletonFactory] should implement ITransient<> (configuration)")]
    public void FactoryWrongType2() {
        var c = new Container();
        c.Build(di => {
            Assert.Throws<InvalidOperationException>(() => di.Scan<WrongConfig>());
        });
    }

    [TestRunner.Test(Description = "Not implement ITransient<> fails at runtime")]
    public void FactoryWrongType3() {
        var c = new Container();
        c.Build(di => {
            // Assert.Throws<InvalidCastException>(() => di.RegisterFactoryFactory(typeof(MyService), Lifetime.Singleton, () => new object()));
        });
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
        [Inject] public ITransient<TransientD> TransientD { get; set; }
    }
    
    [Singleton]
    public class Client2 {
        [Inject] public ITransient<TransientD> TransientD { get; set; }
    }

    [TestRunner.Test(Description = "Test defining a Transient service by name with a Factory")]
    public void FactoryTransientFromConfiguration() {
        TransientD.Instances = 0;
        var c = new Container();
        c.Build(di => {
            di.Scan<TransientFactoryConfiguration>();
            di.Scan<Client1>();
            di.Scan<Client2>();
        });

        Client1 client1 = c.Resolve<Client1>();

        Assert.That(TransientD.Instances, Is.EqualTo(0));
        Assert.That(client1.TransientD.Create(), Is.TypeOf<TransientD>());
        Assert.That(client1.TransientD.Create(), Is.Not.EqualTo(client1.TransientD.Create()));
        Assert.That(TransientD.Instances, Is.EqualTo(3));

        Client2 client2 = c.Resolve<Client2>();

        Assert.That(TransientD.Instances, Is.EqualTo(3));
        Assert.That(client2.TransientD.Create(), Is.TypeOf<TransientD>());
        Assert.That(client2.TransientD.Create(), Is.Not.EqualTo(client2.TransientD.Create()));
        Assert.That(TransientD.Instances, Is.EqualTo(6));
    }
    [TestRunner.Test(Description = "Register a Singleton Factory by type")]
    public void RegisterSingletonAndAddFactoryTest() {
        var c = new Container();
        var x = 0;
        c.Build(di => {
            var provider = Provider.Singleton<Node, Node>(() => {
                x++;
                return new Node { Name = "L" };
            }, null, false);
            di.Register(provider);
            di.Register(Provider.Proxy(provider));
        });
        Assert.That(x, Is.EqualTo(1));
        Assert.That(c.Resolve<Node>().Name.ToString(), Is.EqualTo("L"));
        Assert.That(c.Resolve<ILazy<Node>>().Get(), Is.EqualTo(c.Resolve<Node>()));
        Assert.That(x, Is.EqualTo(1));
    }

    [TestRunner.Test(Description = "Register a Singleton Factory by type lazy")]
    public void RegisterSingletonAndAddFactoryLazyTest() {
        var c = new Container();
        var x = 0;
        c.Build(di => {
            var provider = Provider.Singleton<Node, Node>(() => {
                x++;
                return new Node { Name = "L" };
            }, null, true);
            di.Register(provider);
            di.Register(Provider.Proxy(provider));
        });
        Assert.That(x, Is.EqualTo(0));
        Assert.That(c.Resolve<Node>().Name.ToString(), Is.EqualTo("L"));
        Assert.That(c.Resolve<ILazy<Node>>().Get(), Is.EqualTo(c.Resolve<Node>()));
        Assert.That(x, Is.EqualTo(1));
    }

    
    
    [TestRunner.Test(Description = "Register a Singleton Factory by name")]
    public void RegisterSingletonAndAddFactoryByNameTest() {
        var x = 0;
        var c = new Container();
        c.Build(di => {
            var provider = Provider.Singleton<Node, Node>(() => {
                x++;
                return new Node { Name = "L" };
            }, "X", false);
            di.Register(provider);
            di.Register(Provider.Proxy(provider));
        });
        Assert.That(x, Is.EqualTo(1));
        Assert.That(c.Resolve<Node>("X").Name.ToString(), Is.EqualTo("L"));
        Assert.That(c.Resolve<ILazy<Node>>("Factory:X").Get(), Is.EqualTo(c.Resolve<Node>("X")));
        Assert.That(x, Is.EqualTo(1));
    }

    [TestRunner.Test(Description = "Register a Singleton Factory by name lazy")]
    public void RegisterSingletonAndAddFactoryByNameLazyTest() {
        var x = 0;
        var c = new Container();
        c.Build(di => {
            var provider = Provider.Singleton<Node, Node>(() => {
                x++;
                return new Node { Name = "L" };
            }, "X", true);
            di.Register(provider);
            di.Register(Provider.Proxy(provider));
        });
        Assert.That(x, Is.EqualTo(0));
        Assert.That(c.Resolve<Node>("X").Name.ToString(), Is.EqualTo("L"));
        Assert.That(c.Resolve<ILazy<Node>>("Factory:X").Get(), Is.EqualTo(c.Resolve<Node>("X")));
        Assert.That(x, Is.EqualTo(1));
    }

    
    
    
    
    
    [TestRunner.Test(Description = "Register a Transient Factory by type")]
    public void RegisterTransientAndAddFactoryTest() {
        var x = 0;
        var c = new Container();
        c.Build(di => {
            var provider = Provider.Transient<Node, Node>(() => {
                x++;
                return new Node { Name = "L" };
            });
            di.Register(provider);
            di.Register(Provider.Proxy(provider));
        });
        Assert.That(x, Is.EqualTo(0));
        Assert.That(c.Resolve<Node>().Name.ToString(), Is.EqualTo("L"));
        var factory = c.Resolve<ITransient<Node>>();
        Assert.That(factory.Create(), Is.Not.EqualTo(factory.Create()));
        Assert.That(x, Is.EqualTo(3));
    }

    [TestRunner.Test(Description = "Register a Transient Factory by name")]
    public void RegisterTransientAndAddFactoryByNameTest() {
        var x = 0;
        var c = new Container();
        c.Build(di => {
            var provider = Provider.Transient<Node, Node>(() => {
                x++;
                return new Node { Name = "L" };
            }, "X");
            di.Register(provider);
            di.Register(Provider.Proxy(provider));
        });
        Assert.That(x, Is.EqualTo(0));
        Assert.That(c.Resolve<Node>("X").Name.ToString(), Is.EqualTo("L"));
        var factory = c.Resolve<ITransient<Node>>("Factory:X");
        Assert.That(factory.Create(), Is.Not.EqualTo(factory.Create()));
        Assert.That(x, Is.EqualTo(3));
    }

    [Configuration]
    public class ServiceFactoryConfiguration {
        [Attributes.Factory.Singleton] public MyServiceFactory MyService => new MyServiceFactory();
        [Attributes.Factory.Transient] public MyTransientFactory MyTransient => new MyTransientFactory();
    }

    public class MyServiceFactory : IFactory<MyService>, IInjectable {
        public bool WasInjected = false;
        public static int Instances = 0;
        public static int Gets = 0;

        public MyServiceFactory() {
            Instances++;
        }

        public MyService Create() {
            Assert.That(WasInjected, Is.True);
            Gets++;
            return new MyService();
        }

        public void PostInject() {
            WasInjected = true;
        }

        public static void Reset() {
            Instances = 0;
            Gets = 0;
        }
    }
    
    public class MyTransientFactory : IFactory<MyTransient>, IInjectable {
        public bool WasInjected = false;
        public static int Instances = 0;
        public static int Gets = 0;

        public MyTransientFactory() {
            Instances++;
        }

        public MyTransient Create() {
            Assert.That(WasInjected, Is.True);
            Gets++;
            return new MyTransient();
        }

        public void PostInject() {
            WasInjected = true;
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
        [Inject("MyService")] public MyService MyServiceName { get; set;  }
        [Inject] public MyService MyService { get; set;  }
        [Inject] public ITransient<MyTransient> MyTransient { get; set;  }
    }

    [Transient]
    public class DemoTransient {
        [Inject] public MyService MyService { get; set;  }
        [Inject] public MyTransient MyTransient { get; set;  }
    }

    [TestRunner.Test(Description = "Custom service ITransient (in configuration), inject by name and type")]
    public void ServiceFactoryTests() {
        MyServiceFactory.Reset();
        MyTransientFactory.Reset();
        var c = new Container();
        c.Build(di => {
            di.Scan<ServiceFactoryConfiguration>();
            di.Scan<DemoSingleton>();
            di.Scan<DemoTransient>();
        });

        Assert.That(MyServiceFactory.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(1)); // No lazy, it's created on build

        Assert.That(MyTransientFactory.Instances, Is.EqualTo(1));
        Assert.That(MyTransientFactory.Gets, Is.EqualTo(0));

        var demoSingleton = c.Resolve<DemoSingleton>();
        Assert.That(demoSingleton.MyServiceName, Is.EqualTo(demoSingleton.MyService));

        var d = c.Resolve<DemoTransient>();

        var x1 = demoSingleton.MyService;
        var x3 = c.Resolve<MyService>("MyService");

        Assert.That(x1.WasInjected, Is.True);
        Assert.That(x3.WasInjected, Is.True);
        Assert.That(d.MyService.WasInjected, Is.True);
        
        var t1 = demoSingleton.MyTransient.Create();
        var t3 = c.Resolve<MyTransient>("MyTransient");

        Assert.That(t1.WasInjected, Is.True);
        Assert.That(t3.WasInjected, Is.True);
        Assert.That(d.MyTransient.WasInjected, Is.True);
        
        Assert.That(MyServiceFactory.Instances, Is.EqualTo(1));
        Assert.That(MyServiceFactory.Gets, Is.EqualTo(1));
        Assert.That(MyTransientFactory.Instances, Is.EqualTo(1));
        Assert.That(MyTransientFactory.Gets, Is.EqualTo(3));
    }

    [Attributes.Factory.Singleton(Name="MyService", Lazy = true)]
    public class MyLazyServiceFactoryClass : IFactory<MyService>, IInjectable {
        public bool WasInjected = false;
        public static int Instances = 0;
        public static int Gets = 0;

        public MyLazyServiceFactoryClass() {
            Instances++;
        }

        public MyService Create() {
            Assert.That(WasInjected, Is.True);
            Gets++;
            return new MyService();
        }

        public void PostInject() {
            WasInjected = true;
        }

        public static void Reset() {
            Instances = 0;
            Gets = 0;
        }
    }

    [Attributes.Factory.Transient(Name="MyTransient")]
    public class MyTransientFactoryClass : IFactory<MyTransient>, IInjectable {
        public bool WasInjected = false;
        public static int Instances = 0;
        public static int Gets = 0;

        public MyTransientFactoryClass() {
            Instances++;
        }

        public MyTransient Create() {
            Assert.That(WasInjected, Is.True);
            Gets++;
            return new MyTransient();
        }

        public void PostInject() {
            WasInjected = true;
        }

        public static void Reset() {
            Instances = 0;
            Gets = 0;
        }
    }

    [Singleton]
    public class DemoSingleton2 {
        [Inject("MyService")] public ILazy<MyService> MyServiceName { get; set;  }
        [Inject("Factory:MyService")] public ILazy<MyService> MyServiceFullName { get; set;  }
        [Inject] public ILazy<MyService> MyService { get; set;  }
        [Inject] public ITransient<MyTransient> MyTransient { get; set;  }
    }

    [Transient]
    public class DemoTransient2 {
        [Inject] public MyService MyService { get; set;  }
        [Inject] public MyTransient MyTransient { get; set;  }
    }

    [TestRunner.Test(Description = "Custom service ITransient (in class), inject by name and type")]
    public void ServiceFactoryClassTests() {
        MyLazyServiceFactoryClass.Reset();
        MyTransientFactoryClass.Reset();
        var c = new Container();
        c.Build(di => {
            di.Scan<MyLazyServiceFactoryClass>();
            di.Scan<MyTransientFactoryClass>();
            di.Scan<DemoSingleton2>();
            di.Scan<DemoTransient2>();
        });

        Assert.That(MyLazyServiceFactoryClass.Instances, Is.EqualTo(1));
        Assert.That(MyLazyServiceFactoryClass.Gets, Is.EqualTo(0));

        Assert.That(MyTransientFactoryClass.Instances, Is.EqualTo(1));
        Assert.That(MyTransientFactoryClass.Gets, Is.EqualTo(0));

        var demoSingleton = c.Resolve<DemoSingleton2>();
        Assert.That(demoSingleton.MyServiceName, Is.EqualTo(c.Resolve<ILazy<MyService>>("Factory:MyService")));
        Assert.That(demoSingleton.MyServiceFullName, Is.EqualTo(c.Resolve<ILazy<MyService>>("Factory:MyService")));
        Assert.That(demoSingleton.MyService, Is.EqualTo(c.Resolve<ILazy<MyService>>("Factory:MyService")));

        var d = c.Resolve<DemoTransient2>();

        var x1 = demoSingleton.MyService.Get();
        var x3 = c.Resolve<MyService>("MyService");

        Assert.That(x1.WasInjected, Is.True);
        Assert.That(x3.WasInjected, Is.True);
        Assert.That(d.MyService.WasInjected, Is.True);
        
        var t1 = demoSingleton.MyTransient.Create();
        var t3 = c.Resolve<MyTransient>("MyTransient");

        Assert.That(t1.WasInjected, Is.True);
        Assert.That(t3.WasInjected, Is.True);
        Assert.That(d.MyTransient.WasInjected, Is.True);
        
        Assert.That(MyLazyServiceFactoryClass.Instances, Is.EqualTo(1));
        Assert.That(MyLazyServiceFactoryClass.Gets, Is.EqualTo(1));
        Assert.That(MyTransientFactoryClass.Instances, Is.EqualTo(1));
        Assert.That(MyTransientFactoryClass.Gets, Is.EqualTo(3));
    }
   
    public class Element1 : Node {
    }

    public class Element2 : Node2D {
    }

    public class Element3 : Node3D {
    }

    public class Element4 : Control {
    }

    [Attributes.Factory.Singleton<Node>(Lazy = true)]
    public class Element1Factory : IFactory<Element1> {
        public Element1 Create() => new Element1();
    }
    
    [Attributes.Factory.Transient<Node2D>]
    public class Element2Factory : IFactory<Element2> {
        public Element2 Create() => new Element2();
    }
    public class Element3Factory : IFactory<Element3> {
        public Element3 Create() => new Element3();
    }
    public class Element4Factory : IFactory<Element4> {
        public Element4 Create() => new Element4();
    }

    [Configuration]
    public class Config {
        [Attributes.Factory.Singleton<Node>(Lazy = true)] IFactory<Node3D> Element3 => new Element3Factory();
        [Attributes.Factory.Transient] IFactory<Control> Element4 => new Element4Factory();
    }

    [TestRunner.Test]
    public void ExposeFactoriesWithDifferentTypeTest() {
        var c = new Container();
        c.Build(di => {
            di.Scan<Config>();
            di.Scan<Element1Factory>();
            di.Scan<Element2Factory>();
        });
        // Singleton
        Assert.That(c.Resolve<Node>(), Is.EqualTo(c.Resolve<Node>()));
        Assert.That(c.Resolve<Node>(), Is.TypeOf<Element1>());
        Assert.That(c.Resolve<ILazy<Node>>().Get(), Is.EqualTo(c.Resolve<Node>()));

        // Transient
        Assert.That(c.Resolve<Node2D>(), Is.Not.EqualTo(c.Resolve<Node2D>()));
        Assert.That(c.Resolve<Node2D>(), Is.TypeOf<Element2>());
        Assert.That(c.Resolve<ITransient<Node2D>>().Create(), Is.TypeOf<Element2>());
        Assert.That(c.Resolve<ITransient<Node2D>>().Create(), Is.Not.EqualTo(c.Resolve<Node2D>()));

        // Singleton
        Assert.That(c.Resolve<Node3D>("Element3"), Is.EqualTo(c.Resolve<Node3D>("Element3")));
        Assert.That(c.Resolve<Node3D>("Element3"), Is.TypeOf<Element3>());
        Assert.That(c.Resolve<ILazy<Node3D>>("Factory:Element3").Get(), Is.EqualTo(c.Resolve<Node3D>("Element3")));

        // Transient
        Assert.That(c.Resolve<Control>("Element4"), Is.Not.EqualTo(c.Resolve<Control>("Element4")));
        Assert.That(c.Resolve<Control>("Element4"), Is.TypeOf<Element4>());
        Assert.That(c.Resolve<ITransient<Control>>("Factory:Element4").Create(), Is.Not.EqualTo(c.Resolve<Control>("Element4")));


    }
    
}