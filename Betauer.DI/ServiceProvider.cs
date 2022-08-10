using System;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Betauer.DI {
    public enum Lifetime {
        Transient,
        Singleton
    }

    public interface IProvider {
        public string? GetAlias();
        public bool Primary { get; }
        public Type GetRegisterType();
        public Type GetProviderType();
        public Lifetime GetLifetime();
        public object Get(ResolveContext? context);
        public void OnAddToContainer(Container container);
        public void OnBuildContainer(Container container);
    }

    public abstract class BaseProvider<T> : IProvider where T : class {
        public bool Primary { get; }
        private readonly Type _registerType;
        private readonly Type _providerType;
        private readonly string? _alias;
        public string? GetAlias() => _alias;

        public Type GetRegisterType() => _registerType;
        public Type GetProviderType() => _providerType;

        public BaseProvider(Type registerType, string? alias, bool primary) {
            _registerType = registerType;
            _providerType = typeof(T);
            _alias = alias;
            Primary = primary;
            if (!registerType.IsAssignableFrom(_providerType)) {
                throw new InvalidCastException(
                    $"Can't create a provider of {_providerType.Name} and register with {_registerType}");
            } 
        }

        public abstract Lifetime GetLifetime();
        public abstract object Get(ResolveContext? context);
        public abstract void OnAddToContainer(Container container);
        public abstract void OnBuildContainer(Container container);
        
        internal static void ExecutePostCreateMethods<T>(T instance) {
            var methods = instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var method in methods) {
                if (Attribute.GetCustomAttribute(method, typeof(PostCreateAttribute), false) is PostCreateAttribute) {
                    if (method.GetParameters().Length == 0) {
                        try {
                            method.Invoke(instance, new object[] { });
                        } catch (TargetInvocationException e) {
                            ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                        }
                    } else {
                        throw new Exception($"Method [PostCreate] {method.Name}(...) must have only 0 parameters");
                    }
                }
            }
        }

    }

    public abstract class FactoryProvider<T> : BaseProvider<T> where T : class {
        protected readonly Logger Logger = LoggerFactory.GetLogger(typeof(FactoryProvider<>));
        private readonly Func<T> _factory;

        protected FactoryProvider(Type registerType, Func<T> factory, string? alias, bool primary) : base(registerType, alias, primary) {
            _factory = factory;
        }

        protected T CreateNewInstance(Lifetime lifetime, ResolveContext context) {
            var instance = _factory();
            if (Logger.IsEnabled(TraceLevel.Debug)) {
                Logger.Debug("Creating " + lifetime + " " + instance.GetType().Name + " exposed as " +
                             typeof(T) + ": " + instance.GetHashCode().ToString("X"));
            }
            context.AddInstanceToCache(instance, GetAlias());
            return instance;
        }
    }

    public class SingletonProvider<T> : FactoryProvider<T> where T : class {
        private bool _isSingletonDefined;
        private T? _singleton;

        public SingletonProvider(Type registerType, Func<T> factory, string? alias = null, bool primary = false) : base(registerType, factory, alias, primary) {
        }

        public override Lifetime GetLifetime() => Lifetime.Singleton;

        public override object Get(ResolveContext? context) {
            if (_isSingletonDefined) return _singleton!;
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.IsCached<T>(GetAlias())) {
                T singleton = context.GetFromCache<T>(GetAlias());
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

        public override void OnAddToContainer(Container container) {
            var context = new ResolveContext(container);
            var instance = Get(context);
            container.InjectAllFields(instance, context);
            container.OnCreate?.Invoke(instance);
        }

        public override void OnBuildContainer(Container container) {
            ExecutePostCreateMethods(Get(null));
        }
    }

    public class TransientProvider<T> : FactoryProvider<T> where T : class {
        public TransientProvider(Type registerType, Func<T> factory, string? alias = null, bool primary = false) : base(registerType, factory, alias, primary) {
        }

        public override Lifetime GetLifetime() => Lifetime.Transient;

        public override object Get(ResolveContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));
            T transient;
            if (context.IsCached<T>(GetAlias())) {
                transient = context.GetFromCache<T>(GetAlias());
                Logger.Debug("Get from context " + GetLifetime() + " " + transient.GetType().Name + " exposed as " +
                             typeof(T) + ": " + transient.GetHashCode().ToString("X"));
            } else {
                transient = CreateNewInstance(GetLifetime(), context);
                context.Container.InjectAllFields(transient, context);
                ExecutePostCreateMethods(transient);
            }
            return transient;
        }

        public override void OnAddToContainer(Container container) {
        }

        public override void OnBuildContainer(Container container) {
        }
    }

    public class StaticProvider<T> : BaseProvider<T> where T : class {
        private readonly T _value;

        public StaticProvider(Type registerType, T value, string? alias = null, bool primary = false) : base(registerType, alias, primary) {
            _value = value;
        }

        public override Lifetime GetLifetime() => Lifetime.Singleton;

        public override object Get(ResolveContext? context) {
            return _value;
        }

        public override void OnAddToContainer(Container container) {
        }

        public override void OnBuildContainer(Container container) {
        }
    }
}