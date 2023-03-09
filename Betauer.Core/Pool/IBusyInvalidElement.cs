namespace Betauer.Core.Pool;

public interface IBusyInvalidElement : IBusyElement {
    bool IsInvalid();
}