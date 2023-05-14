using System;
using System.Collections.Generic;

namespace Betauer.Core.Pool.Lifecycle;

public static class PoolTemplates {
    public static PoolTemplate<T>.Builder Create<T>() where T : class => new();

    public static PoolTemplate<T>.Builder Create<T>(Func<T> factory) where T : class => Create<T>().Factory(factory);

    public static PoolTemplate<T> Lifecycle<T>(Func<T> factory, Predicate<IReadOnlyList<T>>? purgeIf = null)
        where T : class, IPoolLifecycle =>
        new PoolTemplate<T>.Builder()
            .Factory(factory)
            .BusyIf(e => e.IsBusy())
            .InvalidIf(e => e.IsInvalid())
            .PurgeIf(purgeIf ?? ((_) => false))
            .Build();

    public static PoolTemplate<T> Lifecycle<T>(Func<T> factory, int purgeIfPoolIsBiggerThan)
        where T : class, IPoolLifecycle {
        return Lifecycle(factory, list => list.Count > purgeIfPoolIsBiggerThan);
    }
}