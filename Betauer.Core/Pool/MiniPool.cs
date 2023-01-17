using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;

namespace Betauer.Core.Pool;

public class MiniPool<T> where T : class {
    private readonly List<T> _pool;
    private readonly Func<T> _factory;
    private readonly Predicate<T> _busy;
    private readonly Predicate<T>? _invalid;
    private readonly int _max;

    public MiniPool(Func<T> factory, Predicate<T> busy, Predicate<T> invalid = null, int size = 4, int max = 200) {
        _factory = factory;
        _busy = busy;
        _invalid = invalid;
        _pool = new List<T>(size);
        _max = Math.Max(max, size);
    }

    public T Get() {
        var span = CollectionsMarshal.AsSpan(_pool);
        
        for (var i = 0; i < span.Length; i++) {
            var element = span[i];
            if (!_busy(element)) {
                GD.Print("LabelHit["+i+"]");
                return element;
            }
        }
        if (_pool.Count >= _max && _invalid != null) _pool.RemoveAll(_invalid);
        if (_pool.Count >= _max) 
            throw new Exception($"MiniPool {typeof(T)} can't accept more than {_max} elements");
        var more =_factory();
        _pool.Add(more);
        return more;
    }
}