using System;

namespace Betauer.DI.Factory;

public interface IFactory<out T> where T : class {
    public T Get();
}

public class Factory<T> : IFactory<T> where T : class {
    private readonly Func<T> _func;

    public Factory(Func<T> func) {
        _func = func;
    }

    public T Get() => _func.Invoke();
}