using Godot;

namespace Betauer.Nodes;

public abstract class BaseHandler : IEventHandler {
    private bool _isEnabled = true;
    private bool _isDestroyed = false;

    public Node.ProcessModeEnum ProcessMode { get; set; }
    public string? Name { get; set; }

    protected BaseHandler(Node.ProcessModeEnum processMode, string? name = null) {
        Name = name;
        ProcessMode = processMode;
    }

    public void Disable() => _isEnabled = false;
    public void Enable() => _isEnabled = true;
    public void Destroy() => _isDestroyed = true;

    public virtual bool IsDestroyed => _isDestroyed;

    public virtual bool IsEnabled(bool isTreePaused) {
        return _isEnabled && ShouldProcess(isTreePaused, ProcessMode);
    }

    public static bool ShouldProcess(bool pause, Node.ProcessModeEnum processMode) {
        if (processMode == Node.ProcessModeEnum.Inherit) return !pause;
        return processMode == Node.ProcessModeEnum.Always ||
               (pause && processMode == Node.ProcessModeEnum.WhenPaused) ||
               (!pause && processMode == Node.ProcessModeEnum.Pausable);
    }


}