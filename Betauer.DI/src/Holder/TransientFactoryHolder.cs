using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Holder;

public class TransientFactoryHolder<T> : Holder<T>, IInjectable where T : class {
    [Inject] protected Container Container { get; set; }
    
    private ITransient<T>? _factory;
    private string? _factoryName;

    public TransientFactoryHolder(string factoryName) {
        _factoryName = factoryName;
    }

    public void PreInject(string factoryName) {
        _factoryName = factoryName;
    }

    public virtual void PostInject() {
        if (_factoryName == null) {
            _factory = Container.Resolve<ITransient<T>>();
        } else {
            var provider = Container.GetProvider(_factoryName);
            if (provider.InstanceType.ImplementsInterface(typeof(ITransient<T>))) {
                _factory = (ITransient<T>)provider.Get();
            } else {
                _factory = Container.Resolve<ITransient<T>>($"{ProxyFactoryProvider.FactoryPrefix}{_factoryName}");
            }
        }
    }

    protected override T CreateValue() {
        return _factory!.Create();
    }
}