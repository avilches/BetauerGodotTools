using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.Core.Pool;

namespace Betauer.Application.Lifecycle;

public class PoolContainer<T> {
    public List<IPool<T>> PoolFromFactories = new();

    public void Add(IPool<T> poolFromFactory) {
        PoolFromFactories.Add(poolFromFactory);
    }

    public void Clear() {
        ForEachPool(pool => pool.Clear());
    }

    public void ForEachPool(Action<IPool<T>> action) {
        PoolFromFactories.ForEach(action);
    }

    public void ForEachElementInAllPools(Action<T> action) {
        PoolFromFactories.ForEach(factory => factory.GetAll().ForEach(action));
    }
}