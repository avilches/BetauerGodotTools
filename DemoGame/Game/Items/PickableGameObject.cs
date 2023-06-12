using Betauer.Application.Persistent;
using Veronenger.Game.Items.Config;

namespace Veronenger.Game.Items;

public abstract class PickableGameObject : GameObject<PickableItemNode> {
    public virtual PickableConfig Config { get; protected set; }
}