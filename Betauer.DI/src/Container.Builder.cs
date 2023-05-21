using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Core;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public partial class Container {
    public partial class Builder {
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