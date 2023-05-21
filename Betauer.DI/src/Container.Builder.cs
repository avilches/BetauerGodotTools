using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Core;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public partial class Container {
    public class Builder {
        private readonly List<IProvider> _pendingToBuild = new();
        private readonly Container _container;
        private readonly Scanner _scanner;
    
        public Builder(Container container) {
            _container = container;
            _scanner = new Scanner(this, _container);
        }
    
        public Builder() {
            _container = new Container();
            _scanner = new Scanner(this, _container);
        }
    
        public Container Build() {
            var toBuild = new List<IProvider>(_pendingToBuild);
            _pendingToBuild.Clear();
            _container.Build(toBuild);
            return _container;
        }
    
        public Builder Register(IProvider provider) {
            _pendingToBuild.Add(provider);
            return this;
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
        public Builder RegisterServiceAndAddFactory(Type registeredType, Type providerType, Lifetime lifetime, Func<object> factory, string? name = null, bool lazy = false) {
            var providerFactory = Provider.Create(registeredType, providerType, lifetime, factory, name, lazy);
            Register(providerFactory);
            return RegisterFactoryFromProvider(providerFactory);
        }
        
        /// <summary>
        /// Using a IProvider as a service factory, register this services:
        /// 
        /// Transients:
        /// - Resolve<ITransient<T>>()
        /// And, if name is not null
        /// - Resolve<ITransient<T>>("Factory:"+name)
        ///  
        /// Singleton:
        /// - Resolve<ILazy<T>>()
        /// And, if name is not null
        /// - Resolve<ILazy<T>>("Factory:"+name)
        /// 
        /// </summary>
        /// <param name="providerFactory"></param>
        /// <returns></returns>
        private Builder RegisterFactoryFromProvider(IProvider providerFactory) {
            // Register the factory as ITransient<T> or ILazy<T> so the user can get the factory using Resolve<ITransient<T>>() or
            // Resolve<ILazy<T>>("Factory:"+name) or and then call to Get() which returns new instances, injecting dependencies.
            Type iFactoryType = (providerFactory.Lifetime == Lifetime.Singleton ? typeof(ILazy<>) : typeof(ITransient<>)).MakeGenericType(providerFactory.ProviderType);
            var factoryName = providerFactory.Name == null ? null : $"{FactoryPrefix}{providerFactory.Name}";
            object CustomFactory() => FactoryTools.CreateProxyFactory(providerFactory.ProviderType, providerFactory);
            Register(Provider.Create(iFactoryType, iFactoryType, Lifetime.Singleton, CustomFactory, factoryName, true));
            return this;
        }

        public static string InnerFactoryPrefix = "InnerFactory:";
        public static string FactoryPrefix = "Factory:";

        /// <summary>
        /// Register a custom instance factory so users can access to the factory and to the services with:
        ///
        /// Transients:
        /// - Resolve<T>()
        /// - Resolve<IInnerFactory<T>>()
        /// - Resolve<ITransient<T>>()
        /// And, if name is not null:
        /// - Resolve<T>(name)
        /// - Resolve<IInnerFactory<T>>("InnerFactory:"+name)
        /// - Resolve<ITransient<T>>("Factory:"+name)
        ///  
        /// Singleton:
        /// - Resolve<T>()
        /// - Resolve<IInnerFactory<T>>()
        /// - Resolve<ILazy<T>>()
        /// And, if name is not null
        /// - Resolve<T>(name)
        /// - Resolve<IInnerFactory<T>>("InnerFactory:"+name)
        /// - Resolve<ILazy<T>>("Factory:"+name) 
        ///  
        /// </summary>
        /// <param name="factoryType"></param>
        /// <param name="lifetime"></param>
        /// <param name="customFactory">The Func<object> must return a factoryType and implements IGet<T> like `class YourFactory<T> : IGet<T>`</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Builder RegisterFactory(Type factoryType, Lifetime lifetime, Func<object> customFactory, string? name = null) {
            if (!factoryType.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidCastException($"Factory {factoryType.GetTypeName()} must implement IGet<T>");
            }
            var type = factoryType.FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];
            ProxyFactory FactoryFromProvider(IProvider provider) => FactoryTools.CreateProxyFactory(type, provider);
            var proxyFactoryType = (lifetime == Lifetime.Singleton ? typeof(ILazy<>) : typeof(ITransient<>)).MakeGenericType(type);
            return RegisterProxyFactory(factoryType, lifetime, customFactory, proxyFactoryType, FactoryFromProvider, name);
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
        /// <param name="customFactory">A function returning the factoryType, like Func<IGet<T>></param>
        /// <param name="proxyFactoryType"></param>
        /// <param name="proxyFactory"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="Exception"></exception>
        public Builder RegisterProxyFactory(Type factoryType, Lifetime lifetime, Func<object> customFactory, Type proxyFactoryType, Func<IProvider, object> proxyFactory, string? name = null) {
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
            var customFactoryName = name == null ? null : $"{InnerFactoryPrefix}{name}";
            var innerFactoryType = typeof(IInnerFactory<>).MakeGenericType(type);
            var customProvider = Provider.Create(innerFactoryType, innerFactoryType, Lifetime.Singleton, customFactory, customFactoryName, false);
            Register(customProvider);
    
            // Register the regular instance factory. It's always lazy, this guarantees the factory is fully injected before the first Get()
            // This is needed to use:
            // - Resolve<T>()
            // - Resolve<T>(name)
            // To create instances injecting dependencies
            Func<object> getFromProviderFactory = FactoryTools.GetGetFromWrapperFactory(type, customProvider); // This is just () => customProvider.Get().Get()
            var provider = Provider.Create(type, type, lifetime, getFromProviderFactory, name, true);
            Register(provider);

            // Register a proxy factory so users can access to the factory and use it to create new instances with dependencies injected
            // This factory could be ILazy<T> or ITransient<T> depending on the lifetime, or a custom proxy factory
            var factoryName = name == null ? null : $"{FactoryPrefix}{name}";
            object ProxyFactory() => proxyFactory(provider);
            Register(Provider.Create(proxyFactoryType, proxyFactoryType, Lifetime.Singleton, ProxyFactory, factoryName, true));
            return this;
        }

        public Builder Scan(IEnumerable<Assembly> assemblies, Func<Type, bool>? predicate = null) {
            assemblies.ForEach(assembly => Scan(assembly, predicate));
            return this;
        }
    
        public Builder Scan(Assembly assembly, Func<Type, bool>? predicate = null) {
            Scan(assembly.GetTypes(), predicate);
            return this;
        }
    
        public Builder Scan(IEnumerable<Type> types, Func<Type, bool>? predicate = null) {
            if (predicate != null) types = types.Where(predicate);
            types.ForEach(type => Scan(type));
            return this;
        }
    
        public Builder Scan<T>() => Scan(typeof(T));
    
        public Builder Scan(Type type) {
            _scanner.Scan(type, null);
            return this;
        }

        public Builder ScanConfiguration(params object[] instances) {
            foreach (var configuration in instances) {
                _scanner.ScanConfiguration(configuration);
            }
            return this;
        }
    }
}