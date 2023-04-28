using Betauer.Core.Pool.Lifecycle;
using Veronenger.Config;
using Veronenger.Persistent;

namespace Veronenger.Character.Npc;


public abstract partial class NpcItemNode : Godot.Node, IPoolLifecycle, INodeWithItem {

	public bool IsBusy() => IsInsideTree();
    public bool IsInvalid() => !IsInstanceValid(this);
    
	public NpcItem.NpcStatus Status => NpcItem.Status;
	public NpcConfig NpcConfig => NpcItem.Config;

	public Item Item { get; set; }
	protected NpcItem NpcItem => (NpcItem)Item;

	public abstract float DistanceToPlayer();
	public abstract bool CanBeAttacked(WeaponItem weapon);
}
