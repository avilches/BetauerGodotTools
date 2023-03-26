using System.Collections.Generic;
using Betauer.Core.Pool.Lifecycle;
using Betauer.DI;
using Betauer.DI.Factory;
using Betauer.NodePath;
using Godot;

namespace Betauer.Application.Lifecycle;

public class PoolFromNodeFactory<T> : BasePoolLifecycle<T>, IInjectable 
    where T : Node, INodePoolLifecycle {
    [Inject] private IFactory<T> Factory { get; set; }
    [Inject] private PoolContainer<INodePoolLifecycle> PoolContainer { get; set; }

    private readonly int _purgeIfBiggerThan = 0;
    protected override bool MustBePurged(IReadOnlyList<T> pool) => pool.Count > _purgeIfBiggerThan;

    public PoolFromNodeFactory(int purgeIfBiggerThan = 0) {
        _purgeIfBiggerThan = purgeIfBiggerThan;
    }

    protected override T Create() {
        var instance = Factory.Get();
        // The instance has all their [Inject] dependencies injected
        NodePathScanner.ScanAndInject(instance);
        instance.Initialize();
        return instance;
    }

    public void PostInject() {
        PoolContainer.Add(this);
    }
}