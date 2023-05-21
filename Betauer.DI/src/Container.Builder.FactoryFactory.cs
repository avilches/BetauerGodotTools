using System;
using Betauer.Core;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public partial class Container {
    public partial class Builder {
        public static readonly string InnerFactoryPrefix = "InnerFactory:";

        /// <summary>
        /// Register a service based on a factory of the factory. It creates a ILazy<T> or ITransient<T> proxy to create the objects
        ///
        /// A proxy factory that create objects with dependencies injected 
        /// - Resolve<ILazy<T>>()                      // if singleton 
        /// - Resolve<ILazy<T>>("Factory:"+name)       // if singleton 
        /// - Resolve<ITransient<T>>()                 // if transient
        /// - Resolve<ITransient<T>>("Factory:"+name)  // if transient
        ///
        /// The inner factory that create objects without dependencies injected
        /// - Resolve<IInnerFactory<T>>()                       // returns the factory 
        /// - Resolve<IInnerFactory<T>>("InnerFactory:"+name)   // returns the factory 
        /// 
        /// </summary>
        /// <param name="lifetime"></param>
        /// <param name="factoryFactory">A function returning a IFactory<T> that creates the objects</param>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <returns></returns>
        public Builder RegisterFactoryFactory<T, TF>(Lifetime lifetime, Func<TF> factoryFactory, string? name = null) 
            where T : class
            where TF : class, IFactory<T> {
            return RegisterFactoryFactory(typeof(TF), lifetime, factoryFactory, name);
        }

        /// <summary>
        /// Register a service based on a factory of the factory.
        ///
        /// The service (it uses the proxy internally):
        /// - Resolve<T>()
        /// - Resolve<T>(name)
        ///
        /// A proxy factory that create objects with dependencies injected 
        /// - Resolve<TP>()                
        /// - Resolve<TP>("Factory:"+name) 
        /// 
        /// </summary>
        /// <param name="lifetime"></param>
        /// <param name="factoryFactory">A function returning a IFactory<T> that creates the objects</param>
        /// <param name="proxyFactory">A function returning the proxy, it can be any type</param>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TF"></typeparam>
        /// <returns></returns>
        public Builder RegisterFactoryFactory<T, TF, TP>(Lifetime lifetime, Func<TF> factoryFactory,
            Func<IProvider, TP> proxyFactory, string? name = null) 
            where T : class
            where TF : class, IFactory<T>
            where TP : class {
            return RegisterFactoryFactory(typeof(TF), lifetime, factoryFactory, typeof(TP), proxyFactory, name);
        }

        public Builder RegisterFactoryFactory(Type factoryType, Lifetime lifetime, Func<object> factoryFactory, string? name = null) {
            if (!factoryType.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidCastException($"Factory {factoryType.GetTypeName()} must implement IGet<T>");
            }
            var type = factoryType.FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];
            ProxyFactory FactoryFromProvider(IProvider provider) => FactoryTools.CreateProxyFactory(type, provider);
            var proxyFactoryType = (lifetime == Lifetime.Singleton ? typeof(ILazy<>) : typeof(ITransient<>)).MakeGenericType(type);
            return RegisterFactoryFactory(factoryType, lifetime, factoryFactory, proxyFactoryType, FactoryFromProvider, name);
        }

        public interface IInnerFactory<out T> : IFactory<T> where T : class { }

        /// <summary>
        /// Register a custom instance factory and a proxy factory, so users can access to the factory and to the services with:
        /// - Resolve<T>()
        /// - Resolve<IInnerFactory<T>>() 
        /// - Resolve<ProxyFactoryType<T>>()  // returns the proxyFactory
        /// And, if name is not null:
        /// - Resolve<T>(name)
        /// - Resolve<IInnerFactory<T>>("InnerFactory:"+name)
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
        public Builder RegisterFactoryFactory(Type factoryType, Lifetime lifetime, Func<object> factoryFactory, Type proxyFactoryType, Func<IProvider, object> proxyFactory, string? name = null) {
            if (!factoryType.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidCastException($"Factory {factoryType.GetTypeName()} must implement IGet<T>");
            }
            var type = factoryType.FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];

            // Creates and register the factory, which only can be accessed with
            // - Resolve("InnerFactory:"+name)
            // - Resolve<TheRealType>("InnerFactory:"+name)
            // This factory returns the instances without inject dependencies in them
            // The factory are always instantiated. The lazy parameter only affects to the instance created by the factory, never to the factory itself.
            // Register the factory allows to inject services IN THE FACTORY.
            var factoryName = name == null ? null : $"{InnerFactoryPrefix}{name}";
            var innerFactoryType = typeof(IInnerFactory<>).MakeGenericType(type);
            var factoryProvider = Provider.Create(innerFactoryType, innerFactoryType, Lifetime.Singleton, factoryFactory, factoryName, false);
            Register(factoryProvider);
    
            // Register the regular instance factory. It's always lazy, this guarantees the factory is fully injected before the first Get()
            // This is needed to use:
            // - Resolve<T>()
            // - Resolve<T>(name)
            // To create instances injecting dependencies
            Func<object> wrapperFactory = FactoryTools.GetGetFromWrapperFactory(type, factoryProvider); // This is just () => customProvider.Get().Get()
            var provider = Provider.Create(type, type, lifetime, wrapperFactory, name, true);
            Register(provider);

            // Register a proxy factory so users can access to the factory and use it to create new instances with dependencies injected
            // This factory could be ILazy<T> or ITransient<T> depending on the lifetime, or a custom proxy factory
            var proxyFactoryName = name == null ? null : $"{FactoryPrefix}{name}";
            object ProxyFactory() => proxyFactory(provider);
            Register(Provider.Create(proxyFactoryType, proxyFactoryType, Lifetime.Singleton, ProxyFactory, proxyFactoryName, true));
            return this;
        }
    }
}