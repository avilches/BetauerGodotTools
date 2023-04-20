using System;
using Betauer.Application.Monitor;
using Betauer.Core.Pool.Lifecycle;
using Godot;
using Betauer.DI.Attributes;
using Betauer.Flipper;
using Betauer.NodePath;
using Veronenger.Character;
using Veronenger.Config;
using Veronenger.Managers;
using Veronenger.Persistent;
using Veronenger.Persistent.Node;
using Vector2 = Godot.Vector2;

namespace Veronenger.Worlds;

public partial class PickableItemNode : ItemNode, IPickableItemNode, IPoolLifecycle {
	public enum State {
		Dropping, Available, PickingUp, Finish
	}
	
	public const float PickingUpSpeed = 300f; // 300px per second
	public const float PickingUpAcceleration = 1.1f; // 2x per second
	public static float DropLateralFriction = 0.95f;

	[Inject] private PlayerConfig PlayerConfig { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

	protected PickableItem PickableItem => (PickableItem)Item;

	[NodePath("Character")] public CharacterBody2D CharacterBody2D;
	[NodePath("Character/Sprite")] public Sprite2D Sprite;
	[NodePath("Character/PickZone")] public Area2D PickZone;

	public KinematicPlatformMotion PlatformBody;
	private State _state = State.Available;
	private Func<Vector2>? _followPosition;
	private Action? _onPickup;
	private float _pickingUpSpeed;
	private double _placingTime;

	public override void PostInject() {
		CollisionLayerManager.PickableItem(this);
		var marker2d = new Marker2D();
		AddChild(marker2d);
		PlatformBody = new KinematicPlatformMotion(CharacterBody2D, new FlipperList(), marker2d, Vector2.Up);
	}

	public override void OnGet() {}

	public override void _Ready() {
		PickZone.LinkMetaToItemId(Item);
		_state = State.Available;
		_followPosition = null;
		_onPickup = null;
		PickableItem.Config.ConfigurePickableSprite2D?.Invoke(Sprite);
		
		// var overlay = DebugOverlayManager.Overlay(CharacterBody2D).Title(Item.Name);
		// AddOverlayMotion(overlay);
		// overlay
		// 	.OpenBox()
		// 	.Graph("Floor", () => PlatformBody.IsOnFloor()).Keep(10).SetChartHeight(10)
		// 	.AddSerie("Slope").Load(() => PlatformBody.IsOnSlope()).EndSerie()
		// 	.EndMonitor()
		// 	.CloseBox()
		// 	.OpenBox()
		// 	.Text("State", () => _state.ToString()).EndMonitor()
		// 	.Text("Pick speed", () => _pickingUpSpeed.ToString("0.000")).EndMonitor()
		// 	.CloseBox();
		// overlay.Scale = new Vector2(0.5f, 0.5f);

	}

	public void AddOverlayMotion(DebugOverlay overlay) {    
		overlay
			.OpenBox()
			.Vector("Motion", () => PlatformBody.Motion, PlayerConfig.MaxSpeed).SetChartWidth(100).EndMonitor()
			.Graph("MotionX", () => PlatformBody.MotionX, -PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed).AddSeparator(0)
			.AddSerie("MotionY").Load(() => PlatformBody.MotionY).EndSerie().EndMonitor()
			.CloseBox()
			.GraphSpeed("Speed", PlayerConfig.JumpSpeed * 2).EndMonitor();
	}

	public override Vector2 GlobalPosition {
		get => CharacterBody2D.GlobalPosition;
		set => CharacterBody2D.GlobalPosition = value;
	}
	
	public void Placing(Vector2 position, Vector2? velocity = null) {
		_state = State.Available;
		_placingTime = 0;
		CharacterBody2D.GlobalPosition = position;
		PlatformBody.Motion = velocity ?? Vector2.Zero;
		PickZone.Monitorable = true;
	}

	public void PlayerDrop(Vector2 position, Vector2? velocity = null) {
		_state = State.Dropping;
		_placingTime = PlayerConfig.DroppingTime;
		CharacterBody2D.GlobalPosition = position;
		PlatformBody.Motion = velocity ?? Vector2.Zero;
		PickZone.Monitorable = false;
	}

	public void FlyingPickup(Func<Vector2> target, Action onPickup) {
		_followPosition = target;
		_onPickup = onPickup;
		_state = State.PickingUp;
		_pickingUpSpeed = PickingUpSpeed;
	}

	public override void _PhysicsProcess(double delta) {
		if (_state == State.Dropping) _PlayerDropping(delta);
		if (_state == State.Available) _Available(delta);
		else if (_state == State.PickingUp) _PickingUp(delta);
	}

	private void _PlayerDropping(double delta) {
		_Available(delta);
		_placingTime -= delta;
		if (_placingTime <= 0f) {
			PickZone.Monitorable = true;
			_state = State.Available;
		}
	}

	private void _Available(double delta) {
		PlatformBody.SetDelta(delta);
		PlatformBody.ApplyGravity(PlayerConfig.FloorGravity, PlayerConfig.MaxFallingSpeed);
		if (PlatformBody.IsOnFloor()) {
			PlatformBody.ApplyLateralFriction(DropLateralFriction, 10f);
		}
		var backup = PlatformBody.MotionX;
		PlatformBody.Move();
		if (PlatformBody.IsJustOnWall()) {
			PlatformBody.MotionX = -backup;
		}
	}

	private void _PickingUp(double dDelta) {
		var delta = (float)dDelta;
		var destination = _followPosition!();
		_pickingUpSpeed *= 1 + (PickingUpAcceleration * delta);
		CharacterBody2D.GlobalPosition = CharacterBody2D.GlobalPosition.MoveToward(destination, delta * _pickingUpSpeed);
		if (CharacterBody2D.GlobalPosition.DistanceTo(destination) < 10) {
			_state = State.Finish;
			PickZone.Monitorable = false;
			_onPickup?.Invoke();
		}
	}
}
