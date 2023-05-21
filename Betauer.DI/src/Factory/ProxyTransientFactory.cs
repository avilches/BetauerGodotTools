using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public class ProxyTransientFactory<T> : ProxyFactory, ITransient<T> where T : class {
    public ProxyTransientFactory(IProvider provider) : base(provider) {
    }

    public virtual T Create() => (T)GetFromProvider();
}