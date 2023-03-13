using System.Collections.Generic;

namespace Betauer.Core.Pool.Basic;

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseBasicPool<T> : IPool<T> {
    public PoolCollection<T> Pool { get; }

    protected BaseBasicPool(PoolCollection<T>? pool = null) {
        Pool = pool ?? new PoolCollection.Stack<T>();
    }

    public T Get() {
        var element = Pool.Count == 0 ? Create() : Pool.Get();
        return OnGet(element);
    }

    public IEnumerable<T> GetElements() {
        while (Pool.Count > 0) {
            yield return OnGet(Pool.Get());
        }
    }

    public void Fill(int desiredSize) {
        while (Pool.Count < desiredSize) {
            Pool.Add(Create());
        }
    }

    public void Return(T element) {
        Pool.Add(OnReturn(element));
    }

    public abstract T Create();

    public virtual T OnGet(T element) => element;

    public virtual T OnReturn(T element) => element;
}