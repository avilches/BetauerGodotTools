using System;

namespace Betauer.Core.Pool;

public class Pool<T> : BasePool<T> {
    private readonly Func<T> _factory;
    
    public event Action<T>? OnGet; 
    public event Action<T>? OnRelease; 
    public event Action<T>? OnDestroy; 
    
    public Pool(Func<T>? factory = null, PoolCollection<T>? pool = null) : base(pool) {
        _factory = factory ?? Activator.CreateInstance<T>;
    }

    protected override T ExecuteCreate() {
        return _factory.Invoke();
    }

    protected override void ExecuteOnGet(T element) {
        OnGet?.Invoke(element);
    }

    protected override void ExecuteOnRelease(T element) {
        OnRelease?.Invoke(element);
    }

    protected override void ExecuteDestroy(T element) {
        OnDestroy?.Invoke(element);
    }
}