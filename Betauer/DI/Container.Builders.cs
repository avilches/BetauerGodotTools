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
        public TBuilder AsAll<T>() => AsAll(typeof(T));
        public TBuilder AsAll(Type type) => As(GetTypesFrom(type));

        public TBuilder As(IEnumerable<Type>? types) {
            if (types != null) {
                foreach (var type in types) As(type);
            }
            return (this as TBuilder)!;
        }

        public TBuilder As(Type? type) {
            if (type != null) Types.Add(type);
            return (this as TBuilder)!;
        }

        private static IEnumerable<Type> GetTypesFrom(Type? type) {
            if (type == null) return new Type[] { };
            var types = type.GetInterfaces().ToList();
            types.Add(type);
            return types;
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
        private Lifestyle _lifestyle = DI.Lifestyle.Singleton;
        private Func<T>? _factory;

        public FactoryProviderBuilder(Container container) : base(container) {
        }

        public FactoryProviderBuilder(Container container, Lifestyle lifestyle, IEnumerable<Type> types) :
            base(container) {
            _lifestyle = lifestyle;
            As(types);
        }

        public static IProviderBuilder Create(Container container, Type type,Lifestyle lifestyle = DI.Lifestyle.Singleton, IEnumerable<Type> types = null) {
            var factoryType = typeof(FactoryProviderBuilder<>).MakeGenericType(new Type[] { type });
            var ctor = factoryType.GetConstructors().First(info =>
                info.GetParameters().Length == 3 &&
                info.GetParameters()[0].ParameterType == typeof(Container) &&
                info.GetParameters()[1].ParameterType == typeof(Lifestyle) &&
                info.GetParameters()[2].ParameterType == typeof(IEnumerable<Type>)
            );
            IProviderBuilder @this = (IProviderBuilder)ctor.Invoke(new object[] { container, lifestyle, types });
            return @this;
        }

        public FactoryProviderBuilder<T> IsTransient() => Lifestyle(DI.Lifestyle.Transient);
        public FactoryProviderBuilder<T> IsSingleton() => Lifestyle(DI.Lifestyle.Singleton);

        public FactoryProviderBuilder<T> Lifestyle(Lifestyle? lifestyle) {
            _lifestyle = lifestyle ?? throw new ArgumentNullException(nameof(lifestyle));
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
            IProvider provider = _lifestyle switch {
                DI.Lifestyle.Singleton => new SingletonFactoryProvider<T>(Types.ToArray(), _factory),
                DI.Lifestyle.Transient => new TransientFactoryProvider<T>(Types.ToArray(), _factory),
                _ => throw new Exception("Unknown lifestyle " + _lifestyle)
            };
            return provider;
        }

    }
}