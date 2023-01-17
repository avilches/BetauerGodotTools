using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public class Factory<T> {
    private readonly Container _container;
    public IProvider Provider { get; }

    public T Get() {
        return (T)Provider.Get(_container);
    }

    private Factory(Container container, IProvider provider) {
        _container = container;
        Provider = provider;
    }
}