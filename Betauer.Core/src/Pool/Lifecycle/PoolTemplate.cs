using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Betauer.Core.Pool.Lifecycle;

public class PoolTemplate<T> : BasePool<T> where T : class {
    private readonly Func<T> _factory;
    private readonly Predicate<T> _busy;
    
    private readonly Func<T, T>? _onGet;
    private readonly Predicate<T>? _invalid;
    private readonly Predicate<IReadOnlyList<T>>? _purgeIf;

    private PoolTemplate(
        Func<T> factory,
        Predicate<T> busy,
        Func<T, T>? onGet = null,
        Predicate<IReadOnlyList<T>>? purgeIf = null,
        Predicate<T>? invalid = null) {
        Debug.Assert(factory != null, nameof(factory) + " != null");
        Debug.Assert(busy != null, nameof(busy) + " != null");
        _factory = factory;
        _busy = busy;
        _onGet = onGet;
        _purgeIf = purgeIf;
        _invalid = invalid;
    }

    protected override T OnGet(T element) {
        return _onGet != null ? _onGet.Invoke(element) : element;
    }

    protected override T Create() {
        return _factory.Invoke();
    }

    protected override bool IsBusy(T node) {
        return _busy.Invoke(node);
    }

    protected override bool IsInvalid(T element) {
        return _invalid != null && _invalid.Invoke(element);
    }

    protected override bool MustBePurged(IReadOnlyList<T> pool) {
        return _invalid != null && _purgeIf != null && _purgeIf.Invoke(pool);
    }

    public class Builder {
        private Func<T> _factory;
        private Predicate<T> _busy;
        private Func<T, T>? _onGet;
        private Predicate<T>? _invalid;
        private Predicate<IReadOnlyList<T>>? _purgeIf;

        internal Builder() {
            _factory = Activator.CreateInstance<T>;
        }

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

        public Builder OnGet(Func<T, T>? onGet) {
            _onGet = onGet;
            return this;
        }

        public Builder PurgeIf(Predicate<IReadOnlyList<T>>? purgeIf) {
            _purgeIf = purgeIf;
            return this;
        }

        public Builder PurgeIfPoolIsBiggerThan(int max) => PurgeIf(list => list.Count > max);

        public PoolTemplate<T> Build() {
            return new PoolTemplate<T>(_factory, _busy, _onGet, _purgeIf, _invalid);
        }
    }
}