using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Betauer.DI {
    public class ResolveContext {
        private readonly Dictionary<string, object> _objectsCache = new Dictionary<string, object>();
        internal readonly Container Container;

        public ResolveContext(Container container) {
            Container = container;
        }

        internal bool IsCached(Type type, string? name) {
            if (name != null) return _objectsCache.ContainsKey(name);
            return _objectsCache.ContainsKey(type.FullName);
        }

        internal object GetFromCache(Type type, string? name) {
            if (name != null) {
                if (_objectsCache.TryGetValue(name, out var o)) {
                    return o;
                }
            } 
            return _objectsCache[type.FullName];
        }

        internal void AddInstanceToCache(Type type, object o, string? name) {
            if (name != null) {
                _objectsCache[name] = o;
            } else {
                _objectsCache[type.FullName] = o;
            }
        }
    }
    
    public class DuplicateServiceException : Exception {
        public DuplicateServiceException(Type type) : base($"Service already registered. Type: {type.Name}") {
        }
        
        public DuplicateServiceException(string name) : base($"Service already registered. Name: {name}") {
        }
    }

    public class Container {
        private readonly Dictionary<Type, IProvider> _fallbackByType = new Dictionary<Type, IProvider>();
        private readonly Dictionary<string, IProvider> _registry = new Dictionary<string, IProvider>();
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
            var name = provider.GetName();
            if (name != null) {
                var registeredTypes = new LinkedList<Type>();
                if (_registry.ContainsKey(name)) throw new DuplicateServiceException(name);
                    _registry[name] = provider;
                    if (_logger.IsEnabled(TraceLevel.Info)) {
                        _logger.Info("Registered " + provider.GetLifetime() + ":" + provider.GetProviderType() +
                                     ". Name: " + name);
                    }
                    if (provider.Primary || !_fallbackByType.ContainsKey(provider.GetRegisterType())) {
                        registeredTypes.AddLast(provider.GetRegisterType());
                        _fallbackByType[provider.GetRegisterType()] = provider;
                        if (_logger.IsEnabled(TraceLevel.Info)) {
                            _logger.Info("Registered " + provider.GetLifetime() + ":" + provider.GetProviderType() +
                                         ". Fallback: " + provider.GetRegisterType().FullName);
                        }
                    }
            } else {
                if (_registry.ContainsKey(provider.GetRegisterType().FullName)) throw new DuplicateServiceException(provider.GetRegisterType());
                _registry[provider.GetRegisterType().FullName] = provider;
                if (_logger.IsEnabled(TraceLevel.Info)) {
                    _logger.Info("Registered " + provider.GetLifetime() + ":" + provider.GetProviderType() +
                                 ". Name: " + provider.GetRegisterType().FullName);
                }
            }
            return provider;
        }

        public bool Contains<T>(string name = null) {
            return Contains(typeof(T), name);
        }

        public bool Contains(string name) {
            return _registry.ContainsKey(name);
        }

        public bool Contains(Type type, string? name = null) {
            if (name == null) return _registry.ContainsKey(type.FullName) || _fallbackByType.ContainsKey(type);
            if (_registry.TryGetValue(name, out var o)) {
                return type.IsAssignableFrom(o.GetProviderType()); // Just check if it can be casted
            }
            return false;
        }

        public IProvider GetProvider<T>() {
            return GetProvider(typeof(T));
        }

        public IProvider GetProvider(Type type) {
            return _registry.TryGetValue(type.FullName, out var found) ? found : _fallbackByType[type];
        }

        public IProvider GetProvider(string name) {
            return _registry[name];
        }

        public bool TryGetProvider<T>(out IProvider? provider) {
            return TryGetProvider(typeof(T), out provider);
        }

        public bool TryGetProvider(Type type, out IProvider? provider) {
            var found = _registry.TryGetValue(type.FullName, out provider);
            if (!found) {
                found = _fallbackByType.TryGetValue(type, out provider);
            }
            if (!found) provider = null;
            return found;
        }
        
        public bool TryGetProvider(string type, out IProvider? provider) {
            var found = _registry.TryGetValue(type, out provider);
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
            return _registry.Values
                .ToHashSet() // remove duplicates
                .Where(provider => provider.GetLifetime() == Lifetime.Singleton &&
                                   typeof(T).IsAssignableFrom(provider.GetProviderType()))
                .Select(provider => provider.Get(context))
                .OfType<T>()
                .ToList();
        }

        public object Resolve(string name) => Resolve(name, null);

        public T Resolve<T>(string name) => (T)Resolve(name, null);
        public T ResolveOr<T>(string name, Func<T> or) {
            try {
                return (T)Resolve(name, null);
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

        internal object Resolve(string name, ResolveContext? context) {
            TryGetProvider(name, out IProvider? provider);
            if (provider != null) return provider.Get(context ?? new ResolveContext(this));
            throw new KeyNotFoundException($"Service not found. name: {name}");
        }


        public void InjectAllFields(object o) {
            InjectAllFields(o, new ResolveContext(this));
        }

        internal void InjectAllFields(object o, ResolveContext context) {
            Injector.InjectAllFields(o, context);
        }
    }
}