using System;
using Betauer.Core.Pool.Lifecycle;
using Godot;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.Flipper;
using Betauer.NodePath;
using Veronenger.Character;
using Veronenger.Config;
using Veronenger.Managers;
using Veronenger.Persistent;
using Veronenger.Persistent.Node;

namespace Veronenger.Worlds;

public partial class PickableItemNode : ItemNode, IPickableItemNode, IPoolLifecycle {
	public enum State {
		Available, PickingUp, Finish
	}
	
	public const float Speed = 300f; // 300px per second
	public const float Acceleration = 2f; // 2x per second

	[Inject] private PlayerConfig PlayerConfig { get; set; }

	protected PickableItem PickableItem => (PickableItem)Item;

	[NodePath("Character")] public CharacterBody2D CharacterBody2D { get; private set; }
	[NodePath("Character/Sprite")] public Sprite2D Sprite { get; private set; }
	[NodePath("Character/PickZone")] public Area2D PickZone { get; private set; }

	private KinematicPlatformMotion _platformBody;
	private State _state = State.Available;
	private Func<Vector2>? _followPosition;
	private Action? _onPickup;

	public override void PostInject() {
		CollisionLayerManager.PickableItem(this);
		var marker2d = new Marker2D();
		AddChild(marker2d);
		_platformBody = new KinematicPlatformMotion(CharacterBody2D, new FlipperList(), marker2d, Vector2.Up);
	}

	public override void OnGet() {}

	public override void _Ready() {
		PickZone.LinkMetaToItemId(Item);
		_state = State.Available;
		_followPosition = null;
		_onPickup = null;
		PickableItem?.Config?.Initialize?.Invoke(this);
	}

	public override Vector2 GlobalPosition {
		get => CharacterBody2D.GlobalPosition;
		set => CharacterBody2D.GlobalPosition = value;
	}

	public void BringTo(Func<Vector2> target, Action onPickup) {
		_followPosition = target;
		_onPickup = onPickup;
		_state = State.PickingUp;
		_currentSpeed = Speed;
	}

	public override void _PhysicsProcess(double delta) {
		if (_state == State.Available) _PhysicsProcessAvailable(delta);
		else if (_state == State.PickingUp) _PhysicsProcessPickingUp(delta);
	}
	
	private float _currentSpeed = 0f;
	public void _PhysicsProcessPickingUp(double dDelta) {
		var delta = (float)dDelta;
		var destination = _followPosition!();
		_currentSpeed *= 1 + Acceleration * delta;
		CharacterBody2D.GlobalPosition = CharacterBody2D.GlobalPosition.MoveToward(destination, delta * _currentSpeed);
		if (CharacterBody2D.GlobalPosition.DistanceTo(destination) < 10) {
			_state = State.Finish;
			_onPickup?.Invoke();
		}
	}

	public void _PhysicsProcessAvailable(double delta) {
		_platformBody.SetDelta(delta);
		_platformBody.ApplyGravity(PlayerConfig.FloorGravity, PlayerConfig.MaxFallingSpeed);

		if (Input.IsActionJustPressed("ui_accept") && _platformBody.IsOnFloor()) {
			_platformBody.MotionY = -PlayerConfig.JumpSpeed;
			_platformBody.MotionX = 100;
		}

		if (_platformBody.IsOnFloor()) {
			_platformBody.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
		} else {
			_platformBody.Move();
		}
	}
}
