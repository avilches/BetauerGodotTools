using Betauer.Core.Pool.Lifecycle;
using Betauer.DI.Factory;

namespace Betauer.Application.Lifecycle.Pool;

public class LifecyclePool<T> : ManagedPool<T> where T : class, IPoolLifecycle {
    public LifecyclePool(int purgeIfBiggerThan = 0) : base(purgeIfBiggerThan) {
    }

    public LifecyclePool(IFactory<T> factory, int purgeIfBiggerThan = 0) : base(factory, purgeIfBiggerThan) {
    }

    public LifecyclePool(string? factoryName, int purgeIfBiggerThan = 0) : base(factoryName, purgeIfBiggerThan) {
    }

    protected override bool IsBusy(T element) => element.IsBusy();
    protected override bool IsInvalid(T element) => element.IsInvalid();
    protected override T OnGet(T element) => (T)element.OnGet();
}