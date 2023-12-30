using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.DI.Factory;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.ServiceProvider; 

public abstract class Provider {
    
    // Static
    
    public static StaticProvider Static<T>(T instance, string? name = null) where T : class {
        return Static<T, T>(instance, name);
    }

    public static StaticProvider Static<TI, T>(T instance, string? name = null) where T : class {
        return new StaticProvider(typeof(TI), typeof(T), instance, name);
    }

    // Singleton
    
    public static SingletonProvider Singleton<T>(string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return Singleton<T, T>((Func<T>)null!, name, lazy, metadata);
    }

    public static SingletonProvider Singleton<T>(IFactory<T> factory, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return Singleton<T, T>(FactoryTools.From(factory), name, lazy, metadata);
    }

    public static SingletonProvider Singleton<T>(Func<T> factory, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return Singleton<T, T>(factory, name, lazy, metadata);
    }

    public static SingletonProvider Singleton<TI, T>(string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return Singleton<TI, T>((Func<T>)null!, name, lazy, metadata);
    }

    public static SingletonProvider Singleton<TI, T>(IFactory<T> factory, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return Singleton<TI, T>(FactoryTools.From(factory), name, lazy, metadata);
    }
    
    public static SingletonProvider Singleton<TI, T>(Func<T> factory, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return new SingletonProvider(typeof(TI), typeof(T), factory, name, lazy, metadata);
    }

    // Transient

    public static TransientProvider Transient<T>(string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        return Transient<T, T>((Func<T>)null!, name, metadata);
    }

    public static TransientProvider Transient<T>(IFactory<T> factory, string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        return Transient<T, T>(FactoryTools.From(factory), name, metadata);
    }

    public static TransientProvider Transient<T>(Func<T> factory, string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        return Transient<T, T>(factory, name, metadata);
    }

    public static TransientProvider Transient<TI, T>(string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        return Transient<TI, T>((Func<T>)null!, name, metadata);
    }

    public static TransientProvider Transient<TI, T>(IFactory<T> factory, string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        return Transient<TI, T>(FactoryTools.From(factory), name, metadata);
    }

    public static TransientProvider Transient<TI, T>(Func<T> factory, string? name = null, Dictionary<string, object>? metadata = null) where T : class {
        return new TransientProvider(typeof(TI), typeof(T), factory, name, metadata);
    }
    
    // Proxy
    
    public static readonly string ProxyPrefix = "Proxy:";

    public static ProxyProvider Lazy(SingletonProvider provider) {
        var proxyPublicType = typeof(ILazy<>).MakeGenericType(provider.PublicType);
        var factoryType = typeof(Proxy.Lazy<>).MakeGenericType(provider.RealType);
        var proxyInstance = (Proxy)Activator.CreateInstance(factoryType, provider)!;
        var name = provider.Name == null ? null : $"{ProxyPrefix}{provider.Name}";
        return new ProxyProvider(proxyPublicType, proxyInstance, name);
    }

    public static ProxyProvider TransientFactory(TransientProvider provider) {
        var proxyPublicType = typeof(ITransient<>).MakeGenericType(provider.PublicType);
        var factoryType = typeof(Proxy.Transient<>).MakeGenericType(provider.RealType);
        var proxyInstance = (Proxy)Activator.CreateInstance(factoryType, provider)!;
        var name = provider.Name == null ? null : $"{ProxyPrefix}{provider.Name}";
        return new ProxyProvider(proxyPublicType, proxyInstance, name);
    }
    
    public static Func<object> CreateCtor(Type type, Lifetime lifetime) {
        if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition || type == typeof(void))
            throw new NotSupportedException($"Can't create a constructor for void, abstract classes, interfaces or generic type definition classes: {type}");
        return lifetime == Lifetime.Singleton ? () => Activator.CreateInstance(type)! : LambdaCtor.CreateCtor(type);
    }

    public static Func<T> CreateCtor<T>(Lifetime lifetime) where T : class {
        if (typeof(T).IsAbstract || typeof(T).IsInterface)
            throw new NotSupportedException($"Can't create a constructor for abstract classes or interfaces: {typeof(T)}");
        return lifetime == Lifetime.Singleton ? Activator.CreateInstance<T> : LambdaCtor<T>.CreateInstance;
    }


    public static Dictionary<string, object> FlagsToMetadata(string? flags, string? moreFlags = null) {
        flags ??= "";
        moreFlags ??= "";
        return (flags+","+moreFlags).Split(",")
            .Where(flag => flag.Length > 0)
            .ToDictionary(valor => valor, _ => (object)true) ?? new Dictionary<string, object>();
    }

    public Container Container { get; set; }
    public Type PublicType { get; }
    public Type RealType { get; }
    public string? Name { get; }
    public abstract Lifetime Lifetime { get; }

    public Dictionary<string, object> Metadata { get; }

    public object GetMetadata(string key) => Metadata[key];
    public T GetMetadata<T>(string key) => (T)Metadata[key];
    public T GetMetadata<T>(string key, T @default) => Metadata.TryGetValue(key, out var value) ? (T)value : @default;
    public bool GetFlag(string key, bool @default = false) => Metadata.TryGetValue(key, out var value) ? (bool)value : @default;

    protected Provider(Type publicType, Type realType, string? name, Dictionary<string, object>? metadata) {
        if (!publicType.IsAssignableFrom(realType)) {
            throw new InvalidCastException(
                $"Can't create a provider of {realType.GetTypeName()} and register with {publicType.GetTypeName()}");
        }
        PublicType = publicType;
        RealType = realType;
        Name = name;
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    public abstract object Get();

    public abstract object Resolve(ResolveContext context);

    public bool CanBeAssignedTo(Type type) {
        return type.IsAssignableFrom(RealType);
    }
}