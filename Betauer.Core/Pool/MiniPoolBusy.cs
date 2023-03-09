using System;

namespace Betauer.Core.Pool;

public class MiniPoolBusy<T> : BaseMiniPool<T> where T : class, IBusyElement {
    private readonly Func<T> _factory;

    public MiniPoolBusy(Func<T> factory, int desiredSize = 4, bool lazy = true) : base(desiredSize) {
        _factory = factory;
        if (!lazy) Fill();
    }

    protected override T Create() => _factory.Invoke();

    protected override bool IsBusy(T element) => element.IsBusy();

    protected override bool IsInvalid(T element) => false;
}