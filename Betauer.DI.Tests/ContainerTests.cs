using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Betauer.DI;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.DI.Tests {
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
    [Only]
    public class ContainerTests : Node {
        [Test(Description = "ResolveOrNull tests")]
        public void ResolveOrNullTests() {
            var di = new Container();
            Assert.That(di.ResolveOr(typeof(string), () => "X"), Is.EqualTo("X"));
            Assert.That(di.ResolveOr(() => "X"), Is.EqualTo("X"));
            Assert.That(di.ResolveOr("O", () => "X"), Is.EqualTo("X"));
        }

        [Test(Description = "Container in container, assert Contains and TryGetProvider")]
        public void Container() {
            var di = new Container();
            Assert.That(di.Resolve<Container>(), Is.EqualTo(di));
            Assert.That(di.Contains<Container>());
            Assert.That(di.Contains<Container>());
            Assert.That(di.Resolve<Container>(), Is.EqualTo(di));
            Assert.That(di.Resolve<Container>(), Is.EqualTo(di));

            Assert.That(di.GetProvider<Container>().Get(new ResolveContext(di)), Is.EqualTo(di));
            Assert.That(di.TryGetProvider<Container>(out var provider));
            Assert.That(!di.TryGetProvider<Node>(out var notFound));
            Assert.That(provider!.Get(new ResolveContext(di)), Is.EqualTo(di));
        }

        [Test(Description = "Resolve by types & alias not found")]
        public void NotFoundTests() {
            var di = new Container();

            // Not found types fail
            Assert.Throws<KeyNotFoundException>(() => di.Resolve<IInterface1>());
            Assert.Throws<KeyNotFoundException>(() => di.Resolve<IInterface1>("X"));
            Assert.Throws<KeyNotFoundException>(() => di.Resolve("X"));
        }

        [Test(Description = "InvalidCastException when register type is not compatible with the provider type")]
        public void WrongCreatingTests() {
            TestDelegate[] x = {
                () => new ContainerBuilder().Static(typeof(Node), new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Static(typeof(Node), new ClassWith1Interface(), "P").Build(),
                () => new ContainerBuilder().Singleton<ClassWith1Interface, IInterface1>().Build(),
                () => new ContainerBuilder().Singleton<ClassWith1Interface, IInterface1>("P").Build(),
                () => new ContainerBuilder().Transient<ClassWith1Interface, IInterface1>().Build(),
                () => new ContainerBuilder().Transient<ClassWith1Interface, IInterface1>("P").Build(),
                () => new ContainerBuilder().Service<ClassWith1Interface, IInterface1>(Lifetime.Singleton).Build(),
                () => new ContainerBuilder().Service<ClassWith1Interface, IInterface1>(Lifetime.Transient, "P").Build(),
            };
            foreach (var func in x) {
                Console.WriteLine($"Test #{x}");
                Assert.Throws<InvalidCastException>(func);
            }
        }

        [Test(Description = "CreateIfNotFound: if type is not found, create a new instance automatically (like transient)")]
        public void CreateTransientIfNotFoundTest() {
            var di = new Container() {
                CreateIfNotFound = true
            };

            var n1 = di.Resolve<Node>();
            var n2 = di.Resolve<Node>();
            Assert.That(n1, Is.Not.Null);
            Assert.That(n1, Is.Not.EqualTo(n2));

            // Not allowed interfaces
            Assert.Throws<MissingMethodException>(() => di.Resolve<IInterface1>());
            // Not allowed abstract classes
            Assert.Throws<MissingMethodException>(() => di.Resolve(typeof(AbstractClass)));
        }

        /*
         * Static instances
         */
        [Test(Description = "Resolve and Contains by type")]
        public void StaticInstanceTest() {
            Func<Container>[] x = {
                () => new ContainerBuilder().Static(typeof(ClassWith1Interface),new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Static(new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Singleton<ClassWith1Interface>().Build(),
                () => new ContainerBuilder().Singleton(() => new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Transient<ClassWith1Interface>().Build(),
                () => new ContainerBuilder().Transient(() => new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Service<ClassWith1Interface>().Build(),
                () => new ContainerBuilder().Service<ClassWith1Interface>(Lifetime.Singleton).Build(),
                () => new ContainerBuilder().Service<ClassWith1Interface>(Lifetime.Transient).Build(),
                () => new ContainerBuilder().Service(() => new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Service(() => new ClassWith1Interface(), Lifetime.Singleton).Build(),
                () => new ContainerBuilder().Service(() => new ClassWith1Interface(), Lifetime.Transient).Build(),
            };
            foreach (var func in x) {
                Console.WriteLine($"Test #{x}");
                var c = func();
                Assert.That(c.Contains<ClassWith1Interface>());
                Assert.That(!c.Contains<IInterface1>());
            
                Assert.That(c.Resolve<ClassWith1Interface>(), Is.TypeOf<ClassWith1Interface>());
                Assert.Throws<KeyNotFoundException>(() => c.Resolve<IInterface1>());
            }
        }
        
        [Test(Description = "Register static instances by other type")]
        public void StaticInstanceOtherTypeTest() {
            Func<Container>[] x = {
                () => new ContainerBuilder().Static(typeof(IInterface1),new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Static<IInterface1>(new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Singleton<IInterface1, ClassWith1Interface>().Build(),
                () => new ContainerBuilder().Singleton<IInterface1>(() => new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Transient<IInterface1, ClassWith1Interface>().Build(),
                () => new ContainerBuilder().Transient<IInterface1>(() => new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>().Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>(Lifetime.Singleton).Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>(Lifetime.Transient).Build(),
                () => new ContainerBuilder().Service<IInterface1>(() => new ClassWith1Interface(), Lifetime.Singleton).Build(),
                () => new ContainerBuilder().Service<IInterface1>(() => new ClassWith1Interface(), Lifetime.Transient).Build(),
            };
            foreach (var func in x) {
                Console.WriteLine($"Test #{x}");
                var c = func();
                Assert.That(!c.Contains<ClassWith1Interface>());
                Assert.That(c.Contains<IInterface1>());

                Assert.Throws<KeyNotFoundException>(() => c.Resolve<ClassWith1Interface>());
                Assert.That(c.Resolve<IInterface1>(), Is.TypeOf<ClassWith1Interface>());
            }
        }
        
        [Test(Description = "Register static instances by name")]
        public void StaticInstanceOtherNameTest() {
            Func<Container>[] x = {
                () => new ContainerBuilder().Static(typeof(ClassWith1Interface),new ClassWith1Interface(), "P").Build(),
                () => new ContainerBuilder().Static(new ClassWith1Interface(), "P").Build(),
                () => new ContainerBuilder().Singleton<ClassWith1Interface>("P").Build(),
                () => new ContainerBuilder().Singleton(() => new ClassWith1Interface(), "P").Build(),
                () => new ContainerBuilder().Transient<ClassWith1Interface>("P").Build(),
                () => new ContainerBuilder().Transient(() => new ClassWith1Interface(), "P").Build(),
                () => new ContainerBuilder().Service<ClassWith1Interface>(Lifetime.Singleton, "P").Build(),
                () => new ContainerBuilder().Service<ClassWith1Interface>(Lifetime.Transient, "P").Build(),
                () => new ContainerBuilder().Service(() => new ClassWith1Interface(), Lifetime.Singleton, "P").Build(),
                () => new ContainerBuilder().Service(() => new ClassWith1Interface(), Lifetime.Transient, "P").Build(),
            };
            foreach (var func in x) {
                Console.WriteLine($"Test #{x}");
                var c = func();
                // By name, any compatible type is ok
                Assert.That(c.Contains<ClassWith1Interface>("P"));
                Assert.That(c.Contains<IInterface1>("P"));
                Assert.That(c.Contains<object>("P"));
                Assert.That(c.Contains("P"));

                Assert.That(c.Contains<ClassWith1Interface>());
                Assert.That(!c.Contains<IInterface1>());

                Assert.That(c.Resolve<ClassWith1Interface>("P"), Is.TypeOf<ClassWith1Interface>());
                Assert.That(c.Resolve<IInterface1>("P"), Is.TypeOf<ClassWith1Interface>());
                Assert.That(c.Resolve<object>("P"), Is.TypeOf<ClassWith1Interface>());
                Assert.That(c.Resolve("P"), Is.TypeOf<ClassWith1Interface>());

                Assert.That(c.Resolve<ClassWith1Interface>(), Is.TypeOf<ClassWith1Interface>());
                Assert.Throws<KeyNotFoundException>(() => c.Resolve<IInterface1>());
            }
        }

        [Test(Description = "Register static instances by name and other type")]
        public void StaticInstanceOtherNameAndTypeTest() {
            Func<Container>[] x = {
                () => new ContainerBuilder().Static(typeof(IInterface1),new ClassWith1Interface(), "P").Build(),
                () => new ContainerBuilder().Static<IInterface1>(new ClassWith1Interface(), "P").Build(),
                () => new ContainerBuilder().Singleton<IInterface1, ClassWith1Interface>("P").Build(),
                () => new ContainerBuilder().Singleton<IInterface1>(() => new ClassWith1Interface(), "P").Build(),
                () => new ContainerBuilder().Transient<IInterface1, ClassWith1Interface>("P").Build(),
                () => new ContainerBuilder().Transient<IInterface1>(() => new ClassWith1Interface(), "P").Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>(Lifetime.Singleton, "P").Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>(Lifetime.Transient, "P").Build(),
                () => new ContainerBuilder().Service<IInterface1>(() => new ClassWith1Interface(), Lifetime.Singleton, "P").Build(),
                () => new ContainerBuilder().Service<IInterface1>(() => new ClassWith1Interface(), Lifetime.Transient, "P").Build(),
            };
            foreach (var func in x) {
                Console.WriteLine($"Test #{x}");
                var c = func();
                // Special case! Registered by interface, the contains doesn't know the factory type
                Assert.That(!c.Contains<ClassWith1Interface>("P"));   
                Assert.That(c.Contains<IInterface1>("P"));
                Assert.That(c.Contains<object>("P"));
                Assert.That(c.Contains("P"));
                
                Assert.That(!c.Contains<ClassWith1Interface>());
                Assert.That(c.Contains<IInterface1>());

                Assert.That(c.Resolve<ClassWith1Interface>("P"), Is.TypeOf<ClassWith1Interface>());
                Assert.That(c.Resolve<IInterface1>("P"), Is.TypeOf<ClassWith1Interface>());
                Assert.That(c.Resolve<object>("P"), Is.TypeOf<ClassWith1Interface>());
                Assert.That(c.Resolve("P"), Is.TypeOf<ClassWith1Interface>());

                Assert.Throws<KeyNotFoundException>(() => c.Resolve<ClassWith1Interface>());
                Assert.That(c.Resolve<IInterface1>(), Is.TypeOf<ClassWith1Interface>());
            }
        }
        
        [Test(Description = "Static tests")]
        public void StaticTests() {
            var n1 = new ClassWith1Interface();
            var n2 = new ClassWith1Interface();
            var n3 = new ClassWith1Interface();
            var n4 = new ClassWith1Interface();
            var b = new ContainerBuilder();
            b.Static(n1);
            b.Static<IInterface1>(n2);
            b.Static(n3, "3");
            b.Static(n4, "4");
            var c = b.Build();
            
            Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n1));
            Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n1));
            Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(n2));
            Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(n2));
            Assert.That(c.Resolve<ClassWith1Interface>("3"), Is.EqualTo(n3));
            Assert.That(c.Resolve<IInterface1>("3"), Is.EqualTo(n3));
            Assert.That(c.Resolve<ClassWith1Interface>("4"), Is.EqualTo(n4));
            Assert.That(c.Resolve<IInterface1>("4"), Is.EqualTo(n4));
        }
        
        [Test(Description = "Singleton tests")]
        public void SingletonTests() {
            var b = new ContainerBuilder();
            b.Singleton<ClassWith1Interface>();
            b.Singleton<IInterface1>(() => new ClassWith1Interface());
            b.Singleton<ClassWith1Interface>("3");
            b.Singleton<IInterface1>(() => new ClassWith1Interface(), "4");
            var c = b.Build();

            var n1 = c.Resolve<ClassWith1Interface>();
            var n2 = c.Resolve<IInterface1>();
            var n3 = c.Resolve<ClassWith1Interface>("3");
            var n4 = c.Resolve<ClassWith1Interface>("4");

            Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n1));
            Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n1));
            Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(n2));
            Assert.That(c.Resolve<IInterface1>(), Is.EqualTo(n2));
            Assert.That(c.Resolve<ClassWith1Interface>("3"), Is.EqualTo(n3));
            Assert.That(c.Resolve<IInterface1>("3"), Is.EqualTo(n3));
            Assert.That(c.Resolve<ClassWith1Interface>("4"), Is.EqualTo(n4));
            Assert.That(c.Resolve<IInterface1>("4"), Is.EqualTo(n4));
        }
        
        [Test(Description = "Transient tests")]
        public void TransientTests() {
            var b = new ContainerBuilder();
            b.Transient<ClassWith1Interface>();
            b.Transient<IInterface1>(() => new ClassWith1Interface());
            b.Transient<ClassWith1Interface>("3");
            b.Transient<IInterface1>(() => new ClassWith1Interface(), "4");
            var c = b.Build();

            var n11 = c.Resolve<ClassWith1Interface>();
            var n12 = c.Resolve<ClassWith1Interface>();
            Assert.That(n11, Is.Not.EqualTo(n12));
            var n21 = c.Resolve<IInterface1>();
            var n22 = c.Resolve<IInterface1>();
            Assert.That(n21, Is.Not.EqualTo(n22));
            var n31 = c.Resolve<ClassWith1Interface>("3");
            var n32 = c.Resolve<ClassWith1Interface>("3");
            Assert.That(n31, Is.Not.EqualTo(n32));
        }
        
        [Test(Description = "Not allow duplicate by type")]
        public void DuplicatedTest() {
            var b = new ContainerBuilder();
            b.Singleton<ClassWith1Interface>();
            b.Singleton<ClassWith1Interface>();
            Assert.Throws<DuplicateServiceException>(() => b.Build());
        }

        [Test(Description = "Fallback to the first element registered by type when register by name and resolve by type")]
        public void FallbackFirstByTypeTest() {
            var b = new ContainerBuilder();
            var n1 = new ClassWith1Interface();
            var n2 = new ClassWith1Interface();
            b.Static(n1, "1");
            b.Static(n2, "2");
            var c = b.Build();
            Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n1));
        }

        [Test(Description = "Fallback to the primary element registered by type when register by name and resolve by type")]
        public void FallbackPrimaryByTypeTest() {
            var b = new ContainerBuilder();
            var n0 = new ClassWith1Interface();
            var n1 = new ClassWith1Interface();
            var n2 = new ClassWith1Interface();
            var n3 = new ClassWith1Interface();
            var n4 = new ClassWith1Interface();
            b.Static(n0);
            b.Static(n1, "1");
            b.Static(n2, "2", true);
            b.Static(n3, "3", true);
            b.Static(n4, "4");
            var c = b.Build();
            Assert.That(c.Resolve<ClassWith1Interface>("1"), Is.EqualTo(n1));
            Assert.That(c.Resolve<ClassWith1Interface>("2"), Is.EqualTo(n2));
            Assert.That(c.Resolve<ClassWith1Interface>("3"), Is.EqualTo(n3));
            Assert.That(c.Resolve<ClassWith1Interface>("4"), Is.EqualTo(n4));

            Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n3));
        }
    }
}