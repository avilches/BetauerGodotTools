using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Pool.Basic;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Logging;
using Betauer.Tools.Reflection;

namespace Betauer.DI;

public partial class Container {
    private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Container));
    private readonly Dictionary<Type, IProvider> _registry = new();
    private readonly Dictionary<string, IProvider> _registryByName = new();
    private readonly Dictionary<Type, IProvider> _fallbackByType = new();
    private readonly Injector _injector;
    public bool CreateIfNotFound { get; set; }
    public event Action<Lifetime, object> OnCreated;
    public event Action<object> OnPostInject;

    private readonly BasicPool<ResolveContext> _resolveContextPool;
    private ResolveContext GetResolveContext() => _resolveContextPool.Get();
    
    public Container() {
        _injector = new Injector(this);
        _resolveContextPool = new(CreateResolveContext);        
        
        // Adding the Container in the Container allows to use [Inject] Container...
        Add(Provider.Static(this));
    }

    private ResolveContext CreateResolveContext() {
        ResolveContext? context = null;
        context = new ResolveContext(this, () => {
            context!.Clear();
            _resolveContextPool!.Return(context);
        });
        return context;
    } 

    public Builder CreateBuilder() => new Builder(this);

    public Container Build(ICollection<IProvider> providers) {
        var context = GetResolveContext();
        providers
            .ForEach(provider => AddToRegistry(provider));
        providers
            .Where(provider => provider is ISingletonProvider { Lazy: false, IsInstanceCreated: false })
            .ForEach(provider => {
                Logger.Debug($"Initializing {provider.Lifetime}:{provider.ProviderType} | Name: {provider.Name}");
                provider.Resolve(context);
            });
        context.End();
        return this;
    }

    public IProvider Add(IProvider provider) {
        AddToRegistry(provider);
        if (provider is ISingletonProvider { Lazy: false, IsInstanceCreated: false }) {
            Logger.Debug($"Initializing {provider.Lifetime}:{provider.ProviderType} | Name: {provider.Name}");
            provider.Get();
        }
        return provider;
    }
    /// <summary>
    /// Register by type only always overwrite. That means register the same type twice will not fail: the second
    /// one will replace the first one.
    /// Register by name will try to register by type too, but only if the type doesn't exist before
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="DuplicateNameException"></exception>
    private IProvider AddToRegistry(IProvider provider) {
        var name = provider.Name;
        if (name != null) {
            if (_registryByName.ContainsKey(name)) throw new DuplicateServiceException(name);
                _registryByName[name] = provider;
                if (provider.Primary || !_fallbackByType.ContainsKey(provider.RegisterType)) {
                    _fallbackByType[provider.RegisterType] = provider;
                    Logger.Debug($"Registered {provider.Lifetime}:{provider.ProviderType} | Name: {name} | Added fallback: {provider.RegisterType.Name}");
                } else {
                    Logger.Debug($"Registered {provider.Lifetime}:{provider.ProviderType} | Name: {name}");
                }
        } else {
            if (_registry.ContainsKey(provider.RegisterType)) throw new DuplicateServiceException(provider.RegisterType);
            _registry[provider.RegisterType] = provider;
            Logger.Debug($"Registered {provider.Lifetime}:{provider.ProviderType}. Name: {provider.RegisterType.Name}");
        }
        provider.Container = this;
        return provider;
    }

    public bool Contains(string name) => _registryByName.ContainsKey(name);
    public bool Contains<T>() => Contains(typeof(T));
    public bool Contains(Type type) => _registry.ContainsKey(type) || _fallbackByType.ContainsKey(type);

    public IProvider GetProvider(string name) => TryGetProvider(name, out var found) ? found! : throw new ServiceNotFoundException(name);
    public IProvider GetProvider<T>() => GetProvider(typeof(T));
    public IProvider GetProvider(Type type) => TryGetProvider(type, out var found) ? found! : throw new ServiceNotFoundException(type);

    public bool TryGetProvider(string name, [MaybeNullWhen(false)] out IProvider provider) => _registryByName.TryGetValue(name, out provider);
    public bool TryGetProvider<T>(out IProvider? provider) => TryGetProvider(typeof(T), out provider);
    public bool TryGetProvider(Type type, out IProvider? provider) {
        var found = _registry.TryGetValue(type, out provider);
        if (!found) found = _fallbackByType.TryGetValue(type, out provider);
        if (!found) provider = null;
        return found;
    }

    public T Resolve<T>() => (T)Resolve(typeof(T));
    public object Resolve(Type type) {
        if (TryResolve(type, out var instance)) {
            return instance;
        }
        throw new ServiceNotFoundException(type);
    }

    public T Resolve<T>(string name) => (T)Resolve(name);
    public object Resolve(string name) {
        if (TryResolve(name, out var instance)) {
            return instance;
        }
        throw new ServiceNotFoundException(name);
    }

    public bool TryResolve<T>(out T instance) {
        var result = TryResolve(typeof(T), out var o);
        instance = (T)o;
        return result;
    }

    public bool TryResolve(Type type, out object instance) {
        if (TryGetProvider(type, out IProvider? provider)) {
            instance = provider!.Get();
            return true;
        }
        if (CreateIfNotFound) {
            AddToRegistry(Provider.Create(type, type, Lifetime.Transient, null));
            // ReSharper disable once TailRecursiveCall
            return TryResolve(type, out instance);
        }
        instance = null;
        return false;
    }

    public bool TryResolve<T>(string name, out T instance) {
        var result = TryResolve(name, out var o);
        instance = (T)o;
        return result;
    }

    public bool TryResolve(string name, out object instance) {
        if (TryGetProvider(name, out IProvider? provider)) {
            instance = provider!.Get();
            return true;
        }
        instance = null;
        return false;
    }
  
    public T ResolveOr<T>(Func<T> or) => TryResolve(typeof(T), out var instance) ? (T)instance : or();
    public T ResolveOr<T>(string name, Func<T> or) => TryResolve(name, out T instance) ? instance : or();

    public List<T> GetAllInstances<T>() {
        var instances = _registry.Values
            .Concat(_fallbackByType.Values)
            .OfType<ISingletonProvider>()
            .Where(provider => typeof(T).IsAssignableFrom(provider.ProviderType))
            .Select(provider => provider.Get())
            .OfType<T>()
            .ToList();
        return instances;
    }

    internal void ExecuteOnCreated(Lifetime lifetime, object instance) {
        OnCreated?.Invoke(lifetime, instance);
    }

    internal void ExecutePostInjectMethods<T>(T instance) {
        if (instance.GetType().ImplementsInterface(typeof(IInjectable))) {
            ((IInjectable)instance).PostInject();
        }
        OnPostInject?.Invoke(instance);
    }

    internal object Resolve(Provider provider) {
        var context = GetResolveContext();
        var instance = provider.Resolve(context);
        context.End();
        return instance;

    }

    internal object Resolve(Type type, ResolveContext context) {
        if (TryGetProvider(type, out IProvider? provider)) {
            return provider!.Resolve(context);
        }
        if (CreateIfNotFound) {
            AddToRegistry(Provider.Create(type, type, Lifetime.Transient));
            // ReSharper disable once TailRecursiveCall
            return Resolve(type, context);
        }
        throw new ServiceNotFoundException(type);
    }

    internal object Resolve(string name, ResolveContext context) {
        if (TryGetProvider(name, out IProvider? provider)) {
            return provider.Resolve(context);
        }
        throw new ServiceNotFoundException(name);
    }

    public void InjectServices(object o) {
        var context = GetResolveContext();
        InjectServices(Lifetime.Transient, o, context);
        context.End();
        ExecutePostInjectMethods(o);
    }

    internal void InjectServices(Lifetime lifetime, object o, ResolveContext context) {
        _injector.InjectServices(lifetime, o, context);
    }
}