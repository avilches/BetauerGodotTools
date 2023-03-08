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
        return Register<T, T>(LambdaCtor<T>.CreateInstance, Lifetime.Transient, name, primary, false);
    }

    public ContainerBuilder Transient<T>(Func<T> factory, string? name = null, bool primary = false) where T : class {
        return Register<T, T>(factory, Lifetime.Transient, name, primary, false);
    }

    public ContainerBuilder Transient<TI, T>(Func<T> factory, string? name = null, bool primary = false) where T : class {
        return Register<TI, T>(factory, Lifetime.Transient, name, primary, false);
    }

    public ContainerBuilder Transient<TI, T>(string? name = null, bool primary = false) where T : class {
        return Register<TI, T>(LambdaCtor<T>.CreateInstance, Lifetime.Transient, name, primary, false);
    }

    public ContainerBuilder Service<T>(Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) where T : class {
        Func<T> factory = lifetime == Lifetime.Singleton ? Activator.CreateInstance<T> : LambdaCtor<T>.CreateInstance;
        return Register<T, T>(factory, lifetime, name, primary, lazy);
    }

    public ContainerBuilder Service<T>(Func<T> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) where T : class {
        return Register<T, T>(factory, lifetime, name, primary, lazy);
    }

    public ContainerBuilder Service<TI, T>(Func<T> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) where T : class {
        return Register<TI, T>(factory, lifetime, name, primary, lazy);
    }

    public ContainerBuilder Service<TI, T>(Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) where T : class {
        Func<T> factory = lifetime == Lifetime.Singleton ? Activator.CreateInstance<T> : LambdaCtor<T>.CreateInstance;
        return Register<TI, T>(factory, lifetime, name, primary, lazy);
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

    public ContainerBuilder RegisterServiceAndAddFactory(Type registeredType, Type type, Func<object> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) {
        var provider = Provider.Create(registeredType, type, factory, lifetime, name, primary, lazy);
        Register(provider);

        Type factoryType = typeof(IFactory<>).MakeGenericType(type);
        object CustomFactory() => FactoryProvider.Create(type, provider);
        Register(factoryType, factoryType, CustomFactory, Lifetime.Singleton, $"IFactory<{type}>:{name}", false, true);
        return this;
    }

    public ContainerBuilder RegisterCustomFactory(Type type, Func<object> customFactory, string? factoryName = null, bool primary = false, bool lazy = false) {
        Type factoryType = typeof(IFactory<>).MakeGenericType(type);
        // Register the Custom Factory
        // The providerCustomFactory is used later to get the factory and inject fields inside it
        var innerType = typeof(IFactory<>).MakeGenericType(factoryType);
        var providerCustomFactory = Provider.Create(innerType, innerType, customFactory.Memoize(), Lifetime.Singleton, null, false, lazy);
        Register(providerCustomFactory);

        // Register instance factory class
        // It maps the service type "type" with the instance factory method in GetFromProviderFactory
        // The GetFromProviderFactory resolve the real factory using the custom factory, which is executed only once.
        // Then, with the real factory instance (which we don't know what type is it, but we know it implements IFactory<type>)
        // we call to the "Get()" method using reflection (through FastMethodInfo, which is very fast because it compiles a lambda to call to the method)
        // The instance is scanned for inject and [PostCreate] as usual.
        var getFromProviderFactory = new FastGetFromProvider(type, providerCustomFactory);
        var providerFactory = Provider.Create(type, type, getFromProviderFactory.Get, Lifetime.Transient, null, false, true);
        Register(providerFactory);

        // Finally, we register a IFactory<> so the user can use the real factory.
        // Instead of expose the original custom factory, it creates a new Factory<type> where every call to Get()
        // invokes the original factory through the providerFactory, which returns an instance and scan for [Inject] and [PostCreate]
        Register(factoryType, factoryType, () => {
            object ProviderFactory() => providerFactory.Get();
            return Factory.Create(type, ProviderFactory);
        }, Lifetime.Singleton, factoryName, primary, true);
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

        if (type.GetAttribute<FactoryAttribute>() is FactoryAttribute factoryAttribute) {
            if (type.HasAttribute<ConfigurationAttribute>()) throw new Exception("Can't use [Factory] and [Configuration] in the same class");
            if (type.HasAttribute<ServiceAttribute>()) throw new Exception("Can't use [Factory] and [Service] in the same class");
            RegisterCustomFactoryFromClass(type, factoryAttribute);
            
        } else if (type.GetAttribute<ServiceAttribute>() is ServiceAttribute serviceAttr) {
            if (type.HasAttribute<ConfigurationAttribute>()) throw new Exception("Can't use [Service] and [Configuration] in the same class");
            RegisterServiceFromClass(type, serviceAttr);
            
        } else if (type.GetAttribute<ConfigurationAttribute>() is ConfigurationAttribute configurationAttribute) {
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
        var serviceGetters = configuration.GetType().GetGetters<ServiceAttribute>(MemberTypes.Method | MemberTypes.Property, ScanMemberFlags);
        foreach (var getter in serviceGetters) RegisterServiceFromGetter(configuration, getter);
        
        var factoryGetters = configuration.GetType().GetGetters<FactoryAttribute>(MemberTypes.Method | MemberTypes.Property, ScanMemberFlags);
        foreach (var getter in factoryGetters) RegisterCustomFactoryFromGetter(configuration, getter);
    }

    private void RegisterServiceFromClass(Type type, ServiceAttribute serviceAttr) {
        var registeredType = serviceAttr.Type ?? type;
        var name = serviceAttr.Name;
        var primary = serviceAttr.Primary || type.HasAttribute<PrimaryAttribute>();
        var lazy = serviceAttr.Lazy || type.HasAttribute<LazyAttribute>();
        var lifetime = serviceAttr.Lifetime;
        object Factory() => Activator.CreateInstance(type)!;
        RegisterServiceAndAddFactory(registeredType, type, Factory, lifetime, name, primary, lazy);
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
        RegisterServiceAndAddFactory(registeredType, type, Factory, lifetime, name, primary, lazy);
    }

    private void RegisterCustomFactoryFromClass(Type type, FactoryAttribute factoryAttribute) {
        if (!type.ImplementsInterface(typeof(IFactory<>))) {
            throw new Exception($"Class {type.FullName} with [Factory] attribute must implement IFactory<T>");
        }
        var genericType = type.GetGenericArguments()[0];
        var primary = factoryAttribute.Primary || type.HasAttribute<PrimaryAttribute>();
        var lazy = factoryAttribute.Lazy || type.HasAttribute<LazyAttribute>();
        object Factory() => Activator.CreateInstance(type)!;
        RegisterCustomFactory(genericType, Factory, factoryAttribute.Name, primary, lazy);
    }

    private void RegisterCustomFactoryFromGetter(object configuration, IGetter<FactoryAttribute> getter) {
        if (!getter.Type.ImplementsInterface(typeof(IFactory<>))) {
            throw new Exception("Member " + getter + " with [Factory] attribute must implement IFactory<T>");
        }
        var factoryAttribute = getter.GetterAttribute;
        var genericType = getter.Type.GetGenericArguments()[0];
        var primary = factoryAttribute.Primary || getter.MemberInfo.HasAttribute<PrimaryAttribute>();
        var name = factoryAttribute.Name ?? getter.Name;
        var lazy = factoryAttribute.Lazy || getter.MemberInfo.HasAttribute<LazyAttribute>();
        object Factory() => getter.GetValue(configuration)!;
        RegisterCustomFactory(genericType, Factory, name, primary, lazy);
    }

    internal class FastGetFromProvider {
        private readonly Type _type;
        private readonly IProvider _provider;
        private FastMethodInfo? _providerGetMethod;

        public FastGetFromProvider(Type type, IProvider provider) {
            _type = type;
            _provider = provider;
        }

        public object Get() {
            var factoryInstance = _provider.Get();
            _providerGetMethod ??= FindGetMethod(factoryInstance);
            return _providerGetMethod.Invoke(factoryInstance)!;
        }

        private FastMethodInfo FindGetMethod(object factoryInstance) {
            return new FastMethodInfo(
                factoryInstance.GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .First(m =>
                        m.Name == nameof(IFactory<object>.Get) &&
                        m.GetParameters().Length == 0 &&
                        m.ReturnType == _type));
        }
    }


    internal abstract class CustomFactoryProvider {
        public abstract object Get();
        internal static CustomFactoryProvider Create(Type type, IProvider provider) {
            var factoryType = typeof(CustomFactoryProvider<>).MakeGenericType(type);
            CustomFactoryProvider instance = (CustomFactoryProvider)Activator.CreateInstance(factoryType, provider)!;
            return instance;
        }
    }

    internal class CustomFactoryProvider<T> : CustomFactoryProvider {
        private readonly IProvider _customFactory;

        public CustomFactoryProvider(IProvider customFactory) {
            _customFactory = customFactory;
        }

        public override object Get() {
            IFactory<T> factory = (IFactory<T>)_customFactory.Get();
            return factory.Get();
        }
    }

    
}