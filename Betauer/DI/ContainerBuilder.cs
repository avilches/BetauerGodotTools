using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : Attribute {
        // public string Name { get; set; } // TODO: use Name so more than one factory can be used per name
        // public Type Type { get; set;  }
        // public Type[] Types { get; set;  }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TransientAttribute : Attribute {
    }

    // TODO: add [Component] attribute: it means the class has inside services exposed as methods like:
    /*

    [Transient]
    public Node CreatePepeBean() {
        return new Node();
    }

    [Singleton]
    public SaveGameManager CreateSaveGameManager() {
        return new SaveGameManager();
    }

    // _container.Register<Node>(() => CreatePepeBean()).IsTransient()
    // _container.Register<SaveGameManager>(() => CreateSaveGameManager()).IsSingleton()

     */

    public class ContainerBuilder {
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(ContainerBuilder));
        private readonly Queue<IProvider> _resolvedPending = new Queue<IProvider>();
        private readonly Queue<IProviderBuilder> _pendingToBuild = new Queue<IProviderBuilder>();
        private readonly Container _container;

        public ContainerBuilder(Node owner) {
            _container = new Container(owner);
        }

        public Container Build() {
            while (_pendingToBuild.Count > 0) {
                var provider = _pendingToBuild.Dequeue().CreateProvider();
                _container.Add(provider);
                if (provider.GetLifetime() == Lifetime.Singleton) {
                    _resolvedPending.Enqueue(provider);
                }
            }
            while (_resolvedPending.Count > 0) {
                var singleton = _resolvedPending.Dequeue();
                _logger.Debug("Resolving Singleton Type: " + singleton.GetProviderType());
                // Get() will also add the instance to the Owner if the service is a Node singleton
                singleton.Get(new ResolveContext(_container));
            }
            return _container;
        }

        public StaticProviderBuilder<T> Instance<T>(T instance) where T : class {
            var builder = new StaticProviderBuilder<T>(instance);
            AddToBuildQueue(builder);
            return builder;
        }

        public FactoryProviderBuilder<T> Register<T>(Func<T> factory) where T : class {
            return Register<T>().With(factory);
        }

        public FactoryProviderBuilder<T> Register<TI, T>(Lifetime lifetime = Lifetime.Singleton) where T : class {
            return Register<T>(lifetime).As<TI>();
        }

        public FactoryProviderBuilder<T> Register<T>(Lifetime lifetime = Lifetime.Singleton) where T : class {
            var builder = new FactoryProviderBuilder<T>().Lifetime(lifetime);
            AddToBuildQueue(builder);
            return builder;
        }

        public IProviderBuilder Register(Type type, Lifetime lifetime, params Type[] types) {
            return Register(type, null, lifetime, types);
        }

        public IProviderBuilder Register(Type type, Func<object> factory = null, Lifetime lifetime = Lifetime.Singleton,
            params Type[] types) {
            var builder = FactoryProviderBuilder.Create(type, lifetime, factory, types);
            AddToBuildQueue(builder);
            return builder;
        }

        public void AddToBuildQueue(IProviderBuilder builder) {
            _pendingToBuild.Enqueue(builder);
        }

        public ContainerBuilder Scan() {
            return Scan(new[] {
                    _container.Owner.GetType().Assembly,
                    GetType().Assembly
                },
                (type) => !type.Namespace!.StartsWith("Betauer.Tests"));
        }

        public ContainerBuilder Scan(IEnumerable<Assembly> assemblies, Predicate<Type>? predicate = null) {
            foreach (var assembly in assemblies) Scan(assembly, predicate);
            return this;
        }

        public ContainerBuilder Scan(Assembly assembly, Predicate<Type>? predicate = null) =>
            Scan(assembly.GetTypes(), predicate);

        public ContainerBuilder Scan(IEnumerable<Type> types, Predicate<Type>? predicate = null) {
            foreach (Type type in types) {
                if (predicate?.Invoke(type) ?? true) Scan(type);
            }
            return this;
        }

        public ContainerBuilder Scan<T>() => Scan(typeof(T));

        public ContainerBuilder Scan(Type type) {
            // TODO: include more types in the attribute
            // Attribute[] attrib = Attribute.GetCustomAttributes(type, false);
            // TransientAttribute tr = attrib.FirstOrDefault(attribute => attribute is TransientAttribute != null) as TransientAttribute;
            if (Attribute.GetCustomAttribute(type, typeof(SingletonAttribute), false) is
                SingletonAttribute singletonAttribute) {
                Register(type, Lifetime.Singleton, new[] { type });
            } else if (Attribute.GetCustomAttribute(type, typeof(TransientAttribute), false) is
                       TransientAttribute transientAttribute) {
                Register(type, Lifetime.Transient, new[] { type });
            }
            return this;
        }
    }
}