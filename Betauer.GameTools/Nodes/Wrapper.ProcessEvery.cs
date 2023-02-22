namespace Betauer.Nodes;

public class ProcessEveryWrapper : IProcessHandler {
    private readonly IProcessHandler _delegate;
    public string? Name { get; }
    private float Every { get; set; }
    private double Accumulated { get; set; } = 0;
    public ProcessEveryWrapper(float every, IProcessHandler @delegate, string? name = null) {
        Every = every;
        Name = name ?? @delegate.Name;
        _delegate = @delegate;
    }

    public void Handle(double delta) {
        Accumulated += delta;
        if (Accumulated >= Every) {
            _delegate.Handle(delta);
            Accumulated = Every - Accumulated;
        }
    }

    public bool IsDestroyed => _delegate.IsDestroyed;
    public bool IsEnabled(bool isTreePaused) => _delegate.IsEnabled(isTreePaused);
    public void Disable() => _delegate.Disable();
    public void Enable() => _delegate.Enable();
    public void Destroy() => _delegate.Destroy();
}