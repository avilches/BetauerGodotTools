namespace Betauer.Core.Pool.Lifecycle;

public abstract class BasePoolLifecycle<T> : BasePool<T> where T : class, IPoolLifecycle {
    protected override bool IsBusy(T element) => element.IsBusy();

    protected override bool IsInvalid(T element) => element.IsInvalid();
}