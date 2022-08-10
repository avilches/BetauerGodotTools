using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Betauer.DI {
    public class ResolveContext {
        private readonly Dictionary<Type, object> _objectsCache = new Dictionary<Type, object>();
        private readonly Dictionary<string, object> _objectsCacheByAlias = new Dictionary<string, object>();
        internal readonly Container Container;

        public ResolveContext(Container container) {
            Container = container;
        }

        internal bool IsCached(Type type, string? alias) {
            if (alias != null) return _objectsCacheByAlias.ContainsKey(alias);
            return _objectsCache.ContainsKey(type);
        }

        internal object GetFromCache(Type type, string? alias) {
            if (alias != null) {
                if (_objectsCacheByAlias.TryGetValue(alias, out var o)) {
                    return o;
                }
            } 
            return _objectsCache[type];
        }

        internal void AddInstanceToCache(Type type, object o, string? alias) {
            if (alias != null) {
                _objectsCacheByAlias[alias] = o;
            } else {
                _objectsCache[type] = o;
            }
        }
    }
    
    public class DuplicateServiceException : Exception {
        public DuplicateServiceException(Type type) : base($"Service already registered. Type: {type.Name}") {
        }
        
        public DuplicateServiceException(string alias) : base($"Service already registered. Alias: {alias}") {
        }
    }

    public class Container {
        private readonly Dictionary<Type, IProvider> _registryByType = new Dictionary<Type, IProvider>();
        private readonly Dictionary<Type, IProvider> _fallbackByType = new Dictionary<Type, IProvider>();
        private readonly Dictionary<string, IProvider> _registryByAlias = new Dictionary<string, IProvider>();
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Container));
        public readonly Injector Injector;
        public bool CreateIfNotFound { get; set; }
        public event Action<Lifetime, object> OnCreate;

        public Container() {
            Injector = new Injector(this);
            // Adding the Container in the Container allows to use [Inject] Container...
            Add(new StaticProvider(typeof(Container),typeof(Container),this));
        }

        public ContainerBuilder CreateBuilder() => new ContainerBuilder(this);

        public Container Build(ICollection<IProvider> providers) {
            foreach (var provider in providers) AddToRegistry(provider);
            foreach (var provider in providers) provider.OnAddToContainer(this);
            foreach (var provider in providers) provider.OnBuildContainer(this);
            return this;
        }

        public IProvider Add(IProvider provider) {
            AddToRegistry(provider);
            provider.OnAddToContainer(this);
            provider.OnBuildContainer(this);
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
            var alias = provider.GetAlias();
            if (alias != null) {
                var registeredTypes = new LinkedList<Type>();
                if (_registryByAlias.ContainsKey(alias)) throw new DuplicateServiceException(alias);
                    _registryByAlias[alias] = provider;
                    if (provider.Primary || !_fallbackByType.ContainsKey(provider.GetRegisterType())) {
                        registeredTypes.AddLast(provider.GetRegisterType());
                        _fallbackByType[provider.GetRegisterType()] = provider;
                    }
                if (_logger.IsEnabled(TraceLevel.Info)) {
                    _logger.Info("Registered " + provider.GetLifetime() + ":" + provider.GetProviderType() + " by types: " +
                                 string.Join(",", registeredTypes) + " - Names: " + alias);
                }
            } else {
                if (_registryByType.ContainsKey(provider.GetRegisterType())) throw new DuplicateServiceException(provider.GetRegisterType());
                _registryByType[provider.GetRegisterType()] = provider;
                if (_logger.IsEnabled(TraceLevel.Info)) {
                    _logger.Info("Registered " + provider.GetLifetime() + ":" + provider.GetProviderType() +
                                 " by types: " + provider.GetRegisterType().Name);
                }
            }
            return provider;
        }

        public bool Contains<T>(string alias = null) {
            return Contains(typeof(T), alias);
        }

        public bool Contains(string alias) {
            return _registryByAlias.ContainsKey(alias);
        }

        public bool Contains(Type type, string? alias = null) {
            if (alias == null) return _registryByType.ContainsKey(type) || _fallbackByType.ContainsKey(type);
            if (_registryByAlias.TryGetValue(alias, out var o)) {
                return type.IsAssignableFrom(o.GetRegisterType()); // Just check if it can be casted
            }
            return false;
        }

        public IProvider GetProvider<T>() {
            return GetProvider(typeof(T));
        }

        public IProvider GetProvider(Type type) {
            return _registryByType.TryGetValue(type, out var found) ? found : _fallbackByType[type];
        }

        public IProvider GetProvider(string alias) {
            return _registryByAlias[alias];
        }

        public bool TryGetProvider<T>(out IProvider? provider) {
            return TryGetProvider(typeof(T), out provider);
        }

        public bool TryGetProvider(Type type, out IProvider? provider) {
            var found = _registryByType.TryGetValue(type, out provider);
            if (!found) {
                found = _fallbackByType.TryGetValue(type, out provider);
            }
            if (!found) provider = null;
            return found;
        }
        
        public bool TryGetProvider(string type, out IProvider? provider) {
            var found = _registryByAlias.TryGetValue(type, out provider);
            if (!found) provider = null;
            return found;
        }

        public object Resolve(Type type) => Resolve(type, null);
        public object ResolveOr(Type type, Func<object> or) {
            try {
                return Resolve(type, null);
            } catch (KeyNotFoundException) {
                return or();
            }
        }

        public T Resolve<T>() => (T)Resolve(typeof(T), null);
        public T ResolveOr<T>(Func<T> or) {
            try {
                return (T)Resolve(typeof(T), null);
            } catch (KeyNotFoundException) {
                return or();
            }
        }

        public List<T> GetAllInstances<T>() {
            var context = new ResolveContext(this);
            return _registryByAlias.Values
                .Concat(_registryByType.Values)
                .ToHashSet() // remove duplicates
                .Where(provider => provider.GetLifetime() == Lifetime.Singleton &&
                                   typeof(T).IsAssignableFrom(provider.GetRegisterType()))
                .Select(provider => provider.Get(context))
                .OfType<T>()
                .ToList();
        }

        public object Resolve(string alias) => Resolve(alias, null);

        public T Resolve<T>(string alias) => (T)Resolve(alias, null);
        public T ResolveOr<T>(string alias, Func<T> or) {
            try {
                return (T)Resolve(alias, null);
            } catch (KeyNotFoundException) {
                return or();
            }
        }

        internal void ExecuteOnCreate(Lifetime lifetime, object instance) {
            OnCreate?.Invoke(lifetime, instance);
        }

        internal object Resolve(Type type, ResolveContext? context) {
            TryGetProvider(type, out IProvider? provider);
            if (provider != null) return provider.Get(context ?? new ResolveContext(this));
            if (CreateIfNotFound) {
                CreateBuilder().Register(type, type, () => Activator.CreateInstance(type), Lifetime.Transient).Build();
                // ReSharper disable once TailRecursiveCall
                return Resolve(type, context);
            }
            throw new KeyNotFoundException($"Service not found. Type: {type.Name}");
        }

        internal object Resolve(string alias, ResolveContext? context) {
            TryGetProvider(alias, out IProvider? provider);
            if (provider != null) return provider.Get(context ?? new ResolveContext(this));
            throw new KeyNotFoundException($"Service not found. Alias: {alias}");
        }


        public void InjectAllFields(object o) {
            InjectAllFields(o, new ResolveContext(this));
        }

        internal void InjectAllFields(object o, ResolveContext context) {
            Injector.InjectAllFields(o, context);
        }
    }
}