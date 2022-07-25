using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Godot;

namespace Betauer.DI {
    public class ResolveContext {
        private readonly Dictionary<Type, object> _objectsCache = new Dictionary<Type, object>();
        private readonly Dictionary<string, object> _objectsCacheByAlias = new Dictionary<string, object>();
        internal readonly Container Container;
        internal readonly bool ExecutePostCreate;

        public ResolveContext(Container container, bool executePostCreate = true) {
            Container = container;
            ExecutePostCreate = executePostCreate;
        }

        internal bool IsCached<T>(string[]? aliases) {
            if (aliases != null && aliases.Length > 0) {
                if (aliases.Any(alias => _objectsCacheByAlias.ContainsKey(alias))) {
                    return true;
                }
            } 
            return _objectsCache.ContainsKey(typeof(T));
        }

        internal T GetFromCache<T>(string[]? aliases) {
            if (aliases != null && aliases.Length > 0) {
                foreach (var alias in aliases) {
                    if (_objectsCacheByAlias.TryGetValue(alias, out var o)) {
                        return (T)o;
                    }
                }
            } 
            return (T)_objectsCache[typeof(T)];
        }

        internal void AddInstanceToCache<T>(T o, string[]? aliases) where T : class {
            if (aliases != null && aliases.Length > 0) {
                foreach (var alias in aliases) {
                    _objectsCacheByAlias[alias] = o;
                }
            } else {
                _objectsCache[typeof(T)] = o;
            }
        }
    }

    public class Container {
        private readonly Dictionary<Type, IProvider> _registry = new Dictionary<Type, IProvider>();
        private readonly Dictionary<string, IProvider> _registryByAlias = new Dictionary<string, IProvider>();
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Container));
        public readonly Node NodeSingletonOwner;
        public readonly Injector Injector;
        public bool CreateIfNotFound { get; set; }

        private readonly LinkedList<IProvider> _providersPending = new LinkedList<IProvider>();

        public Container(Node nodeSingletonOwner) {
            NodeSingletonOwner = nodeSingletonOwner;
            Injector = new Injector(this);
            // Adding the Container in the Container allows to use [Inject] Container...
            Add(new StaticProvider<Container>(new [] { typeof(Container) },this), true);
        }

        public ContainerBuilder CreateBuilder() => new ContainerBuilder(this);

        public Container Build() {
            lock (_providersPending) {
                if (_providersPending.Count > 0) {
                    foreach (var provider in _providersPending) AddToRegistry(provider);
                    foreach (var provider in _providersPending) provider.OnAddToContainer(this);
                    foreach (var provider in _providersPending) provider.OnBuildContainer(this);
                    _providersPending.Clear();
                }
            }
            return this;
        }

        public IProvider Add(IProvider provider, bool addAndBuild) {
            if (addAndBuild) {
                AddToRegistry(provider);
                provider.OnAddToContainer(this);
                provider.OnBuildContainer(this);
            } else {
                lock (_providersPending) {
                    _providersPending.AddLast(provider);
                }
            }
            return provider;
        }

        private IProvider AddToRegistry(IProvider provider) {
            if (provider.GetRegisterTypes().Length == 0) {
                throw new Exception("Provider without types are not allowed");
            }
            var aliases = provider.GetAliases();
            if (aliases != null && aliases.Length > 0) {
                var registeredTypes = new LinkedList<Type>();
                var ignoredTypes = new LinkedList<Type>();
                foreach (var alias in aliases!) {
                    if (_registryByAlias.ContainsKey(alias)) throw new DuplicateNameException(alias);
                    _registryByAlias[alias] = provider;
                }
                foreach (var providerType in provider.GetRegisterTypes()) {
                    if (_registry.ContainsKey(providerType)) {
                        ignoredTypes.AddLast(providerType);
                    } else {
                        registeredTypes.AddLast(providerType);
                        _registry[providerType] = provider;
                    }
                }
                if (_logger.IsEnabled(TraceLevel.Info)) {
                    _logger.Info("Registered " + provider.GetLifetime() + " by types: " +
                                 string.Join(",", registeredTypes) + " - Names: " +
                                 string.Join(",", aliases) + " - Ignored types: " + string.Join(",", ignoredTypes));
                }
            } else {
                foreach (var providerType in provider.GetRegisterTypes()) {
                    if (_registry.ContainsKey(providerType)) throw new DuplicateNameException(providerType.ToString());
                    _registry[providerType] = provider;
                }
                if (_logger.IsEnabled(TraceLevel.Info)) {
                    _logger.Info("Registered " + provider.GetLifetime() + " by types: " +
                                 string.Join(",", provider.GetRegisterTypes().ToList()));
                }
            }
            return provider;
        }

        public bool Contains<T>(Lifetime? lifetime = null) {
            return Contains(typeof(T), lifetime);
        }

        public bool Contains(Type type, Lifetime? lifetime = null) {
            if (_registry.TryGetValue(type, out var o)) {
                return lifetime == null || o.GetLifetime() == lifetime;
            }
            return false;
        }
        public bool Contains(string alias, Lifetime? lifetime = null) {
            if (_registryByAlias.TryGetValue(alias, out var o)) {
                return lifetime == null || o.GetLifetime() == lifetime;
            }
            return false;
        }

        public IProvider GetProvider<T>() {
            return GetProvider(typeof(T));
        }

        public IProvider GetProvider(Type type) {
            return _registry[type];
        }

        public IProvider GetProvider(string alias) {
            return _registryByAlias[alias];
        }

        public bool TryGetProvider<T>(out IProvider? provider) {
            return TryGetProvider(typeof(T), out provider);
        }

        public bool TryGetProvider(Type type, out IProvider? provider) {
            var found = _registry.TryGetValue(type, out provider);
            if (!found) provider = null;
            return found;
        }
        public bool TryGetProvider(string type, out IProvider? provider) {
            var found = _registryByAlias.TryGetValue(type, out provider);
            if (!found) provider = null;
            return found;
        }

        public object Resolve(Type type) {
            return Resolve(type, null);
        }

        public T Resolve<T>() {
            return (T)Resolve(typeof(T), null);
        }

        public object Resolve(string alias) {
            return Resolve(alias, null);
        }

        public T Resolve<T>(string alias) {
            return (T)Resolve(alias, null);
        }

        internal object Resolve(Type type, ResolveContext? context) {
            TryGetProvider(type, out IProvider? provider);
            if (provider != null) return provider.Get(context ?? new ResolveContext(this));
            if (CreateIfNotFound) {
                Add(FactoryProviderBuilder.Create(type, Lifetime.Transient).CreateProvider(), true);
                // ReSharper disable once TailRecursiveCall
                return Resolve(type, context);
            }
            throw new KeyNotFoundException("Type not found: " + type.Name);
        }

        internal object Resolve(string alias, ResolveContext? context) {
            TryGetProvider(alias, out IProvider? provider);
            if (provider != null) return provider.Get(context ?? new ResolveContext(this));
            throw new KeyNotFoundException("Alias not found: " + alias);
        }


        public void InjectAllFields(object o) {
            InjectAllFields(o, new ResolveContext(this));
        }

        internal void InjectAllFields(object o, ResolveContext context) {
            Injector.InjectAllFields(o, context);
        }
    }
}