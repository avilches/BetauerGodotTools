namespace Betauer.Nodes;

public abstract class BaseHandler : IEventHandler {
    private bool _isEnabled = true;
    private bool _isDestroyed = false;

    public string? Name { get; set; }

    protected BaseHandler(string? name = null) {
        Name = name;
    }

    public virtual bool IsEnabled => _isEnabled;
    public void Disable() => _isEnabled = false;
    public void Enable() => _isEnabled = true;

    public virtual bool IsDestroyed => _isDestroyed;
    public void Destroy() => _isDestroyed = true;
}