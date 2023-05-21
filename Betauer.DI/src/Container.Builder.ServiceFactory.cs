using System;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public partial class Container {
    public partial class Builder {
        public static readonly string FactoryPrefix = "Factory:";

        /// <summary>
        /// Register a service and a factory to create them, so it can be accessed with
        ///
        /// The service:
        /// - Resolve<TR>()
        /// - Resolve<TR>(name)
        ///
        /// The factory:
        /// - Resolve<ILazy<TR>>()                      // if singleton 
        /// - Resolve<ILazy<TR>>("Factory:"+name)       // if singleton 
        /// - Resolve<ITransient<TR>>()                 // if transient
        /// - Resolve<ITransient<TR>>("Factory:"+name)  // if transient
        /// 
        /// </summary>
        /// <param name="lifetime"></param>
        /// <param name="factoryFactory">A function that creates the T objects</param>
        /// <param name="name"></param>
        /// <param name="lazy"></param>
        /// <typeparam name="TR"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Builder RegisterServiceFactory<TR, T>(Lifetime lifetime, Func<T> factory, string? name = null, bool lazy = false)
            where T : class
            where TR : class {
            return RegisterServiceFactory(typeof(TR), typeof(T), lifetime, factory, name, lazy);
        }

        public Builder RegisterServiceFactory<TR, T, TP>(Lifetime lifetime, Func<T> factory, Func<IProvider, TP> proxyFactory, string? name = null, bool lazy = false)
            where T : class
            where TR : class
            where TP : class {
            return RegisterServiceFactory(typeof(TR), typeof(T), lifetime, factory, typeof(TP), proxyFactory, name, lazy);
        }

        /// <summary>
        /// Register a regular instance factory so users can access to the factory and to the services with:
        ///
        /// Transients:
        /// - Resolve<T>()
        /// - Resolve<ITransient<T>>()
        /// And, if name is not null:
        /// - Resolve<T>(name)
        /// - Resolve<ITransient<T>>("Factory:"+name)
        ///  
        /// Singleton:
        /// - Resolve<T>()
        /// - Resolve<ILazy<T>>()
        /// And, if name is not null
        /// - Resolve<T>(name)
        /// - Resolve<ILazy<T>>("Factory:"+name) 
        ///  
        /// </summary>
        /// 
        /// <param name="registeredType"></param>
        /// <param name="providerType"></param>
        /// <param name="lifetime"></param>
        /// <param name="factory"></param>
        /// <param name="name"></param>
        /// <param name="lazy"></param>
        /// <returns></returns>
        public Builder RegisterServiceFactory(Type registeredType, Type providerType, Lifetime lifetime, Func<object> factory, string? name = null, bool lazy = false) {
            // Create a proxy with ITransient<T> or ILazy<T> so the user can get the factory using
            // Resolve<ITransient<T>>()
            // Resolve<ITransient<T>>("Factory:"+name)
            // Resolve<ILazy<T>>()
            // Resolve<ILazy<T>>("Factory:"+name)
            ProxyFactory ProxyFactory(IProvider provider) => FactoryTools.CreateProxyFactory(providerType, provider);
            Type proxyFactoryType = (lifetime == Lifetime.Singleton ? typeof(ILazy<>) : typeof(ITransient<>)).MakeGenericType(providerType);
            return RegisterServiceFactory(registeredType, providerType, lifetime, factory, proxyFactoryType, ProxyFactory, name, lazy);
        }

        public Builder RegisterServiceFactory(Type registeredType, Type providerType, Lifetime lifetime, Func<object> factory, Type proxyFactoryType, Func<IProvider, object> proxyFactory, string? name = null, bool lazy = false) {
            // Register the factory so it can be used with Resolve<T>() or Resolve<T>(name)
            var providerFactory = Provider.Create(registeredType, providerType, lifetime, factory, name, lazy);
            Register(providerFactory);

            var proxyFactoryName = providerFactory.Name == null ? null : $"{FactoryPrefix}{name}";
            object ProxyFactory() => proxyFactory(providerFactory);
            Register(Provider.Create(proxyFactoryType, proxyFactoryType, Lifetime.Singleton, ProxyFactory, proxyFactoryName, true));
            return this;
        }
    }
}