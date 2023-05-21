using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.Core.Pool.Lifecycle;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;

namespace Betauer.Application.Lifecycle.Pool;

public abstract class ManagedPool<T> : BasePool<T>, IManagedPool, IInjectable where T : class {
    [Inject] protected Container Container { get; set; }
    private IPoolContainer _poolContainer;
    private ITransient<T>? _factory;
    private readonly string? _factoryName;
    private string? _poolContainerName;

    public int PurgeIfBiggerThan { get; set; } = 0;

    protected ManagedPool(int purgeIfBiggerThan = 0) {
        PurgeIfBiggerThan = purgeIfBiggerThan;
    }

    protected ManagedPool(ITransient<T> factory, int purgeIfBiggerThan = 0) {
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
        if (_poolContainerName != null) {
            SetPoolContainer(Container.Resolve<IPoolContainer>(_poolContainerName));
        }
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
        var instance = _factory!.Create();
        return instance;
    }

    protected override bool MustBePurged(IReadOnlyList<T> pool) => pool.Count > PurgeIfBiggerThan;
}