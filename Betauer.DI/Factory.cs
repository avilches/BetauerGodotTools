using System;

namespace Betauer.DI;

public class Factory {
    protected readonly Func<object> FactoryFunc;

    public Factory(Func<object> factoryFunc) {
        FactoryFunc = factoryFunc;
    }

    public static Factory Create(Type genericType, Func<object> factory) {
        var type = typeof(Factory<>).MakeGenericType(genericType);
        Factory instance = (Factory)Activator.CreateInstance(type, factory)!;
        return instance;
    }
}

public class Factory<T> : Factory, IFactory<T> {
    public Factory(Func<object> factoryFunc) : base(factoryFunc) {
    }

    public T Get() {
        return (T)FactoryFunc.Invoke();
    }
}

