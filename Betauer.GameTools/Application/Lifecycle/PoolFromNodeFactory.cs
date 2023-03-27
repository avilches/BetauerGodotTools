using System.Collections.Generic;
using Betauer.Core.Pool.Lifecycle;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;

namespace Betauer.Application.Lifecycle;

public class PoolFromNodeFactory<T> : BasePoolLifecycle<T>, IInjectable 
    where T : Node, IPoolLifecycle {
    [Inject] private IFactory<T> Factory { get; set; }
    [Inject] private PoolContainer<IPoolLifecycle> PoolContainer { get; set; }

    private readonly int _purgeIfBiggerThan = 0;
    protected override bool MustBePurged(IReadOnlyList<T> pool) => pool.Count > _purgeIfBiggerThan;

    public PoolFromNodeFactory(int purgeIfBiggerThan = 0) {
        _purgeIfBiggerThan = purgeIfBiggerThan;
    }

    protected override T Create() {
        var instance = Factory.Get();
        return instance;
    }

    public void PostInject() {
        PoolContainer.Add(this);
    }
}