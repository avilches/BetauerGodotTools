using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Core;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;

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
            var provider = Provider.Create(registeredType, type, factory, lifetime, name, primary, lazy);
            Register(provider);
    
            Type factoryType = typeof(IFactory<>).MakeGenericType(type);
            object CustomFactory() => ProviderFactory.Create(type, provider);
            Register(Provider.Create(factoryType, factoryType, CustomFactory, Lifetime.Singleton, $"IFactory<{type}>:{name}", false, true));
            return this;
        }
    
        public Builder RegisterCustomFactory(Type type, Func<object> customFactory, string? factoryName = null, bool primary = false, bool lazy = false) {
            Type factoryType = typeof(IFactory<>).MakeGenericType(type);
            // Register the Custom Factory
            // The providerCustomFactory is used later to get the factory and inject fields inside it
            var innerType = typeof(IFactory<>).MakeGenericType(factoryType);
            var providerCustomFactory = Provider.Create(innerType, innerType, customFactory.Memoize(), Lifetime.Singleton, null, false, lazy);
            Register(providerCustomFactory);
    
            // Register instance factory class
            // It maps the service type "type" with the instance factory method in GetFromProviderFactory
            // The GetFromProviderFactory resolve the real factory using the custom factory, which is executed only once.
            // Then, with the real factory instance (which we don't know what type is it, but we know it implements IFactory<type>)
            // we call to the "Get()" method using reflection (through FastMethodInfo, which is very fast because it compiles a lambda to call to the method)
            // The instance is scanned for inject and [PostCreate] as usual.
            var getFromProviderFactory = ProviderCustomFactory.Create(type, providerCustomFactory);
            var providerFactory = Provider.Create(type, type, getFromProviderFactory.Get, Lifetime.Transient, null, false, true);
            Register(providerFactory);
    
            // Finally, we register a IFactory<> so the user can use the real factory.
            // Instead of expose the original custom factory, it creates a new Factory<type> where every call to Get()
            // invokes the original factory through the providerFactory, which returns an instance and scan for [Inject] and [PostCreate]
            Register(Provider.Create(factoryType, factoryType, () => {
                object ProviderFactory() => providerFactory.Get();
                return FuncFactory.Create(type, ProviderFactory);
            }, Lifetime.Singleton, factoryName, primary, true));
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

        public abstract class FuncFactory {
            protected readonly Func<object> FactoryFunc;

            protected FuncFactory(Func<object> factoryFunc) {
                FactoryFunc = factoryFunc;
            }

            public static FuncFactory Create(Type genericType, Func<object> factory) {
                var type = typeof(FuncFactoryTyped<>).MakeGenericType(genericType);
                FuncFactory instance = (FuncFactory)Activator.CreateInstance(type, factory)!;
                return instance;
            }
            
            public class FuncFactoryTyped<T> : FuncFactory, IFactory<T> {
                public FuncFactoryTyped(Func<object> factoryFunc) : base(factoryFunc) {
                }

                public T Get() {
                    return (T)FactoryFunc.Invoke();
                }
            }
        }

        public abstract class ProviderFactory {
            protected readonly IProvider Provider;

            protected ProviderFactory(IProvider provider) {
                Provider = provider;
            }

            public static ProviderFactory Create(Type type, IProvider provider) {
                var factoryType = typeof(ProviderFactoryTyped<>).MakeGenericType(type);
                ProviderFactory instance = (ProviderFactory)Activator.CreateInstance(factoryType, provider)!;
                return instance;
            }     
            
            public class ProviderFactoryTyped<T> : ProviderFactory, IFactory<T> {
                public ProviderFactoryTyped(IProvider provider) : base(provider) {
                }

                public T Get() => (T)Provider.Get();
            }
        }

        public abstract class ProviderCustomFactory {
            public abstract object Get();
            
            public static ProviderCustomFactory Create(Type type, IProvider provider) {
                var factoryType = typeof(ProviderCustomFactoryTyped<>).MakeGenericType(type);
                ProviderCustomFactory instance = (ProviderCustomFactory)Activator.CreateInstance(factoryType, provider)!;
                return instance;
            }

            public class ProviderCustomFactoryTyped<T> : ProviderCustomFactory {
                private readonly IProvider _customFactory;
        
                public ProviderCustomFactoryTyped(IProvider customFactory) {
                    _customFactory = customFactory;
                }
        
                public override object Get() {
                    IFactory<T> factory = (IFactory<T>)_customFactory.Get();
                    return factory.Get();
                }
            }
        }
    }
}