using Betauer.Core.Pool.Lifecycle;
using Betauer.DI;
using Godot;

namespace Veronenger.Persistent.Node;

public abstract partial class ItemNode : Godot.Node, ILinkableItem, IPoolLifecycle, IInjectable {
    protected ItemNode() {
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

    [Inject] public ItemRepository ItemRepository { get; set; }
    protected virtual Item Item { get; set; }
    
    // IItemNode
    public void LinkItem(Item item) {
        Item = item;
    }

    public void RemoveFromWorld() {
        ItemRepository.Remove(Item);
    }
}