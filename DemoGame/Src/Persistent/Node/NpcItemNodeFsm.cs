using System;
using Betauer.Core.Pool.Lifecycle;
using Veronenger.Config;

namespace Veronenger.Persistent.Node;


public abstract partial class NpcItemNodeFsm<TStateKey, TEventKey> : 
	ItemNodeFsm<TStateKey, TEventKey>, INpcItemNode, IPoolLifecycle
	where TStateKey : Enum
	where TEventKey : Enum {
	protected NpcItemNodeFsm(TStateKey initialState, string? name = null, bool processInPhysics = false) :
		base(initialState, name, processInPhysics) {
	}

	public NpcItem.NpcStatus Status => Item.Status;
	public NpcConfig NpcConfig => Item.Config;

	protected override NpcItem Item => (NpcItem)base.Item;

	public abstract float DistanceToPlayer();
	public abstract bool CanBeAttacked(WeaponItem weapon);
}
