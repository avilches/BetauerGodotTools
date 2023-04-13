using System;

namespace Betauer.Core.Pool.Basic;

public class BasicPool<T> : BaseBasicPool<T> {
    private readonly Func<T> _factory;
    public Func<T, T>? GetEvent;
    public Func<T, T>? ReturnEvent;
    
    public BasicPool(Func<T> factory, PoolCollection<T>? pool = null) : base(pool) {
        _factory = factory;
    }

    public override T Create() {
        return _factory.Invoke();
    }

    public override T OnGet(T element) {
        return GetEvent != null ? GetEvent.Invoke(element) : element;
    }

    public override T OnReturn(T element) {
        return ReturnEvent != null ? ReturnEvent.Invoke(element) : element;
    }
}