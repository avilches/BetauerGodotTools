using Betauer.Core.Pool.Lifecycle;
using Godot;

namespace Betauer.Application.Lifecycle;

public interface INodeLifecycle : IPoolLifecycle {
    void Initialize();
    public void AddToScene(Node parent, Vector2 initialPosition);
    public void RemoveFromScene();
    public void Free();
    public void QueueFree();
}