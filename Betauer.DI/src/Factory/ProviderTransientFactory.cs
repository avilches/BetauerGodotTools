using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public class ProviderTransientFactory<T> : ProviderFactory<T>, ITransientFactory<T> where T : class {
    public ProviderTransientFactory(IProvider provider) : base(provider) {
    }
}