using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public abstract class FactoryWrapper {
    public abstract object GetFactory();
}

public class FactoryWrapper<T> : FactoryWrapper where T : class {
    private readonly IProvider _customFactoryProvider;

    public FactoryWrapper(IProvider customFactoryProvider) {
        _customFactoryProvider = customFactoryProvider;
    }

    public override object GetFactory() {
        IGet<T> factory = (IGet<T>)_customFactoryProvider.Get();
        return factory.Get()!;
    }
}