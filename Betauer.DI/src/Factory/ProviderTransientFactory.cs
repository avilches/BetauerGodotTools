using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public class ProviderTransientFactory<T> : ProviderFactory<T>, IFactory<T> where T : class {
    public ProviderTransientFactory(IProvider provider) : base(provider) {
    }
}