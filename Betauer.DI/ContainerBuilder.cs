using System;
using System.Collections.Generic;
using System.Reflection;

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

    [AttributeUsage(AttributeTargets.Method)]
    public class PostCreateAttribute : Attribute {
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

    public class ContainerBuilder {
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(ContainerBuilder));
        private readonly LinkedList<IProvider> _pendingToBuild = new LinkedList<IProvider>();
        private readonly Container _container;

        public ContainerBuilder(Container container) {
            _container = container;
        }

        public ContainerBuilder() {
            _container = new Container();
        }

        public Container Build() {
            _container.Build(_pendingToBuild);
            _pendingToBuild.Clear();
            return _container;
        }


        public ContainerBuilder Static<T>(T instance, string? alias = null, bool primary = false) where T : class {
            Register(new StaticProvider(typeof(T),instance.GetType(), instance, alias, primary));
            return this;
        }

        public ContainerBuilder Static(Type type, object instance, string? alias = null, bool primary = false) {
            Register(new StaticProvider(type, instance.GetType(), instance, alias, primary));
            return this;
        }

        public ContainerBuilder Singleton<T>(string? alias = null, bool primary = false) where T : class {
            return Register<T, T>(Activator.CreateInstance<T>, Lifetime.Singleton, alias, primary);
        }

        public ContainerBuilder Singleton<T>(Func<T> factory, string? alias = null, bool primary = false) where T : class {
            return Register<T, T>(factory, Lifetime.Singleton, alias, primary);
        }

        public ContainerBuilder Singleton<TI, T>(string? alias = null, bool primary = false) where T : class {
            return Register<TI, T>(Activator.CreateInstance<T>, Lifetime.Singleton, alias, primary);
        }

        public ContainerBuilder Transient<T>(string? alias = null, bool primary = false) where T : class {
            return Register<T, T>(Activator.CreateInstance<T>, Lifetime.Transient, alias, primary);
        }

        public ContainerBuilder Transient<T>(Func<T> factory, string? alias = null, bool primary = false) where T : class {
            return Register<T, T>(factory, Lifetime.Transient, alias, primary);
        }

        public ContainerBuilder Transient<TI, T>(string? alias = null, bool primary = false) where T : class {
            return Register<TI, T>(Activator.CreateInstance<T>, Lifetime.Transient, alias, primary);
        }

        public ContainerBuilder Service<T>(Lifetime lifetime = Lifetime.Singleton, string? alias = null, bool primary = false) where T : class {
            return Register<T, T>(Activator.CreateInstance<T>, lifetime, alias, primary);
        }

        public ContainerBuilder Service<T>(Func<T> factory, Lifetime lifetime = Lifetime.Singleton, string? alias = null, bool primary = false) where T : class {
            return Register<T, T>(factory, lifetime, alias, primary);
        }

        public ContainerBuilder Service<TI, T>(Lifetime lifetime = Lifetime.Singleton, string? alias = null, bool primary = false) where T : class {
            return Register<TI, T>(Activator.CreateInstance<T>, lifetime, alias, primary);
        }

        public ContainerBuilder Register<TI, T>(Func<T> factory, Lifetime lifetime, string? alias, bool primary) where T : class {
            Register(typeof(TI), typeof(T), factory, lifetime, alias, primary);
            return this;
        }

        public ContainerBuilder Register(Type registeredType, Type type, Func<object> factory, Lifetime lifetime = Lifetime.Singleton, string? alias = null, bool? primary = false) {
            if (lifetime == Lifetime.Singleton) Register(new SingletonProvider(registeredType, type, factory, alias));
            else Register(new TransientProvider(registeredType, type, factory, alias));
            return this;
        }
        
        public ContainerBuilder Register(IProvider builder) {
            lock (_pendingToBuild) _pendingToBuild.AddLast(builder);
            return this;
        }

        public ContainerBuilder Scan(IEnumerable<Assembly> assemblies, Predicate<Type>? predicate = null) {
            foreach (var assembly in assemblies) Scan(assembly, predicate);
            return this;
        }

        public ContainerBuilder Scan(Assembly assembly, Predicate<Type>? predicate = null) {
            _logger.Info("Scanning " + assembly);
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
                var registeredType = serviceAttr.Type ?? type;
                var alias = serviceAttr.Name;
                Register(registeredType, type, () => Activator.CreateInstance(type), serviceAttr.Lifetime, alias);
                // TODO: stop allowing this?
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
                Register(getter.Attribute.Type ?? getter.Type, getter.Type, () => {
                    var instance = fromConfiguration ? conf ??= Activator.CreateInstance(type) : _container.Resolve(type);
                    return getter.GetValue(instance);
                }, getter.Attribute.Lifetime, getter.Attribute.Name ?? getter.Name);
            }
        }

        private void ScanMemberExposingServices(object instance) {
            var type = instance.GetType();
            foreach (var getter in type.GetPropertiesAndMethods<ServiceAttribute>(ScanMemberFlags)) {
                Register(getter.Attribute.Type ?? getter.Type, getter.Type, () => getter.GetValue(instance), 
                    getter.Attribute.Lifetime, getter.Attribute.Name ?? getter.Name);
            }
        }
    }
}