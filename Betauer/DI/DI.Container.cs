using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    public interface IService {
        public Type[] GetServiceTypes();
        public Lifestyle GetLifestyle();
        public object Resolve(Container repository);
    }

    public class SingletonInstanceService : IService {
        private readonly Type[] _types;
        private readonly object _instance;

        public SingletonInstanceService(Type[] types, object instance) {
            _types = types;
            _instance = instance;
        }

        public object Resolve(Container repository) => _instance;
        public Type[] GetServiceTypes() => _types;
        public Lifestyle GetLifestyle() => Lifestyle.Singleton;
    }

    public abstract class FactoryService<T> : IService {
        protected readonly Logger _logger = LoggerFactory.GetLogger(typeof(Container));
        private readonly Type[] _types;
        private readonly Func<T> _factory;
        public Type[] GetServiceTypes() => _types;

        public FactoryService(Func<T> factory) : this(new Type[] { typeof(T) }, factory) {
        }

        public FactoryService(Type[] types, Func<T> factory) {
            _factory = factory;
            _types = types;
        }

        protected T CreateInstance(Container repository) {
            var o = _factory();
            if (_logger.IsEnabled(TraceLevel.Debug)) {
                _logger.Debug("Creating " + nameof(Lifestyle.Singleton) + " instance: " +
                              string.Join(",", _types.GetEnumerator()) + " " +
                              o.GetType().Name);
            }
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

        public SingletonFactoryService(Type[] types, Func<T> factory) : base(types, factory) {
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

        public TransientFactoryService(Type[] types, Func<T> factory) : base(types, factory) {
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

        // It uses all the types in T (class + interfaces)
        public IService RegisterSingleton<T>(T instance) {
            return RegisterSingleton(typeof(T), instance);
        }

        // It uses all the type in Type (it could be only interfaces)
        public IService RegisterSingleton(Type type, object instance) {
            if (!type.IsInstanceOfType(instance)) {
                throw new ArgumentException("Instance is not type of " + type);
            }
            var singletonInstanceService = new SingletonInstanceService(GetTypesFrom(type).ToArray(), instance);
            Register(singletonInstanceService);
            AfterCreate(instance);
            return singletonInstanceService;
        }

        // It uses all the type in Type (it could be only interfaces)
        public IService Register(Lifestyle lifestyle, Type type, Func<object> factory) {
            return Register(Create(lifestyle, type, factory));
        }

        // It uses all the type in T (it could be only interfaces)
        public IService Register<T>(Lifestyle lifestyle, Func<T> factory) {
            return Register(Create(lifestyle, factory));
        }

        // It uses all the type in T
        public IService Register<T>(Lifestyle lifestyle) {
            return Register(lifestyle, typeof(T));
        }

        // It uses all the type in Type
        public IService Register(Lifestyle lifestyle, Type type) {
            if (!type.IsClass) {
                throw new InvalidOperationException("Type " + type + " is not a class");
            }
            return Register(Create(lifestyle, type, () => Activator.CreateInstance(type)));
        }

        public IService Register(IService service) {
            foreach (var serviceType in service.GetServiceTypes()) {
                _singletons[serviceType] = service;
                if (_logger.IsEnabled(TraceLevel.Info)) {
                    _logger.Info("Registered " + service.GetLifestyle() + " Type " + serviceType.Name + ": " +
                                 string.Join(",", service.GetServiceTypes().GetEnumerator()) + " (Assembly: " +
                                 serviceType.Assembly.GetName() + ")");
                }
            }
            return service;
        }

        private static IService Create(Lifestyle lifestyle, Type type, Func<object> func) {
            return lifestyle switch {
                Lifestyle.Singleton => new SingletonFactoryService<object>(GetTypesFrom(type).ToArray(), func),
                Lifestyle.Transient => new TransientFactoryService<object>(GetTypesFrom(type).ToArray(), func),
                _ => throw new Exception("Unknown lifestyle " + lifestyle)
            };
        }

        private static IService Create<T>(Lifestyle lifestyle, Func<T> func) {
            return lifestyle switch {
                Lifestyle.Singleton => new SingletonFactoryService<T>(GetTypesFrom(typeof(T)).ToArray(), func),
                Lifestyle.Transient => new TransientFactoryService<T>(GetTypesFrom(typeof(T)).ToArray(), func),
                _ => throw new Exception("Unknown lifestyle " + lifestyle)
            };
        }

        private static List<Type> GetTypesFrom(Type type) {
            var types = type.GetInterfaces().ToList();
            types.Remove(typeof(IDisposable));
            types.Add(type);
            return types;
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