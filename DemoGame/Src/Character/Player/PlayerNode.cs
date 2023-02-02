using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Monitor;
using Betauer.Camera;
using Betauer.Core;
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
using Veronenger.Character.Handler;
using Veronenger.Character.Items;
using Veronenger.Managers;
using Veronenger.UI;

namespace Veronenger.Character.Player; 

public enum PlayerState {
	Idle,
	Landing,
	Running,
	Attacking,
	JumpingAttack,
	FallingAttack,
	FallWithCoyote,
	Fall,
	Jumping,
	Hurting,
	Death,
			
	Floating,
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

	[OnReady("Character")] public CharacterBody2D CharacterBody2D;
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

	[Inject] private World World { get; set; }
	[Inject] private PlayerConfig PlayerConfig { get; set; }
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private EventBus EventBus { get; set; }
	[Inject] private InputActionCharacterHandler Handler { get; set; }
	[Inject] private HUD HudScene { get; set; }
	[Inject] private PlayerStatus Status { get; set; }

	public KinematicPlatformMotion PlatformBody { get; private set; }
	public Vector2? InitialPosition { get; set; }
	public Inventory Inventory { get; private set; }

	private float XInput => Handler.Directional.XInput;
	private float YInput => Handler.Directional.YInput;
	private bool IsPressingRight => Handler.Directional.IsPressingRight;
	private bool IsPressingLeft => Handler.Directional.IsPressingLeft;
	private bool IsPressingUp => Handler.Directional.IsPressingUp;
	private bool IsPressingDown => Handler.Directional.IsPressingDown;
	private InputAction Jump => Handler.Jump;
	private InputAction Attack => Handler.Attack;
	private InputAction Float => Handler.Float;

	private float MotionX => PlatformBody.MotionX;
	private float MotionY => PlatformBody.MotionY;

	// private bool IsOnPlatform() => PlatformManager.IsPlatform(Body.GetFloor());
	private bool IsOnFallingPlatform() => PlatformBody.IsOnFloor() &&
										  PlatformManager.IsFallingPlatform(PlatformBody
											  .GetFloorColliders<PhysicsBody2D>().FirstOrDefault());

	// private bool IsMovingPlatform() => PlatformManager.IsMovingPlatform(Body.GetFloor());
	private MonitorText? _coyoteMonitor;
	private MonitorText? _jumpHelperMonitor;

	private readonly DragCameraController _cameraController = new();
	private CharacterWeaponController _characterWeaponController;
	private AttackState _attackState = AttackState.None;
	private readonly GodotStopwatch _stateTimer = new(false, true);

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
		OnBefore += () => PlatformBody.SetDelta(Delta);
		// OnAfter += () => {
		// 	Label.Text = _animationStack.GetPlayingOnce() != null
		// 		? _animationStack.GetPlayingOnce().Name
		// 		: _animationStack.GetPlayingLoop().Name;
		// 	Label.Text += "(" + _attackState + ")";
		// };

		_characterWeaponController = new CharacterWeaponController(_attackArea, _weaponSprite);
		_characterWeaponController.Equip(WeaponManager.Knife);

		Inventory = new Inventory();
		Inventory.OnEquip += (item) => {
			if (item is WeaponItem weapon) {
				_characterWeaponController.Equip(weapon.Type);
			}
		};

		Status.OnHealthUpdate += HudScene.UpdateHealth; 
		Status.Configure(this, PlayerConfig.InitialMaxHealth, PlayerConfig.InitialHealth);

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
			if (Status.AvailableHits > 0 &&
				_attackArea.Monitoring &&
				_attackArea.HasOverlappingAreas()) {

				_attackArea.GetOverlappingAreas()
					.Select(area2D => World.Get<EnemyItem>(area2D.GetWorldId()))
					.Where(enemy => !enemy.ZombieNode.Status.UnderAttack)
					.OrderBy(enemy => enemy.ZombieNode.DistanceToPlayer()) // Ascending, so first element is the closest to the player
					.Take(Status.AvailableHits)
					.ForEach(enemy => {
						Status.AvailableHits--;
						EventBus.Publish(new PlayerAttackEvent(this, enemy, Inventory.WeaponEquipped!));
					});
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
				Status.UpdateHealth(-attacker.ZombieNode.EnemyConfig.Attack);
				if (Status.IsDead()) {
					Send(PlayerEvent.Death, 10000);
				} else {
					Send(PlayerEvent.Hurt, 10000);
				}
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

	private void StartStack() {
		_stateTimer.Restart();
		// Logger.Debug(CurrentState.Key+ ": Attack started");
		_attackState = AttackState.Start;
		Status.AvailableHits = Inventory.WeaponEquipped.EnemiesPerHit;
	}

	private void StopAttack() {
		// Logger.Debug(CurrentState.Key+ ": Attack ended: GodotStopwatch physics: " + _stateTimer.Elapsed);
		_attackState = AttackState.None;
		AnimationAttack.Stop(true);
		Status.AvailableHits = 0;
	}

	public void AnimationCallback_EndShortAttack() {
		if (_attackState == AttackState.Short) {
			// Short means the player didn't attack again, so the attack ends here
			StopAttack();
		} else if (_attackState == AttackState.Long) {
			// The player pressed attack twice, so the short attack is now a long attack, and this signal call is ignored
			Status.AvailableHits = 2;
		}
	}

	public void AnimationCallback_EndLongAttack() {
		StopAttack();
	}

	public void ConfigureStateMachine() {
		Tween? invincibleTween = null;
		void StartInvincibleEffect() {
			const float flashTime = 0.025f;
			invincibleTween?.Kill();
			invincibleTween = CreateTween();
			invincibleTween
				.TweenCallback(Callable.From(() => _mainSprite.Visible = !_mainSprite.Visible))
				.SetDelay(flashTime);
			invincibleTween.SetLoops((int)(PlayerConfig.HurtInvincibleTime / flashTime));
			invincibleTween.Finished += () => {
				Status.Invincible = false;
				_mainSprite.Visible = true;
			};
		}

		var delayedJump = Jump.CreateDelayed();
		OnFree += () => {
			invincibleTween?.Kill();
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

		OnTransition += (args) => _stateTimer.Restart();
		// OnTransition += (args) => Logger.Debug(args.From +" -> "+args.To);

		On(PlayerEvent.Hurt).Set(PlayerState.Hurting);
		On(PlayerEvent.Death).Set(PlayerState.Death);
		
		var jumpJustInTime = false;
		var weaponSpriteVisible = false;

		void InventoryHandler(InputEvent e) {
			if (NextItem.IsEventJustPressed(e)) {
				Inventory.NextItem();
				Inventory.Equip();
			} else if (PrevItem.IsEventJustPressed(e)) {
				Inventory.PrevItem();
				Inventory.Equip();
			}
		}

		State(PlayerState.Landing)
			.OnInput(InventoryHandler)
			// .OnInputBatch(AttackAndJumpHandler)
			.Enter(() => {
				_jumpHelperMonitor?.Show("");
				_coyoteMonitor?.Show("");
				FinishFallFromPlatform();
				jumpJustInTime = delayedJump.WasPressed(PlayerConfig.JumpHelperTime) && CanJump();
			})
			.If(() => Attack.IsJustPressed()).Set(PlayerState.Attacking)
			.If(() => jumpJustInTime).Then(ctx => {
				_jumpHelperMonitor?.Show($"{delayedJump.LastPressed} <= {PlayerConfig.JumpHelperTime:0.00} Done!");
				return ctx.Set(PlayerState.Jumping);
			})
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => true).Set(PlayerState.Running)
			.Build();

		State(PlayerState.Idle)
			.OnInput(InventoryHandler)
			// .OnInputBatch(AttackAndJumpHandler)
			.Enter(() => AnimationIdle.PlayLoop())
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
			})
			.If(() => Jump.IsJustPressed() && IsPressingDown && IsOnFallingPlatform()).Then(
				context => {
					FallFromPlatform();
					return context.Set(PlayerState.Fall);
				})
			.If(() => Jump.IsJustPressed() && Attack.IsJustPressed()).Set(PlayerState.JumpingAttack)
			.If(() => Jump.IsJustPressed() && CanJump()).Set(PlayerState.Jumping)
			.If(() => Attack.IsJustPressed()).Set(PlayerState.Attacking)	
			.If(() => XInput != 0).Set(PlayerState.Running)
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.Fall)
			.Build();

		State(PlayerState.Running)
			.OnInput(InventoryHandler)
			// .OnInputBatch(AttackAndJumpHandler)
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
					return context.Set(PlayerState.Fall);
				})
			.If(() => Jump.IsJustPressed() && Attack.IsJustPressed()).Set(PlayerState.Jumping)
			.If(() => Attack.IsJustPressed()).Set(PlayerState.Attacking)
			.If(() => Jump.IsJustPressed() && CanJump()).Set(PlayerState.Jumping)
			.If(() => XInput == 0 && MotionX == 0).Set(PlayerState.Idle)
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.FallWithCoyote)
			.Build();

		State(PlayerState.Attacking)
			.Enter(() => {
				AnimationAttack.PlayOnce(true);
				StartStack();
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
				if (Attack.IsJustPressed()) {
					if (_attackState == AttackState.Short) {
						// Promoted short attack to long attack 
						_attackState = AttackState.Long;
					} else if (_attackState == AttackState.Long) {
						//
					}
				}
				if (_attackState == AttackState.Start) _attackState = AttackState.Short;
			})
			.If(() => _attackState != AttackState.None).Stay()
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => XInput != 0).Set(PlayerState.Running)
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.FallWithCoyote)
			.Build();
		
		State(PlayerState.JumpingAttack)
			.Enter(() => {
				PlatformBody.MotionY = -PlayerConfig.JumpSpeed;
				AnimationAttack.PlayOnce(true);
				StartStack();
			})
			.If(() => true).Set(PlayerState.FallingAttack)
			.Build();
		
		State(PlayerState.Jumping)
			.OnInput(InventoryHandler)
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
			.If(() => Float.IsJustPressed()).Set(PlayerState.Floating)
			.If(() => MotionY >= 0).Set(PlayerState.Fall)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.Build();

		State(PlayerState.FallingAttack)
			.Enter(() => {
				AnimationAttack.PlayOnce(true);
				StartStack();
			})
			.Execute(() => {
				PlatformBody.Flip(XInput);
				ApplyFloorGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
			})
			.If(() => _attackState != AttackState.None).Stay()
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => XInput != 0).Set(PlayerState.Running)
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.Fall)
			.Build();

		var coyoteTimer = new GodotStopwatch();
		State(PlayerState.FallWithCoyote)
			.OnInput(InventoryHandler)
			.Enter(() => coyoteTimer.Restart().SetAlarm(PlayerConfig.CoyoteJumpTime))
			.Execute(() => {
				if (MotionY > PlayerConfig.StartFallingSpeed) AnimationFall.PlayLoop();
				PlatformBody.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
			})
			.If(() => Attack.IsJustPressed()).Set(PlayerState.FallingAttack)
			.If(() => Jump.IsJustPressed() && CanJump()).Set(PlayerState.Jumping)
			.If(() => Float.IsJustPressed()).Set(PlayerState.Floating)
			.If(() => coyoteTimer.IsAlarm()).Set(PlayerState.Fall)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.Build();

		OnTransition += (args) => {
			if (args is { From: PlayerState.FallWithCoyote, To: PlayerState.Jumping }) {
				_coyoteMonitor?.Show($"{coyoteTimer.Elapsed:0.00} <= {PlayerConfig.CoyoteJumpTime:0.00} Done!");				
			} else if (Jump.IsJustPressed()) {
				_coyoteMonitor?.Show($"{coyoteTimer.Elapsed:0.00} > {PlayerConfig.CoyoteJumpTime:0.00} TOO LATE");
			}
		};
				
		State(PlayerState.Fall)
			.OnInput(InventoryHandler)
			.Execute(() => {
				if (MotionY > PlayerConfig.StartFallingSpeed) AnimationFall.PlayLoop();
				PlatformBody.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
			})
			.If(() => Attack.IsJustPressed()).Set(PlayerState.FallingAttack)
			.If(() => Float.IsJustPressed()).Set(PlayerState.Floating)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.Build();

		State(PlayerState.Floating)
			.OnInput(InventoryHandler)
			.Enter(() => CharacterBody2D.MotionMode = CharacterBody2D.MotionModeEnum.Floating)
			.Execute(() => {
				PlatformBody.AddSpeed(XInput, YInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed,
					PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan, 0);
				PlatformBody.Move();
			})
			.If(() => Float.IsJustPressed()).Set(PlayerState.Fall)
			.Exit(() => CharacterBody2D.MotionMode = CharacterBody2D.MotionModeEnum.Grounded)
			.Build();

		State(PlayerState.Hurting)
			.Enter(() => {
				Status.Invincible = true;
				weaponSpriteVisible = _weaponSprite.Visible;
				AnimationHurt.PlayOnce(true);
			})
			.Execute(() => {
				ApplyAirGravity();
			})
			.If(() => AnimationHurt.Playing).Stay()
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => XInput != 0).Set(PlayerState.Running)
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.Fall)
			.Exit(() => {
				Status.UnderAttack = false;
				_weaponSprite.Visible = weaponSpriteVisible;
				StartInvincibleEffect();
			})
			.Build();

		State(PlayerState.Death)
			.Enter(() => {
				Console.WriteLine("MUERTO");
				EventBus.Publish(MainEvent.EndGame);                                                                     
			})
			.Build();


	}
}
