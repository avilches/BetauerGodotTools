using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Core;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;

namespace Betauer.DI;

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
        var provider = Provider.Create(registeredType, type, factory, lifetime, name, primary, lazy);
        Register(provider);
        return this;
    }

    public ContainerBuilder Register(IProvider provider) {
        _pendingToBuild.Add(provider);
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
        ScanForScanAttributes(type, stack);

        var isConfiguration = type.HasAttribute<ConfigurationAttribute>();
        if (type.GetAttribute<ServiceAttribute>() is ServiceAttribute serviceAttr) {
            if (isConfiguration) {
                throw new Exception("Can't use [Configuration] and [Service] in the same class");
            }
            RegisterServiceFromClass(type, serviceAttr);
            
        } else if (isConfiguration) {
            var configuration = Activator.CreateInstance(type);
            ScanServicesFromConfigurationInstance(configuration!);
        }
        return this;
    }

    private const BindingFlags ScanMemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public ContainerBuilder ScanConfiguration(params object[] instances) {
        foreach (var configuration in instances) {
            ScanForScanAttributes(configuration.GetType(), null);
            ScanServicesFromConfigurationInstance(configuration);
        }
        return this;
    }

    private void ScanForScanAttributes(Type type, HashSet<Type>? stack) {
        foreach (var importAttribute in type.GetAttributes<ScanAttribute>()) {
            stack ??= new HashSet<Type>();
            stack.Add(type);
            importAttribute.GetType().GetGenericArguments()
                .Where(typeToImport => !stack.Contains(typeToImport))
                .ForEach(typeToImport => _Scan(typeToImport, stack));
        }
    }

    private void ScanServicesFromConfigurationInstance(object configuration) {
        // No need to use GetGettersCached, this reflection scan is only done once
        var getters = configuration.GetType().GetGetters<ServiceAttribute>(MemberTypes.Method | MemberTypes.Property, ScanMemberFlags);
        foreach (var getter in getters) RegisterServiceFromGetter(configuration, getter);
    }

    private void RegisterServiceFromClass(Type type, ServiceAttribute serviceAttr) {
        var registeredType = serviceAttr.Type ?? type;
        var name = serviceAttr.Name;
        var primary = serviceAttr.Primary || type.HasAttribute<PrimaryAttribute>();
        var lazy = serviceAttr.Lazy || type.HasAttribute<LazyAttribute>();
        var lifetime = serviceAttr.Lifetime;
        object Factory() => Activator.CreateInstance(type)!;
        DetectFactoryTypeAndRegisterService(registeredType, type, Factory, lifetime, name, primary, lazy);
    }

    private void RegisterServiceFromGetter(object configuration, IGetter<ServiceAttribute> getter) {
        var serviceAttr = getter.GetterAttribute;
        var type = getter.Type;
        var registeredType = serviceAttr.Type ?? type;
        var name = serviceAttr.Name ?? getter.Name;
        var primary = serviceAttr.Primary || getter.MemberInfo.HasAttribute<PrimaryAttribute>();
        var lazy = serviceAttr.Lazy || getter.MemberInfo.HasAttribute<LazyAttribute>();
        var lifetime = serviceAttr.Lifetime;
        object Factory() => getter.GetValue(configuration)!;
        DetectFactoryTypeAndRegisterService(registeredType, type, Factory, lifetime, name, primary, lazy);
    }

    private void DetectFactoryTypeAndRegisterService(Type registeredType, Type type, Func<object> factory, Lifetime lifetime, string name, bool primary, bool lazy) {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IFactory<>)) {
            var genericType = type.GetGenericArguments()[0];
            RegisterServiceFromCustomFactory(registeredType, genericType, factory, name, primary, lazy);
        } else {
            // type == typeof(Node)
            // IFactory<object>().Get() -> new Node()            
            RegisterServiceFromFactory(registeredType, type, factory, lifetime, name, primary, lazy);
        }
    }

    private class Hidden<T> { }
    private void RegisterServiceFromCustomFactory(Type factoryType, Type type, Func<object> customFactory, string name, bool primary, bool lazy) {
        // Register the Factory Factory
        // It's added to the container wrapped with Hidden<> class, so it can't be accesses from outside
        // The providerCustomFactory can be used to get the factory and inject fields inside it
        var innerType = typeof(Hidden<>).MakeGenericType(factoryType);
        var providerCustomFactory = Provider.Create(innerType, innerType, customFactory.Memoize(), Lifetime.Singleton, null, false, lazy);
        Register(providerCustomFactory);

        // Register instance factory class
        // It maps the service type "type" with the instance factory method in GetFromProviderFactory
        // The GetFromProviderFactory resolve the real factory using the custom factory, which is executed only once.
        // Then, with the real factory instance (which we don't know what type is it, but we know it implements IFactory<type>)
        // we call to the "Get()" method using reflection (through FastMethodInfo, which is very fast because it compiles a lambda to call to the method)
        // The instance is scanned for inject and [PostCreate] as usual.
        var getFromProviderFactory = new FastGetFromProvider(type, _container, providerCustomFactory);
        var providerFactory = Provider.Create(type, type, getFromProviderFactory.Get, Lifetime.Transient, null, false, true);
        Register(providerFactory);

        // Finally, we register a IFactory<> so the user can use the real factory.
        // Instead of expose the original custom factory, it creates a new Factory<type> where every call to Get()
        // invokes the original factory through the providerFactory, which returns an instance and scan for [Inject] and [PostCreate]
        Register(factoryType, factoryType, () => {
            object Factory() => providerFactory.Get();
            return ContainerBuilder.Factory.Create(type, Factory);
        }, Lifetime.Singleton, name, primary, true);
    }

    private void RegisterServiceFromFactory(Type registeredType, Type type, Func<object> factory, Lifetime lifetime, string name, bool primary, bool lazy) {
        var provider = Provider.Create(registeredType, type, factory, lifetime, name, primary, lazy);
        Register(provider);

        Type factoryType = typeof(IFactory<>).MakeGenericType(type);
        object CustomFactory() => FactoryProvider.CreateFactoryFromProvider(type, provider);
        Register(factoryType, factoryType, CustomFactory, Lifetime.Singleton, $"IFactory<{type}>:{name}", false, true);
    }

    internal class FastGetFromProvider {
        private readonly Type _type;
        private readonly Container _container;
        private readonly IProvider _provider;
        private FastMethodInfo? _providerGetMethod;

        public FastGetFromProvider(Type type, Container container, IProvider provider) {
            _type = type;
            _container = container;
            _provider = provider;
        }

        public object Get() {
            var factoryInstance = _provider.Get();
            _providerGetMethod ??= new FastMethodInfo(factoryInstance.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .First(m =>
                    m.Name == nameof(IFactory<object>.Get) &&
                    m.GetParameters().Length == 0 &&
                    m.ReturnType == _type));
            return _providerGetMethod.Invoke(factoryInstance)!;
        }
    }


    public class Factory {
        protected Func<object> _factory;

        internal void Use(Func<object> factory) {
            if (_factory != null) throw new Exception("Can't update the factory: it already exists");
            _factory = factory;
        }

        public static Factory Create(Type genericType, Func<object> factory) {
            var type = typeof(Factory<>).MakeGenericType(genericType);
            Factory instance = (Factory)Activator.CreateInstance(type)!;
            instance.Use(factory);
            return instance;
        }
    }

    public class Factory<T> : Factory, IFactory<T> {
        public T Get() {
            return (T)_factory.Invoke();
        }
    }

    internal class FactoryProvider : IFactory<object> {
        public IProvider Provider { get; }

        private FactoryProvider(IProvider provider) {
            Provider = provider;
        }

        public object Get() {
            return Provider.Get();
        }

        internal static object CreateFactoryFromProvider(Type type, IProvider provider) {
            var factoryType = typeof(FactoryProvider<>).MakeGenericType(type);
            var ctor = factoryType.GetConstructors()[0];
            return ctor.Invoke(new object [] { provider });
        }
    }

    internal class FactoryProvider<T> : IFactory<T> {
        private readonly IProvider _factory;

        public FactoryProvider(IProvider factory) {
            _factory = factory;
        }

        public T Get() => (T)_factory.Get();
    }
    
}