using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.ServiceProvider; 

public abstract class Provider : IProvider {
    public static IProvider Static<T>(T instance, string? name = null) where T : class {
        return Singleton(() => instance, name);
    }

    public static IProvider Static<TI, T>(T instance, string? name = null) where T : class {
        return Singleton<TI, T>(() => instance, name);
    }

    public static IProvider Singleton<T>(string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        var factory = CreateDefaultFactory<T>(Lifetime.Singleton);
        return Create<T, T>(Lifetime.Singleton, factory, name, lazy, metadata);
    }

    public static IProvider Singleton<T>(Func<T> factory, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return Create<T, T>(Lifetime.Singleton, factory, name, lazy, metadata);
    }

    public static IProvider Singleton<TI, T>(Func<T> factory, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return Create<TI, T>(Lifetime.Singleton, factory, name, lazy, metadata);
    }

    public static IProvider Singleton<TI, T>(string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        var factory = CreateDefaultFactory<T>(Lifetime.Singleton);
        return Create<TI, T>(Lifetime.Singleton, factory, name, lazy, metadata);
    }

    public static IProvider Transient<T>(string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        var factory = CreateDefaultFactory<T>(Lifetime.Transient);
        return Create<T, T>(Lifetime.Transient, factory, name, false, metadata);
    }

    public static IProvider Transient<T>(Func<T> factory, string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        return Create<T, T>(Lifetime.Transient, factory, name, false, metadata);
    }

    public static IProvider Transient<TI, T>(Func<T> factory, string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        return Create<TI, T>(Lifetime.Transient, factory, name, false, metadata);
    }

    public static IProvider Transient<TI, T>(string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        var factory = CreateDefaultFactory<T>(Lifetime.Transient);
        return Create<TI, T>(Lifetime.Transient, factory, name, false, metadata);
    }

    public static IProvider Service<T>(Lifetime lifetime, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        Func<T> factory = CreateDefaultFactory<T>(lifetime);
        return Create<T, T>(lifetime, factory, name, lazy, metadata);
    }

    public static IProvider Service<T>(Func<T> factory, Lifetime lifetime, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return Create<T, T>(lifetime, factory, name, lazy, metadata);
    }

    public static IProvider Service<TI, T>(Func<T> factory, Lifetime lifetime, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return Create<TI, T>(lifetime, factory, name, lazy, metadata);
    }

    public static IProvider Service<TI, T>(Lifetime lifetime, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        Func<T> factory = CreateDefaultFactory<T>(lifetime);
        return Create<TI, T>(lifetime, factory, name, lazy, metadata);
    }

    public static IProvider Create<TI, T>(Lifetime lifetime, Func<T>? factory = null, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        factory ??= CreateDefaultFactory<T>(lifetime);
        return Create(typeof(TI), typeof(T), lifetime, factory, name, lazy, metadata);
    }

    public static IProvider Create(Type registeredType, Type type, Lifetime lifetime, Func<object>? factory = null, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) {
        factory ??= CreateDefaultFactory(type, lifetime);
        return lifetime == Lifetime.Singleton
            ? new SingletonFactoryProvider(registeredType, type, factory, name, lazy, metadata)
            : new TransientFactoryProvider(registeredType, type, factory, name, metadata);
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
    public abstract Lifetime Lifetime { get; }

    public Dictionary<string, object> Metadata { get; }

    protected Provider(Type registerType, Type providerType, string? name, Dictionary<string, object>? metadata) {
        if (!registerType.IsAssignableFrom(providerType)) {
            throw new InvalidCastException(
                $"Can't create a provider of {providerType.GetTypeName()} and register with {registerType.GetTypeName()}");
        }
        RegisterType = registerType;
        ProviderType = providerType;
        Name = name;
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    public abstract object Get();

    public abstract object Resolve(ResolveContext context);
}