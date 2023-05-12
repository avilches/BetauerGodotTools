using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public abstract class ProviderFactory {
}

public class ProviderFactory<T> : ProviderFactory, IFactory<T> where T : class {
    protected readonly IProvider Provider;

    public ProviderFactory(IProvider provider) {
        Provider = provider;
    }

    public T Get() => (T)Provider.Get();
}

public class ProviderSingletonFactory<T> : ProviderFactory<T>, ISingleton<T> where T : class {
    public ProviderSingletonFactory(IProvider provider) : base(provider) {
    }
}

public class ProviderTransientFactory<T> : ProviderFactory<T>, ITransientFactory<T> where T : class {
    public ProviderTransientFactory(IProvider provider) : base(provider) {
    }
}
