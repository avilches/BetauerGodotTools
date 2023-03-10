using System;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public abstract class ProviderCustomFactory {
    public static ProviderCustomFactory Create(Type type, IProvider provider) {
        var factoryType = typeof(ProviderCustomFactoryImpl<>).MakeGenericType(type);
        ProviderCustomFactory instance = (ProviderCustomFactory)Activator.CreateInstance(factoryType, provider)!;
        return instance;
    }

    public abstract object Get();

    public class ProviderCustomFactoryImpl<T> : ProviderCustomFactory {
        private readonly IProvider _customFactory;
        
        public ProviderCustomFactoryImpl(IProvider customFactory) {
            _customFactory = customFactory;
        }
        
        public override object Get() {
            IFactory<T> factory = (IFactory<T>)_customFactory.Get();
            return factory.Get();
        }
    }
}