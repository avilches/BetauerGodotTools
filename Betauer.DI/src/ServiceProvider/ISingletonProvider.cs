namespace Betauer.DI.ServiceProvider; 

public interface ISingletonProvider : IProvider {
    public bool IsInstanceCreated { get; }
    public bool Lazy { get; }
    public object? Instance { get; }
}