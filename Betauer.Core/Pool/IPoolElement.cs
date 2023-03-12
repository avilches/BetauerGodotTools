namespace Betauer.Core.Pool;

public interface IPoolElement {
    bool IsBusy();
    bool IsInvalid();
    void OnGet();
}