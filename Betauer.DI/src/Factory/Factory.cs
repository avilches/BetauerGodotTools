using System;

namespace Betauer.DI.Factory;

public class Factory<T> : IFactory<T> where T : class {
    private readonly Func<T> _func;

    public Factory(Func<T> func) {
        _func = func;
    }

    public T Create() => _func.Invoke();
}