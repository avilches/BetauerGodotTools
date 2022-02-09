using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.DI {
    public interface IProviderBuilder {
        public IProvider CreateProvider();
    }

    public abstract class TypedProviderBuilder<T, TBuilder> : IProviderBuilder where TBuilder : class, IProviderBuilder {
        protected readonly HashSet<Type> Types = new HashSet<Type>();

        public TBuilder As<T1>() => As(typeof(T1));
        public TBuilder As<T1, T2>() => As(typeof(T1), typeof(T2));
        public TBuilder As<T1, T2, T3>() => As(typeof(T1), typeof(T2), typeof(T3));
        public TBuilder As<T1, T2, T3, T4>() => As(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        public TBuilder As<T1, T2, T3, T4, T5>() => As(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

        public TBuilder AsAll<T>() => AsAll(typeof(T));
        public TBuilder AsAll(Type type) => As(GetTypesFrom(type));

        public TBuilder As(params Type[] types) {
            foreach (var type in types) Types.Add(type);
            return (this as TBuilder)!;
        }

        private static Type[] GetTypesFrom(Type? type) {
            if (type == null) return new Type[] { };
            var types = type.GetInterfaces().ToList();
            types.Add(type);
            return types.ToArray();
        }

        public IProvider CreateProvider() {
            Types.Remove(typeof(IDisposable));
            if (Types.Count == 0) As<T>();
            var typeToBuild = typeof(T);
            foreach (var registeredType in Types) {
                if (!registeredType.IsAssignableFrom(typeToBuild)) {
                    throw new InvalidCastException("Error registering type " + registeredType + " to provide " +
                                                   typeToBuild);
                }
            }
            return CreateTypedProvider();
        }
        protected abstract IProvider CreateTypedProvider();
    }

    public static class FactoryProviderBuilder {
        public static IProviderBuilder Create(Type type,
            Lifetime lifetime, Func<object>? factory = null, IEnumerable<Type> types = null) {
            var factoryType = typeof(FactoryProviderBuilder<>).MakeGenericType(new[] { type });
            var ctor = factoryType.GetConstructors().First(info =>
                info.GetParameters().Length == 3 &&
                info.GetParameters()[0].ParameterType == typeof(Lifetime) &&
                info.GetParameters()[1].ParameterType == typeof(Func<object>) &&
                info.GetParameters()[2].ParameterType == typeof(IEnumerable<Type>)
            );
            IProviderBuilder @this = (IProviderBuilder)ctor.Invoke(new object[] { lifetime, factory, types });
            return @this;
        }
    }

    public class FactoryProviderBuilder<T> : TypedProviderBuilder<T, FactoryProviderBuilder<T>>
        where T : class {
        private Lifetime _lifetime = DI.Lifetime.Singleton;
        private Func<T>? _factory;

        public FactoryProviderBuilder() {
        }

        // This constructor is used by reflection
        public FactoryProviderBuilder(Lifetime lifetime, Func<object>? factory, IEnumerable<Type> types) {
            _lifetime = lifetime;
            As(types.ToArray());
            if (factory != null) {
                _factory = () => (T)factory();
            }
        }

        public FactoryProviderBuilder<T> IsTransient() => Lifetime(DI.Lifetime.Transient);
        public FactoryProviderBuilder<T> IsSingleton() => Lifetime(DI.Lifetime.Singleton);

        public FactoryProviderBuilder<T> Lifetime(Lifetime? lifetime) {
            if (lifetime.HasValue) _lifetime = lifetime.Value;
            return this;
        }

        public FactoryProviderBuilder<T> With(Func<T>? factory) {
            if (factory != null) _factory = factory;
            return this;
        }

        protected override IProvider CreateTypedProvider() {
            var typeToBuild = typeof(T);
            if (_factory == null) {
                if (typeToBuild.IsAbstract || typeToBuild.IsInterface) {
                    throw new ArgumentException("Can't create a default factory with interface or abstract class");
                }
                _factory = Activator.CreateInstance<T>;
            }
            IProvider provider = _lifetime switch {
                DI.Lifetime.Singleton => new SingletonProvider<T>(Types.ToArray(), _factory),
                DI.Lifetime.Transient => new TransientProvider<T>(Types.ToArray(), _factory),
                _ => throw new Exception("Unknown lifetime " + _lifetime)
            };
            return provider;
        }
    }

    public class StaticProviderBuilder<T> : TypedProviderBuilder<T, StaticProviderBuilder<T>> where T : class {
        private readonly T _value;

        public StaticProviderBuilder(T value) {
            _value = value;
        }

        protected override IProvider CreateTypedProvider() {
            return new StaticProvider<T>(Types.ToArray(), _value);
        }
    }

    public class FunctionProviderBuilder<TIn, TOut> : StaticProviderBuilder<Func<TIn, TOut>> {
        public FunctionProviderBuilder(Func<TIn, TOut> value) : base(value) {
        }
    }
}