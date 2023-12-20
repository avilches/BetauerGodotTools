using System;
using Betauer.Core.Nodes;
using Godot;

namespace Betauer.Core.Pool;

public interface INodePool : IPool {
    void FreeAll();
    void QueueFreeAll();
}

/// <summary>
/// A pool with some handy features for Godot Nodes.
/// - When a node is requested, it calls to node.RequestReady(), so you can be sure the _Ready() or Ready signal is called when you add the node to the tree.
/// - When a node is released, it makes sure the node is removed from the tree, so you can be sure the _ExitTree() or TreeExiting/TreeExited signals are called.
///
/// Finally, the pool has two methods to free all the nodes in the pool: FreeAll() and QueueFreeAll(). You can still use RemoveAll(Action<T> action) to
/// do something else with the nodes. 
/// </summary>
/// <typeparam name="T"></typeparam>
public class NodePool<T> : BasePool<T>, INodePool where T : Node {
    private readonly Func<T> _factory;

    public NodePool(Func<T> factory, PoolCollection<T>? pool = null) : base(pool) {
        _factory = factory;
    }

    protected override T ExecuteCreate() {
        return _factory.Invoke();
    }

    protected override void ExecuteOnGet(T node) {
        node.RequestReady();
    }

    protected override void ExecuteOnRelease(T node) {
        node.RemoveFromParent();
    }

    public void FreeAll() {
        RemoveAll(node => node.Free());
    }

    public void QueueFreeAll() {
        RemoveAll(node => node.QueueFree());
    }
}