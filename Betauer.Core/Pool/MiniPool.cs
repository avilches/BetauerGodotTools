using System;
using System.Collections.Generic;

namespace Betauer.Core.Pool;

public class MiniPool<T> : BaseMiniPool<T> where T : class {
    private readonly Predicate<T> _busy;
    private readonly Predicate<T>? _invalid;
    private readonly Predicate<IReadOnlyList<T>>? _purgeIf;
    public static Builder Create() => new();

    public MiniPool(Func<T> factory, Predicate<T> busy, int size, bool lazy,
        Predicate<IReadOnlyList<T>>? purgeIf = null, Predicate<T>? invalid = null) : base(factory, size, lazy) {
        _busy = busy;
        _invalid = invalid;
        _purgeIf = purgeIf;
    }

    protected override bool IsBusy(T element) {
        return _busy(element);
    }

    protected override bool IsInvalid(T element) {
        return _invalid != null && _invalid.Invoke(element);
    }

    protected override bool MustBePurged(IReadOnlyList<T> pool) {
        return _invalid != null && _purgeIf != null && _purgeIf(pool);
    }
    
    public class Builder {
        private Func<T> _factory;
        private Predicate<T> _busy;
        private Predicate<T>? _invalid;
        private Predicate<IReadOnlyList<T>>? _purgeIf;
        private int _size = 4;
        private bool _lazy = true;

        public Builder Factory(Func<T> factory) {
            _factory = factory;
            return this;
        }

        public Builder BusyIf(Predicate<T> busy) {
            _busy = busy;
            return this;
        }

        public Builder InvalidIf(Predicate<T>? invalid) {
            _invalid = invalid;
            return this;
        }

        public Builder InitialSize(int size, bool lazy = true) {
            _size = size;
            _lazy = lazy;
            return this;
        }

        public Builder PurgeIf(Predicate<IReadOnlyList<T>>? purgeIf) {
            _purgeIf = purgeIf;
            return this;
        }

        public Builder PurgeIfPoolIsBiggerThan(int max) => PurgeIf(list => list.Count > max);

        public MiniPool<T> Build() {
            return new MiniPool<T>(_factory, _busy, _size, _lazy, _purgeIf, _invalid);
        }
    }
}