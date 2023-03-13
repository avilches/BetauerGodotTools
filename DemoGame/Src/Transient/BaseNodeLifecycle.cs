using Betauer.Application.Lifecycle;
using Godot;

namespace Veronenger.Transient;

public abstract partial class BaseNodeLifecycle : Node, INodeLifecycle {

    private Vector2 _initialPosition;
    private volatile bool _busy = false;
    public bool IsBusy() => _busy;
    public bool IsInvalid() => !IsInstanceValid(this);

    // From MiniPool
    public abstract void OnGet();
    public abstract void Initialize();
	
    public void AddToScene(Node parent, Vector2 initialPosition) {
        _busy = true;
        _initialPosition = initialPosition;
        RequestReady();
        parent.AddChild(this);
    }

    public override void _Ready() {
        OnStart(_initialPosition);
    }

    protected abstract void OnStart(Vector2 initialPosition);

    public void RemoveFromScene() {
        if (!_busy) return;
        GetParent().RemoveChild(this);
        OnRemoveFromScene();
        _busy = false;
    }

    public abstract void OnRemoveFromScene();

}