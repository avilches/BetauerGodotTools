using System;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public abstract class FactoryProvider {
    protected readonly IProvider Provider;

    protected FactoryProvider(IProvider provider) {
        Provider = provider;
    }

    public static FactoryProvider Create(Type type, IProvider provider) {
        var factoryType = typeof(FactoryProvider<>).MakeGenericType(type);
        FactoryProvider instance = (FactoryProvider)Activator.CreateInstance(factoryType, provider)!;
        return instance;
    }
}

public class FactoryProvider<T> : FactoryProvider, IFactory<T> {
    public FactoryProvider(IProvider provider) : base(provider) {
    }

    public T Get() => (T)Provider.Get();
}

