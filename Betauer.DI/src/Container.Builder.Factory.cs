using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public partial class Container {
    public partial class Builder {

        public FactoryProviders RegisterSingletonFactory<T, TF>(TF factoryInstance, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) 
            where T : class
            where TF : class, IFactory<T> {
            return RegisterSingletonFactory(factoryInstance, name, lazy, metadata);
        }

        public FactoryProviders RegisterTransientFactory<T, TF>(TF factoryInstance, string? name = null, Dictionary<string, object>? metadata = null) 
            where T : class
            where TF : class, IFactory<T> {
            return RegisterTransientFactory(factoryInstance, name, metadata);
        }

        public FactoryProviders RegisterSingletonFactory(object factoryInstance, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) {
            var factoryType = factoryInstance.GetType();
            if (!factoryType.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidCastException($"Factory {factoryType.GetTypeName()} must implement IFactory<>");
            }
            return RegisterFactory(Lifetime.Singleton, factoryInstance.GetType(), factoryInstance, name, lazy, metadata);
        }

        public FactoryProviders RegisterTransientFactory(object factoryInstance, string? name = null, Dictionary<string, object>? metadata = null) {
            var factoryType = factoryInstance.GetType();
            if (!factoryType.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidCastException($"Factory {factoryType.GetTypeName()} must implement IFactory<>");
            }
            return RegisterFactory(Lifetime.Transient, factoryInstance.GetType(), factoryInstance, name, true /* ignored because Transient */, metadata);
        }

        /// <summary>
        /// Register a custom instance factory with a proxy factory, so users can access to the factory and to the services with:
        /// - Resolve<T>()
        /// - Resolve<ILazy<T>>() 
        /// - Resolve<ITransient<T>>() 
        /// And, if name is not null:
        /// - Resolve<T>(name)
        /// - Resolve<ILazy<T>>("Factory:"+name)  // returns the proxyFactory
        /// - Resolve<ITransient<T>>("Factory:"+name)  // returns the proxyFactory
        ///  
        /// </summary>
        /// <param name="factoryType">The factory type, must implements IFactory<T>, where <T> is the service to create</param>
        /// <param name="lifetime"></param>
        /// <param name="factoryInstance">The factory instance, must be IFactory<T> type</param>
        /// <param name="lazy"></param>
        /// <param name="name"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        private FactoryProviders RegisterFactory(Lifetime lifetime, Type /* IFactory<T> */ factoryType, object factoryInstance, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) {
            var type = factoryType.FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];

            // The factory is not really registered in the container. It's added only to the pending providers so it can be processed during the Build(),
            // so it can not be resolved from the container (it's deleted before add it to the container).
            // This is why it doesn't need a name or a type, it doesn't belong to the container registry at all.
            // The factory is not lazy and it will be resolved to ensure the factory is fully injected before the first Get()
            // You can use [Inject] inside the factories.
            AddToInitialInject(factoryInstance);

            // This is just () => ((IFactory<T>)factoryProvider).Create() but it's faster than using reflection to find the "Create()" method and invoke it
            var createMethod = FactoryWrapper.Create(type, factoryInstance).Create; 

            // Register the regular instance factory needed to use:
            // - Resolve<T>()
            // - Resolve<T>(name)
            // To create instances with the dependencies injected.
            // If the factory is a lazy singleton, it will not be registered
            IProvider provider = lifetime == Lifetime.Singleton
                ? new SingletonProvider(type, type, createMethod, name, lazy, metadata)
                : new TransientProvider(type, type, createMethod, name, metadata);

            Register(provider);

            IProvider? proxyProvider = null;
            // Non lazy singletons doesn't need the ILazy<> proxy factory
            if (lifetime == Lifetime.Transient || lazy) {
                // Register the proxy factory so users can get the factory and use it to create new instances with dependencies injected
                proxyProvider = ProxyFactoryProvider.Create(provider);
                Register(proxyProvider);
            }
            return new FactoryProviders(provider, proxyProvider);
        }

        public record FactoryProviders(IProvider Provider, IProvider? ProxyProvider);
    }
}