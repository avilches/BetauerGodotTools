using System;
using System.Collections.Generic;
using Betauer.DI.ServiceProvider;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.DI.Tests {
    public interface IInterface1 {
    }

    public abstract class AbstractClass {
    }

    public class ClassWith1Interface : IInterface1 {
    }

    [TestFixture]
    public class ContainerTests : Node {
        [Test(Description = "ResolveOrNull tests")]
        public void ResolveOrNullTests() {
            var di = new Container();
            var c = new ClassWith1Interface();
            Assert.That(di.ResolveOr<IInterface1>(() => c), Is.EqualTo(c));
            Assert.That(di.ResolveOr(() => "X"), Is.EqualTo("X"));
            Assert.That(di.ResolveOr("O", () => "X"), Is.EqualTo("X"));
        }

        [Test(Description = "Container in container, assert Contains and TryGetProvider")]
        public void Container() {
            var di = new Container();
            Assert.That(di.Resolve<Container>(), Is.EqualTo(di));
            Assert.That(di.Contains<Container>());

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

        [Test(Description = "Resolve, Contains, TryGetProvider and GetProvider: type")]
        public void TypeTest() {
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

                Assert.That(c.GetProvider<ClassWith1Interface>().ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.Throws<KeyNotFoundException>(() => c.GetProvider<IInterface1>());

                Assert.That(c.TryGetProvider<ClassWith1Interface>(out var provider), Is.True);
                Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.TryGetProvider<IInterface1>(out provider), Is.False);
                Assert.That(provider, Is.Null);

                Assert.That(c.Resolve<ClassWith1Interface>(), Is.TypeOf<ClassWith1Interface>());
                Assert.Throws<KeyNotFoundException>(() => c.Resolve<IInterface1>());
            }
        }
        
        [Test(Description = "Resolve, Contains, TryGetProvider and GetProvider: othertype")]
        public void OtherTypeTest() {
            Func<Container>[] x = {
                () => new ContainerBuilder().Static(typeof(IInterface1),new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Static<IInterface1>(new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Singleton<IInterface1, ClassWith1Interface>().Build(),
                () => new ContainerBuilder().Singleton<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Transient<IInterface1, ClassWith1Interface>().Build(),
                () => new ContainerBuilder().Transient<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface()).Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>().Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>(Lifetime.Singleton).Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>(Lifetime.Transient).Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface(), Lifetime.Singleton).Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface(), Lifetime.Transient).Build(),
            };
            foreach (var func in x) {
                Console.WriteLine($"Test #{x}");
                var c = func();
                Assert.That(!c.Contains<ClassWith1Interface>());
                Assert.That(c.Contains<IInterface1>());

                Assert.Throws<KeyNotFoundException>(() => c.GetProvider<ClassWith1Interface>());
                Assert.That(c.GetProvider<IInterface1>().ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));

                Assert.That(c.TryGetProvider<ClassWith1Interface>(out var provider), Is.False);
                Assert.That(provider, Is.Null);
                Assert.That(c.TryGetProvider<IInterface1>(out provider), Is.True);
                Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));

                Assert.Throws<KeyNotFoundException>(() => c.Resolve<ClassWith1Interface>());
                Assert.That(c.Resolve<IInterface1>(), Is.TypeOf<ClassWith1Interface>());
            }
        }
        
        [Test(Description = "Resolve, Contains, TryGetProvider and GetProvider: name")]
        public void NameTest() {
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

                Assert.That(c.GetProvider<ClassWith1Interface>("P").ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.GetProvider<IInterface1>("P").ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.GetProvider<object>("P").ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.GetProvider("P").ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.GetProvider<ClassWith1Interface>().ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.Throws<KeyNotFoundException>(() => c.GetProvider<IInterface1>());
                
                Assert.That(c.TryGetProvider<ClassWith1Interface>("P", out var provider), Is.True);
                Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.TryGetProvider<IInterface1>("P", out provider), Is.True);
                Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.TryGetProvider<object>("P", out provider), Is.True);
                Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.TryGetProvider("P", out provider), Is.True);
                Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.TryGetProvider<ClassWith1Interface>(out provider), Is.True);
                Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.TryGetProvider<IInterface1>(out provider), Is.False);
                Assert.That(provider, Is.Null);
            }
        }

        [Test(Description = "Resolve, Contains, TryGetProvider and GetProvider: name other type")]
        public void NameOtherTypeTest() {
            Func<Container>[] x = {
                () => new ContainerBuilder().Static(typeof(IInterface1),new ClassWith1Interface(), "P").Build(),
                () => new ContainerBuilder().Static<IInterface1>(new ClassWith1Interface(), "P").Build(),
                () => new ContainerBuilder().Singleton<IInterface1, ClassWith1Interface>("P").Build(),
                () => new ContainerBuilder().Singleton<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface(),"P").Build(),
                () => new ContainerBuilder().Transient<IInterface1, ClassWith1Interface>("P").Build(),
                () => new ContainerBuilder().Transient<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface(),"P").Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>(Lifetime.Singleton, "P").Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>(Lifetime.Transient, "P").Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface(), Lifetime.Singleton, "P").Build(),
                () => new ContainerBuilder().Service<IInterface1, ClassWith1Interface>(() => new ClassWith1Interface(), Lifetime.Transient, "P").Build(),
            };
            foreach (var func in x) {
                Console.WriteLine($"Test #{x}");
                var c = func();
                Assert.That(c.Contains<ClassWith1Interface>("P"));   
                Assert.That(c.Contains<IInterface1>("P"));
                Assert.That(c.Contains<object>("P"));
                Assert.That(c.Contains("P"));
                Assert.That(!c.Contains<ClassWith1Interface>());
                Assert.That(c.Contains<IInterface1>());

                Assert.That(c.GetProvider<ClassWith1Interface>("P").ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.GetProvider<IInterface1>("P").ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.GetProvider<object>("P").ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.GetProvider("P").ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.Throws<KeyNotFoundException>(() => c.GetProvider<ClassWith1Interface>());
                Assert.That(c.GetProvider<IInterface1>().ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));

                Assert.That(c.TryGetProvider<ClassWith1Interface>("P", out var provider), Is.True);
                Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.TryGetProvider<IInterface1>("P", out provider), Is.True);
                Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.TryGetProvider<object>("P", out provider), Is.True);
                Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.TryGetProvider("P", out provider), Is.True);
                Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));
                Assert.That(c.TryGetProvider<ClassWith1Interface>(out provider), Is.False);
                Assert.That(provider, Is.Null);
                Assert.That(c.TryGetProvider<IInterface1>(out provider), Is.True);
                Assert.That(provider.ProviderType, Is.EqualTo(typeof(ClassWith1Interface)));

                Assert.That(c.Resolve<ClassWith1Interface>("P"), Is.TypeOf<ClassWith1Interface>());
                Assert.That(c.Resolve<IInterface1>("P"), Is.TypeOf<ClassWith1Interface>());
                Assert.That(c.Resolve<object>("P"), Is.TypeOf<ClassWith1Interface>());
                Assert.That(c.Resolve("P"), Is.TypeOf<ClassWith1Interface>());
                Assert.Throws<KeyNotFoundException>(() => c.Resolve<ClassWith1Interface>());
                Assert.That(c.Resolve<IInterface1>(), Is.TypeOf<ClassWith1Interface>());
            }
        }

        [Test(Description = "GetAllInstances - no transients")]
        public void GetAllInstancesTransitionTests() {
            var b = new ContainerBuilder();
            b.Transient<ClassWith1Interface>();
            b.Transient<ClassWith1Interface>("A");
            var c = b.Build();
            Assert.That(c.GetAllInstances<ClassWith1Interface>(), Is.Empty);
        }

        [Test(Description = "GetAllInstances - singleton")]
        public void GetAllInstancesSingletonTests() {
            var b = new ContainerBuilder();
            b.Singleton<ClassWith1Interface>("A");
            var c = b.Build();
            Assert.That(c.GetAllInstances<ClassWith1Interface>().Count, Is.EqualTo(1));
            Assert.That(c.GetAllInstances<IInterface1>().Count, Is.EqualTo(1));
            Assert.That(c.GetAllInstances<object>().Count, Is.EqualTo(2)); // +1 (Include the container)
        }

        [Test(Description = "GetAllInstances - singleton")]
        public void GetAllInstancesMultipleSingletonTests() {
            var b = new ContainerBuilder();
            b.Transient<Node2D>(); // ignored
            b.Singleton<Node>("n1");
            b.Singleton<Node2D>("n2");
            b.Singleton<ClassWith1Interface>("A");
            b.Static<IInterface1>(new ClassWith1Interface(), "B");
            b.Singleton<ClassWith1Interface>();
            var c = b.Build();
            Assert.That(c.GetAllInstances<ClassWith1Interface>().Count, Is.EqualTo(3));
            Assert.That(c.GetAllInstances<IInterface1>().Count, Is.EqualTo(3));
            Assert.That(c.GetAllInstances<Node>().Count, Is.EqualTo(2));
            Assert.That(c.GetAllInstances<Node2D>().Count, Is.EqualTo(1));
            Assert.That(c.GetAllInstances<object>().Count, Is.EqualTo(6)); // +1 (Include the container)
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
        
        [Test(Description = "Not allow duplicates by type")]
        public void DuplicatedTypeTest() {
            var b1 = new ContainerBuilder();
            b1.Singleton<ClassWith1Interface>();
            b1.Transient<ClassWith1Interface>();
            Assert.Throws<DuplicateServiceException>(() => b1.Build());

            var b2 = new ContainerBuilder();
            b2.Static(new ClassWith1Interface());
            b2.Static(new ClassWith1Interface());
            Assert.Throws<DuplicateServiceException>(() => b2.Build());

        }

        [Test(Description = "Not allow duplicates by name")]
        public void DuplicatedNameTest() {
            var b1 = new ContainerBuilder();
            b1.Singleton(() => new ClassWith1Interface(), "A");
            b1.Transient<IInterface1, ClassWith1Interface>("A");
            Assert.Throws<DuplicateServiceException>(() => b1.Build());

            var b2 = new ContainerBuilder();
            b2.Singleton<ClassWith1Interface>("A");
            b2.Static<IInterface1>(new ClassWith1Interface(), "A");
            Assert.Throws<DuplicateServiceException>(() => b2.Build());

        }

        [Test(Description = "GetProvider and TryGetProvider with fallbacks")]
        public void GetProviderWithFallbackTest() {
            var b = new ContainerBuilder();
            var n1 = new ClassWith1Interface();
            b.Static(n1, "1");
            var c = b.Build();
            Assert.That(c.GetProvider<ClassWith1Interface>(), Is.EqualTo(c.GetProvider("1")));
            Assert.That(c.GetProvider<ClassWith1Interface>(), Is.EqualTo(c.GetProvider<ClassWith1Interface>("1")));
            var foundByName = c.TryGetProvider<ClassWith1Interface>(out var pName);
            var foundFallback = c.TryGetProvider("1", out var fName);
            Assert.That(foundByName);
            Assert.That(foundFallback);
            Assert.That(pName, Is.EqualTo(fName));
        }

        [Test(Description = "Overwrite the fallback with type")]
        public void CreateFallbackTest() {
            var b = new ContainerBuilder();
            var n1 = new ClassWith1Interface();
            var n2 = new ClassWith1Interface();
            var typed = new ClassWith1Interface();
            b.Static(n1, "1");
            b.Static(n2, "2");
            // This one should overwrite the fallback
            b.Static(typed);
            var c = b.Build();
            
            Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(typed));
            Assert.That(c.Resolve<ClassWith1Interface>("1"), Is.EqualTo(n1));
            Assert.That(c.Resolve<ClassWith1Interface>("2"), Is.EqualTo(n2));
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

        [Test(Description = "Fallback to the primary element only if it's not registered by")]
        public void NoFallbackPrimaryByTypeTest() {
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

            Assert.That(c.Resolve<ClassWith1Interface>(), Is.EqualTo(n0));
        }

        [Test(Description = "Fallback to the primary element registered by type when register by name and resolve by type")]
        public void FallbackPrimaryByTypeTest() {
            var b = new ContainerBuilder();
            var n1 = new ClassWith1Interface();
            var n2 = new ClassWith1Interface();
            var n3 = new ClassWith1Interface();
            var n4 = new ClassWith1Interface();
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

        [Test(Description = "Lazy tests")]
        public void LazyTest() {
            var b = new ContainerBuilder();
            b.Singleton<ClassWith1Interface>("NoLazy1");
            b.Singleton<ClassWith1Interface>("NoLazy2");
            b.Singleton<ClassWith1Interface>("Lazy1", false, true);
            b.Singleton<ClassWith1Interface>("Lazy2", false, true);
            var c = b.Build();
            
            var noLazy1Provider = c.GetProvider("NoLazy1") as SingletonFactoryProvider;
            var noLazy2Provider = c.GetProvider("NoLazy2") as SingletonFactoryProvider;
            var lazy1Provider = c.GetProvider("Lazy1") as SingletonFactoryProvider;
            var lazy2Provider = c.GetProvider("Lazy2") as SingletonFactoryProvider;
            
            Assert.That(noLazy1Provider.Lazy, Is.False);
            Assert.That(noLazy1Provider.IsInstanceCreated, Is.True);
            Assert.That(noLazy1Provider.Instance, Is.TypeOf<ClassWith1Interface>());
            
            Assert.That(noLazy2Provider.Lazy, Is.False);
            Assert.That(noLazy2Provider.IsInstanceCreated, Is.True);
            Assert.That(noLazy2Provider.Instance, Is.TypeOf<ClassWith1Interface>());

            Assert.That(lazy1Provider.Lazy, Is.True);
            Assert.That(lazy1Provider.IsInstanceCreated, Is.False);
            Assert.That(lazy1Provider.Instance, Is.Null);
            Assert.That(lazy2Provider.Lazy, Is.True);
            Assert.That(lazy2Provider.IsInstanceCreated, Is.False);
            Assert.That(lazy2Provider.Instance, Is.Null);

            c.Resolve("Lazy1");
            Assert.That(lazy1Provider.Lazy, Is.True);
            Assert.That(lazy1Provider.IsInstanceCreated, Is.True);
            Assert.That(lazy1Provider.Instance, Is.TypeOf<ClassWith1Interface>());
            Assert.That(lazy2Provider.Lazy, Is.True);

            c.Resolve("Lazy2");
            Assert.That(lazy2Provider.Lazy, Is.True);
            Assert.That(lazy2Provider.IsInstanceCreated, Is.True);
            Assert.That(lazy2Provider.Instance, Is.TypeOf<ClassWith1Interface>());

        }

        [Test(Description = "Test OnCreate")]
        public void OnCreateTests() {
            var singletons = new List<object>();
            var transients = new List<object>();
            var calls = 0;
            var c = new Container();
            c.OnCreated += (lifetime, instance) => {
                if (lifetime == Lifetime.Singleton) {
                    singletons.Add(instance);
                } else {
                    transients.Add(instance);
                }
            };
            c.OnCreated += (lifetime, instance) => {
                calls++;
            };
            var b = new ContainerBuilder(c);
            b.Static(new Node2D());
            b.Singleton<ClassWith1Interface>();
            b.Transient<Node>();
            b.Build();
            c.Resolve<Node2D>();
            Assert.That(singletons.Count, Is.EqualTo(1));
            Assert.That(singletons[0], Is.TypeOf<ClassWith1Interface>());
            c.Resolve<Node>();
            Assert.That(transients.Count, Is.EqualTo(1));
            Assert.That(transients[0], Is.TypeOf<Node>());
            c.Resolve<Node>();
            Assert.That(transients.Count, Is.EqualTo(2));
            Assert.That(transients[0], Is.TypeOf<Node>());
            Assert.That(transients[1], Is.TypeOf<Node>());
            Assert.That(transients[1], Is.Not.EqualTo(transients[0]));
            
            Assert.That(calls, Is.EqualTo(singletons.Count + transients.Count));
        }
    }
}