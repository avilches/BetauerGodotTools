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
        public TBuilder AsAll(Type type) => AsAll(GetTypesFrom(type));

        public TBuilder AsAll(IEnumerable<Type> types) {
            foreach (var type in types) As(type);
            return (this as TBuilder)!;
        }

        public TBuilder As(Type type) {
            Types.Add(type);
            return (this as TBuilder)!;
        }

        private static IEnumerable<Type> GetTypesFrom(Type type) {
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

    public abstract class LifestyleFactoryBaseProviderBuilder<TBuilder> : BaseProviderBuilder<TBuilder>
        where TBuilder : class {
        protected Lifestyle _lifestyle = DI.Lifestyle.Singleton;

        public LifestyleFactoryBaseProviderBuilder(Container container) : base(container) {
        }

        public TBuilder IsTransient() => Lifestyle(DI.Lifestyle.Transient);
        public TBuilder IsSingleton() => Lifestyle(DI.Lifestyle.Singleton);

        public TBuilder Lifestyle(Lifestyle? lifestyle) {
            _lifestyle = lifestyle ?? throw new ArgumentNullException(nameof(lifestyle));
            return (this as TBuilder)!;
        }
    }

    public class FactoryProviderBuilder<T> : LifestyleFactoryBaseProviderBuilder<FactoryProviderBuilder<T>>
        where T : class {
        private Func<T>? _factory;

        public FactoryProviderBuilder(Container container) : base(container) {
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
                    throw new InvalidCastException("Error registering type " + registeredType + " to provide " + typeToBuild);
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

    public class RuntimeFactoryProviderBuilder : LifestyleFactoryBaseProviderBuilder<RuntimeFactoryProviderBuilder> {
        private readonly Type _typeToBuild;

        public RuntimeFactoryProviderBuilder(Container container, Type? type) : base(container) {
            _typeToBuild = type ?? throw new ArgumentNullException(nameof(type));
        }

        protected override IProvider CreateProvider() {
            Types.Remove(typeof(IDisposable));
            if (Types.Count == 0) As(_typeToBuild);
            if (_typeToBuild.IsAbstract || _typeToBuild.IsInterface) {
                throw new ArgumentException("Can't create a default factory with interface or abstract class");
            }
            Func<object> factory = () => Activator.CreateInstance(_typeToBuild);
            foreach (var registeredType in Types) {
                GD.Print($"{_typeToBuild.Name}.IsAssignableFrom({registeredType.Name}) = {_typeToBuild.IsAssignableFrom(registeredType)}");
                if (!_typeToBuild.IsAssignableFrom(registeredType)) {
                    throw new InvalidCastException("Error registering type " + registeredType + " to provide " + _typeToBuild);
                }
            }
            IProvider provider = _lifestyle switch {
                DI.Lifestyle.Singleton => new SingletonFactoryProvider<object>(Types.ToArray(), factory),
                DI.Lifestyle.Transient => new TransientFactoryProvider<object>(Types.ToArray(), factory),
                _ => throw new Exception("Unknown lifestyle " + _lifestyle)
            };
            return provider;
        }
    }
}