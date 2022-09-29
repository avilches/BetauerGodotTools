using System;
using System.Collections.Generic;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI {
    public class ResolveContext {
        internal readonly Dictionary<string, object> SingletonCache = new();
        internal readonly List<object> Transients = new();
        internal readonly Stack<string> TransientNameStack = new();
        internal readonly Container Container;

        public ResolveContext(Container container) {
            Container = container;
        }

        internal bool TryGetSingletonFromCache(Type type, string? name, out object? instanceFound) {
            var key = name ?? type.FullName;
            return SingletonCache.TryGetValue(key, out instanceFound);
        }

        internal void AddSingleton(Type type, object instance, string? name) {
            var key = name ?? type.FullName;
            SingletonCache[key] = instance;
        }

        // This stack avoid circular dependencies between transients
        internal void StartTransient(Type type, string? name) {
            var key = name ?? type.FullName;
            if (TransientNameStack.Contains(key)) {
                throw new CircularDependencyException(string.Join("\n", TransientNameStack));
            }
            TransientNameStack.Push(key);
        }

        internal void AddTransient(object instance) {
            Transients.Add(instance);
        }

        internal void EndTransient() {
            TransientNameStack.Pop();
        }

        internal void End() {
            foreach (var instance in SingletonCache.Values) {
                Container.ExecutePostCreateMethods(instance);
                Container.ExecuteOnCreated(Lifetime.Singleton, instance);
            }
            foreach (var instance in Transients) {
                Container.ExecutePostCreateMethods(instance);
                Container.ExecuteOnCreated(Lifetime.Transient, instance);
            }
        }
    }
}