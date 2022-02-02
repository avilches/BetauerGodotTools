using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Godot;
using Object = Godot.Object;

namespace Betauer.DI {
    public interface IProviderBuilder {
        public IProvider Build();
    }

    public abstract class BaseProviderBuilder<TBuilder> : IProviderBuilder where TBuilder : class {
        protected readonly HashSet<Type> Types = new HashSet<Type>();
        protected readonly Container Container;

        protected BaseProviderBuilder(Container? container) {
            Container = container ?? throw new ArgumentNullException(nameof(container));
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

        public IProvider Build() {
            Container.Pending.Remove(this);
            var provider = CreateProvider();
            Container.Add(provider);
            return provider;
        }

        protected abstract IProvider CreateProvider();
    }

    public class FactoryProviderBuilder<T> : BaseProviderBuilder<FactoryProviderBuilder<T>>
        where T : class {
        private Scope _scope = DI.Scope.Singleton;
        private Func<T>? _factory;

        public FactoryProviderBuilder(Container container) : base(container) {
        }

        public FactoryProviderBuilder(Container container, Scope scope, IEnumerable<Type> types) :
            base(container) {
            _scope = scope;
            As(types.ToArray());
        }

        public static IProviderBuilder Create(Container container, Type type,
            Scope scope = DI.Scope.Singleton, IEnumerable<Type> types = null) {
            var factoryType = typeof(FactoryProviderBuilder<>).MakeGenericType(new Type[] { type });
            var ctor = factoryType.GetConstructors().First(info =>
                info.GetParameters().Length == 3 &&
                info.GetParameters()[0].ParameterType == typeof(Container) &&
                info.GetParameters()[1].ParameterType == typeof(Scope) &&
                info.GetParameters()[2].ParameterType == typeof(IEnumerable<Type>)
            );
            IProviderBuilder @this = (IProviderBuilder)ctor.Invoke(new object[] { container, scope, types });
            return @this;
        }

        public FactoryProviderBuilder<T> IsPrototype() => Scope(DI.Scope.Prototype);
        public FactoryProviderBuilder<T> IsSingleton() => Scope(DI.Scope.Singleton);

        public FactoryProviderBuilder<T> Scope(Scope? scope) {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
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
            IProvider provider = _scope switch {
                DI.Scope.Singleton => new SingletonProvider<T>(Types.ToArray(), _factory),
                DI.Scope.Prototype => new PrototypeProvider<T>(Types.ToArray(), _factory),
                _ => throw new Exception("Unknown scope " + _scope)
            };
            return provider;
        }
    }
}