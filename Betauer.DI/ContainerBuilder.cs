using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class SingletonAttribute : Attribute {
        public string? Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PostCreateAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class TransientAttribute : Attribute {
        public string? Name { get; set; }
    }

    public class ContainerBuilder {
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(ContainerBuilder));
        private readonly LinkedList<IProviderBuilder> _pendingToBuild = new LinkedList<IProviderBuilder>();
        private readonly Container _container;

        public ContainerBuilder(Container container) {
            _container = container;
        }

        public ContainerBuilder(Node owner) {
            _container = new Container(owner);
        }

        public Container Build() {
            lock (_pendingToBuild) {
                if (_pendingToBuild.Count > 0) {
                    foreach (var providerBuilder in _pendingToBuild) {
                        _container.Add(providerBuilder.CreateProvider(), false);
                    }
                    _pendingToBuild.Clear();
                    _container.Build();
                }
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

        public FactoryProviderBuilder<T> Register<T>(Func<T> factory, Lifetime lifetime = Lifetime.Singleton)
            where T : class {
            return Register<T>().With(factory).Lifetime(lifetime);
        }

        public FactoryProviderBuilder<T> Register<TI, T>(Func<T> factory, Lifetime lifetime = Lifetime.Singleton)
            where T : class {
            return Register<T>().With(factory).Lifetime(lifetime).As<TI>();
        }

        public FactoryProviderBuilder<T> Register<TI, T>(Lifetime lifetime = Lifetime.Singleton) where T : class {
            return Register<T>(lifetime).As<TI>();
        }

        public FactoryProviderBuilder<T> Register<T>(Lifetime lifetime = Lifetime.Singleton,
            IEnumerable<string>? aliases = null) where T : class {
            var builder = new FactoryProviderBuilder<T>().Lifetime(lifetime).As(aliases);
            AddToBuildQueue(builder);
            return builder;
        }

        public IProviderBuilder Register(Type type, Lifetime lifetime = Lifetime.Singleton,
            IEnumerable<Type>? types = null, IEnumerable<string>? aliases = null) {
            return Register(type, null, lifetime, types, aliases);
        }

        public IProviderBuilder Register(Type type, Func<object> factory, Lifetime lifetime,
            IEnumerable<Type>? types, IEnumerable<string>? aliases = null) {
            var builder = FactoryProviderBuilder.Create(type, lifetime, factory, types, aliases);
            AddToBuildQueue(builder);
            return builder;
        }

        public void AddToBuildQueue(IProviderBuilder builder) {
            lock (_pendingToBuild) _pendingToBuild.AddLast(builder);
        }

        public ContainerBuilder Scan(Predicate<Type>? predicate = null) {
            return Scan(_container.NodeSingletonOwner.GetType().Assembly, predicate);
        }

        public ContainerBuilder Scan(IEnumerable<Assembly> assemblies, Predicate<Type>? predicate = null) {
            foreach (var assembly in assemblies) Scan(assembly, predicate);
            return this;
        }

        public ContainerBuilder Scan(Assembly assembly, Predicate<Type>? predicate = null) {
            _logger.Info("Scanning "+assembly);
            Scan(assembly.GetTypes(), predicate);
            return this;
        }

        public ContainerBuilder Scan(IEnumerable<Type> types, Predicate<Type>? predicate = null) {
            foreach (Type type in types) {
                if (predicate?.Invoke(type) ?? true) Scan(type);
            }
            return this;
        }

        public ContainerBuilder Scan<T>() => Scan(typeof(T));

        public ContainerBuilder Scan(Type type) {
            ExposedService? exposedService = ExposedService.CreateFrom(type);
            if (exposedService == null) {
                // No [Transient] or [Singleton] present in the class, check for [Configuration]
                if (Attribute.GetCustomAttribute(type, typeof(ConfigurationAttribute), false) is ConfigurationAttribute) {
                    ScanMemberExposingServices(type, true);
                    ScanStaticMemberExposingServices(type);
                }
                return this;
            }

            var aliases = exposedService.Name != null ? new[] { exposedService.Name } : null;
            Register(type, exposedService.Lifetime, new[] { type }, aliases);

            if (exposedService.Lifetime == Lifetime.Singleton) {
                ScanMemberExposingServices(type, false);
                ScanStaticMemberExposingServices(type);
            }
            return this;
        }

        public ContainerBuilder ScanConfiguration(params object[] instances) {
            foreach (var instance in instances) {
                ScanMemberExposingServices(instance);
                ScanStaticMemberExposingServices(instance.GetType());
            }
            return this;
        }

        private const BindingFlags ScanMemberFlags = BindingFlags.Public | BindingFlags.NonPublic;

        private void ScanStaticMemberExposingServices(Type type) {
            foreach (var getter in type.GetGetters<SingletonAttribute>(ScanMemberFlags | BindingFlags.Static)) {
                Register(getter.Type, () => getter.GetValue(null) , Lifetime.Singleton, null, 
                    new[] { getter.Attribute.Name ?? getter.Name });
            }
            foreach (var getter in type.GetGetters<TransientAttribute>(ScanMemberFlags | BindingFlags.Static)) {
                Register(getter.Type, () => getter.GetValue(null) , Lifetime.Transient, null, 
                    new[] { getter.Attribute.Name ?? getter.Name });
            }
        }

        private void ScanMemberExposingServices(Type type, bool isConfiguration) {
            // _logger.Debug("Exposing properties and methods " + type;
            object conf = null;
            foreach (var getter in type.GetGetters<SingletonAttribute>(ScanMemberFlags | BindingFlags.Instance)) {
                Register(getter.Type, () => {
                    var instance = isConfiguration ? conf ??= Activator.CreateInstance(type) : _container.Resolve(type);
                    return getter.GetValue(instance);
                }, Lifetime.Singleton, null, new[] { getter.Attribute.Name ?? getter.Name });
            }
            
            foreach (var getter in type.GetGetters<TransientAttribute>(ScanMemberFlags | BindingFlags.Instance)) {
                Register(getter.Type, () => {
                    var instance = isConfiguration ? conf ??= Activator.CreateInstance(type) : _container.Resolve(type);
                    return getter.GetValue(instance);
                }, Lifetime.Transient, null, new[] { getter.Attribute.Name ?? getter.Name });
            }
        }

        private void ScanMemberExposingServices(object instance) {
            var type = instance.GetType();
            foreach (var getter in type.GetGetters<SingletonAttribute>(ScanMemberFlags | BindingFlags.Instance)) {
                Register(getter.Type, () => getter.GetValue(instance), Lifetime.Singleton, null,
                    new[] { getter.Attribute.Name ?? getter.Name });
            }
            
            foreach (var getter in type.GetGetters<TransientAttribute>(ScanMemberFlags | BindingFlags.Instance)) {
                Register(getter.Type, () => getter.GetValue(instance), Lifetime.Transient, null,
                    new[] { getter.Attribute.Name ?? getter.Name });
            }
        }

        private class ExposedService {
            public readonly string? Name;
            public readonly Lifetime Lifetime;

            private ExposedService(string? name, Lifetime lifetime) {
                Name = name;
                Lifetime = lifetime;
            }

            public static ExposedService? CreateFrom(Type type) {
                Attribute[] attributes = Attribute.GetCustomAttributes(type);
                foreach (var attribute in attributes) {
                    switch (attribute) {
                        case SingletonAttribute singleton:
                            return new ExposedService(singleton.Name, Lifetime.Singleton);
                        case TransientAttribute transient:
                            return new ExposedService(transient.Name, Lifetime.Transient);
                    }
                }
                return null;
            }
        }
    }
}