namespace Betauer.Core.Pool;

public interface IInvalidElement : IBusyElement {
    bool IsInvalid();
}