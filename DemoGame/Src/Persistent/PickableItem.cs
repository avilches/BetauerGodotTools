using Veronenger.Config;
using Veronenger.Persistent.Node;

namespace Veronenger.Persistent;

public abstract class PickableItem : Item<IPickableItemNode> {
    public virtual PickableConfig Config { get; protected set; }
}