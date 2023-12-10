using System;
using Betauer.DI.Factory;

namespace Betauer.DI.Holder;

public class Holder {
    public static IMutableHolder<T> From<T>(string factoryName) where T : class {
        return new TransientFactoryHolder<T>(factoryName);
    }

    public static IMutableHolder<T> From<T>(ITransient<T> factory) where T : class {
        return new FuncHolder<T>(factory.Create);
    }

    public static IMutableHolder<T> From<T>(Func<T> factory) where T : class {
        return new FuncHolder<T>(factory);
    }
    
    public static IHolder<T> Chain<TH, T>(string holderName, Func<TH, T> chain) where T : class where TH : class {
        return new HolderChain<TH, T>(holderName, chain);
    }
}

public abstract class Holder<T> : Holder, IMutableHolder<T> where T : class {
    private T? _instance;
    protected abstract T CreateValue();

    public T Get() {
        _instance ??= CreateValue();
        return _instance;
    }

    public void Clear() {
        _instance = null!;
    }
    
    public bool HasValue() {
        return _instance != null;
    }
}