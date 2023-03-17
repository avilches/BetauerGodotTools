using System;
using System.Collections.Generic;
using Betauer.Core.Pool;
using Betauer.Core.Pool.Lifecycle;
using Betauer.DI;
using Betauer.DI.Factory;

namespace Betauer.Application.Lifecycle;

public class PoolFromNodeFactory<T> : BasePoolLifecycle<T>, IPoolFromFactory, IFactory<T>, IInjectable 
    where T : class, INodeLifecycle {
    [Inject] private IFactory<T> Factory { get; set; }
    [Inject] private PoolManager<INodeLifecycle> PoolManager { get; set; }

    private readonly int _purgeIfBiggerThan = 0;
    protected override bool MustBePurged(IReadOnlyList<T> pool) => pool.Count > _purgeIfBiggerThan;

    public PoolFromNodeFactory(int purgeIfBiggerThan = 0) {
        _purgeIfBiggerThan = purgeIfBiggerThan;
    }

    protected override T Create() {
        var instance = Factory.Get();
        instance.Initialize();
        return instance;
    }

    public void PostInject() {
        PoolManager.Add(this);
    }

    T IFactory<T>.Get() {
        return Get();
    }

    INodeLifecycle IPool<INodeLifecycle>.Get() {
        return Get();
    }

    IEnumerable<INodeLifecycle> IPool<INodeLifecycle>.GetAll() {
        return GetAll();
    }
}