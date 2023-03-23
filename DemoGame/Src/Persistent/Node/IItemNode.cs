using Betauer.Application.Lifecycle;

namespace Veronenger.Persistent.Node;

public interface IItemNode : INodeLifecycle {
    void SetItem(Item item);
}