using System;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Factory;

public abstract class ProviderFactory {
    public static ProviderFactory Create(Type type, IProvider provider) {
        var factoryType = typeof(ProviderFactoryImpl<>).MakeGenericType(type);
        ProviderFactory instance = (ProviderFactory)Activator.CreateInstance(factoryType, provider)!;
        return instance;
    }

    public class ProviderFactoryImpl<T> : ProviderFactory, IFactory<T> {
        protected readonly IProvider Provider;

        public ProviderFactoryImpl(IProvider provider) {
            Provider = provider;
        }

        public T Get() => (T)Provider.Get();
    }
}