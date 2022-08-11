using System;
using System.Collections.Generic;
using System.Reflection;

namespace Betauer.DI {
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ScanAttribute : Attribute {
        public Type[] Types { get; set; }

        public ScanAttribute(params Type[] types) {
            Types = types;
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
        public bool Lazy { get; set; } = false;
        public Lifetime Lifetime { get; set; } = Lifetime.Singleton;

        public ServiceAttribute() {
        }

        public ServiceAttribute(Lifetime lifetime) {
            Lifetime = lifetime;
        }

        public ServiceAttribute(string name) {
            Name = name;
        }
        public ServiceAttribute(Type type) {
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class PrimaryAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class LazyAttribute : Attribute {
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

        public ContainerBuilder Singleton<T>(string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Register<T, T>(Activator.CreateInstance<T>, Lifetime.Singleton, name, primary, lazy);
        }

        public ContainerBuilder Singleton<T>(Func<T> factory, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Register<T, T>(factory, Lifetime.Singleton, name, primary, lazy);
        }

        public ContainerBuilder Singleton<TI, T>(Func<T> factory, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Register<TI, T>(factory, Lifetime.Singleton, name, primary, lazy);
        }

        public ContainerBuilder Singleton<TI, T>(string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Register<TI, T>(Activator.CreateInstance<T>, Lifetime.Singleton, name, primary, lazy);
        }

        public ContainerBuilder Transient<T>(string? name = null, bool primary = false) where T : class {
            return Register<T, T>(Activator.CreateInstance<T>, Lifetime.Transient, name, primary, false);
        }

        public ContainerBuilder Transient<T>(Func<T> factory, string? name = null, bool primary = false) where T : class {
            return Register<T, T>(factory, Lifetime.Transient, name, primary, false);
        }

        public ContainerBuilder Transient<TI, T>(Func<T> factory, string? name = null, bool primary = false) where T : class {
            return Register<TI, T>(factory, Lifetime.Transient, name, primary, false);
        }

        public ContainerBuilder Transient<TI, T>(string? name = null, bool primary = false) where T : class {
            return Register<TI, T>(Activator.CreateInstance<T>, Lifetime.Transient, name, primary, false);
        }

        public ContainerBuilder Service<T>(Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Register<T, T>(Activator.CreateInstance<T>, lifetime, name, primary, lazy);
        }

        public ContainerBuilder Service<T>(Func<T> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Register<T, T>(factory, lifetime, name, primary, lazy);
        }

        public ContainerBuilder Service<TI, T>(Func<T> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Register<TI, T>(factory, lifetime, name, primary, lazy);
        }

        public ContainerBuilder Service<TI, T>(Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Register<TI, T>(Activator.CreateInstance<T>, lifetime, name, primary, lazy);
        }

        public ContainerBuilder Register<TI, T>(Func<T> factory, Lifetime lifetime, string? name, bool primary, bool lazy = false) where T : class {
            Register(typeof(TI), typeof(T), factory, lifetime, name, primary, lazy);
            return this;
        }

        public ContainerBuilder Register(Type registeredType, Type type, Func<object> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) {
            if (lifetime == Lifetime.Singleton) Register(new SingletonProvider(registeredType, type, factory, name, primary, lazy));
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


        private static bool Has<T>(MemberInfo type) => Attribute.GetCustomAttribute(type, typeof(T), false) is T;
        
        private ContainerBuilder _Scan(Type type, HashSet<Type>? stack) {
            // Look up for [Scan(typeof(...)]
            foreach (var importAttribute in Attribute.GetCustomAttributes(type, typeof(ScanAttribute), false)) {
                stack ??= new HashSet<Type>();
                stack.Add(type);
                foreach (var typeToImport in ((ScanAttribute)importAttribute).Types) {
                    if (!stack.Contains(typeToImport)) _Scan(typeToImport, stack);
                }
            }
            
            // Look up for [Service]
            if (Attribute.GetCustomAttribute(type, typeof(ServiceAttribute), false) is ServiceAttribute serviceAttr) {
                if (Has<ConfigurationAttribute>(type)) 
                    throw new Exception("Can't use [Configuration] and [Service] in the same class");
                var registeredType = serviceAttr.Type ?? type;
                var name = serviceAttr.Name;
                var primary = serviceAttr.Primary || Has<PrimaryAttribute>(type);
                var lazy = serviceAttr.Lazy || Has<LazyAttribute>(type);
                Func<object> factory = () => Activator.CreateInstance(type);
                Register(registeredType, type, factory, serviceAttr.Lifetime, name, primary, lazy);
            } else {
                // No [Service] present in the class, check for [Configuration]
                if (Has<ConfigurationAttribute>(type)) RegisterConfigurationServices(type, null);
            }
            return this;
        }

        public ContainerBuilder ScanConfiguration(params object[] instances) {
            foreach (var instance in instances) RegisterConfigurationServices(instance.GetType(), instance);
            return this;
        }

        private const BindingFlags ScanMemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private void RegisterConfigurationServices(Type configurationType, object? instance) {
            var conf = instance;
            foreach (var getter in configurationType.GetPropertiesAndMethods<ServiceAttribute>(ScanMemberFlags)) {
                var serviceAttr = getter.Attribute;
                var type = getter.Type;
                var registeredType = serviceAttr.Type ?? type;
                var name = serviceAttr.Name?? getter.Name;
                var primary = serviceAttr.Primary || Has<PrimaryAttribute>(getter.MemberInfo);
                var lazy = serviceAttr.Lazy || Has<LazyAttribute>(getter.MemberInfo);
                Func<object> factory = () => {
                    conf ??= Activator.CreateInstance(configurationType);
                    return getter.GetValue(conf);
                };
                Register(registeredType, type, factory, serviceAttr.Lifetime, name , primary, lazy);
            }
        }
    }
}