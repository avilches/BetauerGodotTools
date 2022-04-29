using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : Attribute {
        public string? Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TransientAttribute : Attribute {
        public string? Name { get; set; }
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

        public StaticProviderBuilder<T> Static<T>(T instance) where T : class {
            var builder = new StaticProviderBuilder<T>(instance);
            AddToBuildQueue(builder);
            return builder;
        }

        public FactoryProviderBuilder<T> Singleton<T>(Func<T> factory = null) where T : class {
            return Register<T>().IsSingleton().With(factory);
        }

        public FactoryProviderBuilder<T> Singleton<TI, T>(Func<T> factory = null) where T : class {
            return Register<T>().IsSingleton().With(factory).As<TI>();
        }

        public FactoryProviderBuilder<T> Transient<T>(Func<T> factory = null) where T : class {
            return Register<T>().IsTransient().With(factory);
        }

        public FactoryProviderBuilder<T> Transient<TI, T>(Func<T> factory = null) where T : class {
            return Register<T>().IsTransient().With(factory).As<TI>();
        }

        public FactoryProviderBuilder<T> Register<T>(Func<T> factory, Lifetime lifetime = Lifetime.Singleton) where T : class {
            return Register<T>().With(factory).Lifetime(lifetime);
        }

        public FactoryProviderBuilder<T> Register<TI, T>(Func<T> factory, Lifetime lifetime = Lifetime.Singleton) where T : class {
            return Register<T>().With(factory).Lifetime(lifetime).As<TI>();
        }

        public FactoryProviderBuilder<T> Register<TI, T>(Lifetime lifetime = Lifetime.Singleton) where T : class {
            return Register<T>(lifetime).As<TI>();
        }

        public FactoryProviderBuilder<T> Register<T>(Lifetime lifetime = Lifetime.Singleton, IEnumerable<string>? aliases = null) where T : class {
            var builder = new FactoryProviderBuilder<T>().Lifetime(lifetime).As(aliases);
            AddToBuildQueue(builder);
            return builder;
        }

        public IProviderBuilder Register(Type type, Lifetime lifetime = Lifetime.Singleton, IEnumerable<Type>? types = null, IEnumerable<string>? aliases = null) {
            return Register(type, null, lifetime, types, aliases);
        }

        public IProviderBuilder Register(Type type, Func<object> factory, Lifetime lifetime,
            IEnumerable<Type>? types, IEnumerable<string>? aliases = null) {
            var builder = FactoryProviderBuilder.Create(type, lifetime, factory, types, aliases);
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
                (type) => !type.Namespace?.StartsWith("Betauer.Tests") ?? true);
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
                var aliases = singletonAttribute.Name != null ? new[] { singletonAttribute.Name } : null;
                Register(type, Lifetime.Singleton, new[] { type }, aliases);
            } else if (Attribute.GetCustomAttribute(type, typeof(TransientAttribute), false) is
                       TransientAttribute transientAttribute) {
                var aliases = transientAttribute.Name != null ? new[] { transientAttribute.Name } : null;
                Register(type, Lifetime.Transient, new[] { type }, aliases);
            }
            return this;
        }
    }
}