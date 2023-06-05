using Betauer.Application.Persistent;
using Veronenger.Config;
using Veronenger.Worlds;

namespace Veronenger.Platform.Persistent;

public abstract class PickableGameObject : GameObject<PickableItemNode> {
    public virtual PickableConfig Config { get; protected set; }
}