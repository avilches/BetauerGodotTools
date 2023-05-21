using Betauer.DI.Factory;
using Godot;

namespace Betauer.Application.Lifecycle.Pool;

public class NodePool<T> : ManagedPool<T> where T : Node {
    public NodePool(int purgeIfBiggerThan = 0) : base(purgeIfBiggerThan) {
    }

    public NodePool(ITransient<T> factory, int purgeIfBiggerThan = 0) : base(factory, purgeIfBiggerThan) {
    }

    public NodePool(string? factoryName, int purgeIfBiggerThan = 0) : base(factoryName, purgeIfBiggerThan) {
    }

    protected override bool IsBusy(T node) => GodotObject.IsInstanceValid(node) && node.IsInsideTree();
    protected override bool IsInvalid(T element) => !GodotObject.IsInstanceValid(element);
}