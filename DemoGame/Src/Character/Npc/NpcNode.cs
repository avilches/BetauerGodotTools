using Betauer.Application.Persistent;
using Godot;
using Veronenger.Config;
using Veronenger.Persistent;

namespace Veronenger.Character.Npc;


public abstract partial class NpcNode : Node, INodeWithGameObject {

	public NpcGameObject.NpcStatus Status => NpcGameObject.Status;
	public NpcConfig NpcConfig => NpcGameObject.Config;

	public GameObject GameObject { get; set; }
	protected NpcGameObject NpcGameObject => (NpcGameObject)GameObject;

	public abstract float DistanceToPlayer();
	public abstract bool CanBeAttacked(WeaponGameObject weapon);
	
	public abstract Vector2 GlobalPosition { get; set; }
}
