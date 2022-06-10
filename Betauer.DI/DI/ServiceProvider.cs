using System;

namespace Betauer.DI {
    public enum Lifetime {
        Transient,
        Singleton
    }

    public interface IProvider {
        public string[]? GetAliases();
        public Type[] GetRegisterTypes();
        public Type GetProviderType();
        public Lifetime GetLifetime();
        public object Get(ResolveContext? context);
    }

    public abstract class BaseProvider<T> : IProvider where T : class {
        private readonly Type[] _registeredTypes;
        private readonly Type _providerType;
        private readonly string[]? _aliases;
        public string[]? GetAliases() => _aliases;

        public Type[] GetRegisterTypes() => _registeredTypes;
        public Type GetProviderType() => _providerType;

        public BaseProvider(Type[] registeredTypes, string[]? aliases = null) {
            _registeredTypes = registeredTypes;
            _providerType = typeof(T);
            _aliases = aliases;
        }

        public abstract Lifetime GetLifetime();
        public abstract object Get(ResolveContext? context);
    }

    public abstract class FactoryProvider<T> : BaseProvider<T> where T : class {
        protected readonly Logger Logger = LoggerFactory.GetLogger(typeof(FactoryProvider<>));
        private readonly Func<T> _factory;

        protected FactoryProvider(Type[] registeredTypes, Func<T> factory, string[]? aliases = null) : base(registeredTypes, aliases) {
            _factory = factory;
        }

        protected T CreateNewInstance(Lifetime lifetime, ResolveContext context) {
            var instance = _factory();
            if (Logger.IsEnabled(TraceLevel.Debug)) {
                Logger.Debug("Creating " + lifetime + " " + instance.GetType().Name + " exposed as " +
                             typeof(T) + ": " + instance.GetHashCode().ToString("X"));
            }
            context.AfterCreate(lifetime, instance, GetAliases());
            return instance;
        }

        public abstract override Lifetime GetLifetime();
        public abstract override object Get(ResolveContext? context);
    }

    public class SingletonProvider<T> : FactoryProvider<T> where T : class {
        private bool _isSingletonDefined;
        private T? _singleton;

        public SingletonProvider(Type[] registeredTypes, Func<T> factory, string[]? aliases = null) : base(registeredTypes, factory, aliases) {
        }

        public override Lifetime GetLifetime() => Lifetime.Singleton;

        public override object Get(ResolveContext? context) {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (_isSingletonDefined) return _singleton!;
            if (context.Has<T>(GetAliases())) {
                T singleton = context.Get<T>(GetAliases());
                Logger.Debug("Get from context " + GetLifetime() + " " + singleton.GetType().Name + " exposed as " +
                             typeof(T) + ": " + singleton.GetHashCode().ToString("X"));
                return singleton;
            }
            lock (this) {
                // Just in case another thread was waiting for the lock
                if (_isSingletonDefined) return _singleton!;
                _singleton = CreateNewInstance(GetLifetime(), context);
                _isSingletonDefined = true;
            }
            return _singleton;
        }
    }

    public class TransientProvider<T> : FactoryProvider<T> where T : class {
        public TransientProvider(Type[] registeredTypes, Func<T> factory, string[]? aliases = null) : base(registeredTypes, factory, aliases) {
        }

        public override Lifetime GetLifetime() => Lifetime.Transient;

        public override object Get(ResolveContext? context) {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Has<T>(GetAliases())) {
                T transient = context.Get<T>(GetAliases());
                Logger.Debug("Get from context " + GetLifetime() + " " + transient.GetType().Name + " exposed as " +
                             typeof(T) + ": " + transient.GetHashCode().ToString("X"));
                return transient;
            }
            return CreateNewInstance(GetLifetime(), context);
        }
    }

    public class StaticProvider<T> : BaseProvider<T> where T : class {
        private readonly T _value;

        public StaticProvider(Type[] registeredTypes, T value, string[]? aliases = null) : base(registeredTypes, aliases) {
            _value = value;
        }

        public override Lifetime GetLifetime() => Lifetime.Singleton;

        public override object Get(ResolveContext? context) {
            return _value;
        }
    }
}