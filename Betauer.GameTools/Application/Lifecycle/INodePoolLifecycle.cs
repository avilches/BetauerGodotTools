using Betauer.Core.Pool.Lifecycle;

namespace Betauer.Application.Lifecycle;

public interface INodePoolLifecycle : IPoolLifecycle {
    // Only needed in Node because packedScene.Instantiate() calls to the default constructor, then it adds the children,
    // so the constructor doesn't have children yet. Having a different method to initialize the class gives to Godot
    // the time needed to instantiate the children and add them to the node. So the user can put the code in this method
    // and be sure the children are filled.
    void Initialize();
}