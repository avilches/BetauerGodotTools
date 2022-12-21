using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Betauer.Core;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Logging;
using Betauer.Tools.Reflection;

namespace Betauer.DI {
    public class Container {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Container));
        private readonly Dictionary<Type, IProvider> _fallbackByType = new();
        private readonly Dictionary<string, IProvider> _registry = new();
        public readonly Injector Injector;
        public bool CreateIfNotFound { get; set; }
        public event Action<Lifetime, object> OnCreated;

        public Container() {
            Injector = new Injector(this);
            // Adding the Container in the Container allows to use [Inject] Container...
            Add(new SingletonInstanceProvider(typeof(Container),typeof(Container),this));
        }

        public ContainerBuilder CreateBuilder() => new ContainerBuilder(this);

        public Container Build(ICollection<IProvider> providers) {
            providers
                .ForEach(provider => AddToRegistry(provider));
            providers
                .Where(provider => provider is ISingletonProvider { Lazy: false, IsInstanceCreated: false })
                .ForEach(provider => provider.Get(this));
            return this;
        }

        public IProvider Add(IProvider provider) {
            AddToRegistry(provider);
            if (provider is ISingletonProvider { Lazy: false, IsInstanceCreated: false }) {
                provider.Get(this);
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
                if (_registry.ContainsKey(name)) throw new DuplicateServiceException(name);
                    _registry[name] = provider;
                    if (provider.Primary || !_fallbackByType.ContainsKey(provider.RegisterType)) {
                        _fallbackByType[provider.RegisterType] = provider;
                        Logger.Debug($"Registered {provider.Lifetime}:{provider.ProviderType} | Name: {name} | Added fallback: {provider.RegisterType.FullName}");
                    } else {
                        Logger.Debug($"Registered {provider.Lifetime}:{provider.ProviderType} | Name: {name}");
                    }
            } else {
                if (_registry.ContainsKey(provider.RegisterType.FullName)) throw new DuplicateServiceException(provider.RegisterType);
                _registry[provider.RegisterType.FullName] = provider;
                Logger.Debug($"Registered {provider.Lifetime}:{provider.ProviderType}. Name: {provider.RegisterType.FullName}");
            }
            return provider;
        }

        public bool Contains(string name) => _registry.ContainsKey(name);
        public bool Contains<T>() => Contains(typeof(T));
        public bool Contains<T>(string name) => Contains(typeof(T), name);
        public bool Contains(Type type) => _registry.ContainsKey(type.FullName) || _fallbackByType.ContainsKey(type);

        public bool Contains(Type type, string name) {
            if (_registry.TryGetValue(name, out var o)) {
                return type.IsAssignableFrom(o.ProviderType); // Just check if it can be casted
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
                return type.IsAssignableFrom(provider.ProviderType) ? provider : throw new InvalidCastException();
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
            if (found) return type.IsAssignableFrom(provider.ProviderType); // Just check if it can be casted
            provider = null;
            return false;
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));
        public object Resolve(Type type) {
            if (TryResolve(type, out var instance)) {
                return instance;
            }
            throw new KeyNotFoundException($"Service not found. Type: {type.Name}");
        }

        public T Resolve<T>(string name) => (T)Resolve(name);
        public object Resolve(string name) {
            if (TryResolve(name, out var instance)) {
                return instance;
            }
            throw new KeyNotFoundException($"Service not found. name: {name}");
        }
    
        public bool TryResolve<T>(out T instance) {
            var result = TryResolve(typeof(T), out var o);
            instance = (T)o;
            return result;
        }

        public bool TryResolve(Type type, out object instance) {
            if (TryGetProvider(type, out IProvider? provider)) {
                var context = new ResolveContext(this);
                instance = provider.Get(context);
                context.End();
                return true;
            }
            if (CreateIfNotFound) {
                CreateBuilder().Register(type, type, () => Activator.CreateInstance(type), Lifetime.Transient).Build();
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
                var context = new ResolveContext(this);
                instance = provider.Get(context);
                context.End();
                return true;
            }
            instance = null;
            return false;
        }
      
        public T ResolveOr<T>(Func<T> or) => TryResolve(typeof(T), out var instance) ? (T)instance : or();
        public T ResolveOr<T>(string name, Func<T> or) => TryResolve(name, out T instance) ? instance : or();

        public List<T> GetAllInstances<T>() {
            var instances = _registry.Values
                .OfType<ISingletonProvider>()
                .Where(provider => typeof(T).IsAssignableFrom(provider.ProviderType))
                .Select(provider => provider.Get(this))
                .OfType<T>()
                .ToList();
            return instances;
        }

        internal void ExecuteOnCreated(Lifetime lifetime, object instance) {
            OnCreated?.Invoke(lifetime, instance);
        }

        internal static void ExecutePostInjectMethods<T>(T instance) {
            var methods = instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var method in methods) {
                if (method.HasAttribute<PostInjectAttribute>()) {
                    if (method.GetParameters().Length == 0) {
                        try {
                            method.Invoke(instance, new object[] { });
                        } catch (TargetInvocationException e) {
                            ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                        }
                    } else {
                        throw new Exception($"Method [PostInject] {method.Name}(...) must have only 0 parameters");
                    }
                }
            }
        }
        
        internal object Resolve(Type type, ResolveContext context) {
            if (TryGetProvider(type, out IProvider? provider)) {
                return provider.Get(context);
            }
            if (CreateIfNotFound) {
                CreateBuilder().Register(type, type, () => Activator.CreateInstance(type), Lifetime.Transient).Build();
                // ReSharper disable once TailRecursiveCall
                return Resolve(type, context);
            }
            throw new KeyNotFoundException($"Service not found. Type: {type.Name}");
        }

        internal object Resolve(string name, ResolveContext context) {
            if (TryGetProvider(name, out IProvider? provider)) {
                return provider.Get(context);
            }
            throw new KeyNotFoundException($"Service not found. name: {name}");
        }


        public void InjectServices(object o) {
            var context = new ResolveContext(this);
            InjectServices(o, context);
            context.End();
            ExecutePostInjectMethods(o);
        }

        internal void InjectServices(object o, ResolveContext context) {
            Injector.InjectServices(o, context);
        }
    }
}