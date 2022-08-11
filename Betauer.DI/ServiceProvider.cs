using System;

namespace Betauer.DI {
    public enum Lifetime {
        Transient,
        Singleton
    }

    public interface IProvider {
        public string? GetName();
        public bool Primary { get; }
        public bool Lazy { get; }
        public Type GetRegisterType();
        public Type GetProviderType();
        public Lifetime GetLifetime();
        public object Get(ResolveContext? context);
    }

    public abstract class BaseProvider : IProvider {
        public bool Primary { get; }
        private readonly Type _registerType;
        private readonly Type _providerType;
        private readonly string? _name;
        public string? GetName() => _name;
        public abstract bool Lazy { get; }

        public Type GetRegisterType() => _registerType;
        public Type GetProviderType() => _providerType;

        public BaseProvider(Type registerType, Type providerType, string? name, bool primary) {
            if (!registerType.IsAssignableFrom(providerType)) {
                throw new InvalidCastException(
                    $"Can't create a provider of {providerType.Name} and register with {registerType}");
            } 
            _registerType = registerType;
            _providerType = providerType;
            _name = name;
            Primary = primary;
        }

        public abstract Lifetime GetLifetime();
        public abstract object Get(ResolveContext context);
    }

    public abstract class FactoryProvider: BaseProvider {
        protected readonly Logger Logger = LoggerFactory.GetLogger(typeof(FactoryProvider));
        private readonly Func<object> _factory;

        protected FactoryProvider(Type registerType, Type providerType, Func<object> factory, string? name, bool primary) : base(registerType, providerType, name, primary) {
            _factory = factory;
        }

        protected object CreateNewInstance(Lifetime lifetime, ResolveContext context) {
            var instance = _factory();
            if (Logger.IsEnabled(TraceLevel.Debug)) {
                Logger.Debug("Creating " + lifetime + " " + instance.GetType().Name + " exposed as " +
                             GetRegisterType().Name + ": " + instance.GetHashCode().ToString("X"));
            }
            context.AddInstanceToCache(GetRegisterType(), instance, GetName());
            return instance;
        }
    }

    public class SingletonProvider : FactoryProvider {
        public bool IsSingletonCreated { get; private set; }
        public override bool Lazy { get; }
        public object? SingletonInstance;

        public SingletonProvider(Type registerType, Type providerType, Func<object> factory, string? name = null, bool primary = false, bool lazy = false) : base(registerType, providerType, factory, name, primary) {
            Lazy = lazy;
        }

        public override Lifetime GetLifetime() => Lifetime.Singleton;

        public override object Get(ResolveContext context) {
            if (IsSingletonCreated) return SingletonInstance!;
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.IsCached(GetRegisterType(), GetName())) {
                object singleton = context.GetFromCache(GetRegisterType(), GetName());
                Logger.Debug("Get from context " + GetLifetime() + " " + singleton.GetType().Name + " exposed as " +
                             GetRegisterType().Name + ": " + singleton.GetHashCode().ToString("X"));
                return singleton;
            }
            lock (this) {
                // Just in case another thread was waiting for the lock
                if (IsSingletonCreated) return SingletonInstance!;
                SingletonInstance = CreateNewInstance(GetLifetime(), context);
                IsSingletonCreated = true;
                context.Container.InjectAllFields(SingletonInstance, context);
            }
            return SingletonInstance;
        }
    }

    public class TransientProvider : FactoryProvider {
        public override bool Lazy => true;

        public TransientProvider(Type registerType, Type providerType, Func<object> factory, string? name = null, bool primary = false) : base(registerType, providerType, factory, name, primary) {
        }

        public override Lifetime GetLifetime() => Lifetime.Transient;

        public override object Get(ResolveContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));
            object transient;
            if (context.IsCached(GetRegisterType(), GetName())) {
                transient = context.GetFromCache(GetRegisterType(), GetName());
                Logger.Debug("Get from context " + GetLifetime() + " " + transient.GetType().Name + " exposed as " +
                             GetRegisterType().Name + ": " + transient.GetHashCode().ToString("X"));
            } else {
                transient = CreateNewInstance(GetLifetime(), context);
                context.Container.InjectAllFields(transient, context);
                Container.ExecutePostCreateMethods(transient);
                context.Container.ExecuteOnCreate(Lifetime.Transient, transient);
            }
            return transient;
        }
    }

    public class StaticProvider : BaseProvider {
        public readonly object StaticInstance;
        public override bool Lazy => false;

        public StaticProvider(Type registerType, Type providerType, object staticInstance, string? name = null, bool primary = false) : base(registerType, providerType, name, primary) {
            StaticInstance = staticInstance;
        }

        public override Lifetime GetLifetime() => Lifetime.Singleton;

        public override object Get(ResolveContext? context) {
            return StaticInstance;
        }
    }
}