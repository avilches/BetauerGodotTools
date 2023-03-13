using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Betauer.Core.Pool.Lifecycle;

public abstract class BasePool<T> : IPool<T>
    where T : class {

    public List<T> Pool { get; } = new();

    protected abstract T Create();
    public virtual T OnGet(T element) => element;
    protected abstract bool IsBusy(T element);
    protected abstract bool IsInvalid(T element);
    protected abstract bool MustBePurged(IReadOnlyList<T> pool);

    public T Get() {
        var span = CollectionsMarshal.AsSpan(Pool);

        for (var i = 0; i < span.Length; i++) {
            var element = span[i];
            if (!IsBusy(element) && !IsInvalid(element)) {
                return OnGet(element);
            }
        }
        if (MustBePurged(Pool)) {
            Pool.RemoveAll(IsInvalid);
        }
        var more = Create();
        Pool.Add(more);
        return more;
    }

    public void Fill(int desiredSize) {
        while (Pool.Count < desiredSize) {
            Pool.Add(Create());
        }
    }

    /// <summary>
    /// Return all valid elements (not busy and not invalid), invoking OnGet before return them 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<T> GetElements() {
        return Pool.Where(element => !IsBusy(element) && !IsInvalid(element)).Select(OnGet);
    }

}