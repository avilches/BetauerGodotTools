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
    
    public static ILazy<T> Chain<TH, T>(string holderName, Func<TH, T> chain) where T : class where TH : class {
        return new HolderChain<TH, T>(holderName, chain);
    }

}

public abstract class Holder<T> : Holder, IMutableHolder<T> where T : class {

    protected T? Instance;
    protected abstract T CreateValue();

    public T Get() {
        Instance ??= CreateValue();
        return Instance;
    }

    public void Clear() {
        Instance = null!;
    }
    
    public bool HasValue() {
        return Instance != null;
    }
}