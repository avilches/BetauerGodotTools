using Veronenger.Config;

namespace Veronenger.Persistent.Node;


public abstract partial class NpcItemNode : ItemNode, INpcItemNode {

	public NpcItem.NpcStatus Status => Item.Status;
	public NpcConfig NpcConfig => Item.Config;

	protected override NpcItem Item => (NpcItem)base.Item;

	public abstract float DistanceToPlayer();
	public abstract bool CanBeAttacked(WeaponItem weapon);
}
