using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;

namespace Betauer.DI {
    public class ContainerBuilder {
        private readonly List<IProvider> _pendingToBuild = new();
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
            Register(new SingletonInstanceProvider(typeof(T), instance.GetType(), instance, name, primary));
            return this;
        }

        public ContainerBuilder Static(Type type, object instance, string? name = null, bool primary = false) {
            Register(new SingletonInstanceProvider(type, instance.GetType(), instance, name, primary));
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
            if (lifetime == Lifetime.Singleton) Register(new SingletonFactoryProvider(registeredType, type, factory, name, primary, lazy));
            else Register(new TransientFactoryProvider(registeredType, type, factory, name, primary));
            return this;
        }
        
        public ContainerBuilder Register(IProvider builder) {
            _pendingToBuild.Add(builder);
            return this;
        }

        public ContainerBuilder Scan(IEnumerable<Assembly> assemblies, Func<Type, bool>? predicate = null) {
            assemblies.ForEach(assembly => Scan(assembly, predicate));
            return this;
        }

        public ContainerBuilder Scan(Assembly assembly, Func<Type, bool>? predicate = null) {
            Scan(assembly.GetTypes(), predicate);
            return this;
        }

        public ContainerBuilder Scan(IEnumerable<Type> types, Func<Type, bool>? predicate = null) {
            if (predicate != null) types = types.Where(predicate);
            types.ForEach(type => Scan(type));
            return this;
        }

        public ContainerBuilder Scan<T>() => Scan(typeof(T));

        public ContainerBuilder Scan(Type type) => _Scan(type, null);

        private ContainerBuilder _Scan(Type type, HashSet<Type>? stack) {
            // Look up for [Scan(typeof(...)]
            foreach (var importAttribute in type.GetAttributes<ScanAttribute>()) {
                stack ??= new HashSet<Type>();
                stack.Add(type);
                importAttribute.Types
                    .Where(typeToImport => !stack.Contains(typeToImport))
                    .ForEach(typeToImport => _Scan(typeToImport, stack));
            }
            
            // Look up for [Service]
            if (type.GetAttribute<ServiceAttribute>() is { } serviceAttr) {
                if (type.HasAttribute<ConfigurationAttribute>()) 
                    throw new Exception("Can't use [Configuration] and [Service] in the same class");
                var registeredType = serviceAttr.Type ?? type;
                var name = serviceAttr.Name;
                var primary = serviceAttr.Primary || type.HasAttribute<PrimaryAttribute>();
                var lazy = serviceAttr.Lazy || type.HasAttribute<LazyAttribute>();
                Func<object> factory = () => Activator.CreateInstance(type);
                Register(registeredType, type, factory, serviceAttr.Lifetime, name, primary, lazy);
            } else {
                // No [Service] present in the class, check for [Configuration]
                if (type.HasAttribute<ConfigurationAttribute>()) RegisterConfigurationServices(type, null);
            }
            return this;
        }

        public ContainerBuilder ScanConfiguration(params object[] instances) {
            instances.ForEach(instance => RegisterConfigurationServices(instance.GetType(), instance));
            return this;
        }

        private const BindingFlags ScanMemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private void RegisterConfigurationServices(Type configurationType, object? instance) {
            var conf = instance;
            // No need to use GetGettersCached, this reflection scan is only done once
            foreach (var getter in configurationType.GetGetters<ServiceAttribute>(MemberTypes.Method | MemberTypes.Property, ScanMemberFlags)) {
                var serviceAttr = getter.GetterAttribute;
                var type = getter.Type;
                var registeredType = serviceAttr.Type ?? type;
                var name = serviceAttr.Name?? getter.Name;
                var primary = serviceAttr.Primary || getter.MemberInfo.HasAttribute<PrimaryAttribute>();
                var lazy = serviceAttr.Lazy || getter.MemberInfo.HasAttribute<LazyAttribute>();
                Func<object> factory = () => {
                    conf ??= Activator.CreateInstance(configurationType);
                    return getter.GetValue(conf);
                };
                Register(registeredType, type, factory, serviceAttr.Lifetime, name , primary, lazy);
            }
        }
    }
}