using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public class ProxySingletonFactory<T> : ProxyFactory, ILazy<T> where T : class {
    public ProxySingletonFactory(IProvider provider) : base(provider) {
    }

    public virtual T Get() => (T)GetFromProvider();
}