namespace Betauer.Nodes;

public interface IEventHandler {
    string? Name { get; }
    bool IsDestroyed { get; }
    public bool IsEnabled(bool isTreePaused);
    public void Disable();
    public void Enable();
    public void Destroy();
}