using System;

namespace Betauer.DI.Factory;

public abstract class Factory<T> : IGet<T> where T : class {
    private readonly Func<T> _func;

    protected Factory(Func<T> func) {
        _func = func;
    }

    public T Get() => _func.Invoke();
}