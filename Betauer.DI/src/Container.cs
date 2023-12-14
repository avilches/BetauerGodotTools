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
    private readonly Dictionary<Type, IProvider> _registryByExposedType = new();
    private readonly Dictionary<string, IProvider> _registryByName = new();
    private readonly List<IProvider> _providers = new();
    public bool CreateIfNotFound { get; set; }
    public event Action<ProviderResolved> OnCreated;
    public event Action<object>? OnPostInject;

    private readonly BasicPool<ResolveContext> _resolveContextPool;

    private bool _busy = false;
    
    public Container() {
        _resolveContextPool = new BasicPool<ResolveContext>(() => new ResolveContext(this));        
        // Adding the Container in the Container allows to use [Inject] Container...
        AddToRegistry(Provider.Static(this));
    }

    public Container Build(Action<Builder> action) {
        var builder = new Builder(this);
        action.Invoke(builder);
        builder.Build();
        return this;
    }

    public T FromContext<T>(Func<ResolveContext, T> action) {
        var context = _resolveContextPool.Get();
        try {
            return action.Invoke(context);
        } finally {
            context.End();
        }
    }

    public void WithContext(Action<ResolveContext> action) {
        var context = _resolveContextPool.Get();
        try {
            action.Invoke(context);
        } finally {
            context.End();
        }
    }
    
    internal Container Build(List<IProvider> providers, List<object> instances) {
        if (_busy) throw new InvalidOperationException("Container is busy");
        _busy = true;
        
        // Add all the providers to the registry first
        providers.ForEach(AddToRegistry);

        // Inject the custom factories first
        WithContext(context => {
            instances.ForEach(instance => {
                Logger.Debug("Initializing {0}", instance.GetType().GetTypeName());
                context.InjectServices(Lifetime.Singleton, instance);
                ExecutePostInjectMethods(instance);
            });

            providers.ForEach(provider => {
                switch (provider) {
                    case TransientProvider:
                    case ProxyProvider:
                        return;
                    case SingletonProvider singletonProvider:
                        if (!singletonProvider.IsInstanceCreated && !singletonProvider.Lazy) {
                            Logger.Debug("Initializing non lazy {0}:{1} | Name: \"{2}\"", Lifetime.Singleton, provider.InstanceType.GetTypeName(), provider.Name);
                            provider.Resolve(context);
                        }
                        break;
                    case StaticProvider staticProvider:
                        if (!staticProvider.IsInitialized) {
                            Logger.Debug("Initializing static {0}:{1} | Name: \"{2}\"", Lifetime.Singleton, provider.InstanceType.GetTypeName(), provider.Name);
                            provider.Resolve(context);
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Unknown provider type: {provider.GetType().GetTypeName()}");
                }
            });
        });
        
        // Check if a lazy singleton was initialized by mistake
        var errors = providers
            .OfType<SingletonProvider>()
            .Where(provider => provider is { Lazy: true, IsInstanceCreated: true })
            .Select(provider => $"- {provider.InstanceType.GetTypeName()} | Name: \"{provider.Name}\"")
            .ToList();
        if (errors.Count > 0) {
            throw new InvalidOperationException("Container initialization failed. These Lazy Singletons are initialized when they shouldn't.\nPlease, remove the Lazy flag or ensure the injection is done using [Inject] ILazy<T> instead of [Inject] T:\n" + string.Join("\n", errors));
        }
        
        _busy = false;
        return this;
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
            if (_registryByName.TryGetValue(name, out var found)) throw new DuplicateServiceException(found.ExposedType, name);
            _registryByName[name] = provider;
        } else {
            if (_registryByExposedType.ContainsKey(provider.ExposedType)) throw new DuplicateServiceException(provider.ExposedType);
            _registryByExposedType[provider.ExposedType] = provider;
            Logger.Debug("Registered {0}:{1} | Type: {2}", provider.Lifetime, provider.InstanceType.GetTypeName(),
                provider.ExposedType.GetTypeName());
        }
        _providers.Add(provider);
        provider.Container = this;
    }

    public bool Contains(string name) => _registryByName.ContainsKey(name);
    public bool Contains<T>() => Contains(typeof(T));
    public bool Contains(Type type) => _registryByExposedType.ContainsKey(type);

    public IProvider GetProvider(string name) => TryGetProvider(name, out var found) ? found! : throw new ServiceNotFoundException(name);
    public IProvider GetProvider<T>() => GetProvider(typeof(T));
    public IProvider GetProvider(Type type) => TryGetProvider(type, out var found) ? found! : throw new ServiceNotFoundException(type);

    public bool TryGetProvider(string name, [MaybeNullWhen(false)] out IProvider provider) => _registryByName.TryGetValue(name, out provider);
    public bool TryGetProvider<T>(out IProvider? provider) => TryGetProvider(typeof(T), out provider);
    public bool TryGetProvider(Type type, [MaybeNullWhen(false)] out IProvider provider) {
        var found = _registryByExposedType.TryGetValue(type, out provider);
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

    public bool TryResolve(Type type, [MaybeNullWhen(false)] out object instance) {
        if (TryGetProvider(type, out IProvider? provider)) {
            instance = provider!.Get();
            return true;
        }
        if (CreateIfNotFound) {
            var transientProvider = new TransientProvider(type, type);
            AddToRegistry(transientProvider);
            instance = transientProvider!.Get();
            return true;
        }
        instance = null;
        return false;
    }

    public bool TryResolve<T>(string name, [MaybeNullWhen(false)] out T instance) {
        var result = TryResolve(name, out var o);
        instance = (T)o;
        return result;
    }

    public bool TryResolve(string name, [MaybeNullWhen(false)] out object instance) {
        if (TryGetProvider(name, out IProvider? provider)) {
            instance = provider!.Get();
            return true;
        }
        instance = null;
        return false;
    }

    public List<T> GetAllInstances<T>() {
        return FromContext(context => Query<T>(Lifetime.Singleton)
            .Select(provider => provider.Resolve(context))
            .Cast<T>().ToList());
    }

    public IEnumerable<IProvider> Query<T>(Lifetime? lifetime = null) {
        return Query(typeof(T), lifetime);
    }

    public IEnumerable<IProvider> Query(Type type, Lifetime? lifetime = null) {
        return _providers
            .Where(provider => type.IsAssignableFrom(provider.InstanceType) && (!lifetime.HasValue || provider.Lifetime == lifetime));
    }
  
    public T ResolveOr<T>(Func<T> or) => TryResolve(typeof(T), out var instance) ? (T)instance : or();
    public T ResolveOr<T>(string name, Func<T> or) => TryResolve(name, out T instance) ? instance : or();

    internal void ExecuteOnCreated(ProviderResolved providerResolved) {
        OnCreated?.Invoke(providerResolved);
    }

    internal void ExecutePostInjectMethods<T>(T instance) {
        if (instance is IInjectable injectable) {
            Logger.Debug("Executing {0} in {1}. HashCode: {2:X}", nameof(IInjectable.PostInject), injectable.GetType().GetTypeName(),
                injectable.GetHashCode());
            injectable.PostInject();
        }
        OnPostInject?.Invoke(instance);
    }

    internal object TryCreateTransientFromInjector(Type type, ResolveContext context) {
        if (TryGetProvider(type, out IProvider provider)) {
            return provider.Resolve(context);
        }
        if (!CreateIfNotFound) throw new ServiceNotFoundException(type);
        var transientProvider = new TransientProvider(type, type);
        AddToRegistry(transientProvider);
        return transientProvider.Resolve(context);
    }

    public void InjectServices(object o) {
        WithContext(context => {
            context.InjectServices(Lifetime.Transient, o);
            context.End();
        });
        ExecutePostInjectMethods(o);
    }
}