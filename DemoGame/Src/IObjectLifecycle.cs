using Betauer.Core.Pool;
using Godot;

namespace Veronenger;

public interface IObjectLifecycle : IPoolElement {
    void Initialize();
    public void AddToScene(Node parent, Vector2 initialPosition);
    public void RemoveFromScene();
}