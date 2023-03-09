using System;

namespace Betauer.Core.Pool;

public class MiniPoolBusyInvalid<T> : BaseMiniPool<T> where T : class, IBusyInvalidElement {
    private readonly Func<T> _factory;

    public MiniPoolBusyInvalid(Func<T> factory, int desiredSize = 4, bool lazy = true) : base(desiredSize) {
        _factory = factory;
        if (!lazy) Fill();
    }

    protected override T Create() => _factory.Invoke();

    protected override bool IsBusy(T element) => element.IsBusy();

    protected override bool IsInvalid(T element) => element.IsInvalid();
}