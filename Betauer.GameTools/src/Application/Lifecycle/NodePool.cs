using System.Collections.Generic;
using Betauer.Core.Pool.Lifecycle;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Lifecycle;

public class NodePool<T> : BasePoolLifecycle<T>, IInjectable 
    where T : Node, IPoolLifecycle {

    [Inject] private Container Container { get; set; }
    private PoolContainer<IPoolLifecycle> _poolContainer;
    private IFactory<T>? _factory;
    private string? _factoryName;
    private string? _poolContainerName;

    public int PurgeIfBiggerThan { get; set; } = 0;

    public NodePool(int purgeIfBiggerThan = 0) {
        PurgeIfBiggerThan = purgeIfBiggerThan;
    }

    public NodePool(IFactory<T> factory, int purgeIfBiggerThan = 0) {
        _factory = factory;
        PurgeIfBiggerThan = purgeIfBiggerThan;
    }

    public NodePool(string? factoryName, int purgeIfBiggerThan = 0) {
        _factoryName = factoryName;
        PurgeIfBiggerThan = purgeIfBiggerThan;
    }

    public void PreInject(string poolContainerName, string? factoryName = null) {
        _poolContainerName = poolContainerName;
        _factoryName ??= factoryName;
    }

    public virtual void PostInject() {
        _factory ??= _factoryName != null
            ? Container.Resolve<IFactory<T>>(_factoryName)
            : Container.Resolve<IFactory<T>>();

        if (_poolContainerName != null) {
            SetPoolContainer(Container.Resolve<PoolContainer<IPoolLifecycle>>(_poolContainerName));
        }
    }

    public void SetPoolContainer(PoolContainer<IPoolLifecycle> poolContainer) {
        _poolContainer = poolContainer;
        _poolContainer.Add(this);
    }

    protected override T Create() {
        var instance = _factory!.Get();
        return instance;
    }
    
    protected override bool MustBePurged(IReadOnlyList<T> pool) => pool.Count > PurgeIfBiggerThan;

}