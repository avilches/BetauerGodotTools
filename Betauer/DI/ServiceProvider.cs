using System;
using Godot;

namespace Betauer.DI {
    public enum Scope {
        Prototype,
        Singleton
    }

    public interface IProvider {
        public Type[] GetRegisterTypes();
        public Scope GetScope();
        public object Resolve(ResolveContext context);
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

        protected T CreateInstance(ResolveContext context) {
            var o = _factory();
            if (Logger.IsEnabled(TraceLevel.Debug)) {
                Logger.Debug("Creating " + nameof(Scope.Singleton) + " " + o.GetType().Name + " exposed as " +
                             typeof(T) + ": " + o.GetHashCode().ToString("X"));
            }
            context.AfterCreate(o);
            return o;
        }

        public abstract Scope GetScope();
        public abstract object Resolve(ResolveContext context);
    }

    public class SingletonProvider<T> : FactoryProvider<T> where T : class {
        private bool _singletonDefined;
        private T? _singleton;

        public SingletonProvider(Type[] types, Func<T> factory) : base(types, factory) {
        }

        public override Scope GetScope() => Scope.Singleton;

        public override object Resolve(ResolveContext context) {
            if (_singletonDefined) return _singleton;
            if (context.Has<T>()) {
                T o = context.Get<T>();
                Logger.Debug("Resolving from context " + nameof(Scope.Singleton) + " " + o.GetType().Name + " exposed as " +
                             typeof(T) + ": " + o.GetHashCode().ToString("X"));
                return o;
            }
            lock (this) {
                // Just in case another thread was waiting for the lock
                if (_singletonDefined) return _singleton;
                _singleton = CreateInstance(context);
                _singletonDefined = true;
            }
            return _singleton;
        }
    }

    public class PrototypeProvider<T> : FactoryProvider<T> where T : class {
        public PrototypeProvider(Type[] types, Func<T> factory) : base(types, factory) {
        }

        public override Scope GetScope() => Scope.Prototype;

        public override object Resolve(ResolveContext context) {
            if (context.Has<T>()) {
                T o = context.Get<T>();
                Logger.Debug("Resolving from context " + nameof(Scope.Singleton) + " " + o.GetType().Name + " exposed as " +
                             typeof(T) + ": " + o.GetHashCode().ToString("X"));
                return o;
            }
            return CreateInstance(context);
        }
    }

}