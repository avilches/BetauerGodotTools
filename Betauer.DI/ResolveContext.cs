using System;
using System.Collections.Generic;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI {
    public class ResolveContext {
        internal Dictionary<string, (Lifetime, object)>? ObjectsCache;
        internal readonly Container Container;

        public ResolveContext(Container container) {
            Container = container;
        }

        internal bool TryGetFromCache(Type type, string? name, out object? instanceFound) {
            var key = name ?? type.FullName;
            if (ObjectsCache?.ContainsKey(key) ?? false) {
                var (l, o) = ObjectsCache[key];
                instanceFound = o;
                return true;
            }
            instanceFound = null;
            return false;
        }

        internal void AddInstanceToCache(Type type, Lifetime lifetime, object o, string? name) {
            var key = name ?? type.FullName;
            ObjectsCache ??= new Dictionary<string, (Lifetime, object)>();
            ObjectsCache[key] = (lifetime, o);
        }

        internal void End() {
            if (ObjectsCache == null) return;
            foreach (var (lifetime, instance) in ObjectsCache.Values) {
                Container.ExecutePostCreateMethods(instance);
                Container.ExecuteOnCreate(lifetime, instance);
            }
        }
    }
}