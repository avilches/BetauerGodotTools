using System.Collections.Generic;

namespace Betauer.Core.Pool.Basic;

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseBasicPool<T> {

    public IPoolCollection<T> Pool { get; }

    protected BaseBasicPool(IPoolCollection<T>? pool = null) {
        Pool = pool ?? new StackPoolCollection<T>();
    }

    public T Get() {
        var element = Pool.Count == 0 ? Create() : Pool.Get();
        OnGet(element);
        return element;
    }

    public IEnumerable<T> GetAll() {
        while (Pool.Count > 0) {
            yield return Get();
        }
    }

    public void Fill(int desiredSize) {
        while (Pool.Count < desiredSize) Pool.Add(Create());
    }

    public void Return(T element) {
        OnReturn(element);
        Pool.Add(element);
    }

    public abstract T Create();

    public virtual void OnGet(T element) { }

    public virtual void OnReturn(T element) { }
}