using System;
using System.Collections.Generic;

namespace Betauer.Core.Pool;

public class MiniPoolBusyInvalid<T> : BaseMiniPool<T> where T : class, IInvalidElement {
    public int PurgeIfSizeIsBiggerThan { get; set; }
    public MiniPoolBusyInvalid(Func<T> factory, int purgeIfSizeIsBiggerThan, int size = 4, bool lazy = true) : base(factory, size, lazy) {
        PurgeIfSizeIsBiggerThan = purgeIfSizeIsBiggerThan;
    }

    protected override bool IsBusy(T element) {
        return element.IsBusy();
    }

    protected override bool IsInvalid(T element) {
        return element.IsInvalid();
    }

    protected override bool MustBePurged(IReadOnlyList<T> pool) {
        return pool.Count > PurgeIfSizeIsBiggerThan;
    }
}