using System;
using Betauer.Application;
using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;
using Container = Betauer.DI.Container;

namespace Betauer.Tests.DI {
    [TestFixture]
    public class FunctionTests : Node {
        private Container? _backup;

        [SetUp]
        public void SetUp() {
            _backup ??= DefaultContainer.Container;
        }

        [TearDown]
        public void TearDown() {
            DefaultContainer.Set(_backup!);
        }

        public class Base {
            internal string baseName = "basePepe";
        }

        [Singleton]
        public class Singleton : Base {
            internal string name = "pepe";
            [Inject] internal Func<string> myName;
        }

        [Test(Description = "Test a regular function as static value")]
        public void FunctionTest() {
            var di = new ContainerBuilder(this);
            di.Scan<Singleton>();
            di.Static<Func<string>>(() => "hello");
            Container c = di.Build();
            var s = c.Resolve<Singleton>();
            Assert.That(s.myName(), Is.EqualTo("hello"));
        }

        [Test(Description = "Test a closure function")]
        public void ClosureTest() {
            var di = new ContainerBuilder(this);
            di.Scan<Singleton>();
            di.Function<Singleton, string>((s) => s.name);
            Container c = di.Build();
            var s = c.Resolve<Singleton>();
            Assert.That(s.myName(), Is.EqualTo("pepe"));
            s.name = "toto";
            Assert.That(s.myName(), Is.EqualTo("toto"));
        }

        [Test(Description = "Test a closure function with base class")]
        public void ClosureTestWithBaseClass() {
            var di = new ContainerBuilder(this);
            di.Scan<Singleton>();
            di.Function<Base, string>((s) => s.baseName);
            Container c = di.Build();
            Singleton s = c.Resolve<Singleton>();
            Assert.That(s.myName(), Is.EqualTo("basePepe"));
            s.baseName = "toto";
            Assert.That(s.myName(), Is.EqualTo("toto"));
        }

        [Test(Description = "Test a closure function has more priority than a static")]
        public void ClosureMoreThanStaticTest() {
            var di = new ContainerBuilder(this);
            di.Scan<Singleton>();
            di.Function<Singleton, string>((s) => s.name);
            di.Static<Func<string>>(() => "hello");
            Container c = di.Build();
            var s = c.Resolve<Singleton>();
            Assert.That(s.myName(), Is.EqualTo("pepe"));
        }

        public class MyNodeBase : DiNode {
            internal string baseName = "basePepe";

        }
        public class MyNode : MyNodeBase {
            internal string name = "pepe";
            [OnReady] internal string myName;
        }

        [Test(Description = "Test a closure function in OnReady")]
        public void OnReadyClosureTest() {
            var di = new ContainerBuilder(this);
            di.Function<MyNode, string>((s) => s.name);
            var container = di.Build();

            var s = new MyNode();
            DefaultContainer.Set(container);
            s._Ready();

            Assert.That(s.myName, Is.EqualTo("pepe"));
        }

        [Test(Description = "Test a closure function in OnReady using a base class")]
        public void OnReadyClosureTestBaseClass() {
            var di = new ContainerBuilder(this);
            di.Function<MyNodeBase, string>((s) => s.baseName);
            var container = di.Build();

            MyNode s = new MyNode();
            DefaultContainer.Set(container);
            s._Ready();

            Assert.That(s.myName, Is.EqualTo("basePepe"));
        }
    }
}