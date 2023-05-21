using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;

namespace Betauer.Application.Lifecycle.Pool;

public class PoolContainer<T> : IPoolContainer {
    public List<IManagedPool> PoolFromFactories = new();

    public void Add(IManagedPool poolFromFactory) {
        var poolType = poolFromFactory.GetType().GetGenericArguments()[0];
        var poolContainerType = GetType().GetGenericArguments()[0];
        if (!poolType.IsSubclassOf(poolContainerType)) {
            throw new Exception($"Pool {poolFromFactory.GetType().GetTypeName()} is not compatible with container {GetType().GetTypeName()}: {poolType.GetTypeName()} is not a subclass of {poolContainerType.GetTypeName()}");
        }
        PoolFromFactories.Add(poolFromFactory);
    }

    public void Clear() {
        ForEachPool(pool => pool.Clear());
    }

    public void ForEachPool(Action<IManagedPool> action) {
        PoolFromFactories.ForEach(action);
    }

    public int BusyCount() => PoolFromFactories.Sum(p => p.BusyCount());
    public int AvailableCount() => PoolFromFactories.Sum(p => p.AvailableCount());
    public int InvalidCount() => PoolFromFactories.Sum(p => p.InvalidCount());
    public void Purge() => PoolFromFactories.ForEach(p => p.Purge());
    
    public IEnumerable<T> GetAllBusy() {
        foreach (var pool in PoolFromFactories) {
            foreach (var element in pool.GetBusy()) {
                yield return (T)element;
            }
        }
    }
    public IEnumerable<T> GetAvailable() {
        foreach (var pool in PoolFromFactories) {
            foreach (var element in pool.GetAvailable()) {
                yield return (T)element;
            }
        }
    }
}