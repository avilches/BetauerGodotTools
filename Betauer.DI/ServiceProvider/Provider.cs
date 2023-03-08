using System;
using Betauer.Tools.Reflection;

namespace Betauer.DI.ServiceProvider {
    public abstract class Provider : IProvider {
        public static IProvider Static<T>(T instance, string? name = null, bool primary = false) where T : class {
            return new SingletonInstanceProvider(typeof(T), instance.GetType(), instance, name, primary);
        }

        public static IProvider Static(Type type, object instance, string? name = null, bool primary = false) {
            return new SingletonInstanceProvider(type, instance.GetType(), instance, name, primary);
        }

        public static IProvider Singleton<T>(string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Create<T, T>(Activator.CreateInstance<T>, Lifetime.Singleton, name, primary, lazy);
        }

        public static IProvider Singleton<T>(Func<T> factory, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Create<T, T>(factory, Lifetime.Singleton, name, primary, lazy);
        }

        public static IProvider Singleton<TI, T>(Func<T> factory, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Create<TI, T>(factory, Lifetime.Singleton, name, primary, lazy);
        }

        public static IProvider Singleton<TI, T>(string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Create<TI, T>(Activator.CreateInstance<T>, Lifetime.Singleton, name, primary, lazy);
        }

        public static IProvider Transient<T>(string? name = null, bool primary = false) where T : class {
            return Create<T, T>(LambdaCtor<T>.CreateInstance, Lifetime.Transient, name, primary, false);
        }

        public static IProvider Transient<T>(Func<T> factory, string? name = null, bool primary = false) where T : class {
            return Create<T, T>(factory, Lifetime.Transient, name, primary, false);
        }

        public static IProvider Transient<TI, T>(Func<T> factory, string? name = null, bool primary = false) where T : class {
            return Create<TI, T>(factory, Lifetime.Transient, name, primary, false);
        }

        public static IProvider Transient<TI, T>(string? name = null, bool primary = false) where T : class {
            return Create<TI, T>(LambdaCtor<T>.CreateInstance, Lifetime.Transient, name, primary, false);
        }

        public static IProvider Service<T>(Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) where T : class {
            Func<T> factory = lifetime == Lifetime.Singleton ? Activator.CreateInstance<T> : LambdaCtor<T>.CreateInstance;
            return Create<T, T>(factory, lifetime, name, primary, lazy);
        }

        public static IProvider Service<T>(Func<T> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Create<T, T>(factory, lifetime, name, primary, lazy);
        }

        public static IProvider Service<TI, T>(Func<T> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Create<TI, T>(factory, lifetime, name, primary, lazy);
        }

        public static IProvider Service<TI, T>(Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) where T : class {
            Func<T> factory = lifetime == Lifetime.Singleton ? Activator.CreateInstance<T> : LambdaCtor<T>.CreateInstance;
            return Create<TI, T>(factory, lifetime, name, primary, lazy);
        }

        public static IProvider Create<TI, T>(Func<T> factory, Lifetime lifetime, string? name, bool primary, bool lazy = false) where T : class {
            return Create(typeof(TI), typeof(T), factory, lifetime, name, primary, lazy);
        }

        public static IProvider Create(Type registeredType, Type type, Func<object> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) {
            return lifetime == Lifetime.Singleton
                ? new SingletonFactoryProvider(registeredType, type, factory, name, primary, lazy)
                : new TransientFactoryProvider(registeredType, type, factory, name, primary);
        }


        public Container Container { get; set; }
        public Type RegisterType { get; }
        public Type ProviderType { get; }
        public string? Name { get; }
        public bool Primary { get; } // If name is not null, only the (last) Primary provider is used as fallback
        public abstract Lifetime Lifetime { get; }

        protected Provider(Type registerType, Type providerType, string? name, bool primary) {
            if (!registerType.IsAssignableFrom(providerType)) {
                throw new InvalidCastException(
                    $"Can't create a provider of {providerType.Name} and register with {registerType}");
            }
            RegisterType = registerType;
            ProviderType = providerType;
            Name = name;
            Primary = name != null && primary; // primary is needed only when the provider has name
        }

        public virtual object Get() {
            var context = Container.NewResolveContext();
            var instance = Get(context);
            context.End();
            return instance;
        }

        public abstract object Get(ResolveContext context);
    }
}