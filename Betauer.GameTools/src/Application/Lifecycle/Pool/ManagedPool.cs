using System;
using System.Collections.Generic;
using Betauer.Core.Pool.Lifecycle;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;

namespace Betauer.Application.Lifecycle.Pool;

public abstract class ManagedPool<T> : BasePool<T>, IManagedPool, IInjectable where T : class {
    [Inject] protected Container Container { get; set; }
    private IPoolContainer _poolContainer;
    private IFactory<T>? _factory;
    private readonly string? _factoryName;
    private string? _poolContainerName;

    public int PurgeIfBiggerThan { get; set; } = 0;

    protected ManagedPool(int purgeIfBiggerThan = 0) {
        PurgeIfBiggerThan = purgeIfBiggerThan;
    }

    protected ManagedPool(IFactory<T> factory, int purgeIfBiggerThan = 0) {
        _factory = factory;
        PurgeIfBiggerThan = purgeIfBiggerThan;
    }

    protected ManagedPool(string? factoryName, int purgeIfBiggerThan = 0) {
        _factoryName = factoryName;
        PurgeIfBiggerThan = purgeIfBiggerThan;
    }

    public void PreInject(string poolContainerName) {
        _poolContainerName = poolContainerName;
    }

    public virtual void PostInject() {
        _factory ??= _factoryName != null
            ? Container.Resolve<IFactory<T>>(_factoryName)
            : Container.Resolve<IFactory<T>>();

        if (_poolContainerName != null) {
            SetPoolContainer(Container.Resolve<IPoolContainer>(_poolContainerName));
        }
    }

    IEnumerable<object> IManagedPool.Drain() {
        return Drain();
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
        var instance = _factory!.Get();
        return instance;
    }

    protected override bool MustBePurged(IReadOnlyList<T> pool) => pool.Count > PurgeIfBiggerThan;
}