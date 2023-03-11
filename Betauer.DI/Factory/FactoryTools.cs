using System;
using System.Linq;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public static class FactoryTools {

    public static Type? GetIFactoryGenericType(Type factoryType) {
        var isIFactoryInterface = factoryType.IsInterface && factoryType.GetGenericTypeDefinition() == typeof(IFactory<>);
        if (isIFactoryInterface) return factoryType.GetGenericArguments()[0];
        var iFactoryInterface = factoryType
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFactory<>));
        return iFactoryInterface?.GetGenericArguments()[0];
    }

    /// <summary>
    /// Returns a IFactory<type> where every call to Get() invokes a provider.Get(), creating a new instance
    /// </summary>
    /// <param name="type"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static ProviderFactory CreateIFactoryFromProvider(Type type, IProvider provider) {
        var factoryType = typeof(ProviderFactory<>).MakeGenericType(type);
        ProviderFactory instance = (ProviderFactory)Activator.CreateInstance(factoryType, provider)!;
        return instance;
    }

    /// <summary>
    /// Returns a IFactory<type> where every call to Get() creates a new instance, taking into account the provider returns
    /// the real IFactory<type>, so it first invokes to provider.Get() to obtain the real IFactory<type>, then it calls
    /// again Get() to this factory to create the new instance.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="customFactoryProvider"></param>
    /// <returns></returns>
    public static ProviderCustomFactory CreateIFactoryFromCustomFactoryProvider(Type type, IProvider customFactoryProvider) {
        var factoryType = typeof(ProviderCustomFactory<>).MakeGenericType(type);
        ProviderCustomFactory instance = (ProviderCustomFactory)Activator.CreateInstance(factoryType, customFactoryProvider)!;
        return instance;
    }
}