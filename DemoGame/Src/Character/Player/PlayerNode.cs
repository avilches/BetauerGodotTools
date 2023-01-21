using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Monitor;
using Betauer.Camera;
using Betauer.Core.Nodes;
using Betauer.Core.Time;
using Betauer.DI;
using Betauer.Flipper;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.OnReady;
using Betauer.StateMachine.Sync;
using Betauer.Tools.Logging;
using Godot;
using Veronenger.Character.Enemy;
using Veronenger.Character.Handler;
using Veronenger.Character.Items;
using Veronenger.Managers;

namespace Veronenger.Character.Player; 

public enum PlayerState {
	Idle,
	Landing,
	Run,
	Attacking,
	JumpAttack,
	FallingAttack,
	FallShort,
	FallLong,
	Jump,
	Hurt,
	Death,
			
	Float,
}

public enum PlayerEvent {
	Hurt,
	Death,
}

public partial class PlayerNode : StateMachineNodeSync<PlayerState, PlayerEvent> {
	public PlayerNode() : base(PlayerState.Idle, "Player.StateMachine", true) {
	}

	public event Action? OnFree;
	public override void _Notification(long what) {
		if (what == NotificationPredelete) OnFree?.Invoke();
	}

	private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PlayerNode));

	[OnReady("Character")] private CharacterBody2D CharacterBody2D;
	[OnReady("Character/Sprites/Weapon")] private Sprite2D _weaponSprite;
	[OnReady("Character/Sprites/Body")] private Sprite2D _mainSprite;
	
	[OnReady("Character/Sprites/AnimationPlayer")] private AnimationPlayer _animationPlayer;
	[OnReady("Character/AttackArea")] private Area2D _attackArea;
	[OnReady("Character/HurtArea")] private Area2D _hurtArea;
	[OnReady("Character/RichTextLabel")] public RichTextLabel Label;
	[OnReady("Character/Detector")] public Area2D PlayerDetector;
	[OnReady("Character/Camera2D")] private Camera2D _camera2D;
	[OnReady("Character/Marker2D")] public Marker2D Marker2D;
	[OnReady("Character/CanJump")] public RayCast2D RaycastCanJump;
	[OnReady("Character/FloorRaycasts")] public List<RayCast2D> FloorRaycasts;

	[Inject] private PlatformManager PlatformManager { get; set; }
	[Inject] private CharacterManager CharacterManager { get; set; }
	[Inject] private WeaponManager WeaponManager { get; set; }
	[Inject] private StageManager StageManager { get; set; }
	[Inject] private InputAction MMB { get; set; }
	[Inject] private InputAction NextItem { get; set; }
	[Inject] private InputAction PrevItem { get; set; }

	[Inject] public World World { get; set; }
	[Inject] public PlayerConfig PlayerConfig { get; set; }
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private EventBus EventBus { get; set; }
	[Inject] private InputActionCharacterHandler Handler { get; set; }

	public KinematicPlatformMotion PlatformBody { get; private set; }
	public Vector2? InitialPosition { get; set; }
	public Inventory Inventory { get; private set; }
	public PlayerStatus Status { get; private set; }

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
	private bool IsOnFallingPlatform() => PlatformBody.IsOnFloor() &&
										  PlatformManager.IsFallingPlatform(PlatformBody
											  .GetFloorColliders<PhysicsBody2D>().FirstOrDefault());

	// private bool IsMovingPlatform() => PlatformManager.IsMovingPlatform(Body.GetFloor());
	private MonitorText? _coyoteMonitor;
	private MonitorText? _jumpHelperMonitor;
	private readonly GodotStopwatch _coyoteFallingTimer = new GodotStopwatch();

	private readonly DragCameraController _cameraController = new();
	private CharacterWeaponController _characterWeaponController;

	public override void _Ready() {
		ConfigureAnimations();
		ConfigureOverlay();
		ConfigureCamera();
		ConfigureCharacter();
		ConfigureAttackArea();
		ConfigureHurtArea();
		ConfigureStateMachine();
		
		LoadState();

		var drawEvent = this.OnDraw(canvas => {
			foreach (var floorRaycast in FloorRaycasts) canvas.DrawRaycast(floorRaycast, Colors.Red);
			canvas.DrawRaycast(RaycastCanJump, Colors.Red);
		});
		drawEvent.Disable();
	}

	private void LoadState() {
		Inventory.Pick(World.Get("K1"));
		Inventory.Pick(World.Get("M1"));
		Inventory.Equip(1);
	}

	private void ConfigureCharacter() {
		if (InitialPosition.HasValue) CharacterBody2D.GlobalPosition = InitialPosition.Value;
		CharacterBody2D.FloorStopOnSlope = true;
		// CharacterBody2D.FloorBlockOnWall = true;
		CharacterBody2D.FloorConstantSpeed = true;
		CharacterBody2D.FloorSnapLength = MotionConfig.SnapLength;
		var flipper = new FlipperList()
			.Sprite2DFlipH(_mainSprite)
			.Sprite2DFlipH(_weaponSprite)
			.ScaleX(_attackArea);
		flipper.IsFacingRight = true;

		PlatformBody = new KinematicPlatformMotion(CharacterBody2D, flipper, Marker2D, MotionConfig.FloorUpDirection,
			FloorRaycasts);
		OnBeforeExecute += () => PlatformBody.SetDelta(Delta);
		OnAfterExecute += () => {
			Label.Text = _animationStack.GetPlayingOnce() != null
				? _animationStack.GetPlayingOnce().Name
				: _animationStack.GetPlayingLoop().Name;
			Label.Text += "(" + attackState + ")";
		};

		_characterWeaponController = new CharacterWeaponController(_attackArea, _weaponSprite);
		_characterWeaponController.Equip(WeaponManager.Knife);

		Inventory = new Inventory();
		Inventory.OnEquip += (item) => {
			if (item is WeaponItem weapon) {
				_characterWeaponController.Equip(weapon.Type);
			}
		};

		Status = new PlayerStatus(PlayerConfig.InitialMaxHealth, PlayerConfig.InitialHealth);

		CharacterManager.RegisterPlayerNode(this);
		CharacterManager.PlayerConfigureCollisions(this);
	}

	private void ConfigureCamera() {
		_cameraController.WithAction(MMB).Attach(_camera2D);
		StageManager.ConfigureStageCamera(_camera2D, PlayerDetector);
	}

	private void ConfigureAttackArea() {
		CharacterManager.PlayerConfigureAttackArea(_attackArea);
		this.OnProcess(delta => {
			if (!Status.AttackConsumed &&
				_attackArea.Monitoring &&
				_attackArea.HasOverlappingAreas()) {
				EnemyItem? target = _attackArea.GetOverlappingAreas()
					.Select(area2D => World.Get<EnemyItem>(area2D.GetWorldId()))
					.Where(enemy => !enemy.ZombieNode.Status.UnderAttack)
					.MinBy(enemy => enemy.ZombieNode.DistanceToPlayer());
				if (target != null) {
					Status.AttackConsumed = true;
					EventBus.Publish(new PlayerAttackEvent(this, target, Inventory.WeaponEquipped!));
				}
			}
		});
	}

	private void ConfigureHurtArea() {
		CharacterManager.PlayerConfigureHurtArea(_hurtArea);
		this.OnProcess(delta => {
			if (Status is { UnderAttack: false, Invincible: false } &&
				_hurtArea.Monitoring &&
				_hurtArea.HasOverlappingAreas()) {
				var attacker = _hurtArea.GetOverlappingAreas()
					.Select(area2D => World.Get<EnemyItem>(area2D.GetWorldId()))
					.MinBy(enemy => enemy.ZombieNode.DistanceToPlayer());
				Status.UnderAttack = true;
				Status.Hurt(attacker.ZombieNode.EnemyConfig.Attack);
				Send(Status.IsDead() ? PlayerEvent.Death : PlayerEvent.Hurt);
			}
		});
	}

	public override void _Input(InputEvent e) {
		base._Input(e);
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


	public void ApplyFloorGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.FloorGravity * factor, PlayerConfig.MaxFallingSpeed);
	}

	public void ApplyAirGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.AirGravity * factor, PlayerConfig.MaxFallingSpeed);
	}

	public enum AttackState {
		None,
		Start,
		Short,
		Long
	}

	public AttackState attackState = AttackState.None;

	public void AnimationCallback_EndShortAttack() {
		if (attackState == AttackState.Short) {
			attackState = AttackState.None;
			AnimationAttack.Stop(true);
		}
	}
	
	public void AnimationCallback_EndLongAttack() {
		attackState = AttackState.None;
	}

	private void ManageInputActions(InputEvent e) {
		if (NextItem.IsEventJustPressed(e)) {
			Inventory.NextItem();
			Inventory.Equip();
		} else if (PrevItem.IsEventJustPressed(e)) {
			Inventory.PrevItem();
			Inventory.Equip();
		}
	}

	public void ConfigureStateMachine() {
		var stateTimer = new GodotStopwatch();
		var recoverTimeout = new GodotTimeout(GetTree(), PlayerConfig.HurtInvincibleTime, () => {
			Status.Invincible = false;
		});
		var delayedJump = ((InputAction)Jump).CreateDelayed();
		OnFree += () => {
			recoverTimeout.Stop();
			delayedJump.Dispose();
		};

		PhysicsBody2D? fallingPlatform = null;
		void FallFromPlatform() {
			fallingPlatform = PlatformBody.GetFloorCollider<PhysicsBody2D>()!;
			PlatformManager.RemovePlatformCollision(fallingPlatform);
		}

		void FinishFallFromPlatform() {
			if (fallingPlatform != null) PlatformManager.ConfigurePlatformCollision(fallingPlatform);
		}

		// OnTransition += args => Console.WriteLine(args.To);

		On(PlayerEvent.Hurt).Set(PlayerState.Hurt);
		On(PlayerEvent.Death).Set(PlayerState.Death);

		var jumpJustInTime = false;
		State(PlayerState.Landing)
			.Enter(() => {
				FinishFallFromPlatform();
				_coyoteFallingTimer.Stop(); // not really needed, but less noise in the debug overlay
				jumpJustInTime = delayedJump.WasPressed(PlayerConfig.JumpHelperTime);
			})
			.Execute(() => {
				if (jumpJustInTime && CanJump()) {
					_jumpHelperMonitor?.Show($"{delayedJump.LastPressed} <= {PlayerConfig.JumpHelperTime.ToString()} Done!");
				} else {
					_jumpHelperMonitor?.Show($"{delayedJump.LastPressed} > {PlayerConfig.JumpHelperTime.ToString()} TOO MUCH TIME");
				}
			})
			.If(() => jumpJustInTime).Set(PlayerState.Jump)
			.If(() => Attack.IsJustPressed()).Set(PlayerState.Attacking)
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => true).Set(PlayerState.Run)
			.Build();

		State(PlayerState.Idle)
			.OnInput(ManageInputActions)
			.Enter(() => {
				AnimationIdle.PlayLoop();
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
			})
			.If(() => Jump.IsJustPressed() && IsPressingDown && IsOnFallingPlatform()).Then(
				context => {
					FallFromPlatform();
					return context.Set(PlayerState.FallShort);
				})
			.If(() => Jump.IsJustPressed() && Attack.IsJustPressed()).Set(PlayerState.JumpAttack)
			.If(() => Jump.IsJustPressed() && CanJump()).Set(PlayerState.Jump)
			.If(() => Attack.IsJustPressed()).Set(PlayerState.Attacking)
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.FallShort)
			.If(() => XInput != 0).Set(PlayerState.Run)
			.Build();

		State(PlayerState.Run)
			.OnInput(ManageInputActions)
			.Execute(() => {
				ApplyFloorGravity();
				if (XInput == 0) {
					if (Math.Abs(MotionX) >= PlayerConfig.SpeedToPlayRunStop) {
						AnimationIdle.PlayLoop();
						AnimationRunStop.PlayOnce();
					}
				} else {
					AnimationRun.PlayLoop();
					AnimationRunStop.Stop(true);
				}
				PlatformBody.Flip(XInput);
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.Friction, 
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
			})
			.If(() => Jump.IsJustPressed() && IsPressingDown && IsOnFallingPlatform()).Then(
				context => {
					FallFromPlatform();
					return context.Set(PlayerState.FallShort);
				})
			.If(() => Jump.IsJustPressed() && Attack.IsJustPressed()).Set(PlayerState.JumpAttack)
			.If(() => Jump.IsJustPressed() && CanJump()).Set(PlayerState.Jump)
			.If(() => Attack.IsJustPressed()).Set(PlayerState.Attacking)
			.If(() => !PlatformBody.IsOnFloor()).Then( 
				context => {
					_coyoteFallingTimer.Restart();
					return context.Set(PlayerState.FallShort);
				})
			.If(() => XInput == 0 && MotionX == 0).Set(PlayerState.Idle)
			.Build();

		State(PlayerState.Attacking)
			.Enter(() => {
				AnimationAttack.PlayOnce(true);
				attackState = AttackState.Start;
				Status.AttackConsumed = false;
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
				if (attackState == AttackState.Start) {
					attackState = AttackState.Short;
				} else {
					if (Attack.IsJustPressed()) {
						if (attackState == AttackState.Short) {
							attackState = AttackState.Long;
						} else if (attackState == AttackState.Long) {
							//
						}
					}
				}
			})
			.If(() => attackState == AttackState.None && !PlatformBody.IsOnFloor()).Set(PlayerState.FallShort)
			.If(() => attackState == AttackState.None && XInput == 0).Set(PlayerState.Idle)
			.If(() => attackState == AttackState.None && XInput != 0).Set(PlayerState.Run)
			.Build();
		
		State(PlayerState.JumpAttack)
			.Enter(() => {
				PlatformBody.MotionY = -PlayerConfig.JumpSpeed;
				AnimationAttack.PlayOnce(true);
				Status.AttackConsumed = false;
				attackState = AttackState.Start;
			})
			.If(() => true).Set((PlayerState.FallingAttack))
			.Build();
		
		State(PlayerState.Jump)
			.OnInput(ManageInputActions)
			.Enter(() => {
				PlatformBody.MotionY = -PlayerConfig.JumpSpeed;
				AnimationFall.PlayLoop();
				AnimationJump.PlayOnce();
			})
			.Execute(() => {
				if (Jump.IsReleased() && MotionY < -PlayerConfig.JumpSpeedMin) {
					PlatformBody.MotionY = -PlayerConfig.JumpSpeedMin;
				}

				PlatformBody.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
			})
			.If(() => Attack.IsJustPressed()).Set(PlayerState.FallingAttack)
			.If(() => Float.IsPressed()).Set(PlayerState.Float)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.If(() => MotionY >= 0).Set(PlayerState.FallShort)
			.Build();

		Tween? knockbackTween = null;
		var weaponSpriteVisible = false;
		State(PlayerState.Hurt)
			.Enter(() => {
				recoverTimeout.Restart();
				stateTimer.Restart().SetAlarm(PlayerConfig.HurtTime);
				Status.Invincible = true;
				PlatformBody.MotionX = PlayerConfig.HurtKnockback.x * PlatformBody.FacingRight;
				PlatformBody.MotionY = PlayerConfig.HurtKnockback.y;
				weaponSpriteVisible = _weaponSprite.Visible;
				AnimationHurt.PlayOnce(true);
				knockbackTween?.Kill();
				knockbackTween = CreateTween();
				knockbackTween.TweenMethod(Callable.From<float>(v => PlatformBody.MotionX = v), PlatformBody.MotionX, 0, PlayerConfig.HurtKnockbackTime).SetTrans(Tween.TransitionType.Cubic);
			})
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.Move();
			})
			.If(stateTimer.IsAlarm).Set(PlayerState.FallLong)
			.Exit(() => {
				Status.UnderAttack = false;
				_weaponSprite.Visible = weaponSpriteVisible;
			})
			.Build();

		State(PlayerState.Death)
			.Enter(() => {
				Console.WriteLine("MUERTO");
				EventBus.Publish(MainEvent.EndGame);
			})
			.Build();

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

		State(PlayerState.FallingAttack)
			.Enter(() => {
				AnimationAttack.PlayOnce(true);
				attackState = AttackState.Start;
			})
			.Execute(() => {
				PlatformBody.Flip(XInput);
				ApplyFloorGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
				if (attackState == AttackState.Start) {
					attackState = AttackState.Short;
				} else {
					if (Attack.IsJustPressed() && false) {
						if (attackState == AttackState.Short) {
							attackState = AttackState.Long;
						} else if (attackState == AttackState.Long) {
							//
						}
					}
				}
			})
			.If(() => attackState == AttackState.None && !PlatformBody.IsOnFloor()).Set(PlayerState.FallLong)
			.If(() => attackState == AttackState.None && XInput == 0).Set(PlayerState.Idle)
			.If(() => attackState == AttackState.None && XInput != 0).Set(PlayerState.Run)
			.Build();

		State(PlayerState.FallShort)
			.OnInput(ManageInputActions)
			.Execute(() => {
				PlatformBody.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
			})
			.If(() => Attack.IsJustPressed()).Set(PlayerState.FallingAttack)
			.If(() => Float.IsPressed()).Set(PlayerState.Float)
			.If(CheckCoyoteJump).Set(PlayerState.Jump)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.If(() => MotionY > PlayerConfig.StartFallingSpeed).Set(PlayerState.FallLong)
			.Build();
				
		State(PlayerState.FallLong)
			.OnInput(ManageInputActions)
			.Enter(() => AnimationFall.PlayLoop())
			.Execute(() => {
				PlatformBody.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
			})
			.If(() => Attack.IsJustPressed()).Set(PlayerState.FallingAttack)
			.If(() => Float.IsPressed()).Set(PlayerState.Float)
			.If(CheckCoyoteJump).Set(PlayerState.Jump)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.Build();

		State(PlayerState.Float)
			.OnInput(ManageInputActions)
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
