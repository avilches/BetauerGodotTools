using System;
using Veronenger.Config;

namespace Veronenger.Persistent.Node;

public abstract partial class NpcItemNodeFsm<TStateKey, TEventKey> : 
	ItemNodeFsm<TStateKey, TEventKey>, INpcItemNode
	where TStateKey : Enum
	where TEventKey : Enum {
	protected NpcItemNodeFsm(TStateKey initialState, string? name = null, bool processInPhysics = false) :
		base(initialState, name, processInPhysics) {
	}

	public NpcItem.NpcStatus Status => NpcItem.Status;
	public NpcConfig NpcConfig => NpcItem.Config;

	protected NpcItem NpcItem => (NpcItem)Item;

	public abstract float DistanceToPlayer();
	public abstract bool CanBeAttacked(WeaponItem weapon);
}
