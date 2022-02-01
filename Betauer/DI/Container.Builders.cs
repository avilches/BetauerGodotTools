using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Godot;
using Object = Godot.Object;

namespace Betauer.DI {
    public interface IServiceBuilder {
        public IService Build();
    }

    public abstract class ServiceBuilder<TBuilder> : IServiceBuilder where TBuilder : class {
        protected readonly HashSet<Type> Types = new HashSet<Type>();
        protected readonly Container Container;

        protected ServiceBuilder(Container? container) {
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

        public IService Build() {
            Container.Pending.Remove(this);
            var service = CreateService();
            Container.Add(service);
            return service;
        }

        protected abstract IService CreateService();
    }

    public class SingletonInstanceServiceBuilder<T> : ServiceBuilder<SingletonInstanceServiceBuilder<T>> {
        private readonly T _instance;

        public SingletonInstanceServiceBuilder(Container? container, T instance) : base(container) {
            _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        protected override IService CreateService() {
            Types.Remove(typeof(IDisposable));
            if (Types.Count == 0) {
                As(_instance.GetType());
            }
            foreach (var type in Types) {
                if (!type.IsInstanceOfType(_instance)) {
                    throw new ArgumentException("Instance is not type of " + type);
                }
            }
            var singletonInstanceService = new SingletonInstanceService<T>(Types.ToArray(), _instance);
            Container.Add(singletonInstanceService);
            Container.AfterCreate(_instance);
            return singletonInstanceService;
        }
    }


    public abstract class LifestyleFactoryServiceBuilder<TBuilder> : ServiceBuilder<TBuilder> where TBuilder : class {
        protected Lifestyle _lifestyle = DI.Lifestyle.Singleton;

        public LifestyleFactoryServiceBuilder(Container container) : base(container) {
        }

        public TBuilder IsTransient() => Lifestyle(DI.Lifestyle.Transient);
        public TBuilder IsSingleton() => Lifestyle(DI.Lifestyle.Singleton);

        public TBuilder Lifestyle(Lifestyle? lifestyle) {
            _lifestyle = lifestyle ?? throw new ArgumentNullException(nameof(lifestyle));
            return (this as TBuilder)!;
        }
    }

    public class FactoryServiceBuilder<T> : LifestyleFactoryServiceBuilder<FactoryServiceBuilder<T>> where T : class {
        private Func<T>? _factory;

        public FactoryServiceBuilder(Container container) : base(container) {
        }

        public FactoryServiceBuilder<T> With(Func<T> factory) {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            return this;
        }

        protected override IService CreateService() {
            Types.Remove(typeof(IDisposable));
            if (Types.Count == 0) As<T>();
            var type = typeof(T);
            if (_factory == null) {
                if (type.IsAbstract || type.IsInterface) {
                    throw new ArgumentException("Can't create a default factory with interface or abstract class");
                }
                _factory = Activator.CreateInstance<T>;
            }
            foreach (var t in Types) {
                if (!type.IsAssignableFrom(t)) {
                    throw new ArgumentException("Instance is not a valid type for " + type);
                }
            }
            IService service = _lifestyle switch {
                DI.Lifestyle.Singleton => new SingletonFactoryService<T>(Types.ToArray(), _factory),
                DI.Lifestyle.Transient => new TransientFactoryService<T>(Types.ToArray(), _factory),
                _ => throw new Exception("Unknown lifestyle " + _lifestyle)
            };
            Container.Add(service);
            return service;
        }
    }

    public class RuntimeFactoryServiceBuilder : LifestyleFactoryServiceBuilder<RuntimeFactoryServiceBuilder> {
        private readonly Type _type;

        public RuntimeFactoryServiceBuilder(Container container, Type? type) : base(container) {
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }

        protected override IService CreateService() {
            Types.Remove(typeof(IDisposable));
            if (Types.Count == 0) As(_type);
            if (_type.IsAbstract || _type.IsInterface) {
                throw new ArgumentException("Can't create a default factory with interface or abstract class");
            }
            Func<object> factory = () => Activator.CreateInstance(_type);

            foreach (var type in Types) {
                if (!type.IsAssignableFrom(_type)) {
                    throw new ArgumentException("Instance is not a valid type for " + type);
                }
            }
            IService service = _lifestyle switch {
                DI.Lifestyle.Singleton => new SingletonFactoryService<object>(Types.ToArray(), factory),
                DI.Lifestyle.Transient => new TransientFactoryService<object>(Types.ToArray(), factory),
                _ => throw new Exception("Unknown lifestyle " + _lifestyle)
            };
            Container.Add(service);
            return service;
        }
    }
}