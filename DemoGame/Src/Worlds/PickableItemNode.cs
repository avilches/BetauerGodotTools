using System;
using Godot;
using Betauer.DI;
using Betauer.Flipper;
using Betauer.NodePath;
using Veronenger.Character;
using Veronenger.Config;
using Veronenger.Managers;
using Veronenger.Persistent;
using Veronenger.Persistent.Node;

namespace Veronenger.Worlds;

public partial class PickableItemNode : ItemNode, IPickableItemNode {
	public enum State {
		Available, PickingUp, Finish
	}

	[Inject] private PlayerConfig PlayerConfig { get; set; }
	
	[NodePath("Character")] public CharacterBody2D CharacterBody2D;
	[NodePath("Character/PickZone")] public Area2D PickZone { get; private set; }

	private KinematicPlatformMotion _platformBody;
	private State _state = State.Available;
	private Func<Vector2>? _followPosition;
	private Action? _onPickup;

	public override void Initialize() {
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
	}

	public override Vector2 GlobalPosition {
		get => CharacterBody2D.GlobalPosition;
		set => CharacterBody2D.GlobalPosition = value;
	}

	public override void OnRemoveFromScene() {
	}

	public void BringTo(Func<Vector2> target, Action onPickup) {
		_followPosition = target;
		_onPickup = onPickup;
		_state = State.PickingUp;
	}

	public override void _PhysicsProcess(double delta) {
		if (_state == State.Available) _PhysicsProcessAvailable(delta);
		else if (_state == State.PickingUp) _PhysicsProcessPickingUp(delta);
	}

	public void _PhysicsProcessPickingUp(double delta) {
		var destination = _followPosition!();
		CharacterBody2D.GlobalPosition = CharacterBody2D.GlobalPosition.MoveToward(destination, (float)delta * PlayerConfig.MaxSpeed);
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
