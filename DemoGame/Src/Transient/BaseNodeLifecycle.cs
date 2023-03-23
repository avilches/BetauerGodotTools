using System;
using Betauer.Application.Lifecycle;
using Godot;

namespace Veronenger.Transient;

public abstract partial class BaseNodeLifecycle : Node, INodeLifecycle {

    
    
    
    
    
    
    
    
    
    
    private volatile bool _busy = false;
    public bool IsBusy() => _busy;
    public bool IsInvalid() => !IsInstanceValid(this);

    // From INodeLifecycle, called by PoolFromNodeFactory
    public abstract void Initialize();

    // From IPoolLifecycle
    public abstract void OnGet();
    
    
    
    
    
    
    public void AddToScene(Node parent, Action? onReady) {
        _busy = true;
        if (onReady != null) {
            RequestReady();
            Connect(Godot.Node.SignalName.Ready, Callable.From(onReady), (uint)ConnectFlags.OneShot);
        }
        parent.AddChild(this);
    }
    
    
    
    
    
    
    public abstract Vector2 GlobalPosition { get; set; }

    public void RemoveFromScene() {
        if (!_busy) return;
        GetParent().RemoveChild(this);
        OnRemoveFromScene();
        _busy = false;
    }

    public abstract void OnRemoveFromScene();

}