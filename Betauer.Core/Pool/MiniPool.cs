using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Godot;

namespace Betauer.Core.Pool;

public class MiniPool<T> where T : class {
    private readonly List<T> _pool;
    private readonly Func<T> _factory;
    private readonly Predicate<T> _busy;
    private readonly Predicate<T>? _invalid;
    private readonly int _max;

    public static MiniPoolBuilder<T> Create() => new();

    public MiniPool(Func<T> factory, Predicate<T> busy, Predicate<T>? invalid, int size, int max, bool fill) {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _busy = busy ?? throw new ArgumentNullException(nameof(busy));
        _invalid = invalid;
        
        _pool = new List<T>(size);
        _max = Math.Max(max, size);
        if (fill) {
            for (var i = 0; i < size; i++) {
                _pool.Add(_factory());
            }
        }
    }

    public T Get() {
        var span = CollectionsMarshal.AsSpan(_pool);

        for (var i = 0; i < span.Length; i++) {
            var element = span[i];
            if (IsValid(element) && !_busy(element)) {
                GD.Print("LabelHit[" + i + "]");
                return element;
            }
        }
        if (_pool.Count >= _max && _invalid != null) _pool.RemoveAll(_invalid);
        if (_pool.Count >= _max)
            throw new Exception($"MiniPool {typeof(T)} can't accept more than {_max} elements");
        var more = _factory();
        _pool.Add(more);
        return more;
    }

    private bool IsValid(T ele) => _invalid == null || !_invalid(ele); 

    public class MiniPoolBuilder<T> where T : class {
        private Func<T> _factory;
        private Predicate<T> _busy;
        private Predicate<T>? _invalid;
        private int _max = 200;
        private int _size = 4;
        private bool _fill = false;

        public MiniPoolBuilder<T> Factory(Func<T> factory) {
            _factory = factory;
            return this;
        }

        public MiniPoolBuilder<T> BusyIf(Predicate<T> busy) {
            _busy = busy;
            return this;
        }

        public MiniPoolBuilder<T> InvalidIf(Predicate<T>? invalid) {
            _invalid = invalid;
            return this;
        }

        public MiniPoolBuilder<T> InitialSize(int size, bool fill = false) {
            _size = size;
            _fill = fill;
            return this;
        }

        public MiniPoolBuilder<T> MaxSize(int max) {
            _max = max;
            return this;
        }

        public MiniPool<T> Build() {
            return new MiniPool<T>(_factory, _busy, _invalid, _size, _max, _fill);
        }
    }
}