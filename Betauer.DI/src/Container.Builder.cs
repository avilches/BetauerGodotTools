using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Core;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public partial class Container {
    public partial class Builder {
        public Container Container { get; }

        private readonly List<IProvider> _providers = new();
        private readonly Scanner _scanner;
        
        public event Action? OnBuildFinished;
    
        public Builder(Container container) {
            Container = container;
            _scanner = new Scanner(this, Container);
        }
    
        public Builder() {
            Container = new Container();
            _scanner = new Scanner(this, Container);
        }

        public Builder Register(IProvider provider) {
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
            _scanner.Scan(type, null);
            return this;
        }

        public Builder ScanConfiguration(params object[] instances) {
            foreach (var configuration in instances) {
                _scanner.ScanConfiguration(configuration);
            }
            return this;
        }

        public Container Build() {
            var toBuild = new List<IProvider>(_providers);
            _providers.Clear();
            Container.Build(toBuild);
            OnBuildFinished?.Invoke();
            return Container;
        }
    }
}