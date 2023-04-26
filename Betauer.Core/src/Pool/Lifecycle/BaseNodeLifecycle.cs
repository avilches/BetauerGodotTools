using Godot;

namespace Betauer.Core.Pool.Lifecycle;

public abstract partial class BaseNodeLifecycle : Node, IPoolLifecycle {
    protected BaseNodeLifecycle() {
        TreeEntered += () => _busy = true;
        TreeExited += () => _busy = false;
    }
    
    private volatile bool _busy = false;
    public bool IsBusy() => _busy;
    public bool IsInvalid() => !IsInstanceValid(this);
}