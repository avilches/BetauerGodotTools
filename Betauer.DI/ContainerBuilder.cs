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
        public bool Primary { get; set; } = false;
        public Lifetime Lifetime { get; set; } = Lifetime.Singleton;

        public ServiceAttribute() {
        }

        public ServiceAttribute(Lifetime lifetime, string name = null, bool primary = false) {
            Lifetime = lifetime;
            Name = name;
            Primary = primary;
        }

        public ServiceAttribute(Lifetime lifetime, Type type) {
            Lifetime = lifetime;
            Type = type;
        }

        public ServiceAttribute(string name, Lifetime lifetime = Lifetime.Singleton, bool primary = false) {
            Name = name;
            Lifetime = lifetime;
            Primary = primary;
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
            var toBuild = new List<IProvider>(_pendingToBuild);
            _pendingToBuild.Clear();
            _container.Build(toBuild);
            return _container;
        }

        public ContainerBuilder Static<T>(T instance, string? name = null, bool primary = false) where T : class {
            Register(new StaticProvider(typeof(T), instance.GetType(), instance, name, primary));
            return this;
        }

        public ContainerBuilder Static(Type type, object instance, string? name = null, bool primary = false) {
            Register(new StaticProvider(type, instance.GetType(), instance, name, primary));
            return this;
        }

        public ContainerBuilder Singleton<T>(string? name = null, bool primary = false) where T : class {
            return Register<T, T>(Activator.CreateInstance<T>, Lifetime.Singleton, name, primary);
        }

        public ContainerBuilder Singleton<T>(Func<T> factory, string? name = null, bool primary = false) where T : class {
            return Register<T, T>(factory, Lifetime.Singleton, name, primary);
        }

        public ContainerBuilder Singleton<TI, T>(string? name = null, bool primary = false) where T : class {
            return Register<TI, T>(Activator.CreateInstance<T>, Lifetime.Singleton, name, primary);
        }

        public ContainerBuilder Transient<T>(string? name = null, bool primary = false) where T : class {
            return Register<T, T>(Activator.CreateInstance<T>, Lifetime.Transient, name, primary);
        }

        public ContainerBuilder Transient<T>(Func<T> factory, string? name = null, bool primary = false) where T : class {
            return Register<T, T>(factory, Lifetime.Transient, name, primary);
        }

        public ContainerBuilder Transient<TI, T>(string? name = null, bool primary = false) where T : class {
            return Register<TI, T>(Activator.CreateInstance<T>, Lifetime.Transient, name, primary);
        }

        public ContainerBuilder Service<T>(Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false) where T : class {
            return Register<T, T>(Activator.CreateInstance<T>, lifetime, name, primary);
        }

        public ContainerBuilder Service<T>(Func<T> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false) where T : class {
            return Register<T, T>(factory, lifetime, name, primary);
        }

        public ContainerBuilder Service<TI, T>(Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false) where T : class {
            return Register<TI, T>(Activator.CreateInstance<T>, lifetime, name, primary);
        }

        public ContainerBuilder Register<TI, T>(Func<T> factory, Lifetime lifetime, string? name, bool primary) where T : class {
            Register(typeof(TI), typeof(T), factory, lifetime, name, primary);
            return this;
        }

        public ContainerBuilder Register(Type registeredType, Type type, Func<object> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false) {
            if (lifetime == Lifetime.Singleton) Register(new SingletonProvider(registeredType, type, factory, name, primary));
            else Register(new TransientProvider(registeredType, type, factory, name, primary));
            return this;
        }
        
        public ContainerBuilder Register(IProvider builder) {
            _pendingToBuild.AddLast(builder);
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
                if (Attribute.GetCustomAttribute(type, typeof(ConfigurationAttribute),
                        false) is ConfigurationAttribute) {
                    throw new Exception("Can't use [Configuration] and [Service] in the same class");
                }
                var registeredType = serviceAttr.Type ?? type;
                var name = serviceAttr.Name;
                Register(registeredType, type, () => Activator.CreateInstance(type), serviceAttr.Lifetime, name, serviceAttr.Primary);
            } else {
                // No [Service] present in the class, check for [Configuration]
                if (Attribute.GetCustomAttribute(type, typeof(ConfigurationAttribute), false) is ConfigurationAttribute) {
                    RegisterConfigurationServices(type, null);
                }
            }
            return this;
        }

        public ContainerBuilder ScanConfiguration(params object[] instances) {
            foreach (var instance in instances) RegisterConfigurationServices(instance.GetType(), instance);
            return this;
        }

        private const BindingFlags ScanMemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private void RegisterConfigurationServices(Type type, object? instance) {
            var conf = instance;
            foreach (var getter in type.GetPropertiesAndMethods<ServiceAttribute>(ScanMemberFlags)) {
                Register(getter.Attribute.Type ?? getter.Type, getter.Type, () => {
                        conf ??= Activator.CreateInstance(type); 
                        return getter.GetValue(conf);
                    }, 
                    getter.Attribute.Lifetime, getter.Attribute.Name ?? getter.Name, getter.Attribute.Primary);
            }
        }
    }
}