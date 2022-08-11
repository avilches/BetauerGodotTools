using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Betauer.DI {
    public class ResolveContext {
        internal readonly Dictionary<string, object> ObjectsCache = new Dictionary<string, object>();
        internal readonly Container Container;

        public ResolveContext(Container container) {
            Container = container;
        }

        internal bool IsCached(Type type, string? name) {
            if (name != null) return ObjectsCache.ContainsKey(name);
            return ObjectsCache.ContainsKey(type.FullName);
        }

        internal object GetFromCache(Type type, string? name) {
            if (name != null) {
                if (ObjectsCache.TryGetValue(name, out var o)) {
                    return o;
                }
            } 
            return ObjectsCache[type.FullName];
        }

        internal void AddInstanceToCache(Type type, object o, string? name) {
            if (name != null) {
                ObjectsCache[name] = o;
            } else {
                ObjectsCache[type.FullName] = o;
            }
        }

        internal void End() {
            foreach (var instance in ObjectsCache.Values) {
                Container.ExecutePostCreateMethods(instance);
                Container.ExecuteOnCreate(Lifetime.Singleton, instance);
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
            foreach (var provider in providers) {
                if (!provider.Lazy && provider is SingletonProvider sp && !sp.IsSingletonCreated) {
                    var context = new ResolveContext(this);
                    try {
                        provider.Get(context);
                    } finally {
                        context.End();
                    }
                }
            }
            return this;
        }

        public IProvider Add(IProvider provider) {
            AddToRegistry(provider);
            if (!provider.Lazy) {
                var context = new ResolveContext(this);
                try {
                    provider.Get(context);
                } finally {
                    context.End();
                }
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
            var name = provider.GetName();
            if (name != null) {
                if (_registry.ContainsKey(name)) throw new DuplicateServiceException(name);
                    _registry[name] = provider;
                    if (_logger.IsEnabled(TraceLevel.Info)) {
                        _logger.Info("Registered " + provider.GetLifetime() + ":" + provider.GetProviderType() +
                                     ". Name: " + name);
                    }
                    if (provider.Primary || !_fallbackByType.ContainsKey(provider.GetRegisterType())) {
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

        public bool Contains(string name) => _registry.ContainsKey(name);
        public bool Contains<T>() => Contains(typeof(T));
        public bool Contains<T>(string name) => Contains(typeof(T), name);
        public bool Contains(Type type) => _registry.ContainsKey(type.FullName) || _fallbackByType.ContainsKey(type);

        public bool Contains(Type type, string name) {
            if (_registry.TryGetValue(name, out var o)) {
                return type.IsAssignableFrom(o.GetProviderType()); // Just check if it can be casted
            }
            return false;
        }

        public IProvider GetProvider(string name) => _registry[name];
        public IProvider GetProvider<T>() => GetProvider(typeof(T));
        public IProvider GetProvider<T>(string name) => GetProvider(typeof(T), name);
        public IProvider GetProvider(Type type) => 
            _registry.TryGetValue(type.FullName, out var found) ? found : _fallbackByType[type];

        public IProvider GetProvider(Type type, string name) {
            if (_registry.TryGetValue(name, out var provider)) {
                return type.IsAssignableFrom(provider.GetProviderType()) ? provider : throw new InvalidCastException();
            }
            throw new KeyNotFoundException(name);
        }

        public bool TryGetProvider(string name, out IProvider? provider) => _registry.TryGetValue(name, out provider);
        public bool TryGetProvider<T>(out IProvider? provider) => TryGetProvider(typeof(T), out provider);
        public bool TryGetProvider<T>(string name, out IProvider? provider) => TryGetProvider(typeof(T), name, out provider);
        public bool TryGetProvider(Type type, out IProvider? provider) {
            var found = _registry.TryGetValue(type.FullName, out provider);
            if (!found) found = _fallbackByType.TryGetValue(type, out provider);
            if (!found) provider = null;
            return found;
        }

        public bool TryGetProvider(Type type, string name, out IProvider? provider) {
            var found = _registry.TryGetValue(name, out provider);
            if (found) return type.IsAssignableFrom(provider.GetProviderType()); // Just check if it can be casted
            provider = null;
            return false;
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));
        public object Resolve(Type type) {
            var context = new ResolveContext(this);
            try {
                return Resolve(type, context);
            } finally {
                context.End();
            }
        }

        public T Resolve<T>(string name) => (T)Resolve(name);
        public object Resolve(string name) {
            var context = new ResolveContext(this);
            try {
                return Resolve(name, context);
            } finally {
                context.End();
            }
        }

        public T ResolveOr<T>(Func<T> or) {
            var context = new ResolveContext(this);
            try {
                return (T)Resolve(typeof(T), context);
            } catch (KeyNotFoundException) {
                return or();
            } finally {
                context.End();
            }
        }

        public T ResolveOr<T>(string name, Func<T> or) {
            var context = new ResolveContext(this);
            try {
                return (T)Resolve(name, context);
            } catch (KeyNotFoundException) {
                return or();
            } finally {
                context.End();
            }
        }

        public List<T> GetAllInstances<T>() {
            var context = new ResolveContext(this);
            try {
                var instances = _registry.Values
                    .ToHashSet() // remove duplicates
                    .Where(provider => provider.GetLifetime() == Lifetime.Singleton &&
                                       typeof(T).IsAssignableFrom(provider.GetProviderType()))
                    .Select(provider => provider.Get(context))
                    .OfType<T>()
                    .ToList();
                return instances;
            } finally {
                context.End();
            }
        }

        internal void ExecuteOnCreate(Lifetime lifetime, object instance) {
            OnCreate?.Invoke(lifetime, instance);
        }

        internal static void ExecutePostCreateMethods<T>(T instance) {
            var methods = instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var method in methods) {
                if (Attribute.GetCustomAttribute(method, typeof(PostCreateAttribute), false) is PostCreateAttribute) {
                    if (method.GetParameters().Length == 0) {
                        try {
                            method.Invoke(instance, new object[] { });
                        } catch (TargetInvocationException e) {
                            ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                        }
                    } else {
                        throw new Exception($"Method [PostCreate] {method.Name}(...) must have only 0 parameters");
                    }
                }
            }
        }
        
        internal object Resolve(Type type, ResolveContext context) {
            TryGetProvider(type, out IProvider? provider);
            if (provider != null) return provider.Get(context);
            if (CreateIfNotFound) {
                CreateBuilder().Register(type, type, () => Activator.CreateInstance(type), Lifetime.Transient).Build();
                // ReSharper disable once TailRecursiveCall
                return Resolve(type, context);
            }
            throw new KeyNotFoundException($"Service not found. Type: {type.Name}");
        }

        internal object Resolve(string name, ResolveContext context) {
            TryGetProvider(name, out IProvider? provider);
            if (provider != null) return provider.Get(context);
            throw new KeyNotFoundException($"Service not found. name: {name}");
        }


        public void InjectAllFields(object o) {
            var context = new ResolveContext(this);
            try {
                InjectAllFields(o, context);
            } finally {
                context.End();
            }
        }

        internal void InjectAllFields(object o, ResolveContext context) {
            Injector.InjectAllFields(o, context);
        }
    }
}