using System;
using System.Collections.Generic;
using Betauer.Core.Pool.Lifecycle;

namespace Betauer.Application.Lifecycle.Pool;

public abstract class ManagedPool<T> : BasePool<T>, IManagedPool where T : class {
    private IPoolContainer _poolContainer;
    
    public Func<T>? Factory { get; set; }

    public int PurgeIfBiggerThan { get; set; } = 0;

    protected ManagedPool(int purgeIfBiggerThan = 0) {
        PurgeIfBiggerThan = purgeIfBiggerThan;
    }

    protected ManagedPool(Func<T> factory, int purgeIfBiggerThan = 0) {
        Factory = factory;
        PurgeIfBiggerThan = purgeIfBiggerThan;
    }

    IEnumerable<object> IManagedPool.GetAvailable() {
        return GetAvailable();
    }

    IEnumerable<object> IManagedPool.GetBusy() {
        return GetBusy();
    }

    public void SetPoolContainer(IPoolContainer poolContainer) {
        if (_poolContainer != null) throw new Exception("PoolContainer already set");
        _poolContainer = poolContainer;
        _poolContainer.Add(this);
    }

    protected override T Create() {
        var instance = Factory();
        return instance;
    }

    protected override bool MustBePurged(IReadOnlyList<T> pool) => pool.Count > PurgeIfBiggerThan;
}