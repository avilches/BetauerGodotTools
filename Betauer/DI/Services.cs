using System;
using Godot;

namespace Betauer.DI {
    public enum Lifestyle {
        Transient,
        Singleton
    }

    public interface IProvider {
        public Type[] GetRegisterTypes();
        public Lifestyle GetLifestyle();
        public object Resolve(Container repository);
    }

    public class SingletonInstanceProvider<T> : IProvider {
        private readonly Type[] _types;
        private readonly T _instance;

        public SingletonInstanceProvider(Type[] types, T instance) {
            _types = types;
            _instance = instance;
        }

        public Lifestyle GetLifestyle() => Lifestyle.Singleton;
        public Type[] GetRegisterTypes() => _types;
        public object Resolve(Container repository) => _instance;
    }

    public abstract class FactoryProvider<T> : IProvider where T : class {
        protected readonly Logger Logger = LoggerFactory.GetLogger(typeof(Container));
        private readonly Type[] _types;
        private readonly Func<T> _factory;
        public Type[] GetRegisterTypes() => _types;

        public FactoryProvider(Type[] types, Func<T> factory) {
            _factory = factory;
            _types = types;
        }

        protected T CreateInstance(Container repository) {
            var o = _factory();
            if (Logger.IsEnabled(TraceLevel.Debug)) {
                Logger.Debug("Creating " + nameof(Lifestyle.Singleton) + " " + o.GetType().Name + " " +
                             (o == null ? "null" : o.GetHashCode().ToString("X")));
            }
            repository.AfterCreate(o);
            return o;
        }

        public abstract Lifestyle GetLifestyle();
        public abstract object Resolve(Container repository);
    }

    public class SingletonFactoryProvider<T> : FactoryProvider<T> where T : class {
        private bool _singletonDefined;
        private T? _singleton;

        public SingletonFactoryProvider(Type[] types, Func<T> factory) : base(types, factory) {
        }

        public override Lifestyle GetLifestyle() => Lifestyle.Singleton;

        public override object Resolve(Container repository) {
            if (_singletonDefined) return _singleton;
            lock (this) {
                // Just in case another thread was waiting for the lock
                if (_singletonDefined) return _singleton;
                _singleton = CreateInstance(repository);
                _singletonDefined = true;
            }
            return _singleton;
        }
    }

    public class TransientFactoryProvider<T> : FactoryProvider<T> where T : class {
        public TransientFactoryProvider(Type[] types, Func<T> factory) : base(types, factory) {
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
}