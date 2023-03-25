using Betauer.Core.Nodes;
using Veronenger.Persistent.Node;

namespace Veronenger.Persistent;

public abstract class Item {
    public int Id { get; internal set; }
    public string Name { get; internal set; }
    public string? Alias { get; internal set; }

    public abstract IItemNode? GetItemNode();
    public abstract void UnlinkNode();
}

public abstract class Item<T> : Item
    where T : class, IItemNode {
    public T? ItemNode { get; internal set; }

    public void LinkNode<TN>(TN npcItemNode) where TN : Godot.Node, T {
        ItemNode = npcItemNode;
        npcItemNode.SetItem(this);
    }

    public override IItemNode? GetItemNode() => ItemNode;

    public override void UnlinkNode() {
        (ItemNode as Godot.Node)!.RemoveFromParent();
        ItemNode = null;
    }
}