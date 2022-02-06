using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using NUnit.Framework;

namespace Betauer.DI {
    internal class ResolvedEntry {
        internal Type type;
        public object instance;
    }

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
        private readonly Queue<IProvider> _resolvedPending = new Queue<IProvider>();
        public readonly Node Owner;
        public readonly Scanner Scanner;
        public readonly Injector Injector;
        public Action<object>? OnInstanceCreated { get; set; }
        public bool CreateIfNotFound { get; set; }

        public void EnqueueProvider(IProvider provider) {
            _resolvedPending.Enqueue(provider);
        }

        internal readonly LinkedList<IProviderBuilder> PendingToBuild = new LinkedList<IProviderBuilder>();

        public Container(Node owner) {
            Owner = owner;
            Scanner = new Scanner(this);
            Injector = new Injector(this);
            Instance<Container>(this)
                .Build(); // Adding the Container in the Container allows to use [Inject] Container c;
        }

        public void Build() {
            foreach (var providerBuilder in PendingToBuild.ToList()) {
                providerBuilder.Build(); // the Build() already deletes itself from the PendingToBuild list
            }
            while (_resolvedPending.Count > 0) {
                var singleton = _resolvedPending.Dequeue();
                _logger.Debug("Resolving Singleton Type: " + singleton);
                singleton.Get(
                    new ResolveContext(
                        this)); // Resolve() will also add the instance to the Owner if the service is a Node singleton
            }
        }

        public StaticProviderBuilder<T> Instance<T>(T instance) where T : class {
            var builder = new StaticProviderBuilder<T>(this, instance);
            PendingToBuild.AddLast(builder);
            return builder;
        }

        public FactoryProviderBuilder<T> Register<T>(Func<T> factory) where T : class {
            return Register<T>().With(factory);
        }

        public FactoryProviderBuilder<T> Register<TI, T>(Lifetime lifetime = Lifetime.Singleton) where T : class {
            return Register<T>(lifetime).As<TI>();
        }

        public FactoryProviderBuilder<T> Register<T>(Lifetime lifetime = Lifetime.Singleton) where T : class {
            var builder = new FactoryProviderBuilder<T>(this).Lifetime(lifetime);
            PendingToBuild.AddLast(builder);
            return builder;
        }

        public IProviderBuilder Register(Type type, Lifetime lifetime, params Type[] types) {
            return Register(type, null, lifetime, types);
        }

        public IProviderBuilder Register(Type type, Func<object> factory = null, Lifetime lifetime = Lifetime.Singleton,
            params Type[] types) {
            var builder = FactoryProviderBuilder.Create(this, type, lifetime, factory, types);
            PendingToBuild.AddLast(builder);
            return builder;
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
            if (provider.GetLifetime() == Lifetime.Singleton) {
                // It's a node type, so let's schedule a Resolve() in the Build() stage so they can be built and added to the Owner
                EnqueueProvider(provider);
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