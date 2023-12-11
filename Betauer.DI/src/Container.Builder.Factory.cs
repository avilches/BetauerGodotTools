using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public partial class Container {
    public partial class Builder {
        public static readonly string FactoryPrefix = "Factory:";

        /// <summary>
        /// Register a factory from a provider. It creates a ILazy<T> or ITransient<T> proxy to create the objects.
        /// So, having a provider that works with Resolve<T>(name), it creates a proxy that works
        /// with Resolve<ITransient<T>>("Factory:"+name) or Resolve<ILazy<T>>("Factory:"+name)
        /// </summary>
        /// <returns></returns>
        public FactoryProviders RegisterFactory(IProvider provider) {
            var proxyFactoryType = (provider.Lifetime == Lifetime.Singleton ? typeof(ILazy<>) : typeof(ITransient<>)).MakeGenericType(provider.ProviderType);
            var proxyFactoryName = provider.Name == null ? null : $"{FactoryPrefix}{provider.Name}";
            object ProxyFactory() => CreateProxyFactory(provider.ProviderType, provider);
            var factoryProvider = new SingletonProvider(proxyFactoryType, proxyFactoryType, ProxyFactory, proxyFactoryName, true);
            Register(factoryProvider);
            return new FactoryProviders(provider, factoryProvider);
        }

        /// <summary>
        /// Register a service based on a factory of the factory. So, instead of having a Func<T> like () => new T(), it uses
        /// a Func<IFactory<T>> like () => new YourFactory<T>() where YourFactory<T> must implements IFactory<T> interface and the method T Create() to returns T
        /// It register the <T> service so it can be accessed with Resolve<T>() and Resolve<T>(name), and it creates a ILazy<T> or ITransient<T> proxy to create the objects
        ///
        /// You will get the the service with:
        /// - Resolve<T>()
        /// - Resolve<T>(name)
        /// 
        /// You can also get a factory to build the services (a proxy that allows to create objects with dependencies injected): 
        /// - Resolve<ILazy<T>>()                      // singleton: you have to use Get() to get the singleton service 
        /// - Resolve<ILazy<T>>("Factory:"+name)       // singleton: you have to use Get() to get the singleton service 
        /// - Resolve<ITransient<T>>()                 // transient: you have to use Create() to build transient services
        /// - Resolve<ITransient<T>>("Factory:"+name)  // transient: you have to use Create() to build transient services
        /// 
        /// </summary>
        /// <param name="lifetime">The lifetime of the objects creates by your own factory</param>
        /// <param name="factoryFactory">A function returning a IFactory<T> that creates the objects</param>
        /// <param name="name"></param>
        /// <param name="metadata"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <returns></returns>
        public CustomFactoryProviders RegisterFactory<T, TF>(Lifetime lifetime, TF factoryFactory, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) 
            where T : class
            where TF : class, IFactory<T> {
            return RegisterFactory(typeof(TF), lifetime, factoryFactory, name, lazy, metadata);
        }

        // Dynamic version, without generics
        public CustomFactoryProviders RegisterFactory(Type factoryType, Lifetime lifetime, object factoryFactory, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) {
            if (!factoryType.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidCastException($"Factory {factoryType.GetTypeName()} must implement IFactory<T>");
            }
            if (!factoryFactory.GetType().ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidCastException($"Factory {factoryFactory.GetType().GetTypeName()} must implement IFactory<T>");
            }
            var type = factoryType.FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];
            ProxyFactory ProxyFactory(IProvider provider) => CreateProxyFactory(type, provider);
            var proxyFactoryType = (lifetime == Lifetime.Singleton ? typeof(ILazy<>) : typeof(ITransient<>)).MakeGenericType(type);
            return RegisterFactory(factoryType, lifetime, factoryFactory, proxyFactoryType, ProxyFactory, name, lazy, metadata);
        }

        /// <summary>
        /// Register a service based on a factory of the factory. So, instead of having a Func<T> like () => new T(), it uses
        /// a Func<IFactory<T>> like () => new YourFactory<T>() where YourFactory<T> must implements IFactory<T> interface and the method T Create() to returns T
        /// It register the <T> service so it can be accessed with Resolve<T>() and Resolve<T>(name), and it creates your own proxy factory.
        ///
        /// You will get the the service with:
        /// - Resolve<T>()
        /// - Resolve<T>(name)
        ///
        /// A proxy factory that create objects with dependencies injected 
        /// - Resolve<TP>()                
        /// - Resolve<TP>("Factory:"+name)
        ///
        /// It's your responsibility to create the proxy factory, it can be any type and it should use the IProvider to create the objects. 
        /// 
        /// </summary>
        /// <param name="lifetime">The lifetime of the objects creates by your own factory</param>
        /// <param name="factoryFactory">A function returning a IFactory<T> that creates the objects</param>
        /// <param name="proxyFactory">A function returning the proxy, it can be any type</param>
        /// <param name="name"></param>
        /// <param name="metadata"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <typeparam name="TP"></typeparam>
        /// <returns></returns>
        public CustomFactoryProviders RegisterFactory<T, TF, TP>(Lifetime lifetime, TF factoryFactory, Func<IProvider, TP> proxyFactory, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) 
            where T : class
            where TF : class, IFactory<T>
            where TP : class {
            return RegisterFactory(typeof(TF), lifetime, factoryFactory, typeof(TP), proxyFactory, name, lazy, metadata);
        }

        /// <summary>
        /// Register a custom instance factory and a proxy factory, so users can access to the factory and to the services with:
        /// - Resolve<T>()
        /// - Resolve<IInnerFactory<T>>() 
        /// - Resolve<ProxyFactoryType<T>>()  // returns the proxyFactory
        /// And, if name is not null:
        /// - Resolve<T>(name)
        /// - Resolve<ProxyFactoryType<T>>("Factory:"+name)  // returns the proxyFactory
        ///  
        /// </summary>
        /// <param name="factoryType">The factory type, must implements IGet<T>, where <T> is the service to create</param>
        /// <param name="lifetime"></param>
        /// <param name="factoryFactory">A function returning the factoryType, like Func<IGet<T>></param>
        /// <param name="proxyFactoryType"></param>
        /// <param name="proxyFactory"></param>
        /// <param name="lazy"></param>
        /// <param name="name"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="Exception"></exception>
        public CustomFactoryProviders RegisterFactory(Type factoryType, Lifetime lifetime, object factoryFactory, Type proxyFactoryType, Func<IProvider, object> proxyFactory, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) {
            if (!factoryType.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidCastException($"Factory {factoryType.GetTypeName()} must implement IGet<T>");
            }
            if (!factoryFactory.GetType().ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidCastException($"Factory {factoryFactory.GetType().GetTypeName()} must implement IFactory<T>");
            }
            var type = factoryType.FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];

            // The factory is not really registered in the container. It's added only to the pending providers so it can be processed during the Build(),
            // so it can not be resolved from the container (it's deleted before add it to the container).
            // This is why it doesn't need a name or a type, it doesn't belong to the container registry at all.
            // The factory is not lazy and it will be resolved to ensure the factory is fully injected before the first Get()
            // You can use [Inject] inside the factories.
            var factoryProvider = new FactoryProvider(factoryFactory, lifetime, lazy);
            Register(factoryProvider);
    
            // Register the regular instance factory needed to use:
            // - Resolve<T>()
            // - Resolve<T>(name)
            // To create instances with the dependencies injected.
            var providerFactoryWrapper = ProviderFactoryWrapper.CreateFromProvider(type, factoryProvider);
            Func<object> factory = providerFactoryWrapper.Create; // This is just () => factoryProvider.Get().Create()
            IProvider provider = lifetime == Lifetime.Singleton
                ? new SingletonProvider(type, type, factory, name, lazy, metadata)
                : new TransientProvider(type, type, factory, name, metadata);
            Register(provider);

            // Register the proxy factory so users can get the factory and use it to create new instances with dependencies injected
            var proxyFactoryName = name == null ? null : $"{FactoryPrefix}{name}";
            object ProxyFactory() => proxyFactory(provider);
            var proxyProvider = new SingletonProvider(proxyFactoryType, proxyFactoryType, ProxyFactory, proxyFactoryName, true);
            Register(proxyProvider);
            
            return new CustomFactoryProviders(factoryProvider, provider, proxyProvider);
        }

        /// <summary>
        /// Returns a class that wraps a provider (implementing ITransient<T> or ILazy<T>) so it can be exposed to the users 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        private static ProxyFactory CreateProxyFactory(Type type, IProvider provider) {
            if (provider.Lifetime == Lifetime.Singleton) {
                var factoryType = typeof(ProxySingletonFactory<>).MakeGenericType(type);
                ProxyFactory instance = (ProxyFactory)Activator.CreateInstance(factoryType, provider)!;
                return instance;
            } else {
                var factoryType = typeof(ProxyTransientFactory<>).MakeGenericType(type);
                ProxyFactory instance = (ProxyFactory)Activator.CreateInstance(factoryType, provider)!;
                return instance;
            }
        }

        public record CustomFactoryProviders(IProvider CustomFactoryProvider, IProvider Provider, IProvider ProxyProvider);
        public record FactoryProviders(IProvider Provider, IProvider ProxyProvider);
    }
}