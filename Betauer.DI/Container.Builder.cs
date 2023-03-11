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
            var provider = Provider.Create(registeredType, type, lifetime, factory, name, primary, lazy);
            Register(provider);
    
            Type factoryType = typeof(IFactory<>).MakeGenericType(type);
            object CustomFactory() => ProviderFactory.Create(type, provider);
            Register(Provider.Create(factoryType, factoryType, Lifetime.Singleton, CustomFactory, $"IFactory<{type}>:{name}", false, true));
            return this;
        }
    
        public Builder RegisterCustomFactory(Type type, Func<object> customFactory, string? factoryName = null, bool primary = false, bool lazy = false) {
            Type factoryType = typeof(IFactory<>).MakeGenericType(type);
            // Register the Custom Factory
            // The providerCustomFactory is used later to get the factory, build the instances and inject fields inside them
            var innerType = typeof(IFactory<>).MakeGenericType(factoryType);
            var providerCustomFactory = Provider.Create(innerType, innerType, Lifetime.Singleton, customFactory, null, false, lazy);
            Register(providerCustomFactory);
    
            // Register the regular instance factory class
            // Using the getFromProviderFactory.GetCustomFactory makes the factory of this provider the Get method in the custom factory 
            var getFromProviderFactory = ProviderCustomFactory.Create(type, providerCustomFactory);
            var providerFactory = Provider.Create(type, type, Lifetime.Transient, getFromProviderFactory.GetCustomFactory, null, false, true);
            Register(providerFactory);
    
            // Finally, we register a IFactory<> so the user can use the real factory.
            // Instead of expose the original custom factory, it creates a new Factory<type> where every call to Get()
            // invokes the original factory through the providerFactory, which returns an instance and scan for [Inject] and PostInject and OnCreated
            Register(Provider.Create(factoryType, factoryType, Lifetime.Singleton, () => {
                object ProviderFactory() => providerFactory.Get();
                return FuncFactory.Create(type, ProviderFactory);
            }, factoryName, primary, true));
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