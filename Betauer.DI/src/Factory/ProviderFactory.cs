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