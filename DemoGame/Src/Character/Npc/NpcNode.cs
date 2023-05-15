using Betauer.Application.Persistent;
using Godot;
using Veronenger.Config;
using Veronenger.Persistent;

namespace Veronenger.Character.Npc;


public abstract partial class NpcNode : Node, INodeGameObject {

	public GameObject GameObject { get; set; }
	public NpcGameObject NpcGameObject => (NpcGameObject)GameObject;
	public NpcConfig NpcConfig => NpcGameObject.Config;

	public abstract float DistanceToPlayer();
	public abstract bool CanBeAttacked(WeaponGameObject weapon);
	
	public abstract Vector2 GlobalPosition { get; set; }
}
