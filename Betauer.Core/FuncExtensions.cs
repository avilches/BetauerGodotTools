using System;

namespace Betauer.Core; 

public class MemoizedFunction<T> {
    private readonly Func<T> _factory;
    private T? _singleton;

    private MemoizedFunction(Func<T> factory) {
        _factory = factory;
    }

    private T Get() {
        return _singleton ??= _factory.Invoke();
    }

    internal static Func<T> Create(Func<T> customFactory) {
        return new MemoizedFunction<T>(customFactory).Get;
    }
}


public static class FuncExtensions {
    public static Func<T> Memoize<T>(this Func<T> function) {
        return MemoizedFunction<T>.Create(function);
    }
    
}