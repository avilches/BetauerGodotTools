using System;

namespace Betauer.DI.Holder;

public class FuncHolder<T> : Holder<T> where T : class {
    private readonly Func<T> _factory;

    public FuncHolder(Func<T> factory) {
        _factory = factory;
    }

    protected override T CreateInstance() {
        return _factory();
    }
}