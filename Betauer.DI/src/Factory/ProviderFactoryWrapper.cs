using System;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public abstract class ProviderFactoryWrapper {

    /// <summary>
    /// It creates a ProviderFactory<type>() with a provider that returns a IFactory<type>
    /// The ProviderFactory has a method a Create() method that resolves the factory from the provider and calls the Create() method to it
    /// </summary>
    /// <param name="type"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static ProviderFactoryWrapper CreateFromProvider(Type type, IProvider provider) {
        var factoryType = typeof(ProviderFactoryWrapper<>).MakeGenericType(type);
        return (ProviderFactoryWrapper)Activator.CreateInstance(factoryType, provider)!;
    }
    
    public abstract object Create();
}

public class ProviderFactoryWrapper<T> : ProviderFactoryWrapper where T : class {
    private readonly IProvider _factoryProvider;

    public ProviderFactoryWrapper(IProvider factoryProvider) {
        _factoryProvider = factoryProvider;
    }

    public override object Create() {
        IFactory<T> factory = (IFactory<T>)_factoryProvider.Get();
        return factory.Create();
    }
}