using System;
using Betauer.Core;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public abstract class FactoryWrapper {

    /// <summary>
    /// Cast at compile time a IFactory<T> from object to IFactory<object>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    public static FactoryWrapper Create(Type type, object factory) {
        if (!factory.GetType().ImplementsInterface(typeof(IFactory<>))) {
            throw new InvalidCastException($"Factory {factory.GetType().GetTypeName()} must implement IFactory<>");
        }
        var factoryType = typeof(Typed<>).MakeGenericType(type);
        return (FactoryWrapper)Activator.CreateInstance(factoryType, factory)!;
    }
    
    public abstract object Create();
    
    public class Typed<T> : FactoryWrapper, IFactory<T> where T : class {
        private readonly IFactory<T> _factoryProvider;

        public Typed(IFactory<T> factoryProvider) {
            _factoryProvider = factoryProvider;
        }

        public override T Create() {
            return _factoryProvider.Create();
        }
    }
}

