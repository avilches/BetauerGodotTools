using System;

namespace Betauer.Core.Pool;

public abstract class BaseMiniPoolBusyInvalid<T> : BaseMiniPool<T> where T : class, IPoolElement {
    protected BaseMiniPoolBusyInvalid(int desiredSize = 4) : base(desiredSize) {}

    protected override bool IsBusy(T element) => element.IsBusy();

    protected override bool IsInvalid(T element) => element.IsInvalid();

    public override void OnGet(T element) => element.OnGet();
}

public class MiniPoolBusyInvalid<T> : BaseMiniPoolBusyInvalid<T> where T : class, IPoolElement {
    private readonly Func<T> _factory;

    public MiniPoolBusyInvalid(Func<T> factory, int desiredSize = 4, bool lazy = true) : base(desiredSize) {
        _factory = factory;
        if (!lazy) Fill();
    }

    protected override T Create() => _factory.Invoke();
}