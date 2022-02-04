using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Object = Godot.Object;

namespace Betauer.Memory {
    public static class DefaultObjectPool {
        public static ObjectPool Registry { get; private set; } = new ObjectPool();
        public static bool UsePool = true;

        public static void SetPool(ObjectPool objectPool) {
            Registry = objectPool;
        }

        public static IObjectPool<T> Pool<T>() where T : class, IRecyclable {
            return Registry.Pool<T>();
        }

        public static T Get<T>() where T : class, IRecyclable {
            if (!UsePool) {
                var recyclable = (T)Activator.CreateInstance(typeof(T));
                recyclable.SetPool(null);
                return recyclable;
            }
            return Registry.Get<T>();
        }
    }

    public interface IObjectPool {
        bool Return(IRecyclable o);
        int ReturnAll();
        ICollection<IRecyclable> GetAvailable();
        ICollection<IRecyclable> GetInUse();
    }

    public class ObjectPool : IObjectPool {
        internal static readonly Logger Logger = LoggerFactory.GetLogger(typeof(ObjectPool));
        public readonly Dictionary<Type, IObjectPool> Pools = new Dictionary<Type, IObjectPool>();

        public IObjectPool<T> Pool<T>() where T : IRecyclable {
            return (IObjectPool<T>)Pool(typeof(T));
        }

        public IObjectPool Pool(Type key) {
            if (Pools.TryGetValue(key, out IObjectPool pool)) return pool;
            lock (Pools) {
                if (Pools.TryGetValue(key, out pool)) return pool;
                var typeObjectPool = typeof(ObjectPool<>).MakeGenericType(new [] { key });
                pool = (IObjectPool)Activator.CreateInstance(typeObjectPool);
                Pools[key] = pool;
                return pool;
            }
        }

        public T Get<T>() where T : class, IRecyclable {
            return Pool<T>().Get();
        }

        int IObjectPool.ReturnAll() {
            return Pools.Values.Sum(pool => pool.ReturnAll());
        }

        public ICollection<IRecyclable> GetAvailable() {
            var available = new LinkedList<IRecyclable>();
            foreach (var pool in Pools.Values) {
                foreach (var item in pool.GetAvailable()) {
                    available.AddLast(item);
                }
            }
            return available;
        }

        public bool Return(IRecyclable o) {
            return Pool(o.GetType()).Return(o);
        }

        public ICollection<IRecyclable> GetInUse() {
            var inUse = new LinkedList<IRecyclable>();
            foreach (var pool in Pools.Values) {
                foreach (var item in pool.GetInUse()) {
                    inUse.AddLast(item);
                }
            }
            return inUse;
        }
    }

    public interface IObjectPool<out T> : IObjectPool {
        public T Get();
    }

    public class ObjectPool<T> : IObjectPool<T> where T : IRecyclable {
        private readonly Queue<IRecyclable> _available = new Queue<IRecyclable>();
        private readonly IDictionary<int, IRecyclable> _using = new Dictionary<int, IRecyclable>();
        private readonly Func<T> _factory;

        public ObjectPool() {
            _factory = () => (T)Activator.CreateInstance(typeof(T));
        }

        public ObjectPool(Func<T> factory) {
            _factory = factory;
        }

        public T Get() {
            lock (this) {
                var exists = _available.Count > 0;
                IRecyclable item;
                if (exists) {
                    item = _available.Dequeue();
                    if (item is Object o && !Object.IsInstanceValid(o)) {
                        item = _factory();
                        ObjectPool.Logger.Debug(
                            "Detected invalid Godot.Object, creating new instance of type " + typeof(T) + ": " + item);
                    } else {
                        ObjectPool.Logger.Debug("Getting instance from pool " + typeof(T) + ": " + item);
                    }
                } else {
                    item = _factory();
                    ObjectPool.Logger.Debug("Creating new instance of type " + typeof(T) + ": " + item);
                }
                item.SetPool(this);
                _using[item.GetHashCode()] = item;
                return (T)item;
            }
        }

        public bool Return(IRecyclable rec) {
            lock (this) {
                var item = (T)rec;
                if (_available.Contains(item)) {
                    ObjectPool.Logger.Warning("Returning ignored: instance is already in the pool " + typeof(T) + ": " +
                                              item);
                    return false;
                }
                ObjectPool.Logger.Debug("Returning instance to pool " + typeof(T) + ": " + item);
                _using.Remove(item.GetHashCode());
                _available.Enqueue(item);
                return true;
            }
        }

        public int ReturnAll() {
            var x = 0;
            lock (this) {
                x += _using.Values.ToList().Count(Return);
            }
            return x;
        }

        public ICollection<IRecyclable> GetInUse() => _using.Values.ToList();
        public ICollection<IRecyclable> GetAvailable() => _available.ToList();
    }
}