using System;
using Betauer.DI.Attributes;

namespace Betauer.DI.Holder;

public class HolderChain<TH, T> : IHolder<T>, IInjectable 
    where T : class 
    where TH : class {
    
    [Inject] protected Container Container { get; set; }
    
    private IMutableHolder<TH>? _holder;
    private readonly Func<TH, T> _chain;
    private string? _holderName;

    public HolderChain(IMutableHolder<TH>? holder, Func<TH, T> chain) {
        _holder = holder;
        _chain = chain;
    }

    public HolderChain(string holderName, Func<TH, T> chain) {
        _holderName = holderName;
        _chain = chain;
    }

    public HolderChain(Func<TH, T> chain) {
        _chain = chain;
    }

    public void PreInject(string factoryName) {
        _holderName = factoryName;
    }

    public virtual void PostInject() {
        if (_holder == null) {
            _holder = Container.Resolve<IMutableHolder<TH>>(_holderName!);
        }
    }

    public T Get() {
        return _chain(_holder!.Get());
    }
}