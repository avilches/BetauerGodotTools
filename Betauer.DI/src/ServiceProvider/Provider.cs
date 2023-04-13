using System;
using Betauer.Core;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.ServiceProvider {
    public abstract class Provider : IProvider {
        public static IProvider Static<T>(T instance, string? name = null, bool primary = false) where T : class {
            return Singleton(() => instance, name, primary);
        }

        public static IProvider Static<TI, T>(T instance, string? name = null, bool primary = false) where T : class {
            return Singleton<TI, T>(() => instance, name, primary);
        }

        public static IProvider Singleton<T>(string? name = null, bool primary = false, bool lazy = false) where T : class {
            var factory = CreateDefaultFactory<T>(Lifetime.Singleton);
            return Create<T, T>(Lifetime.Singleton, factory, name, primary, lazy);
        }

        public static IProvider Singleton<T>(Func<T> factory, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Create<T, T>(Lifetime.Singleton, factory, name, primary, lazy);
        }

        public static IProvider Singleton<TI, T>(Func<T> factory, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Create<TI, T>(Lifetime.Singleton, factory, name, primary, lazy);
        }

        public static IProvider Singleton<TI, T>(string? name = null, bool primary = false, bool lazy = false) where T : class {
            var factory = CreateDefaultFactory<T>(Lifetime.Singleton);
            return Create<TI, T>(Lifetime.Singleton, factory, name, primary, lazy);
        }

        public static IProvider Transient<T>(string? name = null, bool primary = false) where T : class {
            var factory = CreateDefaultFactory<T>(Lifetime.Transient);
            return Create<T, T>(Lifetime.Transient, factory, name, primary, false);
        }

        public static IProvider Transient<T>(Func<T> factory, string? name = null, bool primary = false) where T : class {
            return Create<T, T>(Lifetime.Transient, factory, name, primary, false);
        }

        public static IProvider Transient<TI, T>(Func<T> factory, string? name = null, bool primary = false) where T : class {
            return Create<TI, T>(Lifetime.Transient, factory, name, primary, false);
        }

        public static IProvider Transient<TI, T>(string? name = null, bool primary = false) where T : class {
            var factory = CreateDefaultFactory<T>(Lifetime.Transient);
            return Create<TI, T>(Lifetime.Transient, factory, name, primary, false);
        }

        public static IProvider Service<T>(Lifetime lifetime, string? name = null, bool primary = false, bool lazy = false) where T : class {
            Func<T> factory = CreateDefaultFactory<T>(lifetime);
            return Create<T, T>(lifetime, factory, name, primary, lazy);
        }

        public static IProvider Service<T>(Func<T> factory, Lifetime lifetime, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Create<T, T>(lifetime, factory, name, primary, lazy);
        }

        public static IProvider Service<TI, T>(Func<T> factory, Lifetime lifetime, string? name = null, bool primary = false, bool lazy = false) where T : class {
            return Create<TI, T>(lifetime, factory, name, primary, lazy);
        }

        public static IProvider Service<TI, T>(Lifetime lifetime, string? name = null, bool primary = false, bool lazy = false) where T : class {
            Func<T> factory = CreateDefaultFactory<T>(lifetime);
            return Create<TI, T>(lifetime, factory, name, primary, lazy);
        }

        public static IProvider Create<TI, T>(Lifetime lifetime, Func<T>? factory = null, string? name = null, bool primary = false, bool lazy = false) where T : class {
            factory ??= CreateDefaultFactory<T>(lifetime);
            return Create(typeof(TI), typeof(T), lifetime, factory, name, primary, lazy);
        }

        public static IProvider Create(Type registeredType, Type type, Lifetime lifetime, Func<object>? factory = null, string? name = null, bool primary = false, bool lazy = false) {
            factory ??= CreateDefaultFactory(type, lifetime);
            return lifetime == Lifetime.Singleton
                ? new SingletonFactoryProvider(registeredType, type, factory, name, primary, lazy)
                : new TransientFactoryProvider(registeredType, type, factory, name, primary);
        }

        public static Func<object> CreateDefaultFactory(Type type, Lifetime lifetime) {
            if (type.IsAbstract || type.IsInterface)
                throw new MissingMethodException(
                    $"Can't create default factory for and abstract or interface type: {type.GetTypeName()}");
            return lifetime == Lifetime.Singleton ? () => Activator.CreateInstance(type)! : LambdaCtor.CreateCtor(type);
        }

        public static Func<T> CreateDefaultFactory<T>(Lifetime lifetime) {
            if (typeof(T).IsAbstract || typeof(T).IsInterface)
                throw new MissingMethodException(
                    $"Can't create default factory for and abstract or interface type: {typeof(T).GetTypeName()}");
            return lifetime == Lifetime.Singleton ? Activator.CreateInstance<T> : LambdaCtor<T>.CreateInstance;
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
                    $"Can't create a provider of {providerType.GetTypeName()} and register with {registerType.GetTypeName()}");
            }
            RegisterType = registerType;
            ProviderType = providerType;
            Name = name;
            Primary = name != null && primary; // primary is needed only when the provider has name
        }

        public object Get() {
            return Container.Resolve(this);
        }

        public abstract object Resolve(ResolveContext context);
    }
}