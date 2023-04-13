namespace Betauer.Core.Pool.Lifecycle;

public interface IPoolLifecycle {
    bool IsBusy();
    bool IsInvalid();
    void OnGet() {}
}