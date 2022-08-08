using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ScanAttribute : Attribute {
        public Type Type { get; set; }

        public ScanAttribute(Type type) {
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class ServiceAttribute : Attribute {
        public Type? Type { get; set; }
        public string? Name { get; set; }
        public Lifetime Lifetime { get; set; } = Lifetime.Singleton;

        public ServiceAttribute() {
        }

        public ServiceAttribute(Lifetime lifetime, string name = null) {
            Lifetime = lifetime;
            Name = name;
        }

        public ServiceAttribute(Lifetime lifetime, Type type) {
            Lifetime = lifetime;
            Type = type;
        }

        public ServiceAttribute(string name, Lifetime lifetime = Lifetime.Singleton) {
            Name = name;
            Lifetime = lifetime;
        }
        public ServiceAttribute(Type type, Lifetime lifetime = Lifetime.Singleton) {
            Type = type;
            Lifetime = lifetime;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PostCreateAttribute : Attribute {
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
            _container.Build(_pendingToBuild.Select(p => p.CreateProvider()).ToList());
            _pendingToBuild.Clear();
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

        public ContainerBuilder Scan(Type type) => _Scan(type, null);  

        private ContainerBuilder _Scan(Type type, HashSet<Type>? stack) {
            // Look up for [Scan(typeof(...)]
            foreach (var importAttribute in Attribute.GetCustomAttributes(type, typeof(ScanAttribute), false)) {
                stack ??= new HashSet<Type>();
                stack.Add(type);
                var typeToImport = ((ScanAttribute)importAttribute).Type;
                if (!stack.Contains(typeToImport)) _Scan(typeToImport, stack);
            }
            
            // Look up for [Service]
            if (Attribute.GetCustomAttribute(type, typeof(ServiceAttribute), false) is ServiceAttribute serviceAttr) {
                var types = serviceAttr.Name == null ? new[] { serviceAttr.Type ?? type } : null;
                var aliases = serviceAttr.Name != null ? new[] { serviceAttr.Name } : null;
                Register(type, serviceAttr.Lifetime, types, aliases);
                if (serviceAttr.Lifetime == Lifetime.Singleton) {
                    // [Service] singletons can expose other services as it would have [Configuration]
                    ScanMemberExposingServices(type, false);
                }
            } else {
                // No [Service] present in the class, check for [Configuration]
                if (Attribute.GetCustomAttribute(type, typeof(ConfigurationAttribute), false) is ConfigurationAttribute) {
                    ScanMemberExposingServices(type, true);
                }
            }
            return this;
        }

        public ContainerBuilder ScanConfiguration(params object[] instances) {
            foreach (var instance in instances) {
                ScanMemberExposingServices(instance);
            }
            return this;
        }

        private const BindingFlags ScanMemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private void ScanMemberExposingServices(Type type, bool fromConfiguration) {
            // _logger.Debug("Exposing properties and methods " + type;
            object conf = null;
            foreach (var getter in type.GetPropertiesAndMethods<ServiceAttribute>(ScanMemberFlags)) {
                var types = getter.Attribute.Type == null ? null : new[] { getter.Attribute.Type };  
                var alias = getter.Attribute.Type != null ? null : new[] { getter.Attribute.Name ?? getter.Name };
                Register(getter.Type, () => {
                    var instance = fromConfiguration ? conf ??= Activator.CreateInstance(type) : _container.Resolve(type);
                    return getter.GetValue(instance);
                }, getter.Attribute.Lifetime, types, alias);
            }
        }

        private void ScanMemberExposingServices(object instance) {
            var type = instance.GetType();
            foreach (var getter in type.GetPropertiesAndMethods<ServiceAttribute>(ScanMemberFlags)) {
                var types = getter.Attribute.Type == null ? null : new[] { getter.Attribute.Type };  
                var alias = getter.Attribute.Type != null ? null : new[] { getter.Attribute.Name ?? getter.Name };
                Register(getter.Type, () => getter.GetValue(instance), getter.Attribute.Lifetime, types, alias);
            }
        }
    }
}