using System;
using Betauer.Application.Monitor;
using Betauer.Core.Pool.Lifecycle;
using Godot;
using Betauer.DI.Attributes;
using Betauer.Flipper;
using Betauer.FSM.Sync;
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
		None, Dropping, Available, PickingUp, Finish
	}

	public enum Event {
		Spawn, PlayerDrops, PlayerPicksUp
	}

	[Inject] private PlayerConfig PlayerConfig { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

	protected PickableItem PickableItem => (PickableItem)Item;

	[NodePath("Character")] public CharacterBody2D CharacterBody2D;
	[NodePath("Character/Sprite")] public Sprite2D Sprite;
	[NodePath("Character/PickZone")] public Area2D PickZone;

	public KinematicPlatformMotion PlatformBody;
	private State _state = State.Available;
	private Func<Vector2> _playerPosition;
	private Action? _onPickup;
	private float _pickingUpSpeed;
	private readonly FsmNodeSync<State, Event> _fsm = new(State.None, "PickableItem.FSM", true);

	public override void PostInject() {
		PlatformBody = new KinematicPlatformMotion(CharacterBody2D, new FlipperList(), CharacterBody2D, Vector2.Up);
		CollisionLayerManager.PickableItem(this);
		AddChild(_fsm);
		ConfigureFsm();
	}

	public override void OnGet() {}

	public override void _Ready() {
		PickZone.LinkMetaToItemId(Item);
		_state = State.Available;
		_playerPosition = null;
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

	public void Spawn(Vector2 position, Vector2? velocity = null) {
		CharacterBody2D.GlobalPosition = position;
		PlatformBody.Motion = velocity ?? Vector2.Zero;
		_fsm.Send(Event.Spawn);
	}

	public void PlayerDrop(Vector2 position, Vector2? velocity = null) {
		CharacterBody2D.GlobalPosition = position;
		PlatformBody.Motion = velocity ?? Vector2.Zero;
		_fsm.Send(Event.PlayerDrops);
	}

	public void PlayerPicksUp(Func<Vector2> playerPosition, Action onPickup) {
		_playerPosition = playerPosition;
		_onPickup = onPickup;
		_fsm.Send(Event.PlayerPicksUp);
	}

	private void ConfigureFsm() {
		var placingTime = 0d;

		_fsm.On(Event.Spawn).Set(State.Available);
		_fsm.On(Event.PlayerDrops).Set(State.Dropping);
		_fsm.On(Event.PlayerPicksUp).Set(State.PickingUp);

		_fsm.State(State.None).Build();
		_fsm.State(State.Dropping)
			.Enter(() => {
				placingTime = PlayerConfig.DroppingTime;
				PickZone.Monitorable = false;
			})
			.Execute(() => {
				FallAndBounce(_fsm.Delta);
				placingTime -= _fsm.Delta;
			})
			.If(() => placingTime <= 0f).Set(State.Available)
			.Build();
		

		_fsm.State(State.Available)
			.Enter(() => PickZone.Monitorable = true)
			.Execute(() => FallAndBounce(_fsm.Delta))
			.Build();

		_fsm.State(State.PickingUp)
			.Enter(() => _pickingUpSpeed = PlayerConfig.PickingUpSpeed)
			.Execute(() => {
				var delta = (float)_fsm.Delta;
				var destination = _playerPosition();
				_pickingUpSpeed *= 1 + (PlayerConfig.PickingUpAcceleration * delta);
				CharacterBody2D.GlobalPosition = CharacterBody2D.GlobalPosition.MoveToward(destination, delta * _pickingUpSpeed);
			})
			.If(() => CharacterBody2D.GlobalPosition.DistanceTo(_playerPosition()) < PlayerConfig.PickupDoneDistance).Set(State.Finish)
			.Build();

		_fsm.State(State.Finish)
			.Enter(() => {
				PickZone.Monitorable = false;
				_onPickup?.Invoke();
			})
			.If(() => true).Set(State.None)
			.Build();

		// This ensure the FSM is initialized and can receive events in the _Ready stage
		_fsm.Execute();
	}

	private void FallAndBounce(double delta) {
		PlatformBody.SetDelta(delta);
		PlatformBody.ApplyGravity(PlayerConfig.FloorGravity, PlayerConfig.MaxFallingSpeed);
		if (PlatformBody.IsOnFloor()) {
			PlatformBody.ApplyLateralFriction(PlayerConfig.DropLateralFriction, PlayerConfig.StopIfSpeedIsLessThan);
		}
		var motionX = PlatformBody.MotionX;
		PlatformBody.Move();
		if (PlatformBody.IsJustOnWall()) {
			PlatformBody.MotionX = -motionX;
		}
	}

}
