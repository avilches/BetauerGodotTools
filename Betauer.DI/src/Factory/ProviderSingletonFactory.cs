using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public class ProviderSingletonFactory<T> : ProviderFactory<T>, ISingletonFactory<T> where T : class {
    public ProviderSingletonFactory(IProvider provider) : base(provider) {
    }
}