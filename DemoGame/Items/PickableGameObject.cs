using Betauer.Application.Persistent;
using Veronenger.Main.Config;
using Veronenger.Worlds;

namespace Veronenger.Persistent;

public abstract class PickableGameObject : GameObject<PickableItemNode> {
    public virtual PickableConfig Config { get; protected set; }
}