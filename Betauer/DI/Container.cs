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
            return Register<T>().With(() => instance).Lifestyle(Lifestyle.Singleton);
        }

        public FactoryProviderBuilder<T> Register<T>(Func<T> factory) where T : class {
            return Register<T>().With(factory);
        }

        public FactoryProviderBuilder<T> Register<T>(Lifestyle lifestyle) where T : class {
            return Register<T>().Lifestyle(lifestyle);
        }

        public FactoryProviderBuilder<T> Register<T>() where T : class {
            var builder = new FactoryProviderBuilder<T>(this);
            Pending.AddLast(builder);
            return builder;
        }

        public IProviderBuilder Register(Type type, Lifestyle lifestyle = Lifestyle.Singleton, IEnumerable<Type> types = null) {
            return FactoryProviderBuilder<object>.Create(this, type, lifestyle, types);
        }

        public IProvider Add(IProvider provider) {
            if (provider.GetRegisterTypes().Length == 0) {
                throw new Exception("Provider withoout types are not allowed");
            }
            foreach (var providerType in provider.GetRegisterTypes()) {
                _registry[providerType] = provider;
            }
            if (_logger.IsEnabled(TraceLevel.Info)) {
                _logger.Info("Registered " + provider.GetLifestyle() + " Type: " +
                             string.Join(",", provider.GetRegisterTypes().ToList()));
            }
            return provider;
        }

        public T Resolve<T>() {
            return (T)Resolve(typeof(T));
        }

        public bool Exist<T>() {
            return Exist(typeof(T));
        }

        public bool Exist(Type key) {
            return _registry.ContainsKey(key);
        }

        public object Resolve(Type type) {
            return Resolve(type, new ResolveContext(this));
        }

        internal object Resolve(Type type, ResolveContext context) {
            var provider = _registry[type];
            var o = provider.Resolve(context);
            return o;
        }

        internal void AfterCreate<T>(T instance, ResolveContext context) {
            OnInstanceCreated?.Invoke(instance);
            if (instance is Node node) Owner.AddChild(node);
            AutoWire(instance, context);
        }

        public void AutoWire(object o) {
            AutoWire(o, new ResolveContext(this));
        }

        internal void AutoWire(object o, ResolveContext context) {
            Scanner.AutoWire(o, context);
        }

        public void LoadOnReadyNodes(Node o) {
            Scanner.LoadOnReadyNodes(o);
        }
    }
}