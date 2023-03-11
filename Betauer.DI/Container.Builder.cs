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
            _scanner = new Scanner(this);
        }
    
        public Builder() {
            _container = new Container();
            _scanner = new Scanner(this);
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
    
        public Builder RegisterServiceAndAddFactory(Type registeredType, Type type, Func<object> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) {
            // Register the regular instance factory so using Resolve<T>() or Resolve<T>(name) will create new instances, injecting dependencies.
            var providerFactory = Provider.Create(registeredType, type, lifetime, factory, name, primary, lazy);
            Register(providerFactory);
    
            // Register the factory as IFactory<> so the user can get the real factory using Resolve<IFactory<T>>() or
            // Resolve<IFactory<T>>("Factory:"+name) and then call to Get() which returns new instances, injecting dependencies.
            Type factoryType = typeof(IFactory<>).MakeGenericType(type);
            var factoryName = name == null ? null : $"Factory:{name}";
            object CustomFactory() => FactoryTools.CreateIFactoryFromProvider(type, providerFactory);
            Register(Provider.Create(factoryType, factoryType, Lifetime.Singleton, CustomFactory, factoryName, primary, true));
            return this;
        }

        public Builder RegisterCustomFactory<T>(Func<T> customFactory, string? name = null, bool primary = false, bool lazy = false) {
            return RegisterCustomFactory(typeof(T), () => customFactory(), name, primary, lazy);
        }

        public Builder RegisterCustomFactory(Type factoryType, Func<object> customFactory, string? name = null, bool primary = false, bool lazy = false) {
            var type = FactoryTools.GetIFactoryGenericType(factoryType);
            if (type == null) throw new InvalidCastException();

            // Register the real factory, it can be accessed with
            // - Resolve<the factory type>()
            // - Resolve<IFactory<T>>("InnerFactory:"+name)
            // This factory returns the instances without inject dependencies in them 
            var customFactoryName = name == null ? null : $"InnerFactory:{name}";
            var customProvider = Provider.Create(factoryType, factoryType, Lifetime.Singleton, customFactory, customFactoryName, primary, lazy);
            Register(customProvider);
    
            // Register the regular instance factory
            // - Resolve<T>() or Resolve<T>(name) will create new instances, injecting dependencies.
            Func<object> getFromProviderFactory = FactoryTools.ProviderGetFactoryGet(type, customProvider); // This is just () => customProvider.Get().Get()
            var provider = Provider.Create(type, type, Lifetime.Transient, getFromProviderFactory, name, primary, true);
            Register(provider);

            // Register a factory as IFactory<T> so the user can get it with
            // - Resolve<IFactory<T>>()
            // - Resolve<IFactory<T>>("Factory:"+name)
            // This factory is a wrapper which return instances injecting dependencies.
            var factoryName = name == null ? null : $"Factory:{name}";
            object CustomFactory() => FactoryTools.CreateIFactoryFromProvider(type, provider);
            var iFactoryType = typeof(IFactory<>).MakeGenericType(type);
            Register(Provider.Create(iFactoryType, iFactoryType, Lifetime.Singleton, CustomFactory, factoryName, primary, true));
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