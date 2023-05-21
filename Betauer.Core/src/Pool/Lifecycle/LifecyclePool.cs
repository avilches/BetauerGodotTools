using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Betauer.Core.Pool.Lifecycle;

/// <summary>
/// A pool where elements are always kept in the pool.
/// When an element is requested, the pool is searched for an element that is not busy and not invalid.
/// If no element is found, a new element is created and added to the pool. Then it's returned.
///
/// Clients must define when an element is busy or invalid overriding the IsBusy and IsInvalid methods.
/// Busy elements are not returned by the pool (are ignored).
/// Invalid elements are deleted from the pool (are purged). The purge is executed when there is no valid
/// elements to return and the method MustBePurged returns true. 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BasePool<T> where T : class {

    public List<T> Pool { get; } = new();

    protected abstract T Create();
    protected abstract bool IsBusy(T node);
    protected virtual T OnGet(T element) => element;
    protected abstract bool IsInvalid(T element);
    protected abstract bool MustBePurged(IReadOnlyList<T> pool);

    /// <summary>
    /// Returns a valid element from the pool (non busy and non invalid).
    /// Creating a new one if no valid element is found.
    /// </summary>
    /// <returns></returns>
    public T Get() {
        var span = CollectionsMarshal.AsSpan(Pool);

        for (var i = 0; i < span.Length; i++) {
            var element = span[i];
            if (!IsBusy(element) && !IsInvalid(element)) {
                return OnGet(element);
            }
        }
        if (MustBePurged(Pool)) {
            Purge();
        }
        var more = Create();
        Pool.Add(more);
        return OnGet(more);
    }

    public void Purge() => Pool.RemoveAll(IsInvalid);

    public int BusyCount() => Pool.Count(e => !IsInvalid(e) && IsBusy(e));
    public int AvailableCount() => Pool.Count(e => !IsInvalid(e) && !IsBusy(e));
    public int InvalidCount() => Pool.Count(IsInvalid);
    
    /// <summary>
    /// Returns all the valid elements from the pool (non busy and non invalid).
    /// </summary>
    /// <returns></returns>
    public IEnumerable<T> GetAvailable() => Pool.Where(e => !IsInvalid(e) && !IsBusy(e)).Select(OnGet);

    public IEnumerable<T> GetBusy() => Pool.Where(e => !IsInvalid(e) && IsBusy(e));

    public void Fill(int desiredSize) {
        while (Pool.Count < desiredSize) {
            Pool.Add(Create());
        }
    }

    public void Clear() => Pool.Clear();
}