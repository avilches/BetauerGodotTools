using System;
using System.Threading.Tasks;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.DI.Tests {
    [TestFixture]
    [Only]
    public class FunctionTests : Node {
        private Container? _backup;

        public class Base {
            internal string baseName = "basePepe";
        }

        [Service]
        public class Singleton : Base {
            internal string name = "pepe";
            [Inject] internal Func<string> myName;
        }

        [Test(Description = "Test a regular function as static value")]
        public void FunctionTest() {
            var di = new ContainerBuilder();
            di.Scan<Singleton>();
            di.Static<Func<string>>(() => "hello");
            Container c = di.Build();
            var s = c.Resolve<Singleton>();
            Assert.That(s.myName(), Is.EqualTo("hello"));
        }

        [Test(Description = "Test a closure function")]
        public void ClosureTest() {
            var di = new ContainerBuilder();
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
            var di = new ContainerBuilder();
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
            var di = new ContainerBuilder();
            di.Scan<Singleton>();
            di.Function<Singleton, string>((s) => s.name);
            di.Static<Func<string>>(() => "hello");
            Container c = di.Build();
            var s = c.Resolve<Singleton>();
            Assert.That(s.myName(), Is.EqualTo("pepe"));
        }

    }
}