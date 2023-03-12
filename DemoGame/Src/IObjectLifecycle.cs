using Betauer.Core.Pool;

namespace Veronenger;

public interface IObjectLifecycle : IPoolElement {
    void Initialize();
    public void RemoveFromScene();
}