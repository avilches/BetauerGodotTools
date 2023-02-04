using Godot;

namespace Betauer.Nodes;

public abstract class BaseEventHandler : IEventHandler {
    private bool _isEnabled = true;
    private bool _isDestroyed = false;

    public Node.ProcessModeEnum ProcessMode { get; set; } = Node.ProcessModeEnum.Inherit;
    public string? Name { get; set; }

    protected BaseEventHandler(string? name, Node.ProcessModeEnum processMode) {
        Name = name;
        ProcessMode = processMode;
    }

    public void Disable() => _isEnabled = false;
    public void Enable() => _isEnabled = true;
    public void Destroy() => _isDestroyed = true;

    public virtual bool IsDestroyed => _isDestroyed;

    public virtual bool IsEnabled(bool isTreePaused) {
        return _isEnabled && NodeHandler.ShouldProcess(isTreePaused, ProcessMode);
    }
}