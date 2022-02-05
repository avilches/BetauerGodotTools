using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.DI {
    public interface IProviderBuilder {
        public IProvider Build();
    }

    public abstract class BaseProviderBuilder : IProviderBuilder {
        protected readonly Container Container;

        protected BaseProviderBuilder(Container? container) {
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IProvider Build() {
            Container.PendingToBuild.Remove(this);
            var provider = CreateProvider();
            Container.Add(provider);
            return provider;
        }

        protected abstract IProvider CreateProvider();
    }

    public abstract class TypedProviderBuilder<TBuilder> : BaseProviderBuilder where TBuilder : class {
        protected readonly HashSet<Type> Types = new HashSet<Type>();

        protected TypedProviderBuilder(Container? container) : base(container) {
        }

        public TBuilder As<T>() => As(typeof(T));
        public TBuilder As<T1, T2>() => As(typeof(T1), typeof(T2));
        public TBuilder As<T1, T2, T3>() => As(typeof(T1), typeof(T2), typeof(T3));
        public TBuilder As<T1, T2, T3, T4>() => As(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        public TBuilder As<T1, T2, T3, T4, T5>() => As(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

        public TBuilder AsAll<T>() => AsAll(typeof(T));
        public TBuilder AsAll(Type type) => As(GetTypesFrom(type));

        public TBuilder As(params Type[] types) {
            foreach (var type in types) As(type);
            return (this as TBuilder)!;
        }

        public TBuilder As(Type? type) {
            if (type != null) Types.Add(type);
            return (this as TBuilder)!;
        }

        private static Type[] GetTypesFrom(Type? type) {
            if (type == null) return new Type[] { };
            var types = type.GetInterfaces().ToList();
            types.Add(type);
            return types.ToArray();
        }
    }

    public static class FactoryProviderBuilder {
        public static IProviderBuilder Create(Container container, Type type,
            Lifetime lifetime, Func<object>? factory = null, IEnumerable<Type> types = null) {
            var factoryType = typeof(FactoryProviderBuilder<>).MakeGenericType(new[] { type });
            var ctor = factoryType.GetConstructors().First(info =>
                info.GetParameters().Length == 4 &&
                info.GetParameters()[0].ParameterType == typeof(Container) &&
                info.GetParameters()[1].ParameterType == typeof(Lifetime) &&
                info.GetParameters()[2].ParameterType == typeof(Func<object>) &&
                info.GetParameters()[3].ParameterType == typeof(IEnumerable<Type>)
            );
            IProviderBuilder @this = (IProviderBuilder)ctor.Invoke(new object[] { container, lifetime, factory, types });
            return @this;
        }
    }

    public class FactoryProviderBuilder<T> : TypedProviderBuilder<FactoryProviderBuilder<T>>
        where T : class {
        private Lifetime _lifetime = DI.Lifetime.Singleton;
        private Func<T>? _factory;

        public FactoryProviderBuilder(Container container) : base(container) {
        }

        // This constructor is used by reflection
        public FactoryProviderBuilder(Container container, Lifetime lifetime, Func<object>? factory, IEnumerable<Type> types) :
            base(container) {
            _lifetime = lifetime;
            As(types.ToArray());
            if (factory != null) {
                _factory = () => (T)factory();
            }
        }

        public FactoryProviderBuilder<T> IsTransient() => Lifetime(DI.Lifetime.Transient);
        public FactoryProviderBuilder<T> IsSingleton() => Lifetime(DI.Lifetime.Singleton);

        public FactoryProviderBuilder<T> Lifetime(Lifetime? lifetime) {
            _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            return this;
        }

        public FactoryProviderBuilder<T> With(Func<T> factory) {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            return this;
        }

        protected override IProvider CreateProvider() {
            Types.Remove(typeof(IDisposable));
            if (Types.Count == 0) As<T>();
            var typeToBuild = typeof(T);
            if (_factory == null) {
                if (typeToBuild.IsAbstract || typeToBuild.IsInterface) {
                    throw new ArgumentException("Can't create a default factory with interface or abstract class");
                }
                _factory = Activator.CreateInstance<T>;
            }
            foreach (var registeredType in Types) {
                if (!registeredType.IsAssignableFrom(typeToBuild)) {
                    throw new InvalidCastException("Error registering type " + registeredType + " to provide " +
                                                   typeToBuild);
                }
            }
            IProvider provider = _lifetime switch {
                DI.Lifetime.Singleton => new SingletonProvider<T>(Types.ToArray(), _factory),
                DI.Lifetime.Transient => new TransientProvider<T>(Types.ToArray(), _factory),
                _ => throw new Exception("Unknown lifetime " + _lifetime)
            };
            return provider;
        }
    }

    public class FunctionProviderBuilder<TIn, TOut> : BaseProviderBuilder {
        private readonly Func<TIn, TOut> _factory;

        public FunctionProviderBuilder(Container? container, Func<TIn, TOut> factory) : base(container) {
            _factory = factory;
        }

        protected override IProvider CreateProvider() {
            return new SingletonProvider<Func<TIn, TOut>>(new[] { typeof(Func<TIn, TOut>) }, () => _factory);
        }
    }
}