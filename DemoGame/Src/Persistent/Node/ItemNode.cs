using System;
using Betauer.DI;
using Godot;

namespace Veronenger.Persistent.Node;

public abstract partial class ItemNode : Godot.Node, IItemNode {

    
    
    
    
    
    
    
    [Inject] public ItemRepository ItemRepository { get; set; }
    
    protected Item Item;
    private volatile bool _busy = false;
    public bool IsBusy() => _busy;
    public bool IsInvalid() => !IsInstanceValid(this);

    // From INodeLifecycle, called by PoolFromNodeFactory
    public abstract void Initialize();
    
    // From IPoolLifecycle
    public abstract void OnGet();

    // IItemNode
    public void SetItem(Item item) {
        Item = item;
    }

    public void AddToScene(Godot.Node parent, Action? onReady) {
        _busy = true;
        if (onReady != null) {
            RequestReady();
            Connect(Godot.Node.SignalName.Ready, Callable.From(onReady), (uint)ConnectFlags.OneShot);
        }
        parent.AddChild(this);
    }

    
    public void RemoveFromWorld() {
        ItemRepository.Remove(Item);
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