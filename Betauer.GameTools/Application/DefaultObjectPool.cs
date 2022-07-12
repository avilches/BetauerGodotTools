using System;
using Betauer.Memory;

namespace Betauer.Application {
    public static class DefaultObjectPool {
        public static ObjectPool Registry { get; private set; } = new ObjectPool();
        public static bool Enabled = true;

        public static void SetPool(ObjectPool objectPool) {
            Registry = objectPool;
        }
    
        public static IObjectPool<T> Pool<T>() where T : class, IRecyclable {
            return Registry.Pool<T>();
        }

        public static T Get<T>() where T : class, IRecyclable {
            if (!Enabled) {
                var recyclable = (T)Activator.CreateInstance(typeof(T));
                recyclable.SetPool(null);
                return recyclable;
            }
            return Registry.Get<T>();
        }
    }
}