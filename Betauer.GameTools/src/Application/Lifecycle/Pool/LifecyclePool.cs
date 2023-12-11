using System;
using Betauer.Core.Pool.Lifecycle;

namespace Betauer.Application.Lifecycle.Pool;
           
public class LifecyclePool<T> : ManagedPool<T> where T : class, IPoolLifecycle {
    public LifecyclePool(Func<T> factory, int purgeIfBiggerThan = 0) : base(factory, purgeIfBiggerThan) {
    }

    protected override bool IsBusy(T element) => element.IsBusy();
    protected override bool IsInvalid(T element) => element.IsInvalid();
    protected override T OnGet(T element) => (T)element.OnGet();
}        