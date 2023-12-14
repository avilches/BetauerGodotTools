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
        public Container Container { get; }

        private readonly List<Provider> _providers = new();
        private readonly List<object> _instances = new();
        
        public event Action? OnBuildFinished;
    
        internal Builder(Container container) {
            Container = container;
        }

        public void RegisterFactory(object factory) {
            if (!factory.GetType().ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidCastException($"Factory {factory.GetType().GetTypeName()} must implement IFactory<>");
            }
            _instances.Add(factory);
        }

        public void RegisterFactory<T>(IFactory<T> factory) where T : class {
            _instances.Add(factory);
        }

        public Builder Register(Provider provider) {
            _providers.Add(provider);
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
            Scanner.Scan(this, type, null);
            return this;
        }

        public Builder ScanConfiguration(params object[] instances) {
            foreach (var configuration in instances) {
                Scanner.ScanConfiguration(this, configuration);
            }
            return this;
        }

        internal void Build() {
            Container.Build(_providers, _instances);
            OnBuildFinished?.Invoke();
        }
    }
}