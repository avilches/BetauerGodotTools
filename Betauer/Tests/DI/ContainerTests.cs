using System;
using System.Collections;
using System.Collections.Generic;
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
            var di = new Container(this);
            IProvider s = null;

            // Implicit ignore IDisposable (Node implements it)
            var node = new Node();
            // Ensure IDisposable is ignored. Assert that Node implements IDisposable
            Assert.That(typeof(Node).GetInterfaces(), Contains.Item(typeof(IDisposable)));

            s = di.Instance(node).Build();
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(!di.Exist<IDisposable>());

            // Explicit ignore IDisposable (Node implements it)
            di = new Container(this);
            var mySingleton = new ClassWith2Interfaces();
            s = di.Instance(mySingleton).AsAll<IDisposable>().As<IDisposable>().Build();
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(!di.Exist<IDisposable>());
        }

        [Test(Description = "Register singleton instances")]
        public void RegisterSingletonInstanceServiceAllTypes() {
            var di = new Container(this);
            IProvider s = null;

            // Class with no interfaces
            var node = new Node();
            // Ensure IDisposable is ignored. Assert that Node implements IDisposable
            s = di.Instance(node).Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(Node)));
            Assert.That(di.Resolve<Node>(), Is.EqualTo(node));

            // Instances of the same Type can be overriden
            var instance2 = new Node();
            s = di.Instance(instance2).Build();
            Assert.That(di.Resolve<Node>(), Is.EqualTo(instance2));
            Assert.That(di.Resolve(typeof(Node)), Is.EqualTo(instance2));

            // Class with type and all nested interfaces by default
            di = new Container(this);
            var classWith2Interfaces = new ClassWith2Interfaces();
            s = di.Instance(classWith2Interfaces).Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<ClassWith2Interfaces>(), Is.EqualTo(classWith2Interfaces));
            Assert.That(!di.Exist<IInterface1>());
            Assert.That(!di.Exist<IInterface2>());
            Assert.That(!di.Exist<IInterface2_2>());

            // Class with only one interfaces only. Try to resolve using the class will not work
            // IDisposable
            di = new Container(this);
            var mySingleton = new ClassWith2Interfaces();
            s = di.Instance<IInterface2>(mySingleton).As<IDisposable>().Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(di.Resolve<IInterface2>(), Is.EqualTo(mySingleton));
            Assert.That(!di.Exist<ClassWith2Interfaces>());
            Assert.That(!di.Exist<IInterface1>());
        }

        /*
         * Factories
         */
        [Test(Description = "Register factory using As<Type> register only <Type>")]
        public void RegisterTypeByDefault() {
            var di = new Container(this);

            // Use As<T>() by default
            var s = di.Register(() => new ClassWith1Interface()).Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith1Interface)));
            Assert.That(di.Resolve<ClassWith1Interface>().GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(!di.Exist<IInterface1>());
        }

        [Test(Description = "Register factory uses As<I> where I is different thant Type register only <I>")]
        public void RegisterTypeByDefaultWithInterface() {
            var di = new Container(this);
            var s = di.Register<IInterface1>(() => new ClassWith1Interface()).Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(di.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(!di.Exist<ClassWith1Interface>());
        }

        [Test(Description = "Register factory using AsAll<class> + IsTransient")]
        public void RegisterTypedTypedFactoryWithInterfaces() {
            var di = new Container(this);

            // AsAll with the class uses the Type and all the interfaces
            di = new Container(this);
            var s = di.Register(() => new ClassWith2Interfaces()).IsTransient().AsAll<ClassWith2Interfaces>().Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(4));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(di.Resolve(typeof(ClassWith2Interfaces)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface1)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface2_2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2_2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2_2)));
        }

        [Test(Description = "Register a factory AsAll<interface>")]
        public void RegisterSingleTypedFactory() {
            var di = new Container(this);
            var s = di.Register<IInterface2>(() => new ClassWith2Interfaces()).AsAll<IInterface2>().Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton)); // singleton by default
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(2));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(di.Resolve(typeof(IInterface2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface2_2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2_2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!di.Exist<ClassWith2Interfaces>());
            Assert.That(!di.Exist<IInterface1>());
        }

        [Test(Description = "Register a factory using type from Func<T>")]
        public void RegisterTypedTypedFactoryWithInterfaces2() {
            var di = new Container(this);
            var s = di.Register(() => (IInterface1)new ClassWith1Interface()).Build();
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(di.Resolve(typeof(IInterface1)).GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(di.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(!di.Exist<ClassWith1Interface>());
        }

        [Test(Description = "Register a factory using a smaller type from Func<T>")]
        public void RegisterSubclassFactory() {
            var di = new Container(this);
            var s = di.Register<IInterface2>(() => new ClassWith2Interfaces()).Build();
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(di.Resolve(typeof(IInterface2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!di.Exist<ClassWith2Interfaces>());
            Assert.That(!di.Exist<IInterface1>());
            Assert.That(!di.Exist<IInterface2_2>());

            di = new Container(this);
            s = di.Register(() => new Sprite()).As<CanvasItem>().Build();
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(CanvasItem)));
            Assert.That(di.Resolve<CanvasItem>().GetType(), Is.EqualTo(typeof(Sprite)));
            Assert.That(!di.Exist<Sprite>());
            Assert.That(!di.Exist<Node>());
        }

        [Test(Description = "Factories lifestyle")]
        public void RegisterFactory() {
            var di = new Container(this);
            IProvider s = null;

            // by typed factory
            s = di.Register(() => new Control()).IsTransient().Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(Control)));

            s = di.Register(() => new Control()).IsSingleton().Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(Control)));

            // by object typed factory + type
            s = di.Register<CanvasItem>(Lifetime.Transient).With(() => new Control()).Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(CanvasItem)));

            s = di.Register<CanvasItem>(Lifetime.Singleton).With(() => new Control()).Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(CanvasItem)));
        }

        [Test(Description = "Register factory with wrong type fails")]
        public void RegisterWrong() {
            var di = new Container(this);
            // Can't bind a subclass creating an upper class
            try {
                di.Register(() => new Node2D()).As<Sprite>().Build();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }

            // ClassWith2Interfaces doesn't implement IEnumerable
            try {
                di.Register(() => new ClassWith2Interfaces()).AsAll<IEnumerable>().Build();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }
        }

        /*
         * Auto factories
         */
        [Test(Description = "Auto factories with Type")]
        public void RegisterAutoFactory() {
            var di = new Container(this);
            IProvider s = null;

            // auto factory
            s = di.Register<ClassWith2Interfaces>().Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(di.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!di.Exist<IInterface1>());
            Assert.That(!di.Exist<IInterface2>());
            Assert.That(!di.Exist<IInterface2_2>());

            // auto factory
            di = new Container(this);
            s = di.Register<IInterface1, ClassWith2Interfaces>(Lifetime.Singleton).Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(di.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!di.Exist<ClassWith2Interfaces>());
            Assert.That(!di.Exist<IInterface2>());
            Assert.That(!di.Exist<IInterface2_2>());

            // auto factory with all classes
            di = new Container(this);
            s = di.Register<ClassWith2Interfaces>(Lifetime.Transient).AsAll<ClassWith2Interfaces>().Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(4));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(di.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2_2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
        }

        [Test(Description = "Auto factories with Type in runtime")]
        public void RegisterRuntimeAutoFactory() {
            var di = new Container(this);
            IProvider s = null;

            di = new Container(this);
            s = di.Register(typeof(ClassWith2Interfaces), Lifetime.Transient).Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(di.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!di.Exist<IInterface1>());
            Assert.That(!di.Exist<IInterface2>());
            Assert.That(!di.Exist<IInterface2_2>());

            // Runtime Type needs the array of type to be explicit
            di = new Container(this);
            s = di.Register(typeof(ClassWith2Interfaces), Lifetime.Transient,
                new[] { typeof(IInterface1), typeof(IInterface2) }).Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(2));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(di.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!di.Exist<ClassWith2Interfaces>());
            Assert.That(!di.Exist<IInterface2_2>());

            di = new Container(this);
            s = di.Register(typeof(ClassWith2Interfaces),
                Lifetime.Singleton,
                typeof(IInterface1), typeof(IInterface2_2)).Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(2));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface1)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(di.Resolve<IInterface1>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2_2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!di.Exist<ClassWith2Interfaces>());
            Assert.That(!di.Exist<IInterface2>());

            // Runtime Type will use the class registered type as default
            di = new Container(this);
            s = di.Register(typeof(ClassWith2Interfaces), Lifetime.Transient).Build();
            Assert.That(s.GetLifetime(), Is.EqualTo(Lifetime.Transient));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!di.Exist<IInterface1>());
            Assert.That(!di.Exist<IInterface2>());
            Assert.That(!di.Exist<IInterface2_2>());
        }

        [Test(Description = "Register auto factory with wrong type fails")]
        public void RegisterTAutoFactoryWrongType() {
            var di = new Container(this);

            // no interfaces
            try {
                di.Register<IInterface2_2>().Build();
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
                Assert.That(e.Message, Is.EqualTo("Can't create a default factory with interface or abstract class"));
            }
            try {
                di.Register(typeof(IInterface2_2)).Build();
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
                Assert.That(e.Message, Is.EqualTo("Can't create a default factory with interface or abstract class"));
            }

            // no abstract
            try {
                di.Register<AbstractClass>().Build();
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
                Assert.That(e.Message, Is.EqualTo("Can't create a default factory with interface or abstract class"));
            }
            try {
                di.Register(typeof(AbstractClass)).Build();
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
                Assert.That(e.Message, Is.EqualTo("Can't create a default factory with interface or abstract class"));
            }

            // Node doesn't implement IEnumerable
            try {
                di.Register<Node>().As<IEnumerable>().Build();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }

            try {
                di.Register(typeof(Node), Lifetime.Singleton, new[] { typeof(IEnumerable) }).Build();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }

            // class is in an upper level than expected
            try {
                di.Register<Node>().As<Sprite>().Build();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }

            try {
                di.Register(typeof(Node), Lifetime.Singleton, new[] { typeof(Sprite) }).Build();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }
        }

        /*
         * OnInstanceCreated
         */
        [Test(Description = "Singleton instance OnInstanceCreated is executed on Resolve (only the first time)")]
        public void RegisterSingletonInstanceOnInstanceCreated() {
            var di = new Container(this);
            var x = 0;
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y" + ++x);
            var instance = new Node();
            di.Instance(instance).Build();

            Assert.That(x, Is.EqualTo(0));
            Assert.That(instance.GetMeta("x"), Is.Null);

            var i1 = di.Resolve<Node>();
            Assert.That(i1, Is.EqualTo(instance));
            Assert.That(i1.GetMeta("x"), Is.EqualTo("y1"));

            // Second time is not executed
            var i2 = di.Resolve<Node>();
            Assert.That(i2, Is.EqualTo(instance));
            Assert.That(i2.GetMeta("x"), Is.EqualTo("y1"));
        }

        [Test(Description = "Singleton Factory OnInstanceCreated is executed on Resolve (only the first time)")]
        public void RegisterSingletonFactoryOnInstanceCreated() {
            var di = new Container(this);
            var x = 0;
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y" + ++x);
            di.Register(() => new Node()).Build();
            Assert.That(x, Is.EqualTo(0));

            var i1 = di.Resolve<Node>();
            Assert.That(x, Is.EqualTo(1));
            Assert.That(i1.GetMeta("x"), Is.EqualTo("y1"));
            Assert.That(i1.GetMeta("x"), Is.EqualTo("y1"));

            // Second time is not executed
            var i2 = di.Resolve<Node>();
            Assert.That(x, Is.EqualTo(1));
            Assert.That(i2, Is.EqualTo(i1));
            Assert.That(i2.GetMeta("x"), Is.EqualTo("y1"));
        }

        [Test(Description = "Transient Factory OnInstanceCreated is executed in every resolve")]
        public void RegisterTransientFactoryOnInstanceCreated() {
            var di = new Container(this);
            var x = 0;
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y" + ++x);
            di.Register<Node>(Lifetime.Transient).With(() => new Node()).Build();
            di.Register<Control>(Lifetime.Transient).With(() => new Control()).Build();
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y1"));
            Assert.That(di.Resolve<Control>().GetMeta("x"), Is.EqualTo("y2"));
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y3"));
        }

        /*
         * Node Singleton are added to the owner when resolved
         */
        [Test(Description = "Register a instance Node adds it as child when it's resolved")]
        public void RegisterNodeInstance() {
            var di = new Container(this);
            // Register instance
            var instance = new Node();
            di.Instance(instance).Build();
            Assert.That(!GetChildren().Contains(instance));

            di.Resolve<Node>();
            Assert.That(GetChildren().Contains(instance));
        }

        [Test(Description = "Register a Singleton Factory Node adds it as child when resolved")]
        public void RegisterNodeSingletonFactory() {
            var di = new Container(this);
            // Register instance
            di.Register(() => new Node()).Build();

            var instance = di.Resolve<Node>();
            Assert.That(GetChildren().Contains(instance));
        }

        [Test(Description = "Register a transient Factory Node adds it as child when resolved")]
        public void RegisterNodeTransientFactory() {
            var di = new Container(this);
            // Register instance

            di.Register(() => new Node()).IsTransient().Build();
            var instance1 = di.Resolve<Node>();
            Assert.That(GetChildren().Contains(instance1));

            var instance2 = di.Resolve<Node>();
            Assert.That(GetChildren().Contains(instance2));

            Assert.That(instance1, Is.Not.EqualTo(instance2));
        }

        /**
         * Lifecycle
         */
        [Test(Description = "Register a Singleton factory is executed only the first time")]
        public void RegisterSingletonFactory() {
            var di = new Container(this);
            var n = 0;
            var s = di.Register(() => {
                n++;
                return new Node();
            }).Build();

            // Ensures that factory is called only the first time
            Assert.That(n, Is.EqualTo(0));
            var instance1 = di.Resolve<Node>();
            Assert.That(n, Is.EqualTo(1));

            var instance2 = di.Resolve<Node>();
            Assert.That(n, Is.EqualTo(1));

            // Ensure 2 instances are equal
            Assert.That(instance1, Is.EqualTo(instance2));
        }

        [Test(Description = "Register a Transient factory and is executed only the first time")]
        public void RegisterTransientFactory() {
            var di = new Container(this);
            var n = 0;
            var s = di.Register(() => {
                n++;
                return new Node();
            }).IsTransient().Build();

            // Ensures that factory is called only the first time
            Assert.That(n, Is.EqualTo(0));
            var instance1 = di.Resolve<Node>();
            Assert.That(n, Is.EqualTo(1));

            var instance2 = di.Resolve<Node>();
            Assert.That(n, Is.EqualTo(2));

            // Ensure 2 instances are different
            Assert.That(instance1, Is.Not.EqualTo(instance2));
        }

        [Test(Description = "Register a Singleton auto factory by Type is executed only the first time")]
        public void RegisterSingletonAutoFactory() {
            var di = new Container(this);
            var n = 0;
            var s = di.Register(typeof(Node), Lifetime.Singleton).Build();

            // Ensures that factory is called only the first time
            var instance1 = di.Resolve<Node>();
            var instance2 = di.Resolve<Node>();

            // Ensure 2 instances are equal
            Assert.That(instance1, Is.EqualTo(instance2));
        }

        [Test(Description = "Register a Transient auto factory by Type is executed only the first time")]
        public void RegisterTransientAutoFactory() {
            var di = new Container(this);
            var n = 0;
            var s = di.Register(typeof(Node), Lifetime.Transient).Build();

            // Ensures that factory is called only the first time
            var instance1 = di.Resolve<Node>();
            var instance2 = di.Resolve<Node>();

            // Ensure 2 instances are different
            Assert.That(instance1, Is.Not.EqualTo(instance2));
        }

        [Test(Description = "Container Build")]
        public void ContainerBuild() {
            var di = new Container(this);
            di.Register<Node>();
            di.Register<IInterface1, ClassWith1Interface>();

            Assert.That(!di.Exist<Node>());
            Assert.That(!di.Exist<IInterface1>());

            di.Build();
            Assert.That(di.Exist<Node>());
            Assert.That(di.Exist<IInterface1>());
        }


        /*
         * Special
         */

        [Test(Description = "Register a lambda as instance can be called as method")]
        public void RegisterFactoryAsInstance() {
            var di = new Container(this);
            var n = 0;
            di.Instance<Func<int>>(() => ++n).Build();

            Func<int> resolve = di.Resolve<Func<int>>();

            Assert.That(resolve(), Is.EqualTo(1));
            Assert.That(resolve(), Is.EqualTo(2));
        }
    }
}