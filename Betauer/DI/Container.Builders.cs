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


    public class FactoryServiceBuilder<T> : ServiceBuilder<FactoryServiceBuilder<T>> where T : class {
        private Lifestyle _lifestyle = DI.Lifestyle.Singleton;
        private Func<T>? _factory;

        public FactoryServiceBuilder(Container container) : base(container) {
        }

        public FactoryServiceBuilder<T> IsTransient() => Lifestyle(DI.Lifestyle.Transient);
        public FactoryServiceBuilder<T> IsSingleton() => Lifestyle(DI.Lifestyle.Singleton);

        public FactoryServiceBuilder<T> Lifestyle(Lifestyle? lifestyle) {
            _lifestyle = lifestyle ?? throw new ArgumentNullException(nameof(lifestyle));
            return this;
        }

        public FactoryServiceBuilder<T> With(Func<T> factory) {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            return this;
        }

        protected override IService CreateService() {
            Types.Remove(typeof(IDisposable));
            var target = typeof(T);
            if (typeof(T) == typeof(object)) {
                if (Types.Count == 0) {
                    throw new ArgumentException("Type not defined");
                }
                target = Types.ToList().Find(type => type.IsClass && !type.IsAbstract);
                if (target == null) {
                    throw new ArgumentException("Valid class not found in types");
                }
                _factory ??= () => (T)Activator.CreateInstance(target);
            } else {
                if (Types.Count == 0) As<T>();
                if (_factory == null) {
                    if (typeof(T).IsAbstract || typeof(T).IsInterface) {
                        throw new ArgumentException("Can't create a default factory with interface or abstract class");
                    }
                    _factory = Activator.CreateInstance<T>;
                }
            }

            foreach (var type in Types) {
                // GD.Print("Si buscas por " + type + ", te devuelvo un " + target);
                if (!type.IsAssignableFrom(target)) {
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

    public class FactoryServiceBuilder : FactoryServiceBuilder<object> {
        public FactoryServiceBuilder(Container container) : base(container) {
        }
    }
}