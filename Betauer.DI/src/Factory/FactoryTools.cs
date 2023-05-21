using System;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public static class FactoryTools {

    /// <summary>
    /// Returns a class that wraps a provider (implementing ITransient<T> or ILazy<T>) so it can be exposed to the users 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static ProxyFactory CreateProxyFactory(Type type, IProvider provider) {
        if (provider.Lifetime == Lifetime.Singleton) {
            var factoryType = typeof(ProxySingletonFactory<>).MakeGenericType(type);
            ProxyFactory instance = (ProxyFactory)Activator.CreateInstance(factoryType, provider)!;
            return instance;
        } else {
            var factoryType = typeof(ProxyTransientFactory<>).MakeGenericType(type);
            ProxyFactory instance = (ProxyFactory)Activator.CreateInstance(factoryType, provider)!;
            return instance;
        }
    }

    /// <summary>
    /// It creates a FactoryWrapper<type>() with a provider inside and returns the GetFactory method, 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static Func<object> GetGetFromWrapperFactory(Type type, IProvider provider) {
        var factoryType = typeof(FactoryWrapper<>).MakeGenericType(type);
        FactoryWrapper instance = (FactoryWrapper)Activator.CreateInstance(factoryType, provider)!;
        return instance.GetFactory;
    }
}