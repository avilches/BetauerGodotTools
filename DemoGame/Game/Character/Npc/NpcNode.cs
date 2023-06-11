using System;
using Betauer.Application.Persistent;
using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game.Config;
using Veronenger.Game.Items;

namespace Veronenger.Game.Character.Npc;


public abstract partial class NpcNode : Node, INodeGameObject {
	[Inject] public Random Random { get; set; }

	private GameObject _gameObject;

	public GameObject GameObject {
		get => _gameObject;
		set {
			_gameObject = value;
			if (value != null) Name = $"{_gameObject.Name}-{_gameObject.Id}";
		}
	}

	public NpcGameObject NpcGameObject => (NpcGameObject)GameObject;
	public NpcConfig NpcConfig => NpcGameObject.Config;

	public abstract float DistanceToPlayer();
	public abstract bool CanBeAttacked(WeaponGameObject weapon);
	
	public abstract Vector2 GlobalPosition { get; set; }
	public abstract Vector2 Velocity { get; set; }
	public abstract bool IsFacingRight { get; set; }
}
