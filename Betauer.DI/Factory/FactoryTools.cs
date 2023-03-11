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
    /// Returns () => ((IFactory<T>)provider.Get()).Get()
    /// A class is needed to store the T and do a cast in compile time.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static Func<object> ProviderGetFactoryGet(Type type, IProvider provider) {
        var factoryType = typeof(ProviderCustomFactory<>).MakeGenericType(type);
        ProviderCustomFactory instance = (ProviderCustomFactory)Activator.CreateInstance(factoryType, provider)!;
        return instance.GetCustomFactory;
    }
}