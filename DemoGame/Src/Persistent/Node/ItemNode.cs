using Betauer.Core.Pool.Lifecycle;
using Betauer.DI;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Persistent.Node;

public abstract partial class ItemNode : Godot.Node, ILinkableItem, IPoolLifecycle {
    public bool IsBusy() => IsInsideTree();
    public bool IsInvalid() => !IsInstanceValid(this);

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