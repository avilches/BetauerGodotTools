using System;
using Betauer.Application.Lifecycle;
using Betauer.DI;
using Godot;


namespace Veronenger.Persistent.Node;

public abstract partial class ItemNode : Godot.Node, ILinkableItem, INodePoolLifecycle {
    protected ItemNode() {
        TreeEntered += () => _busy = true;
        TreeExited += () => _busy = false;
    }

    
    
    
    
    
    
    
    [Inject] public ItemRepository ItemRepository { get; set; }
    
    protected virtual Item Item { get; set; }
    private volatile bool _busy = false;
    public bool IsBusy() => _busy;
    public bool IsInvalid() => !IsInstanceValid(this);

    // From INodeLifecycle, called by PoolFromNodeFactory
    public abstract void Initialize();
    
    // From IPoolLifecycle
    public abstract void OnGet();

    // IItemNode
    public void LinkItem(Item item) {
        Item = item;
    }

    public void RemoveFromWorld() {
        ItemRepository.Remove(Item);
    }

    public abstract Vector2 GlobalPosition { get; set; }
}