using System;

namespace Betauer.DI.Factory;

public abstract class Factory<T> : IFactory<T> where T : class {
    private readonly Func<T> _func;

    protected Factory(Func<T> func) {
        _func = func;
    }

    public T Create() => _func.Invoke();
}