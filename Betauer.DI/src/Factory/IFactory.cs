using System;

namespace Betauer.DI.Factory;

public interface IFactory<out T> where T : class {
    public T Get();
}

public interface ISingleton<out T> : IFactory<T> where T : class {
    
}

public interface ITransientFactory<out T> : IFactory<T> where T : class { }

public abstract class Factory<T> : IFactory<T> where T : class {
    private readonly Func<T> _func;

    protected Factory(Func<T> func) {
        _func = func;
    }

    public T Get() => _func.Invoke();
}

public class Singleton<T> : Factory<T>, ISingleton<T> where T : class {
    public Singleton(Func<T> func) : base(func) {
    }
}

public class TransientFactory<T> : Factory<T>, ITransientFactory<T> where T : class {
    public TransientFactory(Func<T> func) : base(func) {
    }
}
