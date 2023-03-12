using System;

namespace Betauer.Core.Pool.Basic;

public class BasicPool<T> : BaseBasicPool<T> {
    private readonly Func<T> _factory;
    public event Action<T>? OnGetEvent;
    public event Action<T>? OnReturnEvent;
    
    public BasicPool(Func<T> factory, IPoolCollection<T>? pool = null) : base(pool) {
        _factory += factory;
    }

    public override T Create() {
        return _factory.Invoke();
    }

    public override void OnGet(T element) {
        OnGetEvent?.Invoke(element);
    }

    public override void OnReturn(T element) {
        OnReturnEvent?.Invoke(element);
    }
}