using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Betauer.DI {
    public class Container : Node {
        private readonly Dictionary<Type, IService> _registry = new Dictionary<Type, IService>();
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Container));
        public Action<object>? OnInstanceCreated;
        public Node Owner;
        public readonly Scanner Scanner;

        public Container(Node owner) {
            Owner = owner;
            Scanner = new Scanner(this);
        }

        internal readonly LinkedList<IServiceBuilder> Pending = new LinkedList<IServiceBuilder>();

        public void Build() {
            foreach (var serviceBuilder in Pending.ToList()) {
                serviceBuilder.Build();
            }
        }

        public SingletonInstanceServiceBuilder<T> Instance<T>(T instance) {
            var singletonInstanceServiceBuilder = new SingletonInstanceServiceBuilder<T>(this, instance);
            Pending.AddLast(singletonInstanceServiceBuilder);
            return singletonInstanceServiceBuilder;
        }

        public FactoryServiceBuilder<T> Register<T>(Func<T> factory) where T : class => Register<T>().With(factory);

        public FactoryServiceBuilder<T> Register<T>(Lifestyle lifestyle) where T : class =>
            Register<T>().Lifestyle(lifestyle);

        public FactoryServiceBuilder<T> Register<T>() where T : class {
            var factoryServiceBuilder = new FactoryServiceBuilder<T>(this);
            Pending.AddLast(factoryServiceBuilder);
            return factoryServiceBuilder;
        }

        public FactoryServiceBuilder<object> Register(Type type) {
            var factoryServiceBuilder = new FactoryServiceBuilder<object>(this);
            factoryServiceBuilder.As(type);
            Pending.AddLast(factoryServiceBuilder);
            return factoryServiceBuilder;
        }

        public IService Add(IService service) {
            if (service.GetServiceTypes().Length == 0) {
                throw new Exception("Services with 0 types are not allowed");
            }
            foreach (var serviceType in service.GetServiceTypes()) {
                _registry[serviceType] = service;
                if (_logger.IsEnabled(TraceLevel.Info)) {
                    _logger.Info("Registered " + service.GetLifestyle() + " Type " + serviceType.Name + ": " +
                                 string.Join(",", service.GetServiceTypes().GetEnumerator()) + " (Assembly: " +
                                 serviceType.Assembly.GetName() + ")");
                }
            }
            return service;
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
            var service = _registry[type];
            var o = service.Resolve(this);
            return o;
        }

        internal void AfterCreate<T>(T instance) {
            OnInstanceCreated?.Invoke(instance);
            if (instance is Node node) Owner.AddChild(node);
            AutoWire(instance);
        }

        public void AutoWire(object o) {
            Scanner.AutoWire(o);
        }

        public void LoadOnReadyNodes(Node o) {
            Scanner.LoadOnReadyNodes(o);
        }
    }
}