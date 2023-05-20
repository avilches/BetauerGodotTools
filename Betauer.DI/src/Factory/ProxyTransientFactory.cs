using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public class ProxyTransientFactory<T> : ProxyFactory, IFactory<T> where T : class {
    public ProxyTransientFactory(IProvider provider) : base(provider) {
    }

    public virtual T Get() => (T)GetFromProvider();
}