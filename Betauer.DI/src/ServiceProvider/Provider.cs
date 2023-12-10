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
        return Singleton<T, T>(null!, name, lazy, metadata);
    }

    public static IProvider Singleton<T>(Func<T> factory, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return Singleton<T, T>(factory, name, lazy, metadata);
    }

    public static IProvider Singleton<TI, T>(string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return Singleton<TI, T>(null!, name, lazy, metadata);
    }

    public static IProvider Singleton<TI, T>(Func<T> factory, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return new SingletonFactoryProvider(typeof(TI), typeof(T), factory, name, lazy, metadata);
    }
    
    

    public static IProvider Transient<T>(string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        return Transient<T, T>(null!, name, metadata);
    }

    public static IProvider Transient<T>(Func<T> factory, string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        return Transient<T, T>(factory, name, metadata);
    }

    public static IProvider Transient<TI, T>(string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        return Transient<TI, T>(null!, name, metadata);
    }

    public static IProvider Transient<TI, T>(Func<T> factory, string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        return new TransientFactoryProvider(typeof(TI), typeof(T), factory, name, metadata);
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