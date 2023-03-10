using System;

namespace Betauer.Core.Pool;

public abstract class BaseMiniPoolBusy<T> : BaseMiniPool<T> where T : class, IBusyElement {
    protected BaseMiniPoolBusy(int desiredSize) : base(desiredSize) {}

    protected override bool IsBusy(T element) => element.IsBusy();

    protected override bool IsInvalid(T element) => false;
}

public class MiniPoolBusy<T> : BaseMiniPoolBusy<T> where T : class, IBusyElement {
    private readonly Func<T> _factory;

    public MiniPoolBusy(Func<T> factory, int desiredSize = 4, bool lazy = true) : base(desiredSize) {
        _factory = factory;
        if (!lazy) Fill();
    }

    protected override T Create() => _factory.Invoke();
}