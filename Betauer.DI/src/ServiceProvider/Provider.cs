using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.DI.Factory;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.ServiceProvider; 

public abstract class Provider : IProvider {
    public static IProvider Static<T>(T instance, string? name = null) where T : class {
        return Static<T, T>(instance, name);
    }

    public static IProvider Static<TI, T>(T instance, string? name = null) where T : class {
        return new StaticProvider(typeof(TI), typeof(T), instance, name);
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
        return new SingletonProvider(typeof(TI), typeof(T), null, factory, name, lazy, metadata);
    }
    
/*    
    public static IProvider ScopedSingleton<T>(string scope, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return ScopedSingleton<T, T>(null!, name, lazy, metadata);
    }

    public static IProvider ScopedSingleton<T>(Func<T> factory, string scope, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return ScopedSingleton<T, T>(factory, scope, name, lazy, metadata);
    }

    public static IProvider ScopedSingleton<TI, T>(string scope, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return ScopedSingleton<TI, T>(null!, scope, name, lazy, metadata);
    }

    public static IProvider ScopedSingleton<TI, T>(Func<T> factory, string scope, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) where T : class {
        return new SingletonProvider(typeof(TI), typeof(T), scope, factory, name, lazy, metadata);
    }
*/
    

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
        return new TransientProvider(typeof(TI), typeof(T), factory, name, metadata);
    }


    public static IProvider Proxy(IProvider provider) {
        return ProxyProvider.Create(provider);
    }



    public static IProvider SingletonFactory(object factoryInstance, string? scope, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) {
        var factoryType = factoryInstance.GetType();
        if (!factoryType.ImplementsInterface(typeof(IFactory<>))) {
            throw new InvalidCastException($"Factory {factoryType.GetTypeName()} must implement IFactory<>");
        }
        var type = factoryType.FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];
        // This is just () => ((IFactory<T>)factoryProvider).Create() but it's faster than using reflection to find the "Create()" method and invoke it
        var createMethod = FactoryWrapper.Create(type, factoryInstance).Create;
        return new SingletonProvider(type, type, scope, createMethod, name, lazy, metadata);
    }
    
    public static IProvider TransientFactory(object factoryInstance, string? name = null, Dictionary<string, object>? metadata = null) {
        var factoryType = factoryInstance.GetType();
        if (!factoryType.ImplementsInterface(typeof(IFactory<>))) {
            throw new InvalidCastException($"Factory {factoryType.GetTypeName()} must implement IFactory<>");
        }
        var type = factoryType.FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];
        // This is just () => ((IFactory<T>)factoryProvider).Create() but it's faster than using reflection to find the "Create()" method and invoke it
        var createMethod = FactoryWrapper.Create(type, factoryInstance).Create;
        return new TransientProvider(type, type, createMethod, name, metadata);
    }
    
    


    public static Func<object> CreateCtor(Type type, Lifetime lifetime) {
        if (type.IsAbstract || type.IsInterface)
            throw new MissingMethodException($"Can't create constructor for abstract or interface type: {type}");
        return lifetime == Lifetime.Singleton ? () => Activator.CreateInstance(type)! : LambdaCtor.CreateCtor(type);
    }

    public static Func<T> CreateCtor<T>(Lifetime lifetime) {
        if (typeof(T).IsAbstract || typeof(T).IsInterface)
            throw new MissingMethodException($"Can't create constructor for abstract or interface type: {typeof(T)}");
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
    public Type ExposedType { get; }
    public Type InstanceType { get; }
    public string? Name { get; }
    public abstract Lifetime Lifetime { get; }

    public Dictionary<string, object> Metadata { get; }

    protected Provider(Type exposedType, Type instanceType, string? name, Dictionary<string, object>? metadata) {
        if (!exposedType.IsAssignableFrom(instanceType)) {
            throw new InvalidCastException(
                $"Can't create a provider of {instanceType.GetTypeName()} and register with {exposedType.GetTypeName()}");
        }
        ExposedType = exposedType;
        InstanceType = instanceType;
        Name = name;
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    public abstract object Get();

    public abstract object Resolve(ResolveContext context);
}