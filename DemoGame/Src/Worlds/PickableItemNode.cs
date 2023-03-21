using System;
using Godot;
using Betauer.DI;
using Betauer.Flipper;
using Betauer.NodePath;
using Veronenger.Character;
using Veronenger.Config;
using Veronenger.Managers;
using Veronenger.Persistent.Node;

namespace Veronenger.Worlds;

public partial class PickableItemNode : ItemNode {
	public enum State {
		Available, PickingUp
	}

	[Inject] private PlayerConfig PlayerConfig { get; set; }
	
	[NodePath("Character")] public CharacterBody2D CharacterBody2D;
	[NodePath("Character/PickZone")] public Area2D PickZone { get; private set; }

	private KinematicPlatformMotion _platformBody;
	private State _state = State.Available;
	private Func<Vector2>? _followPosition;

	public override void Initialize() {
		CollisionLayerManager.PickableItem(this);
		var marker2d = new Marker2D();
		AddChild(marker2d);
		_platformBody = new KinematicPlatformMotion(CharacterBody2D, new FlipperList(), marker2d, Vector2.Up);
	}

	public override void OnGet() {}

	protected override void OnStart(Vector2 initialPosition) {
		CharacterBody2D.GlobalPosition = initialPosition;
		_state = State.Available;
		_followPosition = null;
	}

	public override void OnRemoveFromScene() {
	}

	public void BringTo(Func<Vector2> target) {
		_followPosition = target;
		_state = State.PickingUp;
	}

	public override void _PhysicsProcess(double delta) {
		if (_state == State.Available) _PhysicsProcessAvailable(delta);
		else if (_state == State.PickingUp) _PhysicsProcessPickingUp(delta);
	}

	public void _PhysicsProcessPickingUp(double delta) {
		var destination = _followPosition!();
		CharacterBody2D.GlobalPosition = CharacterBody2D.GlobalPosition.MoveToward(destination, (float)delta * PlayerConfig.MaxSpeed);
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
