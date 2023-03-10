using System;

namespace Betauer.DI.Factory;

public abstract class FuncFactory {
    public static FuncFactory Create(Type genericType, Func<object> factory) {
        var type = typeof(FuncFactoryImpl<>).MakeGenericType(genericType);
        FuncFactory instance = (FuncFactory)Activator.CreateInstance(type, factory)!;
        return instance;
    }
            
    public class FuncFactoryImpl<T> : FuncFactory, IFactory<T> {
        protected readonly Func<object> FactoryFunc;

        public FuncFactoryImpl(Func<object> factoryFunc) {
            FactoryFunc = factoryFunc;
        }

        public T Get() {
            return (T)FactoryFunc.Invoke();
        }
    }
}