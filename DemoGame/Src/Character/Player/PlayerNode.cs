using System;
using System.Collections.Generic;
using System.Linq;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Application.Monitor;
using Betauer.Camera;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Restorer;
using Betauer.Core.Time;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Betauer.StateMachine.Sync;
using Godot;
using Veronenger.Character.Handler;
using Veronenger.Managers;

namespace Veronenger.Character.Player; 

public readonly struct Attack {
	public readonly float Damage;

	public Attack(float damage) {
		Damage = damage;
	}
}

public enum PlayerState {
	Idle,
	Landing,
	Run,
	FallShort,
	FallLong,
	Jump,
	Death,
			
	Float,
}

public enum PlayerEvent {
	Death
}
	
public partial class PlayerNode : StateMachineNodeSync<PlayerState, PlayerEvent> {
	public PlayerNode() : base(PlayerState.Idle, "Player.StateMachine", true) {
	}
	
	[OnReady("PlayerBody/Weapon")] private Node2D _weaponNode;
	[OnReady("PlayerBody/Sprite2D")] private Sprite2D _mainSprite;
	[OnReady("PlayerBody/AttackArea")] private Area2D _attackArea;
	[OnReady("PlayerBody/DamageArea")] private Area2D _damageArea;
	[OnReady("PlayerBody/RichTextLabel")] public RichTextLabel Label;
	[OnReady("PlayerBody/Detector")] public Area2D PlayerDetector;
	[OnReady("PlayerBody/Sprite2D/AnimationPlayer")] private AnimationPlayer _animationPlayer;
	[OnReady("PlayerBody/Camera2D")] private Camera2D _camera2D;

	[OnReady("PlayerBody/Marker2D")] public Marker2D Marker2D;
	[OnReady("PlayerBody/CanJump")] public RayCast2D RaycastCanJump;
	[OnReady("PlayerBody/FloorRaycasts")] public List<RayCast2D> FloorRaycasts;

	[Inject] private PlatformManager PlatformManager { get; set; }
	[Inject] private CharacterManager CharacterManager { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private StageManager StageManager { get; set; }
	[Inject] private InputAction MMB { get; set; }

	[Inject] public PlayerConfig PlayerConfig { get; set;}
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private Bus Bus { get; set; }
	[Inject] private InputActionCharacterHandler Handler { get; set; }

	private float XInput => Handler.Directional.XInput;
	private float YInput => Handler.Directional.YInput;
	private bool IsPressingRight => Handler.Directional.IsPressingRight;
	private bool IsPressingLeft => Handler.Directional.IsPressingLeft;
	private bool IsPressingUp => Handler.Directional.IsPressingUp;
	private bool IsPressingDown => Handler.Directional.IsPressingDown;
	private IAction Jump => Handler.Jump;
	private IAction Attack => Handler.Attack;
	private IAction Float => Handler.Float;
		
	private float MotionX => PlatformBody.MotionX;
	private float MotionY => PlatformBody.MotionY;


	// private bool IsOnPlatform() => PlatformManager.IsPlatform(Body.GetFloor());
	private bool IsOnFallingPlatform() => PlatformBody.IsOnFloor() && PlatformManager.IsFallingPlatform(PlatformBody.GetFloorColliders<PhysicsBody2D>().FirstOrDefault());
	// private bool IsMovingPlatform() => PlatformManager.IsMovingPlatform(Body.GetFloor());
	private MonitorText? _coyoteMonitor;
	private MonitorText? _jumpHelperMonitor;
	private DelayedAction _delayedJump;
	private readonly GodotStopwatch _coyoteFallingTimer = new GodotStopwatch();

	public KinematicPlatformMotion PlatformBody;
		
	public ILoopStatus AnimationIdle { get; private set; }
	public ILoopStatus AnimationRun { get; private set; }
	public IOnceStatus AnimationRunStop { get; private set; }
	public IOnceStatus AnimationJump { get; private set; }
	public ILoopStatus AnimationFall { get; private set; }
	public IOnceStatus AnimationAttack { get; private set; }
	public IOnceStatus AnimationJumpAttack { get; private set; }

	public IOnceStatus PulsateTween;
	public ILoopStatus DangerTween;
	public IOnceStatus SqueezeTween;

	private readonly DragCameraController _cameraController = new();
	private AnimationStack _animationStack;
	private AnimationStack _tweenStack;

	public override void _Ready() {
		var characterBody2D = GetNode<CharacterBody2D>("PlayerBody");
		if (InitialPosition != null) characterBody2D.GlobalPosition = InitialPosition;
			
		CreateAnimations(characterBody2D);

		_delayedJump = ((InputAction)Jump).CreateDelayed();
		_cameraController.WithAction(MMB).Attach(_camera2D);

		var flipper = new FlipperList()
			.Sprite2DFlipH(_mainSprite)
			.ScaleX(_weaponNode)
			.ScaleX(_attackArea);
		flipper.IsFacingRight = true;

		PlatformBody = new KinematicPlatformMotion(characterBody2D, flipper, Marker2D, MotionConfig.FloorUpDirection, FloorRaycasts);

		_attackArea.EnableAllShapes(false);
		CharacterManager.RegisterPlayerNode(this);
		CharacterManager.PlayerConfigureCollisions(this);
		CharacterManager.PlayerConfigureAttackArea2D(_attackArea);

		PlayerDetector.CollisionLayer = 0;
		PlayerDetector.CollisionMask = 0;
		StageManager.ConfigureStageCamera(_camera2D, PlayerDetector);
			
		_attackArea.Monitoring = false;
		// CharacterManager.ConfigurePlayerDamageArea2D(_damageArea);

		characterBody2D.FloorStopOnSlope = true;
		// characterBody2D.FloorBlockOnWall = true;
		characterBody2D.FloorConstantSpeed = true;
		characterBody2D.FloorSnapLength = MotionConfig.SnapLength;

		OnBeforeExecute += PlatformBody.SetDelta;
		// OnTransition += args => Console.WriteLine(args.To);
		Bus.Subscribe(Enqueue);
		GroundStates();
		AirStates();

		var overlay = DebugOverlayManager.Overlay(characterBody2D)
			.Title("Player")
			.SetMaxSize(1000, 1000);

		AddOverlayHelpers(overlay);
		AddOverlayStates(overlay);
		AddOverlayMotion(overlay);
		AddOverlayCollisions(overlay);

		// DebugOverlayManager.Overlay(this)
		//     .Title("Player")
		//     .Text("AnimationStack",() => _animationStack.GetPlayingLoop()?.Name + " " + _animationStack.GetPlayingOnce()?.Name).EndMonitor()
		//     .Text("TweenStack", () => _tweenStack.GetPlayingLoop()?.Name + " " + _tweenStack.GetPlayingOnce()?.Name).EndMonitor()
		//     .Add(new HBoxContainer().NodeBuilder()
		//         .Button("DangerTween.PlayLoop", () => DangerTween.PlayLoop()).End()
		//         .Button("DangerTween.Stop", () => DangerTween.Stop()).End()
		//         .TypedNode)
		//     .Add(new HBoxContainer().NodeBuilder()
		//         .Button("PulsateTween.PlayOnce", () => PulsateTween.PlayOnce()).End()
		//         .Button("PulsateTween.Stop", () => PulsateTween.Stop()).End()
		//         .TypedNode)
		//     .Add(new HBoxContainer().NodeBuilder()
		//         .Button("SqueezeTween.PlayOnce(kill)", () => SqueezeTween.PlayOnce(true)).End()
		//         .Button("SqueezeTween.Stop", () => SqueezeTween.Stop()).End()
		//         .TypedNode);
	}

	private void CreateAnimations(CharacterBody2D characterBody2D) {
		var restorer = new MultiRestorer() 
			.Add(characterBody2D.CreateRestorer(Properties.Modulate, Properties.Scale2D))
			.Add(_mainSprite.CreateRestorer(Properties.Modulate, Properties.Scale2D));
		restorer.Save();
		
		_animationStack = new AnimationStack("Player.AnimationStack").SetAnimationPlayer(_animationPlayer);
		_tweenStack = new AnimationStack("Player.AnimationStack");

		AnimationIdle = _animationStack.AddLoopAnimation("Idle");
		AnimationRun = _animationStack.AddLoopAnimation("Run");
		AnimationRunStop = _animationStack.AddOnceAnimation("RunStop");
		AnimationJump = _animationStack.AddOnceAnimation("Jump");
		AnimationFall = _animationStack.AddLoopAnimation("Fall");
		AnimationAttack = _animationStack.AddOnceAnimation("Attack").OnStart(() => _attackArea.EnableAllShapes())
			.OnEnd(() => _attackArea.EnableAllShapes(false));
		AnimationJumpAttack = _animationStack.AddOnceAnimation("JumpAttack");

		PulsateTween = _tweenStack.AddOnceTween("Pulsate", CreateMoveLeft()).OnEnd(restorer.Restore);
		DangerTween = _tweenStack.AddLoopTween("Danger", CreateDanger()).OnEnd(restorer.Restore);
		SqueezeTween = _tweenStack.AddOnceTween("Squeeze", CreateSqueeze()).OnEnd(restorer.Restore);
	}

	private IAnimation CreateReset() {
		var seq = SequenceAnimation.Create(_mainSprite)
			.AnimateSteps(Properties.Modulate)
			.From(new Color(1, 1, 1, 0))
			.To(new Color(1, 1, 1, 1), 1)
			.EndAnimate();
		// seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 1), 0.1f);
		// seq.Parallel().AddProperty(this, "scale", new Vector2(1f, 1f), 0.1f);
		return seq;
	}

	private IAnimation CreateMoveLeft() {
		var seq = KeyframeAnimation.Create(_mainSprite)
			.SetDuration(2f)
			.AnimateKeys(Properties.Modulate)
			.KeyframeTo(0.25f, new Color(1, 1, 1, 0))
			.KeyframeTo(0.75f, new Color(1, 1, 1, 0.5f))
			.KeyframeTo(1f, new Color(1, 1, 1, 1))
			.EndAnimate()
			.AnimateKeys<Vector2>(Properties.Scale2D)
			.KeyframeTo(0.5f, new Vector2(1.4f, 1f))
			.KeyframeTo(1f, new Vector2(1f, 1f))
			.EndAnimate();
		// seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 0), 1f).SetTrans(Tween.TransitionType.Cubic);
		// seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 1), 1f).SetTrans(Tween.TransitionType.Cubic);
		return seq;
	}

	private IAnimation CreateDanger() {
		var seq = SequenceAnimation.Create(_mainSprite)
			.AnimateSteps<Color>(Properties.Modulate, Easings.CubicInOut)
			.To(new Color(1, 0, 0, 1), 1)
			.To(new Color(1, 1, 1, 1), 1)
			.EndAnimate();
		return seq;
	}

	private IAnimation CreateSqueeze() {
		var seq = SequenceAnimation.Create(this)
			.AnimateSteps(Properties.Scale2D, Easings.SineInOut)
			.To(new Vector2(1.4f, 1f), 0.25f)
			.To(new Vector2(1f, 1f), 0.25f)
			.EndAnimate()
			.SetLoops(2);
		return seq;
	}

	public override void _Input(InputEvent e) {
		if (e.IsLeftDoubleClick()) _camera2D.Position = Vector2.Zero;
		if (e.IsKeyPressed(Key.Q)) {
			// _camera2D.Zoom -= new Vector2(0.05f, 0.05f);
		} else if (e.IsKeyPressed(Key.W)) {
			// _camera2D.Zoom = new Vector2(1, 1);
		} else if (e.IsKeyPressed(Key.E)) {
			// _camera2D.Zoom += new Vector2(0.05f, 0.05f);
		}
	}

	public bool CanJump() => !RaycastCanJump.IsColliding(); 

	public override void _Process(double delta) {
		Label.Text = _animationStack.GetPlayingOnce() != null
			? _animationStack.GetPlayingOnce().Name
			: _animationStack.GetPlayingLoop().Name;
		// QueueRedraw();
	}

	// public override void _Draw() {
		// foreach (var floorRaycast in FloorRaycasts) floorRaycast.DrawRaycast(this, Colors.Red);
		// RaycastCanJump.DrawRaycast(this, Colors.Red);
	// }

	public bool IsAttacking => AnimationJumpAttack.Playing || AnimationAttack.Playing;
	public Vector2 InitialPosition { get; set; }

	public void AddOverlayHelpers(DebugOverlay overlay) {
		_jumpHelperMonitor = overlay.Text("JumpHelper");
		overlay.Text("CoyoteFallingTimer", () => _coyoteFallingTimer.ToString());
		_coyoteMonitor = overlay.Text("Coyote");
	}

	public void AddOverlayStates(DebugOverlay overlay) {
		overlay
			.OpenBox()
				.Text("State", () => CurrentState.Key.ToString()).EndMonitor()
			.CloseBox();
	}

	public void AddOverlayMotion(DebugOverlay overlay) {
		overlay
			.OpenBox()
				.Vector("Motion", () => PlatformBody.Motion, PlayerConfig.MaxSpeed).SetChartWidth(100).EndMonitor()
				.Graph("MotionX", () => PlatformBody.MotionX, -PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed).AddSeparator(0)
					.AddSerie("MotionY").Load(() => PlatformBody.MotionY).EndSerie()
				.EndMonitor()
			.CloseBox()
			.GraphSpeed("Speed", PlayerConfig.JumpSpeed * 2).EndMonitor();

	}
	
	public void AddOverlayCollisions(DebugOverlay overlay) {    
		overlay
			.Graph("Floor", () => PlatformBody.IsOnFloor()).Keep(10).SetChartHeight(10)
				.AddSerie("Slope").Load(() => PlatformBody.IsOnSlope()).EndSerie()
			.EndMonitor()
			.Text("Floor", () => PlatformBody.GetFloorCollisionInfo()).EndMonitor()
			.Text("Ceiling", () => PlatformBody.GetCeilingCollisionInfo()).EndMonitor()
			.Text("Wall", () => PlatformBody.GetWallCollisionInfo()).EndMonitor();
	}


	public void ApplyFloorGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.FloorGravity * factor, PlayerConfig.MaxFallingSpeed);
	}

	public void ApplyAirGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.AirGravity * factor, PlayerConfig.MaxFallingSpeed);
	}

	public override void _Notification(long what) {
		if (what == NotificationPredelete) {
			_delayedJump?.Dispose();
		}
	}

	public void GroundStates() {
		bool CheckGroundAttack() {
			if (!Attack.IsJustPressed()) return false;
			// Attack was pressed
				AnimationAttack.PlayOnce();
			return true;
		}

		PhysicsBody2D? fallingPlatform = null;
		void FallFromPlatform() {
			fallingPlatform = PlatformBody.GetFloorCollider<PhysicsBody2D>()!;
			PlatformManager.RemovePlatformCollision(fallingPlatform);
		}

		void FinishFallFromPlatform() {
			if (fallingPlatform != null) PlatformManager.ConfigurePlatformCollision(fallingPlatform);
		}

		On(PlayerEvent.Death).Then(ctx => ctx.Set(PlayerState.Death));

		var jumpJustInTime = false;
		State(PlayerState.Landing)
			.Enter(() => {
				FinishFallFromPlatform();
				_coyoteFallingTimer.Stop(); // not really needed, but less noise in the debug overlay
				jumpJustInTime = _delayedJump.WasPressed(PlayerConfig.JumpHelperTime);
			})
			.Execute(() => {
				if (jumpJustInTime && CanJump()) {
					_jumpHelperMonitor?.Show($"{_delayedJump.LastPressed} <= {PlayerConfig.JumpHelperTime.ToString()} Done!");
				} else {
					_jumpHelperMonitor?.Show($"{_delayedJump.LastPressed} > {PlayerConfig.JumpHelperTime.ToString()} TOO MUCH TIME");
				}
			})
			.If(() => jumpJustInTime).Set(PlayerState.Jump)
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => true).Set(PlayerState.Run)
			.Build();

		State(PlayerState.Idle)
			.Enter(() => {
				AnimationIdle.PlayLoop();
			})
			.Execute(() => {
				CheckGroundAttack();
				ApplyFloorGravity();
				PlatformBody.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
			})
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.FallShort)
			.If(() => Jump.IsJustPressed() && IsPressingDown && IsOnFallingPlatform()).Then(
				context => {
					FallFromPlatform();
					return context.Set(PlayerState.FallShort);
				})
			.If(() => Jump.IsJustPressed() && CanJump()).Set(PlayerState.Jump)
			.If(() => XInput != 0).Set(PlayerState.Run)
			.Build();

		State(PlayerState.Run)
			.Execute(() => {
				CheckGroundAttack();

				ApplyFloorGravity();
				if (IsAttacking) {
					PlatformBody.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
				} else {
					if (XInput == 0) {
						if (Math.Abs(MotionX) >= PlayerConfig.MaxSpeed * 0.95f) {
							AnimationIdle.PlayLoop();
							AnimationRunStop.PlayOnce();
						}
					} else {
						AnimationRun.PlayLoop();
						AnimationRunStop.Stop(true);
					}
					PlatformBody.Flip(XInput);
					PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.Friction, 
						PlayerConfig.StopIfSpeedIsLessThan, 0);
				}
			})
			.If(() => !PlatformBody.IsOnFloor()).Then( 
				context => {
					_coyoteFallingTimer.Restart();
					return context.Set(PlayerState.FallShort);
				})
			.If(() => Jump.IsJustPressed() && IsPressingDown && IsOnFallingPlatform()).Then(
				context => {
					FallFromPlatform();
					return context.Set(PlayerState.FallShort);
				})
			.If(() => Jump.IsJustPressed() && CanJump()).Set(PlayerState.Jump)
			.If(() => XInput == 0 && MotionX == 0).Set(PlayerState.Idle)
			.Build();

		State(PlayerState.Death)
			.Enter(() => {
				Console.WriteLine("MUERTO");
				Bus.Publish(MainEvent.EndGame);
			})
			.Build();

	}

	public void AirStates() {

		bool CheckAirAttack() {
			if (!Attack.IsJustPressed()) return false;
			// Attack was pressed
			AnimationJumpAttack.PlayOnce();
			return true;
		}

		bool CheckCoyoteJump() {
			if (!Jump.IsJustPressed()) return false;
			// Jump was pressed
			if (!_coyoteFallingTimer.IsRunning) return false;
				
			_coyoteFallingTimer.Stop();
			if (_coyoteFallingTimer.Elapsed <= PlayerConfig.CoyoteJumpTime) {
				_coyoteMonitor?.Show($"{_coyoteFallingTimer.Elapsed.ToString()} <= {PlayerConfig.CoyoteJumpTime.ToString()} Done!");
				return true;
			}
			_coyoteMonitor?.Show($"{_coyoteFallingTimer.Elapsed.ToString()} > {PlayerConfig.CoyoteJumpTime.ToString()} TOO LATE");
			return false;
		}

		State(PlayerState.Jump)
			.Enter(() => {
				PlatformBody.MotionY = -PlayerConfig.JumpSpeed;
				AnimationFall.PlayLoop();
				AnimationJump.PlayOnce();
			})
			.Execute(() => {
				CheckAirAttack();

				if (Jump.IsReleased() && MotionY < -PlayerConfig.JumpSpeedMin) {
					PlatformBody.MotionY = -PlayerConfig.JumpSpeedMin;
				}

				PlatformBody.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, 0);
			})
			.If(() => Float.IsPressed()).Set(PlayerState.Float)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.If(() => MotionY >= 0).Set(PlayerState.FallShort)
			.Build();
				

		State(PlayerState.FallShort)
			.Execute(() => {
				CheckAirAttack();

				PlatformBody.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, 0);
			})
			.If(() => Float.IsPressed()).Set(PlayerState.Float)
			.If(CheckCoyoteJump).Set(PlayerState.Jump)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.If(() => MotionY > PlayerConfig.StartFallingSpeed).Set(PlayerState.FallLong)
			.Build();
				
		State(PlayerState.FallLong)
			.Enter(() => {
				AnimationFall.PlayLoop();
			})
			.Execute(() => {
				CheckAirAttack();
				PlatformBody.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed,
					PlayerConfig.AirResistance, PlayerConfig.StopIfSpeedIsLessThan, 0);
			})
			.If(() => Float.IsPressed()).Set(PlayerState.Float)
			.If(CheckCoyoteJump).Set(PlayerState.Jump)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.Build();

		State(PlayerState.Float)
			.Enter(() => {
				PlatformBody.CharacterBody.MotionMode = CharacterBody2D.MotionModeEnum.Floating;
			})
			.Execute(() => {
				PlatformBody.AddSpeed(XInput, YInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed,
					PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan, 0);
				PlatformBody.Move();
			})
			.If(() => Float.IsPressed()).Set(PlayerState.FallShort)
			.Exit(() => {
				PlatformBody.CharacterBody.MotionMode = CharacterBody2D.MotionModeEnum.Grounded;
			})
			.Build();

	}
}
