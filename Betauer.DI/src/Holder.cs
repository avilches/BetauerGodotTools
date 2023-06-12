using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;

namespace Betauer.DI;

public class Holder<T> : IInjectable where T : class {
    [Inject] protected Container Container { get; set; }
    private ITransient<T>? _factory;
    private string? _factoryName;

    private T? _instance;

    public Holder(ITransient<T> factory) {
        _factory = factory;
    }

    public Holder(string factoryName) {
        _factoryName = factoryName;
    }

    public void PreInject(string factoryName) {
        _factoryName = factoryName;
    }

    public virtual void PostInject() {
        if (_factory == null) {
            if (_factoryName == null) {
                _factory = Container.Resolve<ITransient<T>>();
            } else {
                var provider = Container.GetProvider(_factoryName);
                if (provider.ProviderType.ImplementsInterface(typeof(ITransient<T>))) {
                    _factory = (ITransient<T>)provider.Get();
                } else {
                    _factory = Container.Resolve<ITransient<T>>($"{Container.Builder.FactoryPrefix}{_factoryName}");
                }
            }
        }
    }
    
    public T Get() {
        _instance ??= _factory!.Create();
        return _instance;
    }

    public void Reset() {
        _instance = null!;
    }
    
    public bool HasInstance() {
        return _instance != null;
    }
}