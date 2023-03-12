using System;
using Betauer.Core.Pool;
using Betauer.DI;
using Betauer.DI.Factory;

namespace Veronenger;

public class PoolFactory<T> : BaseMiniPoolBusyInvalid<T>, IFactory<T>, IInjectable 
    where T : class, IObjectLifecycle {
    [Inject] private IFactory<T> Factory { get; set; }
    [Inject] private PoolManager PoolManager { get; set; }

    protected override T Create() {
        var instance = Factory.Get();
        instance.Initialize();
        return instance;
    }

    public void PostInject() {
        PoolManager.Add(this);
    }
}

[Service]
public class PoolManager {
    public event Action? OnRemoveFromScene;
    public void Add<T>(PoolFactory<T> poolFactory) where T : class, IObjectLifecycle {
        OnRemoveFromScene += () => poolFactory.Pool.ForEach(z => z.RemoveFromScene());
    }

    public void RemoveFromScene() {
        OnRemoveFromScene?.Invoke();
    }
}