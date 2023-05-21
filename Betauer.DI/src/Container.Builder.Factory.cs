using System;
using Betauer.Core;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public partial class Container {
    public partial class Builder {
        public interface IInnerFactory<out T> : IFactory<T> where T : class { }
        public static readonly string InnerFactoryPrefix = "InnerFactory:";
        public static readonly string FactoryPrefix = "Factory:";

        /// <summary>
        /// Register a factory from a provider. It creates a ILazy<T> or ITransient<T> proxy to create the objects.
        /// So, having a provider that works with Resolve<T>(name), it creates a proxy that works
        /// with Resolve<ITransient<T>>("Factory:"+name) or Resolve<ILazy<T>>("Factory:"+name)
        /// </summary>
        /// <returns></returns>
        public Builder RegisterFactory(IProvider provider) {
            var proxyFactoryType = (provider.Lifetime == Lifetime.Singleton ? typeof(ILazy<>) : typeof(ITransient<>)).MakeGenericType(provider.ProviderType);
            var proxyFactoryName = provider.Name == null ? null : $"{FactoryPrefix}{provider.Name}";
            object ProxyFactory() => CreateProxyFactory(provider.ProviderType, provider);
            var factoryProvider = Provider.Create(proxyFactoryType, proxyFactoryType, Lifetime.Singleton, ProxyFactory, proxyFactoryName, true);
            return Register(factoryProvider);
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
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <returns></returns>
        public Builder RegisterFactory<T, TF>(Lifetime lifetime, Func<TF> factoryFactory, string? name = null) 
            where T : class
            where TF : class, IFactory<T> {
            return RegisterFactory(typeof(TF), lifetime, factoryFactory, name);
        }

        // Dynamic version, without generics
        public Builder RegisterFactory(Type factoryType, Lifetime lifetime, Func<object> factoryFactory, string? name = null) {
            if (!factoryType.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidCastException($"Factory {factoryType.GetTypeName()} must implement IFactory<T>");
            }
            var type = factoryType.FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];
            ProxyFactory ProxyFactory(IProvider provider) => CreateProxyFactory(type, provider);
            var proxyFactoryType = (lifetime == Lifetime.Singleton ? typeof(ILazy<>) : typeof(ITransient<>)).MakeGenericType(type);
            return RegisterFactory(factoryType, lifetime, factoryFactory, proxyFactoryType, ProxyFactory, name);
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
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <typeparam name="TP"></typeparam>
        /// <returns></returns>
        public Builder RegisterFactory<T, TF, TP>(Lifetime lifetime, Func<TF> factoryFactory, Func<IProvider, TP> proxyFactory, string? name = null) 
            where T : class
            where TF : class, IFactory<T>
            where TP : class {
            return RegisterFactory(typeof(TF), lifetime, factoryFactory, typeof(TP), proxyFactory, name);
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
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="Exception"></exception>
        public Builder RegisterFactory(Type factoryType, Lifetime lifetime, Func<object> factoryFactory, Type proxyFactoryType, Func<IProvider, object> proxyFactory, string? name = null) {
            if (!factoryType.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidCastException($"Factory {factoryType.GetTypeName()} must implement IGet<T>");
            }
            var type = factoryType.FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];

            // Register the factory in the container, but it can't be resolved (it uses a random name and a private class as identifier)
            // The factory is not lazy. The lazy parameter only affects to the instance created by the factory, never to the factory itself.
            // Register the factory as a service allows to the developers to [Inject] services in the factory.
            var factoryName = name == null ? null : $"{InnerFactoryPrefix}{name}";
            var innerFactoryType = typeof(IInnerFactory<>).MakeGenericType(type);
            var factoryProvider = Provider.Create(innerFactoryType, innerFactoryType, Lifetime.Singleton, factoryFactory, factoryName, false);
            Register(factoryProvider);
    
            // Register the regular instance factory. It's always lazy, this guarantees the factory is fully injected before the first Get()
            // This is needed to use:
            // - Resolve<T>()
            // - Resolve<T>(name)
            // To create instances injecting dependencies
            var providerFactoryWrapper = ProviderFactoryWrapper.CreateFromProvider(type, factoryProvider);
            Func<object> factory = providerFactoryWrapper.Create; // This is just () => factoryProvider.Get().Create()
            var provider = Provider.Create(type, type, lifetime, factory, name, true);
            Register(provider);

            // Register the proxy factory so users can get the factory and use it to create new instances with dependencies injected
            var proxyFactoryName = name == null ? null : $"{FactoryPrefix}{name}";
            object ProxyFactory() => proxyFactory(provider);
            var proxyProvider = Provider.Create(proxyFactoryType, proxyFactoryType, Lifetime.Singleton, ProxyFactory, proxyFactoryName, true);
            Register(proxyProvider);
            
            return this;
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
    }
}