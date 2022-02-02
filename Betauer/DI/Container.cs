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

        internal void AfterCreate<T>(T o) where T : class {
            _objects[typeof(T)] = o;
            Container.AfterCreate(o, this);
        }
    }

    public class Container : Node {
        private readonly Dictionary<Type, IProvider> _registry = new Dictionary<Type, IProvider>();
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Container));
        public Action<object>? OnInstanceCreated;
        public Node Owner;
        public readonly Scanner Scanner;

        public Container(Node owner) {
            Owner = owner;
            Scanner = new Scanner(this);
        }

        internal readonly LinkedList<IProviderBuilder> Pending = new LinkedList<IProviderBuilder>();

        public void Build() {
            foreach (var providerBuilder in Pending.ToList()) {
                providerBuilder.Build();
            }
        }

        public FactoryProviderBuilder<T> Instance<T>(T instance) where T : class {
            return Register<T>().With(() => instance).Scope(Scope.Singleton);
        }

        public FactoryProviderBuilder<T> Register<T>(Func<T> factory) where T : class {
            return Register<T>().With(factory);
        }

        public FactoryProviderBuilder<T> Register<T>(Scope scope) where T : class {
            return Register<T>().Scope(scope);
        }

        public FactoryProviderBuilder<T> Register<T>() where T : class {
            var builder = new FactoryProviderBuilder<T>(this);
            Pending.AddLast(builder);
            return builder;
        }

        public IProviderBuilder Register(Type type, Scope scope = Scope.Singleton, params Type[] types) {
            return FactoryProviderBuilder<object>.Create(this, type, scope, types);
        }

        public IProvider Add(IProvider provider) {
            if (provider.GetRegisterTypes().Length == 0) {
                throw new Exception("Provider withoout types are not allowed");
            }
            foreach (var providerType in provider.GetRegisterTypes()) {
                _registry[providerType] = provider;
            }
            if (_logger.IsEnabled(TraceLevel.Info)) {
                _logger.Info("Registered " + provider.GetScope() + " Type: " +
                             string.Join(",", provider.GetRegisterTypes().ToList()));
            }
            return provider;
        }

        public T Resolve<T>() {
            return (T)Resolve(typeof(T));
        }

        public bool Exist<T>(Scope? scope = null) {
            return Exist(typeof(T), scope);
        }

        public bool Exist(Type key, Scope? scope = null) {
            if (_registry.TryGetValue(key, out var o)) {
                return scope == null || o.GetScope() == scope;
            }
            return false;
        }

        public object Resolve(Type type) {
            return Resolve(type, new ResolveContext(this));
        }

        internal object Resolve(Type type, ResolveContext context) {
            var provider = _registry[type];
            var o = provider.Resolve(context);
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

        internal void AfterCreate<T>(T instance, ResolveContext context) {
            OnInstanceCreated?.Invoke(instance);
            if (instance is Node node) Owner.AddChild(node);
            AutoWire(instance, context);
        }
    }
}