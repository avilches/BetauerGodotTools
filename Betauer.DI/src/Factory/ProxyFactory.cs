using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public abstract class ProxyFactory {
    protected readonly IProvider Provider;
    protected ProxyFactory(IProvider provider) {
        Provider = provider;
    }
    protected object GetFromProvider() => Provider.Get();
}


