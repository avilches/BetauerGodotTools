using System;

namespace Betauer.DI {
    public enum Lifetime {
        Transient,
        Singleton
    }

    public interface IProvider {
        public string? Name { get; }
        public bool Primary { get; }
        public Type RegisterType { get; }
        public Type ProviderType { get; }
        public Lifetime Lifetime { get; }
        public object Get(ResolveContext context);
        public object Get(Container container);
    }

    public interface ISingletonProvider : IProvider {
        public bool IsInstanceCreated { get; }
        public bool Lazy { get; }
        public object? Instance { get; }
    }

    public abstract class BaseProvider : IProvider {
        public bool Primary { get; }
        public Type RegisterType { get; }
        public Type ProviderType { get; }
        public string? Name { get; }
        public abstract Lifetime Lifetime { get; }

        protected BaseProvider(Type registerType, Type providerType, string? name, bool primary) {
            if (!registerType.IsAssignableFrom(providerType)) {
                throw new InvalidCastException(
                    $"Can't create a provider of {providerType.Name} and register with {registerType}");
            } 
            RegisterType = registerType;
            ProviderType = providerType;
            Name = name;
            Primary = primary;
        }

        public abstract object Get(ResolveContext context);
        public abstract object Get(Container context);
    }

    public class SingletonInstanceProvider : BaseProvider, ISingletonProvider {
        public override Lifetime Lifetime => Lifetime.Singleton;
        public bool IsInstanceCreated => true;
        public object Instance { get; }
        public bool Lazy => true;

        public SingletonInstanceProvider(Type registerType, Type providerType, object instance, string? name = null, bool primary = false) : base(registerType, providerType, name, primary) {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
        public override object Get(ResolveContext context) => Instance;
        public override object Get(Container container) => Instance;
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
                             RegisterType.Name + ": " + instance.GetHashCode().ToString("X"));
            }
            context.AddInstanceToCache(RegisterType, instance, Name);
            return instance;
        }
    }

    public class SingletonFactoryProvider : FactoryProvider, ISingletonProvider {
        public override Lifetime Lifetime => Lifetime.Singleton;
        public bool IsInstanceCreated { get; private set; }
        public bool Lazy { get; }
        public object? Instance { get; private set; }

        public SingletonFactoryProvider(Type registerType, Type providerType, Func<object> factory, string? name = null, bool primary = false, bool lazy = false) : base(registerType, providerType, factory, name, primary) {
            Lazy = lazy;
        }

        public override object Get(Container container) {
            if (IsInstanceCreated) return Instance!;
            var context = new ResolveContext(container);
            try {
                return Get(context);
            } finally {
                context.End();
            }
        }

        public override object Get(ResolveContext context) {
            if (IsInstanceCreated) return Instance!;
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.TryGetFromCache(RegisterType, Name, out var singleton)) {
                Logger.Debug("Get from context " + Lifetime + " " + singleton.GetType().Name + " exposed as " +
                             RegisterType.Name + ": " + singleton.GetHashCode().ToString("X"));
                return singleton;
            }
            lock (this) {
                // Just in case another thread was waiting for the lock
                if (IsInstanceCreated) return Instance!;
                Instance = CreateNewInstance(Lifetime, context);
                IsInstanceCreated = true;
                context.Container.InjectServices(Instance, context);
            }
            return Instance;
        }
    }

    public class TransientFactoryProvider : FactoryProvider {
        public override Lifetime Lifetime => Lifetime.Transient;

        public TransientFactoryProvider(Type registerType, Type providerType, Func<object> factory, string? name = null, bool primary = false) : base(registerType, providerType, factory, name, primary) {
        }

        public override object Get(Container container) {
            var context = new ResolveContext(container);
            try {
                return Get(context);
            } finally {
                context.End();
            }
        }

        public override object Get(ResolveContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.TryGetFromCache(RegisterType, Name, out var transient)) {
                Logger.Debug("Get from context " + Lifetime + " " + transient.GetType().Name + " exposed as " +
                             RegisterType.Name + ": " + transient.GetHashCode().ToString("X"));
            } else {
                transient = CreateNewInstance(Lifetime, context);
                context.Container.InjectServices(transient, context);
                Container.ExecutePostCreateMethods(transient);
                context.Container.ExecuteOnCreate(Lifetime.Transient, transient);
            }
            return transient;
        }
    }

}