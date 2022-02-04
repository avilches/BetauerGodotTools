using System;
using Godot;

namespace Betauer.DI {
    public enum Lifetime {
        Transient,
        Singleton
    }

    public interface IProvider {
        public Type[] GetRegisterTypes();
        public Lifetime GetLifetime();
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

        protected T CreateNewInstance(Lifetime lifetime, ResolveContext context) {
            var o = _factory();
            if (Logger.IsEnabled(TraceLevel.Debug)) {
                Logger.Debug("Creating " + lifetime + " " + o.GetType().Name + " exposed as " +
                             typeof(T) + ": " + o.GetHashCode().ToString("X"));
            }
            context.AfterCreate(lifetime, o);
            return o;
        }

        public abstract Lifetime GetLifetime();
        public abstract object Resolve(ResolveContext context);
    }

    public class SingletonProvider<T> : FactoryProvider<T> where T : class {
        private bool _singletonDefined;
        private T? _singleton;

        public SingletonProvider(Type[] types, Func<T> factory) : base(types, factory) {
        }

        public override Lifetime GetLifetime() => Lifetime.Singleton;

        public override object Resolve(ResolveContext context) {
            if (_singletonDefined) return _singleton;
            if (context.Has<T>()) {
                T o = context.Get<T>();
                Logger.Debug("Resolving from context " + GetLifetime() + " " + o.GetType().Name + " exposed as " +
                             typeof(T) + ": " + o.GetHashCode().ToString("X"));
                return o;
            }
            lock (this) {
                // Just in case another thread was waiting for the lock
                if (_singletonDefined) return _singleton;
                _singleton = CreateNewInstance(GetLifetime(), context);
                _singletonDefined = true;
            }
            return _singleton;
        }
    }

    public class TransientProvider<T> : FactoryProvider<T> where T : class {
        public TransientProvider(Type[] types, Func<T> factory) : base(types, factory) {
        }

        public override Lifetime GetLifetime() => Lifetime.Transient;

        public override object Resolve(ResolveContext context) {
            if (context.Has<T>()) {
                T o = context.Get<T>();
                Logger.Debug("Resolving from context " + GetLifetime() + " " + o.GetType().Name + " exposed as " +
                             typeof(T) + ": " + o.GetHashCode().ToString("X"));
                return o;
            }
            return CreateNewInstance(GetLifetime(), context);
        }
    }

}