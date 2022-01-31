using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    public interface IService {
        public Type GetServiceType();
        public Lifestyle GetLifestyle();
        public object Resolve(Container repository);
    }

    public class SingletonInstanceService : IService {
        private readonly Type _type;
        private readonly object _instance;

        public SingletonInstanceService(Type type, object instance) {
            _type = type;
            _instance = instance;
        }

        public object Resolve(Container repository) => _instance;
        public Type GetServiceType() => _type;
        public Lifestyle GetLifestyle() => Lifestyle.Singleton;
    }

    public abstract class FactoryService<T> : IService {
        protected readonly Logger _logger = LoggerFactory.GetLogger(typeof(Container));
        private readonly Type _type;
        private readonly Func<T> _factory;
        public Type GetServiceType() => _type;

        public FactoryService(Func<T> factory) : this(typeof(T), factory) {
        }

        public FactoryService(Type type, Func<T> factory) {
            _factory = factory;
            _type = type;
        }

        protected T CreateInstance(Container repository) {
            var o = _factory();
            _logger.Debug("Creating " + nameof(Lifestyle.Singleton) + " instance: " + _type.Name + " " +
                          o.GetType().Name);
            repository.AfterCreate(o);
            return o;
        }

        public abstract Lifestyle GetLifestyle();
        public abstract object Resolve(Container repository);
    }

    public class SingletonFactoryService<T> : FactoryService<T> {
        private bool _singletonDefined;
        private T _singleton;

        public SingletonFactoryService(Func<T> factory) : base(factory) {
        }

        public SingletonFactoryService(Type type, Func<T> factory) : base(type, factory) {
        }

        public override Lifestyle GetLifestyle() => Lifestyle.Singleton;

        public override object Resolve(Container repository) {
            if (_singletonDefined) return _singleton;
            lock (this) {
                // Just in case another was waiting for the lock
                if (_singletonDefined) return _singleton;
                _singletonDefined = true;
                _singleton = CreateInstance(repository);
            }
            return _singleton;
        }
    }

    public class TransientFactoryService<T> : FactoryService<T> {
        public TransientFactoryService(Func<T> factory) : base(factory) {
        }

        public TransientFactoryService(Type type, Func<T> factory) : base(type, factory) {
        }

        public override Lifestyle GetLifestyle() => Lifestyle.Transient;

        public override object Resolve(Container repository) {
            return CreateInstance(repository);
        }
    }

    public class SingletonFactoryWithNode<T> {
        private readonly Func<Node, T> _factory;

        public SingletonFactoryWithNode(Func<Node, T> factory) {
            _factory = factory;
        }

        // public T Build() {
        // return new T;
        // }

        public T Build(Node node) {
            return _factory(node);
        }
    }

    public enum Lifestyle {
        Transient,
        Singleton
    }

    public class Container : Node {
        private readonly Dictionary<Type, IService> _singletons = new Dictionary<Type, IService>();
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Container));
        public Action<object>? OnInstanceCreated;
        public Node Owner;
        public readonly Scanner Scanner;

        public Container(Node owner) {
            Owner = owner;
            Scanner = new Scanner(this);
        }

        public IService RegisterSingleton<T>(T instance) {
            return RegisterSingleton(typeof(T), instance);
        }

        public IService RegisterSingleton(Type type, object instance) {
            if (!type.IsInstanceOfType(instance)) {
                throw new ArgumentException("Instance is not type of " + type);
            }
            var singletonInstanceService = new SingletonInstanceService(type, instance);
            Register(type, singletonInstanceService);
            AfterCreate(instance);
            return singletonInstanceService;
        }

        public IService Register(Lifestyle lifestyle, Type type, Func<object> factory) {
            return Register(type, Create(lifestyle, type, factory));
        }

        public IService Register<T>(Lifestyle lifestyle, Func<T> factory) {
            return Register(typeof(T), Create(lifestyle, factory));
        }

        public IService Register<T>(Lifestyle lifestyle) {
            return Register(lifestyle, typeof(T));
        }

        public IService Register(Lifestyle lifestyle, Type type) {
            return Register(type, Create(lifestyle, type, () => Activator.CreateInstance(type)));
        }

        public IService Register(Type type, IService service) {
            _singletons[type] = service;
            _logger.Info("Registered " + service.GetLifestyle() + " Type " + type.Name + ": " +
                         service.GetServiceType().Name + " (Assembly: " +
                         type.Assembly.GetName() + ")");
            return service;
        }

        private static IService Create(Lifestyle lifestyle, Type type, Func<object> func) {
            return lifestyle switch {
                Lifestyle.Singleton => new SingletonFactoryService<object>(type, func),
                Lifestyle.Transient => new TransientFactoryService<object>(type, func),
                _ => throw new Exception("Unknown lifestyle " + lifestyle)
            };
        }

        private static IService Create<T>(Lifestyle lifestyle, Func<T> func) {
            return lifestyle switch {
                Lifestyle.Singleton => new SingletonFactoryService<T>(func),
                Lifestyle.Transient => new TransientFactoryService<T>(func),
                _ => throw new Exception("Unknown lifestyle " + lifestyle)
            };
        }

        public T Resolve<T>() {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type) {
            var service = _singletons[type];
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