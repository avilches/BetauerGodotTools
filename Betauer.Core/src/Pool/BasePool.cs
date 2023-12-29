using System;
using System.Collections.Generic;

namespace Betauer.Core.Pool;

/// <summary>
/// A pool that uses a lifo/fifo collection to store the elements.
/// 
/// GetOrCreate() will create (and return) a new element when the pool is empty
/// When an element is requested, it disappears from the pool.
///
/// Elements need to be returned to the pool to be reused later again.
///
/// This Pool acts like a collection where you put element you don't use, and
/// when you can extract elements to use them and return to them later.
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BasePool<T> : IPool {
    protected PoolCollection<T> Elements { get; }
    
    public Type ElementType => typeof(T);
    public int StatsCreated { get; private set; } = 0;
    public int StatsRecycled { get; private set; } = 0;

    protected BasePool(PoolCollection<T>? pool = null) {
        Elements = pool ?? new PoolCollection<T>.Stack();
    }

    public T GetOrCreate() {
        T element;
        if (Elements.Count == 0) {
            element = ExecuteCreate();
            StatsCreated++;
        } else {
            element = Elements.Get();
            StatsRecycled++;
        }
        ExecuteOnGet(element);
        return element;
    }

    public void Fill(int desiredSize) {
        while (Elements.Count < desiredSize) {
            Elements.Add(ExecuteCreate());
        }
    }

    public void Release(T element) {
        ExecuteOnRelease(element);
        Elements.Add(element);
    }

    public IEnumerable<T> GetAll() {
        while (Elements.Count > 0) {
            var element = Elements.Get();
            ExecuteOnGet(element);
            yield return element;
        }
    }

    public void RemoveAll() {
        RemoveAll(null);
    }

    protected void RemoveAll(Action<T>? destroyAction) {
        while (Elements.Count > 0) {
            var element = Elements.Get();
            destroyAction?.Invoke(element);
            ExecuteDestroy(element);
        }
    }

    public int Size() {
        return Elements.Count;
    }

    protected abstract T ExecuteCreate();

    protected virtual void ExecuteOnGet(T element) {
    }

    protected virtual void ExecuteOnRelease(T element) {
    }

    protected virtual void ExecuteDestroy(T element) {
    }
}