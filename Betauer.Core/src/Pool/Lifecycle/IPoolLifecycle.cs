namespace Betauer.Core.Pool.Lifecycle;

public interface IPoolLifecycle {
    bool IsBusy();
    bool IsInvalid();
    IPoolLifecycle OnGet() => this;
}