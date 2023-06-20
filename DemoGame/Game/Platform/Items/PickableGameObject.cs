using Betauer.Application.Persistent;
using Veronenger.Game.Platform.Items.Config;

namespace Veronenger.Game.Platform.Items;

public abstract class PickableGameObject : GameObject<PickableItemNode> {
    public virtual PickableConfig Config { get; protected set; }
}