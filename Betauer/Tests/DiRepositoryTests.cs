using System;
using System.Collections.Generic;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    public interface INoMySingleton {
    }

    public interface IMySingleton {
    }

    public class MySingleton : IMySingleton {
    }

    public class MySingleton2 : IMySingleton {
    }


    [TestFixture]
    public class DiRepositoryTests : Node {
        [Test(Description = "Types not found")]
        public void NotFound() {
            var di = new DiRepository(this);

            // Not found types fail
            try {
                di.Resolve<IMySingleton>();
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            try {
                di.Resolve(typeof(IMySingleton));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
        }

        /*
         * OnInstanceCreated
         */
        [Test(Description = "Singleton instance OnInstanceCreated. No need to resolve to execute the hook")]
        public void RegisterSingletonInstanceOnInstanceCreated() {
            var di = new DiRepository(this);
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y");
            var instance = new Node();
            di.RegisterSingleton(instance);
            Assert.That(instance.GetMeta("x"), Is.EqualTo("y"));
        }

        [Test(Description = "Singleton Factory OnInstanceCreated")]
        public void RegisterSingletonFactoryOnInstanceCreated() {
            var di = new DiRepository(this);
            var x = 0;
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y" + ++x);
            var instance = new Node();
            di.Register(() => instance, Lifestyle.Singleton);
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y1"));
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y1"));
        }

        [Test(Description = "Singleton Factory OnInstanceCreated")]
        public void RegisterTransientFactoryOnInstanceCreated() {
            var di = new DiRepository(this);
            var x = 0;
            di.OnInstanceCreated = (o) => ((Node)o).SetMeta("x", "y" + ++x);
            di.Register(() => new Node(), Lifestyle.Transient);
            di.Register(() => new Control(), Lifestyle.Transient);
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y1"));
            Assert.That(di.Resolve<Control>().GetMeta("x"), Is.EqualTo("y2"));
            Assert.That(di.Resolve<Node>().GetMeta("x"), Is.EqualTo("y3"));
        }

        /*
         * Node Singleton are added to the owner when created
         */
        [Test(Description = "Register a Singleton Node adds it as child when added. No need to resolve")]
        public void RegisterNodeInstance() {
            var di = new DiRepository(this);
            // Register instance
            var instance = new Node();
            di.RegisterSingleton(instance);
            Assert.That(GetChildren().Contains(instance));
        }

        [Test(Description = "Register a Singleton Factory Node adds it as child when resolved. No need to resolve")]
        public void RegisterNodeSingletonFactory() {
            var di = new DiRepository(this);
            // Register instance
            var instance = new Node();
            di.Register(() => instance, Lifestyle.Singleton);
            Assert.That(!GetChildren().Contains(instance));

            di.Resolve<Node>();
            Assert.That(GetChildren().Contains(instance));
        }

        /*
         * Access to singleton instance by type
         */

        [Test(Description = "Register a Singleton instance is only accessible by its Type")]
        public void RegisterSingletonInstance() {
            var di = new DiRepository(this);

            // Register instance
            var instance = new Node();
            di.RegisterSingleton(instance);
            Assert.That(di.Resolve<Node>(), Is.EqualTo(instance));
            Assert.That(di.Resolve(typeof(Node)), Is.EqualTo(instance));

            // Instances of the same Type can be overriden
            var instance2 = new Node();
            di.RegisterSingleton(instance2);
            Assert.That(di.Resolve<Node>(), Is.EqualTo(instance2));
            Assert.That(di.Resolve(typeof(Node)), Is.EqualTo(instance2));
        }

        [Test(Description =
            "Register a Singleton instance with an interface as Type is only accessible by interface - Generic")]
        public void RegisterSingletonInterfaceUsingGeneric() {
            var di = new DiRepository(this);

            var mySingleton = new MySingleton();
            di.RegisterSingleton<IMySingleton>(mySingleton);
            Assert.That(di.Resolve<IMySingleton>(), Is.EqualTo(mySingleton));
            Assert.That(di.Resolve(typeof(IMySingleton)), Is.EqualTo(mySingleton));
            try {
                // The instance type can't be resolved, it's only possible if the interface type is used
                di.Resolve(typeof(MySingleton));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            // Interface can be overriden by other instance of other type
            var mySingleton2 = new MySingleton2();
            di.RegisterSingleton<IMySingleton>(mySingleton2);
            Assert.That(di.Resolve<IMySingleton>(), Is.EqualTo(mySingleton2));
            Assert.That(di.Resolve(typeof(IMySingleton)), Is.EqualTo(mySingleton2));
            try {
                di.Resolve(typeof(MySingleton2));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }
        }

        [Test(Description =
            "Register a Singleton instance with an interface as Type is only accessible by interface - Type")]
        public void RegisterSingletonInterfaceUsingType() {
            var di = new DiRepository(this);

            var mySingleton = new MySingleton();
            // The instance should implement the interface
            di.RegisterSingleton(typeof(IMySingleton), mySingleton);
            Assert.That(di.Resolve<IMySingleton>(), Is.EqualTo(mySingleton));
            Assert.That(di.Resolve(typeof(IMySingleton)), Is.EqualTo(mySingleton));
            try {
                // The instance type can't be resolved, it's only possible if the interface type is used
                di.Resolve(typeof(MySingleton));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            // Interface can be overriden by other instance of other type
            var mySingleton2 = new MySingleton2();
            di.RegisterSingleton(typeof(IMySingleton), mySingleton2);
            Assert.That(di.Resolve<IMySingleton>(), Is.EqualTo(mySingleton2));
            Assert.That(di.Resolve(typeof(IMySingleton)), Is.EqualTo(mySingleton2));
            try {
                di.Resolve(typeof(MySingleton2));
                Assert.That(false, "It should fail!");
            } catch (KeyNotFoundException e) {
            }

            try {
                // The instance type doesn't match the interface, so it fails
                di.RegisterSingleton(typeof(INoMySingleton), mySingleton);
                Assert.That(false, "It should fail!");
            } catch (ArgumentException e) {
            }
        }

        /**
         * Factories
         */

        [Test(Description = "Register a Singleton factory is executed only the first time")]
        public void RegisterSingletonFactory() {
            var di = new DiRepository(this);
            var n = 0;
            di.Register(() => ++n, Lifestyle.Singleton);

            // Ensures that factory is called only the first time
            Assert.That(n, Is.EqualTo(0));
            Assert.That(di.Resolve<int>(), Is.EqualTo(1));
            Assert.That(di.Resolve<int>(), Is.EqualTo(1));
        }

        [Test(Description = "Register a Singleton type creates a factory and is executed only the first time")]
        public void RegisterSingletonType() {
            var di = new DiRepository(this);
            di.Register<Node>(Lifestyle.Singleton);

            // Ensures that factory is called only the first time
            Node node1 = di.Resolve<Node>();
            Node node2 = di.Resolve<Node>();
            Assert.That(node1, Is.Not.Null);
            Assert.That(node1.GetHashCode(), Is.EqualTo(node2.GetHashCode()));
        }

        [Test(Description = "Register Transient factory is executed every time")]
        public void RegisterTransientFactory() {
            var di = new DiRepository(this);
            var n = 0;
            di.Register(() => ++n, Lifestyle.Transient);

            Assert.That(di.Resolve<int>(), Is.EqualTo(1));
            Assert.That(di.Resolve<int>(), Is.EqualTo(2));
        }


        [Test(Description = "Register Transient type creates a factory and is executed every time")]
        public void RegisterTransientType() {
            var di = new DiRepository(this);
            di.Register<Node>(Lifestyle.Transient);

            // Ensures that factory is called only the first time
            Node node1 = di.Resolve<Node>();
            Node node2 = di.Resolve<Node>();
            Assert.That(node1, Is.Not.Null);
            Assert.That(node2, Is.Not.Null);
            Assert.That(node1.GetHashCode(), Is.Not.EqualTo(node2.GetHashCode()));
        }

        [Test(Description = "Register a factory (Singleton or Transient) with a wrong type fails")]
        public void RegisterTypedFactoryWrongType() {
            var di = new DiRepository(this);
            di.Register(typeof(string), () => 1, Lifestyle.Transient);
            di.Register(typeof(int), () => "", Lifestyle.Singleton);

            try {
                di.Resolve<string>();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {

            }
            try {
                di.Resolve<int>();
                Assert.That(false, "It should fail!");
            } catch (InvalidCastException) {
            }
        }

        /*
         * Types
         */

        [Test(Description = "Register a factory with a compatible type")]
        public void RegisterTypedTypedFactory() {
            var di = new DiRepository(this);
            di.Register(typeof(IMySingleton), () => new MySingleton(), Lifestyle.Transient);

            Assert.That(di.Resolve<IMySingleton>().GetType(), Is.EqualTo(typeof(MySingleton)));
        }

        /*
         * Special
         */

        [Test(Description = "Register a lambda as instance can be called as method")]
        public void RegisterFactoryAsInstance() {
            var di = new DiRepository(this);
            var n = 0;
            di.RegisterSingleton<Func<int>>(() => ++n);

            Func<int> resolve = di.Resolve<Func<int>>();

            Assert.That(resolve(), Is.EqualTo(1));
            Assert.That(resolve(), Is.EqualTo(2));
        }
    }
}