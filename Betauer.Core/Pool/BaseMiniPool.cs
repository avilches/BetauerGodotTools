using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Betauer.Core.Pool;

// TODO: Create a simple mini pool where the objects must be returned to use them again
public abstract class BaseMiniPool<T> where T : class {
    private readonly List<T> _pool;
    private readonly Func<T> _factory;

    public BaseMiniPool(Func<T> factory, int size = 4, bool lazy = true) {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        _pool = new List<T>(size);
        if (!lazy) {
            for (var i = 0; i < size; i++) {
                _pool.Add(_factory());
            }
        }
    }

    public T Get() {
        var span = CollectionsMarshal.AsSpan(_pool);

        for (var i = 0; i < span.Length; i++) {
            var element = span[i];
            if (!IsInvalid(element) && !IsBusy(element)) {
                // GD.Print("Get " + typeof(T) + "[" + i + "] of "+span.Length);
                return element;
            }
        }
        if (MustBePurged(_pool)) {
            // GD.Print("Purging " + typeof(T) + " pool (size is " + _pool.Count);
            _pool.RemoveAll(IsInvalid);
        }
        var more = _factory();
        _pool.Add(more);
        return more;
    }

    /// <summary>
    /// Returns a copy of the pool elements. It can be used to dispose them
    /// </summary>
    public List<T> Elements => new(_pool);

    protected abstract bool IsBusy(T element);
    protected abstract bool IsInvalid(T element);
    protected abstract bool MustBePurged(IReadOnlyList<T> pool);
}