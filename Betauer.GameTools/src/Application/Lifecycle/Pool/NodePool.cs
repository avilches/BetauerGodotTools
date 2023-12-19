using System;
using Betauer.Core.Nodes;
using Godot;

namespace Betauer.Application.Lifecycle.Pool;

public class NodePool<T> : ManagedPool<T> where T : Node {
    public NodePool(Func<T> factory, int purgeIfBiggerThan = 0) : base(factory, purgeIfBiggerThan) {
    }
    
    protected override T OnGet(T element) {
        element.SetMeta(NodePoolExtensions.NodeBusyKey, NodePoolExtensions.NodeBusyValue);
        return element;
    }

    protected override bool IsBusy(T node) => node.HasMeta(NodePoolExtensions.NodeBusyKey);
    protected override bool IsInvalid(T element) => !GodotObject.IsInstanceValid(element);
}

public static class NodePoolExtensions {
    
    public static readonly StringName NodeBusyKey = new("__pool_busy");
    public static readonly Variant NodeBusyValue = Variant.From(true);
    
    public static void Release(this Node node) {
        if (!GodotObject.IsInstanceValid(node)) return;
        node.RemoveFromParent();
        node.RequestReady(); // next time the node is added to the tree, it will trigger the _Ready method and Ready signal
        if (node.HasMeta(NodeBusyKey)) node.RemoveMeta(NodeBusyKey);
    }
}