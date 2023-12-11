using System;
using Godot;

namespace Betauer.Application.Lifecycle.Pool;

public class NodePool<T> : ManagedPool<T> where T : Node {
    public NodePool(Func<T> factory, int purgeIfBiggerThan = 0) : base(factory, purgeIfBiggerThan) {
    }

    protected override bool IsBusy(T node) => GodotObject.IsInstanceValid(node) && node.IsInsideTree();
    protected override bool IsInvalid(T element) => !GodotObject.IsInstanceValid(element);
}