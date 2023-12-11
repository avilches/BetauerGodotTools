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

namespace Betauer.DI;

public partial class Container {
    private static readonly Logger Logger = LoggerFactory.GetLogger<Container>();
    private readonly Dictionary<Type, IProvider> _registryByType = new();
    private readonly Dictionary<string, IProvider> _registryByName = new();
    private readonly List<IProvider> _providers = new();
    private readonly Injector _injector;
    public bool CreateIfNotFound { get; set; }
    public event Action<ProviderResolved> OnCreated;
    public event Action<object>? OnPostInject;

    private readonly BasicPool<ResolveContext> _resolveContextPool;
    private ResolveContext GetResolveContext() => _resolveContextPool.Get();

    private bool _busy = false;
    
    public Container() {
        _injector = new Injector(this);
        _resolveContextPool = new(CreateResolveContext);        
        
        // Adding the Container in the Container allows to use [Inject] Container...
        Add(Provider.Static(this));
    }

    private ResolveContext CreateResolveContext() {
        ResolveContext? context = null;
        context = new ResolveContext(this, () => {
            _resolveContextPool!.Return(context);
        });
        return context;
    } 

    public Builder CreateBuilder() => new Builder(this);

    public Container Build(List<IProvider> providers) {
        if (_busy) throw new InvalidOperationException("Container is busy");
        _busy = true;
        
        providers.ForEach(provider => provider.Container = this);
        var factories = providers.OfType<FactoryProvider>().ToList();
        providers.RemoveAll(provider => provider is FactoryProvider);
        providers.ForEach(AddToRegistry);

        var context = GetResolveContext();
        factories.ForEach(provider => {
            Logger.Debug($"Initializing factory {provider.ProviderType.GetTypeName()}");
            provider.Resolve(context);
        });
        context.End();
        context = GetResolveContext();
        
        providers
            .OfType<ISingletonProvider>()
            .Where(provider => provider is { Lazy: false, IsInstanceCreated: false })
            .ForEach(provider => {
                Logger.Debug($"Initializing non lazy {Lifetime.Singleton}:{provider.ProviderType.GetTypeName()} | Name: \"{provider.Name}\"");
                provider.Resolve(context);
            });
        context.End();
        
        var errors = providers
            .OfType<ISingletonProvider>()
            .Where(provider => provider is { Lazy: true, IsInstanceCreated: true } && (provider.Name == null || !provider.Name.StartsWith("Factory:")))
            .Select(provider => $"- {provider.ProviderType.GetTypeName()} | Name: \"{provider.Name}\"")
            .ToList();
        if (errors.Count > 0) {
            throw new InvalidOperationException("Container initialization failed. These Lazy Singletons are initialized when they shouldn't.\nPlease, remove the Lazy flag or ensure the injection is done using [Inject] ILazy<T> instead of [Inject] T:\n" + string.Join("\n", errors));
        }
        
        _busy = false;
        return this;
    }

    public void Add(IProvider provider) {
        if (_busy) throw new InvalidOperationException("Container is busy");
        _busy = true;
        AddToRegistry(provider);
        if (provider is ISingletonProvider { Lazy: false }) {
            Logger.Debug($"Initializing non lazy {Lifetime.Singleton}:{provider.ProviderType.GetTypeName()} | Name: \"{provider.Name}\"");
            provider.Get();
        }
        _busy = false;
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
    private void AddToRegistry(IProvider provider) {
        var name = provider.Name;
        if (name != null) {
            if (_registryByName.ContainsKey(name)) throw new DuplicateServiceException(name);
                _registryByName[name] = provider;
        } else {
            if (_registryByType.ContainsKey(provider.RegisterType)) throw new DuplicateServiceException(provider.RegisterType);
            _registryByType[provider.RegisterType] = provider;
            Logger.Debug($"Registered {provider.Lifetime}:{provider.ProviderType.GetTypeName()} | Type: {provider.RegisterType.GetTypeName()}");
        }
        _providers.Add(provider);
        provider.Container = this;
    }

    public bool Contains(string name) => _registryByName.ContainsKey(name);
    public bool Contains<T>() => Contains(typeof(T));
    public bool Contains(Type type) => _registryByType.ContainsKey(type);

    public IProvider GetProvider(string name) => TryGetProvider(name, out var found) ? found! : throw new ServiceNotFoundException(name);
    public IProvider GetProvider<T>() => GetProvider(typeof(T));
    public IProvider GetProvider(Type type) => TryGetProvider(type, out var found) ? found! : throw new ServiceNotFoundException(type);

    public bool TryGetProvider(string name, [MaybeNullWhen(false)] out IProvider provider) => _registryByName.TryGetValue(name, out provider);
    public bool TryGetProvider<T>(out IProvider? provider) => TryGetProvider(typeof(T), out provider);
    public bool TryGetProvider(Type type, out IProvider? provider) {
        var found = _registryByType.TryGetValue(type, out provider);
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
            AddToRegistry(new TransientProvider(type, type));
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

    public List<T> GetAllInstances<T>() {
        var context = GetResolveContext();
        return Query<T>(Lifetime.Singleton)
            .Select(provider => provider.Resolve(context))
            .Cast<T>()
            .ToList();
    }

    public List<IProvider> Query<T>(Lifetime? lifetime = null) {
        return Query(typeof(T), lifetime);
    }

    public List<IProvider> Query(Type type, Lifetime? lifetime) {
        return _providers
            .Where(provider => type.IsAssignableFrom(provider.ProviderType) && (!lifetime.HasValue || provider.Lifetime == lifetime))
            .ToList();
    }
  
    public T ResolveOr<T>(Func<T> or) => TryResolve(typeof(T), out var instance) ? (T)instance : or();
    public T ResolveOr<T>(string name, Func<T> or) => TryResolve(name, out T instance) ? instance : or();

    internal void ExecuteOnCreated(ProviderResolved providerResolved) {
        OnCreated?.Invoke(providerResolved);
    }

    internal void ExecutePostInjectMethods<T>(T instance) {
        if (instance is IInjectable injectable) {
            Logger.Debug($"Executing {nameof(IInjectable.PostInject)} in {injectable.GetType().GetTypeName()}. HashCode: {injectable.GetHashCode():X}");
            injectable.PostInject();
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
            AddToRegistry(new TransientProvider(type, type));
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