using System;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.FSM.Sync;
using Betauer.NodePath;
using Betauer.Physics;
using Godot;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Player;
using Veronenger.Game.Platform;

namespace Veronenger.Game.Items;

public partial class PickableItemNode : Node, IInjectable, INodeGameObject {
	public enum State {
		None, Dropping, Available, PickingUp, Finish
	}

	public enum Event {
		Spawn, PlayerDrops, PlayerPicksUp
	}

	[Inject] private PlayerConfig PlayerConfig { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

	public GameObject GameObject {
		get => _gameObject;
		set {
			_gameObject = value;
			if (value != null) Name = $"{_gameObject.Name}-{_gameObject.Id}";
		}
	}

	public PickableGameObject PickableGameObject => (PickableGameObject)GameObject;
	public Vector2 GlobalPosition => CharacterBody2D.GlobalPosition;
	public Vector2 Velocity => PlatformBody.Motion;

	[NodePath("Character")] public CharacterBody2D CharacterBody2D;
	[NodePath("Character/Sprite")] public Sprite2D Sprite;
	[NodePath("Character/PickZone")] public Area2D PickZone;

	public KinematicPlatformMotion PlatformBody;
	private float _delta;
	private readonly FsmSync<State, Event> _fsm = new(State.None, "PickableItem.FSM");
	private GameObject _gameObject;

	// State
	private State _state = State.Available;
	private Func<Vector2> _playerPosition;
	private Action? _onPickup;
	
	public void PostInject() {
		PlatformBody = new KinematicPlatformMotion(CharacterBody2D, MotionConfig.FloorUpDirection);
		ConfigureFsm();
	}

	public override void _Ready() {
		CollisionLayerConfig.PlayerPickableItem(this);
		PickZone.SetCollisionNode(this);
		_state = State.Available;
		_playerPosition = null;
		_onPickup = null;
		PickableGameObject.Config.ConfigurePickableSprite2D(Sprite);
		
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

	public override void _PhysicsProcess(double delta) {
		_delta = (float)delta;
		_fsm.Execute();
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
				FallAndBounce();
				placingTime -= _delta;
			})
			.If(() => placingTime <= 0f).Set(State.Available)
			.Build();
		

		_fsm.State(State.Available)
			.Enter(() => PickZone.Monitorable = true)
			.Execute(FallAndBounce)
			.Build();

		var pickingUpSpeed = 0f;
		_fsm.State(State.PickingUp)
			.Enter(() => pickingUpSpeed = PlayerConfig.PickingUpSpeed)
			.Execute(() => {
				var delta = _delta;
				var destination = _playerPosition();
				pickingUpSpeed *= 1 + (PlayerConfig.PickingUpAcceleration * delta);
				CharacterBody2D.GlobalPosition = CharacterBody2D.GlobalPosition.MoveToward(destination, delta * pickingUpSpeed);
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

	private void FallAndBounce() {
		PlatformBody.ApplyGravity(PlayerConfig.FloorGravity, PlayerConfig.MaxFallingSpeed, _delta);
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