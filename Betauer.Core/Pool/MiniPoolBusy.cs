using System;
using System.Collections.Generic;

namespace Betauer.Core.Pool;

public class MiniPoolBusy<T> : BaseMiniPool<T> where T : class, IBusyElement {
    
    public MiniPoolBusy(Func<T> factory, int size = 4, bool lazy = true) : base(factory, size, lazy) {
        
    }

    protected override bool IsBusy(T element) {
        return element.IsBusy();
    }

    protected override bool IsInvalid(T element) {
        throw new NotImplementedException();
    }

    protected override bool MustBePurged(IReadOnlyList<T> pool) {
        return false;
    }
}