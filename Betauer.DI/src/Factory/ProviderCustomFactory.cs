using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public abstract class ProviderCustomFactory {
    public abstract object GetCustomFactory();
}

public class ProviderCustomFactory<T> : ProviderCustomFactory where T : class {
    private readonly IProvider _customFactoryProvider;

    public ProviderCustomFactory(IProvider customFactoryProvider) {
        _customFactoryProvider = customFactoryProvider;
    }

    public override object GetCustomFactory() {
        IFactory<T> factory = (IFactory<T>)_customFactoryProvider.Get();
        return factory.Get()!;
    }
}