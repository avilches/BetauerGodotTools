using System;
using System.Collections.Generic;
using System.Diagnostics;

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

public class PoolTemplate<T> : BasePool<T> where T : class {
    private readonly Func<T> _factory;
    private readonly Predicate<T> _busy;
    private readonly Predicate<T>? _invalid;
    private readonly Predicate<IReadOnlyList<T>>? _purgeIf;

    private PoolTemplate(
        Func<T> factory,
        Predicate<T> busy,
        Predicate<IReadOnlyList<T>>? purgeIf = null,
        Predicate<T>? invalid = null) {
        Debug.Assert(factory != null, nameof(factory) + " != null");
        Debug.Assert(busy != null, nameof(busy) + " != null");
        _factory = factory;
        _busy = busy;
        _purgeIf = purgeIf;
        _invalid = invalid;
    }

    protected override T Create() {
        return _factory.Invoke();
    }

    protected override bool IsBusy(T element) {
        return _busy.Invoke(element);
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
        private Predicate<T>? _invalid;
        private Predicate<IReadOnlyList<T>>? _purgeIf;

        internal Builder() {
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

        public Builder PurgeIf(Predicate<IReadOnlyList<T>>? purgeIf) {
            _purgeIf = purgeIf;
            return this;
        }

        public Builder PurgeIfPoolIsBiggerThan(int max) => PurgeIf(list => list.Count > max);

        public PoolTemplate<T> Build() {
            return new PoolTemplate<T>(_factory, _busy, _purgeIf, _invalid);
        }
    }
}