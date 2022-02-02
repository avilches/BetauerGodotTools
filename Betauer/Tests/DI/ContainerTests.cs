using System;
using System.Collections;
using System.Collections.Generic;
using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.Tests.DI
{
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
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(Node)));
            Assert.That(s.Resolve(di), Is.EqualTo(node));
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
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.Resolve(di), Is.EqualTo(classWith2Interfaces));
            Assert.That(di.Resolve<ClassWith2Interfaces>(), Is.EqualTo(classWith2Interfaces));
            Assert.That(!di.Exist<IInterface1>());
            Assert.That(!di.Exist<IInterface2>());
            Assert.That(!di.Exist<IInterface2_2>());

            // Class with only one interfaces only. Try to resolve using the class will not work
            // IDisposable
            di = new Container(this);
            var mySingleton = new ClassWith2Interfaces();
            s = di.Instance(mySingleton).As<IInterface2>().As<IDisposable>().Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.Resolve(di), Is.EqualTo(mySingleton));
            Assert.That(di.Resolve<IInterface2>(), Is.EqualTo(mySingleton));
            Assert.That(!di.Exist<ClassWith2Interfaces>());
            Assert.That(!di.Exist<IInterface1>());
        }

        /*
         * Factories
         */
        [Test(Description = "Register factory uses As<Type> by default, ignoring interfaces")]
        public void RegisterTypeByDefault() {
            var di = new Container(this);

            // Use As<T>() by default
            var s = di.Register<ClassWith1Interface>(Lifestyle.Transient).Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith1Interface)));
            Assert.That(s.Resolve(di).GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(di.Resolve<ClassWith1Interface>().GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
            Assert.That(!di.Exist<IInterface1>());
        }

        [Test(Description = "Register factory using AsAll<class>")]
        public void RegisterTypedTypedFactoryWithInterfaces() {
            var di = new Container(this);

            // AsAll with the class uses the Type and all the interfaces
            di = new Container(this);
            var s = di.Register<ClassWith2Interfaces>().IsTransient().AsAll<ClassWith2Interfaces>().Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
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
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton)); // singleton by default
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(2));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2)));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(IInterface2_2)));
            Assert.That(s.Resolve(di).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
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
            Assert.That(s.Resolve(di).GetType(), Is.EqualTo(typeof(ClassWith1Interface)));
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
            Assert.That(s.Resolve(di).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve(typeof(IInterface2)).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<IInterface2>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!di.Exist<ClassWith2Interfaces>());
            Assert.That(!di.Exist<IInterface1>());
            Assert.That(!di.Exist<IInterface2_2>());

            di = new Container(this);
            s = di.Register(() => new Sprite()).As<CanvasItem>().Build();
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(CanvasItem)));
            Assert.That(s.Resolve(di).GetType(), Is.EqualTo(typeof(Sprite)));
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
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(Control)));

            s = di.Register(() => new Control()).IsSingleton().Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(Control)));

            // by object typed factory + type
            s = di.Register<CanvasItem>(Lifestyle.Transient).With(() => new Control()).Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(CanvasItem)));

            s = di.Register<CanvasItem>(Lifestyle.Singleton).With(() => new Control()).Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
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
            s = di.Register<ClassWith2Interfaces>(Lifestyle.Singleton).Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.Resolve(di).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(di.Resolve<ClassWith2Interfaces>().GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
            Assert.That(!di.Exist<IInterface1>());
            Assert.That(!di.Exist<IInterface2>());
            Assert.That(!di.Exist<IInterface2_2>());

            // auto factory with all classes
            di = new Container(this);
            s = di.Register<ClassWith2Interfaces>(Lifestyle.Transient).AsAll<ClassWith2Interfaces>().Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Transient));
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
            s = di.Register(typeof(ClassWith2Interfaces)).Build();
            Assert.That(s.GetLifestyle(), Is.EqualTo(Lifestyle.Singleton));
            Assert.That(s.GetRegisterTypes(), Contains.Item(typeof(ClassWith2Interfaces)));
            Assert.That(s.GetRegisterTypes().Length, Is.EqualTo(1));
            Assert.That(s.Resolve(di).GetType(), Is.EqualTo(typeof(ClassWith2Interfaces)));
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

            // class is in an upper level than expected
            try {
                di.Register<Node>().As<Sprite>().Build();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }
        }

        /*
         * OnInstanceCreated
         */
        [Test(Description = "Singleton instance OnInstanceCreated. No need to resolve to execute the hook")]
        public void RegisterSingletonInstanceOnInstanceCreated() {
            var di = new Container(this);
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y");
            var instance = new Node();
            var s = di.Instance(instance).Build();
            Assert.That(instance.GetMeta("x"), Is.EqualTo("y"));
        }

        [Test(Description = "Singleton Factory OnInstanceCreated")]
        public void RegisterSingletonFactoryOnInstanceCreated() {
            var di = new Container(this);
            var x = 0;
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y" + ++x);
            var instance = new Node();
            di.Register(() => instance).Build();
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y1"));
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y1"));
        }

        [Test(Description = "Singleton Factory OnInstanceCreated")]
        public void RegisterTransientFactoryOnInstanceCreated() {
            var di = new Container(this);
            var x = 0;
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y" + ++x);
            di.Register<Node>(Lifestyle.Transient).With(() => new Node()).Build();
            di.Register<Control>(Lifestyle.Transient).With(() => new Control()).Build();
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y1"));
            Assert.That(di.Resolve<Control>().GetMeta("x"), Is.EqualTo("y2"));
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y3"));
        }

        /*
         * Node Singleton are added to the owner when created
         */
        [Test(Description = "Register a Singleton Node adds it as child when added. No need to resolve")]
        public void RegisterNodeInstance() {
            var di = new Container(this);
            // Register instance
            var instance = new Node();
            di.Instance(instance).Build();
            Assert.That(GetChildren().Contains(instance));
        }

        [Test(Description = "Register a Singleton Factory Node adds it as child when resolved. No need to resolve")]
        public void RegisterNodeSingletonFactory() {
            var di = new Container(this);
            // Register instance
            var instance = new Node();
            di.Register(() => instance).Build();
            Assert.That(!GetChildren().Contains(instance));

            di.Resolve<Node>();
            Assert.That(GetChildren().Contains(instance));
        }

        /**
         * Lifecycle
         */
        [Test(Description = "Register a Singleton factory is executed only the first time")]
        public void RegisterSingletonFactory() {
            var di = new Container(this);
            var n = 0;
            // var s = di.Register(() => ++n).Build();

            // Ensures that factory is called only the first time
            // Assert.That(n, Is.EqualTo(0));
            // Assert.That(di.Resolve<int>(), Is.EqualTo(1));
            // Assert.That(di.Resolve<int>(), Is.EqualTo(1));
        }

        [Test(Description = "Register a Singleton type creates auto factory and is executed only the first time")]
        public void RegisterSingletonType() {
            var di = new Container(this);
            var s = di.Register<Node>(Lifestyle.Singleton).Build();

            // Ensures that factory is called only the first time
            Node node1 = di.Resolve<Node>();
            Node node2 = di.Resolve<Node>();
            Assert.That(node1, Is.Not.Null);
            Assert.That(node1.GetHashCode(), Is.EqualTo(node2.GetHashCode()));
        }

        [Test(Description = "Register Transient factory is executed every time")]
        public void RegisterTransientFactory() {
            var di = new Container(this);
            var n = 0;
            // di.Register(() => ++n).IsTransient().Build();

            // Assert.That(di.Resolve<int>(), Is.EqualTo(1));
            // Assert.That(di.Resolve<int>(), Is.EqualTo(2));
        }


        [Test(Description = "Register Transient type creates a factory and is executed every time")]
        public void RegisterTransientType() {
            var di = new Container(this);
            di.Register<Node>(Lifestyle.Transient).Build();

            // Ensures that factory is called only the first time
            Node node1 = di.Resolve<Node>();
            Node node2 = di.Resolve<Node>();
            Assert.That(node1, Is.Not.Null);
            Assert.That(node2, Is.Not.Null);
            Assert.That(node1.GetHashCode(), Is.Not.EqualTo(node2.GetHashCode()));
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