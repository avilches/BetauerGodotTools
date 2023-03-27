using Betauer.Core.Pool.Lifecycle;
using Betauer.DI;
using Godot;

namespace Veronenger.Transient;

public abstract partial class BaseNodeLifecycle : Node, IPoolLifecycle, IInjectable {
    protected BaseNodeLifecycle() {
        TreeEntered += () => _busy = true;
        TreeExited += () => _busy = false;
    }
    
    private volatile bool _busy = false;
    public bool IsBusy() => _busy;
    public bool IsInvalid() => !IsInstanceValid(this);

    public abstract void PostInject();

    // From IPoolLifecycle
    public abstract void OnGet();

    public abstract Vector2 GlobalPosition { get; set; }

}