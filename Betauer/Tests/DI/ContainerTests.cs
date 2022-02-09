using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.Tests.DI {
    public interface IInterface1 {
    }

    public interface IInterface2 : IInterface2_2 {
    }

    public interface IInterface2_2 {
    }

    public abstract class AbstractClass : IInterface1 {
    }

    public class ClassWith1Interface : IInterface1 {
    }

    public class ClassWith2Interfaces : IInterface1, IInterface2 {
    }

    [TestFixture]
    public class ContainerTests : Node {
        [Test(Description = "Container in container, assert Contains and TryGetProvider")]
        public void Container() {
            var di = new Container(this);
            Assert.That(di.Resolve<Container>(), Is.EqualTo(di));
            Assert.That(di.Contains<Container>());
            Assert.That(di.Contains<Container>(Lifetime.Singleton));
            Assert.That(!di.Contains<Container>(Lifetime.Transient));
            Assert.That(di.Resolve(typeof(Container)), Is.EqualTo(di));

            Assert.That(di.GetProvider<Container>().Get(new ResolveContext(di)), Is.EqualTo(di));
            Assert.That(di.TryGetProvider<Container>(out var provider));
            Assert.That(!di.TryGetProvider<Node>(out var notFound));
            Assert.That(provider!.Get(new ResolveContext(di)), Is.EqualTo(di));
        }

        [Test(Description = "Types not found")]
        public void NotFound() {
            var di = new Container(this);

            // Not found types fail
            try {
                di.Resolve<IInterface1>();
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            try {
                di.Resolve(typeof(IInterface1));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
        }

        [Test(Description = "Types not found -> create transient automatically")]
        public void CreateTransientIfNotFound() {
            var di = new Container(this);
            di.CreateIfNotFound = true;

            var n1 = di.Resolve<Node>();
            var n2 = di.Resolve<Node>();
            Assert.That(n1, Is.Not.Null);
            Assert.That(n1, Is.Not.EqualTo(n2));

            // Not allowed interfaces
            try {
                di.Resolve<IInterface1>();
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
            }

            // Not allowed abstract classes
            try {
                di.Resolve(typeof(AbstractClass));
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
            }
        }

        /*
         * Singleton instances
         */
        [Test(Description = "Register singleton instances ignores IDisposable")]
        public void RegisterSingletonInstanceIgnoreIDisposable() {
            var di = new ContainerBuilder(this);
            IProvider s = null;

            // Implicit ignore IDisposable (Node implements it)
            var node = new Node();
            // Ensure IDisposable is ignored. Assert that Node implements IDisposable
            Assert.That(typeof(Node).GetInterfaces(), Contains.Item(typeof(IDisposable)));

            s = di.Static(node).CreateProvider();
            var c = di.Build();
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(!c.Contains<IDisposable>());

            // Explicit ignore IDisposable (Node implements it)
            di = new ContainerBuilder(this);
            var mySingleton = new ClassWith2Interfaces();
            s = di.Static(mySingleton).AsAll<IDisposable>().As<IDisposable>().CreateProvider();
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(!c.Contains<IDisposable>());
        }

        [Test(Description = "Register singleton instances")]
        public void RegisterSingletonInstanceServiceAllTypes() {
            var di = new ContainerBuilder(this);
            Container c;
            IProvider s = null;

            // Class with no interfaces
            var node = new Node();
            // Ensure IDisposable is ignored. Assert that Node implements IDisposable
            s = di.Static(node).CreateProvider();
            c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(Node)));
            Assert.That(c.Resolve<Node>(), Is.EqualTo(node));

            // Instances of the same Type can be overriden
            var instance2 = new Node();
            s = di.Static(instance2).CreateProvider();
            c = di.Build();
            Assert.That(c.Resolve<Node>(), Is.EqualTo(instance2));
            Assert.That(c.Resolve(typeof(Node)), Is.EqualTo(instance2));

            // Class with type and all nested interfaces by default
            di = new ContainerBuilder(this);
            var classWith2Interfaces = new ClassWith2Interfaces();
            s = di.Static(classWith2Interfaces).CreateProvider();
            c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<ClassWith2Interfaces>(), Is.EqualTo(classWith2Interfaces));
            Assert.That(!c.Contains<IInterface1>());
            Assert.That(!c.Contains<IInterface2>());
            Assert.That(!c.Contains<IInterface2_2>());

            // Class with only one interfaces only. Try to resolve using the class will not work
            // IDisposable
            di = new ContainerBuilder(this);
            var mySingleton = new ClassWith2Interfaces();
            s = di.Static<IInterface2>(mySingleton).As<IDisposable>().CreateProvider();
            c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(c.Resolve<IInterface2>(), Is.EqualTo(mySingleton));
            Assert.That(!c.Contains<ClassWith2Interfaces>());
            Assert.That(!c.Contains<IInterface1>());
        }

        /*
         * Factories
         */
        [Test(Description = "Register factory using As<Type> register only <Type>")]
        public void RegisterTypeByDefault() {
            var di = new ContainerBuilder(this);

            // Use As<T>() by default
            var s = di.Register(() => new ClassWith1Interface()).CreateProvider();
            var c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith1Interface)));
            Assert.That(c.Resolve<ClassWith1Interface>().GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(!c.Contains<IInterface1>());
        }

        [Test(Description = "Register factory uses As<I> where I is different thant Type register only <I>")]
        public void RegisterTypeByDefaultWithInterface() {
            var di = new ContainerBuilder(this);
            var s = di.Register<IInterface1>(() => new ClassWith1Interface()).CreateProvider();
            var c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(c.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(!c.Contains<ClassWith1Interface>());
        }

        [Test(Description = "Register factory using AsAll<class> + IsTransient")]
        public void RegisterTypedTypedFactoryWithInterfaces() {
            var di = new ContainerBuilder(this);

            // AsAll with the class uses the Type and all the interfaces
            di = new ContainerBuilder(this);
            var s = di.Register(() => new ClassWith2Interfaces()).IsTransient().AsAll<ClassWith2Interfaces>().CreateProvider();
            var c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(4));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(c.Resolve(typeof(ClassWith2Interfaces)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve(typeof(IInterface1)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve(typeof(IInterface2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve(typeof(IInterface2_2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<IInterface2_2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2_2)));
        }

        [Test(Description = "Register a factory AsAll<interface>")]
        public void RegisterSingleTypedFactory() {
            var di = new ContainerBuilder(this);
            var s = di.Register<IInterface2>(() => new ClassWith2Interfaces()).AsAll<IInterface2>().CreateProvider();
            var c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton)); // singleton by default
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(2));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(c.Resolve(typeof(IInterface2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve(typeof(IInterface2_2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<IInterface2_2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!c.Contains<ClassWith2Interfaces>());
            Assert.That(!c.Contains<IInterface1>());
        }

        [Test(Description = "Register a factory using type from Func<T>")]
        public void RegisterTypedTypedFactoryWithInterfaces2() {
            var di = new ContainerBuilder(this);
            var s = di.Register(() => (IInterface1)new ClassWith1Interface()).CreateProvider();
            var c = di.Build();
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(c.Resolve(typeof(IInterface1)).GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(c.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(!c.Contains<ClassWith1Interface>());
        }

        [Test(Description = "Register a factory using a smaller type from Func<T>")]
        public void RegisterSubclassFactory() {
            var di = new ContainerBuilder(this);
            var s = di.Register<IInterface2>(() => new ClassWith2Interfaces()).CreateProvider();
            var c = di.Build();
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(c.Resolve(typeof(IInterface2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!c.Contains<ClassWith2Interfaces>());
            Assert.That(!c.Contains<IInterface1>());
            Assert.That(!c.Contains<IInterface2_2>());

            di = new ContainerBuilder(this);
            s = di.Register(() => new Sprite()).As<CanvasItem>().CreateProvider();
            c = di.Build();
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(CanvasItem)));
            Assert.That(c.Resolve<CanvasItem>().GetType(), Is.EqualTo(typeof(Sprite)));
            Assert.That(!c.Contains<Sprite>());
            Assert.That(!c.Contains<Node>());
        }

        [Test(Description = "Factories lifestyle")]
        public void RegisterFactory() {
            var di = new ContainerBuilder(this);
            IProvider s = null;

            // by typed factory
            s = di.Register(() => new Control()).IsTransient().CreateProvider();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(Control)));

            s = di.Register(() => new Control()).IsSingleton().CreateProvider();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(Control)));

            // by object typed factory + type
            s = di.Register<CanvasItem>(Lifetime.Transient).With(() => new Control()).CreateProvider();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(CanvasItem)));

            s = di.Register<CanvasItem>(Lifetime.Singleton).With(() => new Control()).CreateProvider();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(CanvasItem)));
        }

        [Test(Description = "Register factory with wrong type fails")]
        public void RegisterWrong() {
            var di = new ContainerBuilder(this);
            // Can't bind a subclass creating an upper class
            try {
                di.Register(() => new Node2D()).As<Sprite>().CreateProvider();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }

            // ClassWith2Interfaces doesn't implement IEnumerable
            try {
                di.Register(() => new ClassWith2Interfaces()).AsAll<IEnumerable>().CreateProvider();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }
        }

        /*
         * Auto factories
         */
        [Test(Description = "Auto factories with Type")]
        public void RegisterAutoFactory() {
            var di = new ContainerBuilder(this);
            IProvider s = null;

            // auto factory
            s = di.Register<ClassWith2Interfaces>().CreateProvider();
            var c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(c.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!c.Contains<IInterface1>());
            Assert.That(!c.Contains<IInterface2>());
            Assert.That(!c.Contains<IInterface2_2>());

            // auto factory
            di = new ContainerBuilder(this);
            s = di.Register<IInterface1, ClassWith2Interfaces>(Lifetime.Singleton).CreateProvider();
            c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(c.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!c.Contains<ClassWith2Interfaces>());
            Assert.That(!c.Contains<IInterface2>());
            Assert.That(!c.Contains<IInterface2_2>());

            // auto factory with all classes
            di = new ContainerBuilder(this);
            s = di.Register<ClassWith2Interfaces>(Lifetime.Transient).AsAll<ClassWith2Interfaces>().CreateProvider();
            c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(4));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(c.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<IInterface2_2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
        }

        [Test(Description = "Auto factories with Type in runtime")]
        public void RegisterRuntimeAutoFactory() {
            var di = new ContainerBuilder(this);
            IProvider s = null;

            di = new ContainerBuilder(this);
            s = di.Register(typeof(ClassWith2Interfaces), Lifetime.Transient).CreateProvider();
            var c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(c.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!c.Contains<IInterface1>());
            Assert.That(!c.Contains<IInterface2>());
            Assert.That(!c.Contains<IInterface2_2>());

            // Runtime Type needs the array of type to be explicit
            di = new ContainerBuilder(this);
            s = di.Register(typeof(ClassWith2Interfaces), Lifetime.Transient,
                new[] { typeof(IInterface1), typeof(IInterface2) }).CreateProvider();
            c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(2));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(c.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!c.Contains<ClassWith2Interfaces>());
            Assert.That(!c.Contains<IInterface2_2>());

            di = new ContainerBuilder(this);
            s = di.Register(typeof(ClassWith2Interfaces),
                Lifetime.Singleton,
                typeof(IInterface1), typeof(IInterface2_2)).CreateProvider();
            c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(2));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(c.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<IInterface2_2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!c.Contains<ClassWith2Interfaces>());
            Assert.That(!c.Contains<IInterface2>());

            // Runtime Type will use the class registered type as default
            di = new ContainerBuilder(this);
            s = di.Register(typeof(ClassWith2Interfaces), Lifetime.Transient).CreateProvider();
            c = di.Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(c.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!c.Contains<IInterface1>());
            Assert.That(!c.Contains<IInterface2>());
            Assert.That(!c.Contains<IInterface2_2>());
        }

        [Test(Description = "Register auto factory with wrong type fails")]
        public void RegisterTAutoFactoryWrongType() {
            var di = new ContainerBuilder(this);

            // no interfaces
            try {
                di.Register<IInterface2_2>().CreateProvider();
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
                Assert.That(e.Message, Is.EqualTo("Can't create a default factory with interface or abstract class"));
            }
            try {
                di.Register(typeof(IInterface2_2)).CreateProvider();
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
                Assert.That(e.Message, Is.EqualTo("Can't create a default factory with interface or abstract class"));
            }

            // no abstract
            try {
                di.Register<AbstractClass>().CreateProvider();
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
                Assert.That(e.Message, Is.EqualTo("Can't create a default factory with interface or abstract class"));
            }
            try {
                di.Register(typeof(AbstractClass)).CreateProvider();
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
                Assert.That(e.Message, Is.EqualTo("Can't create a default factory with interface or abstract class"));
            }

            // Node doesn't implement IEnumerable
            try {
                di.Register<Node>().As<IEnumerable>().CreateProvider();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }

            try {
                di.Register(typeof(Node), Lifetime.Singleton, new[] { typeof(IEnumerable) }).CreateProvider();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }

            // class is in an upper level than expected
            try {
                di.Register<Node>().As<Sprite>().CreateProvider();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }

            try {
                di.Register(typeof(Node), Lifetime.Singleton, new[] { typeof(Sprite) }).CreateProvider();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }
        }

        /*
         * OnInstanceCreated is affected by factories only
         */
        [Test(Description = "Singleton instance doesn't execute OnInstanceCreated")]
        public void RegisterSingletonInstanceOnInstanceCreated() {
            var di = new ContainerBuilder(this);
            var x = 0;
            var c = di.Build();
            c.OnInstanceCreated = (o) => x++;
            var instance = new Node();
            di.Static(instance);
            Assert.That(x, Is.EqualTo(0));
            di.Build();
            c.Resolve<Node>();
            Assert.That(x, Is.EqualTo(0));
        }

        [Test(Description = "Singleton Factory OnInstanceCreated is executed on Resolve (only the first time)")]
        public void RegisterSingletonFactoryOnInstanceCreated() {
            var di = new ContainerBuilder(this);
            var x = 0;
            di.Build().OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y" + ++x);
            di.Register(() => new Node()).CreateProvider();
            Assert.That(x, Is.EqualTo(0));

            var i1 = di.Build().Resolve<Node>();
            Assert.That(x, Is.EqualTo(1));
            Assert.That(i1.GetMeta("x"), Is.EqualTo("y1"));
            Assert.That(i1.GetMeta("x"), Is.EqualTo("y1"));

            // Second time is not executed
            var i2 = di.Build().Resolve<Node>();
            Assert.That(x, Is.EqualTo(1));
            Assert.That(i2, Is.EqualTo(i1));
            Assert.That(i2.GetMeta("x"), Is.EqualTo("y1"));
        }

        [Test(Description = "Transient Factory OnInstanceCreated is executed in every resolve")]
        public void RegisterTransientFactoryOnInstanceCreated() {
            var di = new ContainerBuilder(this);
            var x = 0;
            var c = di.Build();
            c.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y" + ++x);
            di.Register<Node>(Lifetime.Transient).With(() => new Node()).CreateProvider();
            di.Register<Control>(Lifetime.Transient).With(() => new Control()).CreateProvider();
            di.Build();
            Assert.That(c.Resolve<Node>().GetMeta("x"), Is.EqualTo("y1"));
            Assert.That(c.Resolve<Control>().GetMeta("x"), Is.EqualTo("y2"));
            Assert.That(c.Resolve<Node>().GetMeta("x"), Is.EqualTo("y3"));
        }

        [Test(Description = "Resolve unregistered types the OnInstanceCreated is executed in every resolve")]
        public void ResolveUnregisteredTransientFactoryOnInstanceCreated() {
            var di = new ContainerBuilder(this);
            var x = 0;
            var c = di.Build();
            c.CreateIfNotFound = true;
            c.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y" + ++x);
            Assert.That(c.Resolve<Node>().GetMeta("x"), Is.EqualTo("y1"));
            Assert.That(c.Resolve<Node>().GetMeta("x"), Is.EqualTo("y2"));
        }

        /*
         * Node Singleton are added to the owner when resolved
         */
        [Test(Description = "Register a instance Node doesn't adds it as child when it's resolved")]
        public void RegisterNodeInstanceAddedToOwner() {
            var di = new ContainerBuilder(this);
            var instance = new Node();
            di.Static(instance).CreateProvider();
            Assert.That(!GetChildren().Contains(instance));

            di.Build().Resolve<Node>();
            Assert.That(!GetChildren().Contains(instance));
        }

        [Test(Description = "Register a instance Node doesn't add it as child on Build")]
        public void RegisterNodeInstanceAddedToOwnerOnBuild() {
            var di = new ContainerBuilder(this);
            var instance = new Node();
            di.Static(instance).CreateProvider();
            Assert.That(!GetChildren().Contains(instance));

            di.Build();
            Assert.That(!GetChildren().Contains(instance));
        }

        [Test(Description = "Register a Singleton Factory Node adds it as child when resolved")]
        public void RegisterNodeSingletonFactoryAddedToOwner() {
            var di = new ContainerBuilder(this);
            di.Register(() => new Node()).CreateProvider();

            var instance = di.Build().Resolve<Node>();
            Assert.That(GetChildren().Contains(instance));
        }

        [Test(Description = "Register a Singleton Factory Node adds it as child on build")]
        public void RegisterNodeSingletonFactoryAddedToOwnerOnBuild() {
            var di = new ContainerBuilder(this);
            Node instance = null;
            di.Register(() => {
                instance = new Node();
                return instance;
            }).CreateProvider();

            di.Build();
            Assert.That(GetChildren().Contains(instance));
        }

        [Test(Description = "Register a transient Factory Node is not added as child when resolved or on build")]
        public void RegisterNodeTransientFactoryAddedToOwner() {
            var di = new ContainerBuilder(this);
            // Register instance

            di.Register(() => new Node()).IsTransient().CreateProvider();
            di.Build();
            var instance1 = di.Build().Resolve<Node>();
            Assert.That(!GetChildren().Contains(instance1));
        }

        /**
         * Lifecycle
         */
        [Test(Description = "Register a Singleton factory is executed only the first time")]
        public void RegisterSingletonFactory() {
            var di = new ContainerBuilder(this);
            var n = 0;
            var s = di.Register(() => {
                n++;
                return new Node();
            }).CreateProvider();

            // Ensures that factory is called only the first time
            Assert.That(n, Is.EqualTo(0));
            var instance1 = di.Build().Resolve<Node>();
            Assert.That(n, Is.EqualTo(1));

            var instance2 = di.Build().Resolve<Node>();
            Assert.That(n, Is.EqualTo(1));

            // Ensure 2 instances are equal
            Assert.That(instance1, Is.EqualTo(instance2));
        }

        [Test(Description = "Register a Transient factory and is executed only the first time")]
        public void RegisterTransientFactory() {
            var di = new ContainerBuilder(this);
            var n = 0;
            var s = di.Register(() => {
                n++;
                return new Node();
            }).IsTransient().CreateProvider();

            // Ensures that factory is called only the first time
            Assert.That(n, Is.EqualTo(0));
            var instance1 = di.Build().Resolve<Node>();
            Assert.That(n, Is.EqualTo(1));

            var instance2 = di.Build().Resolve<Node>();
            Assert.That(n, Is.EqualTo(2));

            // Ensure 2 instances are different
            Assert.That(instance1, Is.Not.EqualTo(instance2));
        }

        [Test(Description = "Register a Singleton auto factory by Type is executed only the first time")]
        public void RegisterSingletonAutoFactory() {
            var di = new ContainerBuilder(this);
            var n = 0;
            var s = di.Register(typeof(Node), Lifetime.Singleton).CreateProvider();

            // Ensures that factory is called only the first time
            var instance1 = di.Build().Resolve<Node>();
            var instance2 = di.Build().Resolve<Node>();

            // Ensure 2 instances are equal
            Assert.That(instance1, Is.EqualTo(instance2));
        }

        [Test(Description = "Register a Transient auto factory by Type is executed only the first time")]
        public void RegisterTransientAutoFactory() {
            var di = new ContainerBuilder(this);
            var n = 0;
            var s = di.Register(typeof(Node), Lifetime.Transient).CreateProvider();

            // Ensures that factory is called only the first time
            var instance1 = di.Build().Resolve<Node>();
            var instance2 = di.Build().Resolve<Node>();

            // Ensure 2 instances are different
            Assert.That(instance1, Is.Not.EqualTo(instance2));
        }

        [Test(Description = "Container Build")]
        public void ContainerBuild() {
            var di = new ContainerBuilder(this);
            di.Register<Node>();
            di.Register<IInterface1, ClassWith1Interface>();
            var c = di.Build();
            Assert.That(c.Contains<Node>());
            Assert.That(c.Contains<IInterface1>());
        }


        /*
         * Special
         */

        [Test(Description = "Register a lambda as instance can be called as method")]
        public void RegisterFactoryAsInstance() {
            var di = new ContainerBuilder(this);
            var n = 0;
            di.Static<Func<int>>(() => ++n).CreateProvider();

            Func<int> resolve = di.Build().Resolve<Func<int>>();

            Assert.That(resolve(), Is.EqualTo(1));
            Assert.That(resolve(), Is.EqualTo(2));
        }
    }
}