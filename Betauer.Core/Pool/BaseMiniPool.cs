using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Betauer.Core.Pool;

public abstract class BaseMiniPool<T> where T : class {
    public readonly List<T> Pool;
    public int DesiredDesiredSize;

    protected BaseMiniPool(int desiredSize = 4) {
        Pool = new List<T>(desiredSize);
        DesiredDesiredSize = desiredSize;
    }

    public void Fill() {
        while (Pool.Count < DesiredDesiredSize) Pool.Add(Create());
    }

    public T Get() {
        var span = CollectionsMarshal.AsSpan(Pool);

        for (var i = 0; i < span.Length; i++) {
            var element = span[i];
            if (!IsInvalid(element) && !IsBusy(element)) {
                // GD.Print("Get " + typeof(T) + "[" + i + "] of "+span.Length);
                return element;
            }
        }
        if (MustBePurged(Pool)) {
            // GD.Print("Purging " + typeof(T) + " pool (size is " + _pool.Count);
            Pool.RemoveAll(IsInvalid);
        }
        var more = Create();
        Pool.Add(more);
        return more;
    }

    /// <summary>
    /// Returns a copy of the pool elements. It can be used to dispose them
    /// </summary>
    public List<T> Elements => new(Pool);

    protected abstract T Create();
    protected abstract bool IsBusy(T element);
    protected abstract bool IsInvalid(T element);
    
    protected virtual bool MustBePurged(IReadOnlyList<T> pool) => Pool.Count > DesiredDesiredSize;
}