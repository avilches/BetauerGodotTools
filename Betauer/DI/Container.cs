using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.DI {
    public class ResolveContext {
        internal readonly Dictionary<Type, object> _objects = new Dictionary<Type, object>();
        internal readonly Container Container;

        internal ResolveContext(Container container) {
            Container = container;
        }

        internal bool Has<T>() {
            return _objects.ContainsKey(typeof(T));
        }

        internal T Get<T>() {
            return (T)_objects[typeof(T)];
        }

        internal void AfterCreate<T>(Lifetime lifetime, T o) where T : class {
            _objects[typeof(T)] = o;
            Container.AfterCreate(this, lifetime, o);
        }
    }

    public class Container {
        private readonly Dictionary<Type, IProvider> _registry = new Dictionary<Type, IProvider>();
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Container));
        public readonly Node Owner;
        public readonly Injector Injector;
        public Action<object>? OnInstanceCreated { get; set; }
        public bool CreateIfNotFound { get; set; }

        public Container(Node owner) {
            Owner = owner;
            Injector = new Injector(this);
            Add(new StaticProvider<Container>(new Type[] { typeof(Container) },
                this)); // Adding the Container in the Container allows to use [Inject] Container...
        }

        public IProvider Add(IProvider provider) {
            if (provider.GetRegisterTypes().Length == 0) {
                throw new Exception("Provider without types are not allowed");
            }
            foreach (var providerType in provider.GetRegisterTypes()) {
                _registry[providerType] = provider;
            }
            if (_logger.IsEnabled(TraceLevel.Info)) {
                _logger.Info("Registered " + provider.GetLifetime() + " Type: " +
                             string.Join(",", provider.GetRegisterTypes().ToList()));
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

        public IProvider GetProvider<T>() {
            return GetProvider(typeof(T));
        }

        public IProvider GetProvider(Type type) {
            return _registry[type];
        }

        public bool TryGetProvider<T>(out IProvider? provider) {
            return TryGetProvider(typeof(T), out provider);
        }

        public bool TryGetProvider(Type type, out IProvider? provider) {
            var found = _registry.TryGetValue(type, out provider);
            if (!found) provider = null;
            return found;
        }

        public T Resolve<T>(ResolveContext? context = null) {
            return (T)Resolve(typeof(T), context);
        }

        public object Resolve(Type type, ResolveContext? context = null) {
            TryGetProvider(type, out IProvider? provider);
            if (provider != null) return provider.Get(context ?? new ResolveContext(this));
            if (CreateIfNotFound)
                return CreateNonRegisteredTransientInstance(type, context ?? new ResolveContext(this));
            throw new KeyNotFoundException("Type not found: " + type.Name);
        }

        private object CreateNonRegisteredTransientInstance(Type type, ResolveContext context) {
            if (!type.IsClass || type.IsAbstract)
                throw new ArgumentException("Can't create an instance of a interface or abstract class");
            var o = Activator.CreateInstance(type);
            AfterCreate(context, Lifetime.Transient, o);
            return o;
        }

        public void InjectAllFields(object o) {
            InjectAllFields(o, new ResolveContext(this));
        }

        public void LoadOnReadyNodes(Node o) {
            Injector.LoadOnReadyNodes(o);
        }

        internal void InjectAllFields(object o, ResolveContext context) {
            Injector.InjectAllFields(o, context);
        }

        internal void AfterCreate<T>(ResolveContext context, Lifetime lifetime, T instance) {
            OnInstanceCreated?.Invoke(instance);
            InjectAllFields(instance, context);
            if (lifetime == Lifetime.Singleton && instance is Node node) {
                Owner.AddChild(node);
            }
        }
    }
}