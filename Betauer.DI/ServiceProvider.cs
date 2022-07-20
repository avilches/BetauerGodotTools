using System;
using System.Reflection;
using Godot;

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
        public void OnAddToContainer(Container container);
        public void OnBuildContainer(Container container);
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
        public abstract void OnAddToContainer(Container container);
        public abstract void OnBuildContainer(Container container);
        
        internal static void ExecutePostCreateMethods<T>(T instance) {
            var methods = instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var method in methods) {
                if (Attribute.GetCustomAttribute(method, typeof(PostCreateAttribute), false) is PostCreateAttribute) {
                    if (method.GetParameters().Length == 0) {
                        method.Invoke(instance, new object[] { });
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

        protected FactoryProvider(Type[] registeredTypes, Func<T> factory, string[]? aliases = null) : base(registeredTypes, aliases) {
            _factory = factory;
        }

        protected T CreateNewInstance(Lifetime lifetime, ResolveContext context) {
            var instance = _factory();
            if (Logger.IsEnabled(TraceLevel.Debug)) {
                Logger.Debug("Creating " + lifetime + " " + instance.GetType().Name + " exposed as " +
                             typeof(T) + ": " + instance.GetHashCode().ToString("X"));
            }
            context.AddInstanceToCache(instance, GetAliases());
            return instance;
        }
    }

    public class SingletonProvider<T> : FactoryProvider<T> where T : class {
        private bool _isSingletonDefined;
        private T? _singleton;

        public SingletonProvider(Type[] registeredTypes, Func<T> factory, string[]? aliases = null) : base(registeredTypes, factory, aliases) {
        }

        public override Lifetime GetLifetime() => Lifetime.Singleton;

        public override object Get(ResolveContext? context) {
            if (_isSingletonDefined) return _singleton!;
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.IsCached<T>(GetAliases())) {
                T singleton = context.GetFromCache<T>(GetAliases());
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
            var context = new ResolveContext(container, false);
            var instance = Get(context);
            container.InjectAllFields(instance, context);
            if (instance is Node node) container.NodeSingletonOwner?.AddChild(node);
        }

        public override void OnBuildContainer(Container container) {
            ExecutePostCreateMethods(Get(null));
        }
    }

    public class TransientProvider<T> : FactoryProvider<T> where T : class {
        public TransientProvider(Type[] registeredTypes, Func<T> factory, string[]? aliases = null) : base(registeredTypes, factory, aliases) {
        }

        public override Lifetime GetLifetime() => Lifetime.Transient;

        public override object Get(ResolveContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));
            T transient;
            if (context.IsCached<T>(GetAliases())) {
                transient = context.GetFromCache<T>(GetAliases());
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

        public StaticProvider(Type[] registeredTypes, T value, string[]? aliases = null) : base(registeredTypes, aliases) {
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