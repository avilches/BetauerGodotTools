using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

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

    public class Container : Node {
        private readonly Dictionary<Type, IProvider> _registry = new Dictionary<Type, IProvider>();
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Container));
        public Action<object>? OnInstanceCreated;
        public Node Owner;
        public readonly Scanner Scanner;
        public bool CreateIfNotFound { get; set; }
        public bool IsReady { get; private set; } = false;

        internal readonly LinkedList<IProviderBuilder> PendingToBuild = new LinkedList<IProviderBuilder>();
        internal readonly Queue<Node> PendingToAdd = new Queue<Node>();

        public Container(Node owner) {
            Owner = owner;
            Scanner = new Scanner(this);
        }

        public void Build() {
            foreach (var providerBuilder in PendingToBuild.ToList()) {
                providerBuilder.Build(); // the 
            }
        }

        public override void _Ready() {
            IsReady = true;
            AddPendingToRootViewport();
        }

        private void AddPendingToRootViewport() {
            if (!IsReady) return;
            while (PendingToAdd.Count > 0) {
                var autoload = PendingToAdd.Dequeue();
                AddSingletonToRootViewport(autoload);
            }
        }

        private void AddSingletonToRootViewport(Node autoload) {
            Node viewport = GetTree().Root;
            viewport.AddChild(autoload);
        }

        public override void _ExitTree() {
            IsReady = false;
        }

        public FactoryProviderBuilder<T> Instance<T>(T instance) where T : class {
            return Register<T>().With(() => instance).Lifetime(Lifetime.Singleton);
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

        public IProviderBuilder Register(Type type, Lifetime lifetime = Lifetime.Singleton, params Type[] types) {
            var builder = FactoryProviderBuilder.Create(this, type, lifetime, types);
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
            return provider;
        }

        public T Resolve<T>() {
            return (T)Resolve(typeof(T));
        }

        public bool Exist<T>(Lifetime? lifetime = null) {
            return Exist(typeof(T), lifetime);
        }

        public bool Exist(Type key, Lifetime? lifetime = null) {
            if (_registry.TryGetValue(key, out var o)) {
                return lifetime == null || o.GetLifetime() == lifetime;
            }
            return false;
        }

        public object Resolve(Type type) {
            return Resolve(type, new ResolveContext(this));
        }

        internal object Resolve(Type type, ResolveContext context) {
            var found = _registry.TryGetValue(type, out IProvider provider);
            if (found) return provider!.Resolve(context);
            if (CreateIfNotFound) return CreateNonRegisteredTransientInstance(type, context);
            throw new KeyNotFoundException("Type not found: " + type.Name);
        }

        private object CreateNonRegisteredTransientInstance(Type type, ResolveContext context) {
            if (!type.IsClass || type.IsAbstract)
                throw new ArgumentException("Can't create an instance of a interface or abstract class");
            var o = Activator.CreateInstance(type);
            AfterCreate(context, Lifetime.Transient, o);
            return o;
        }

        public void AutoWire(object o) {
            AutoWire(o, new ResolveContext(this));
        }

        public void LoadOnReadyNodes(Node o) {
            Scanner.LoadOnReadyNodes(o);
        }

        internal void AutoWire(object o, ResolveContext context) {
            Scanner.AutoWire(o, context);
        }

        internal void AfterCreate<T>(ResolveContext context, Lifetime lifetime, T instance) {
            OnInstanceCreated?.Invoke(instance);
            if (lifetime == Lifetime.Singleton) {
                if (instance is Node node) {
                    if (IsReady) {
                        AddSingletonToRootViewport(node);
                    } else {
                        PendingToAdd.Enqueue(node);
                    }
                }
            }
            AutoWire(instance, context);
        }
    }
}