using Veronenger.Config;
using Veronenger.Worlds;

namespace Veronenger.Persistent;

public abstract class PickableItem : Item<PickableItemNode> {
    public virtual PickableConfig Config { get; protected set; }
}