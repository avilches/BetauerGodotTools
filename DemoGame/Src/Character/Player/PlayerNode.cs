using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Animation.AnimationPlayer;
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
using Veronenger.Character.InputActions;
using Veronenger.Items;
using Veronenger.Managers;
using Veronenger.UI;

namespace Veronenger.Character.Player; 

public enum PlayerState {
	Idle,
	Landing,
	Running,
	MeleeAttack,
	MeleeAttackAir,
	RangeAttack,
	RangeAttackAir,
	FallWithCoyote,
	Fall,
	Jumping,
	Hurting,
	Death,
			
	Floating,
}

public enum PlayerEvent {
	Attack,
	Hurt,
	Death,
}

public partial class PlayerNode : StateMachineNodeSync<PlayerState, PlayerEvent> {
	public PlayerNode() : base(PlayerState.Idle, "Player.StateMachine", true) {
	}

	public event Action? OnFree;
	public override void _Notification(int what) {
		if (what == NotificationPredelete) OnFree?.Invoke();
	}

	private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PlayerNode));

	[OnReady("Character")] public CharacterBody2D CharacterBody2D;
	[OnReady("Character/Sprites/Weapon")] private Sprite2D _weaponSprite;
	[OnReady("Character/Sprites/Body")] private Sprite2D _mainSprite;
	
	[OnReady("Character/Sprites/AnimationPlayer")] private AnimationPlayer _animationPlayer;
	[OnReady("Character/AttackArea1")] private Area2D _attackArea1;
	[OnReady("Character/AttackArea2")] private Area2D _attackArea2;
	[OnReady("Character/HurtArea")] private Area2D _hurtArea;
	[OnReady("Character/RichTextLabel")] public RichTextLabel Label;
	[OnReady("Character/Detector")] public Area2D PlayerDetector;
	[OnReady("Character/Camera2D")] private Camera2D _camera2D;
	[OnReady("Character/Marker2D")] public Marker2D Marker2D;
	[OnReady("Character/CanJump")] public RayCast2D RaycastCanJump;
	[OnReady("Character/FloorRaycasts")] public List<RayCast2D> FloorRaycasts;

	[Inject] private Game Game { get; set; }
	[Inject] private PlatformManager PlatformManager { get; set; }
	[Inject] private CharacterManager CharacterManager { get; set; }
	[Inject] private WeaponConfigManager WeaponConfigManager { get; set; }
	[Inject] private StageManager StageManager { get; set; }
	[Inject] private InputAction MMB { get; set; }
	[Inject] private InputAction NextItem { get; set; }
	[Inject] private InputAction PrevItem { get; set; }

	[Inject] private World World { get; set; }
	[Inject] private PlayerConfig PlayerConfig { get; set; }
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private EventBus EventBus { get; set; }
	[Inject] private PlayerInputActions Handler { get; set; }
	[Inject] private HUD HudScene { get; set; }
	[Inject] private PlayerStatus Status { get; set; }

	public KinematicPlatformMotion PlatformBody { get; private set; }
	public Vector2? InitialPosition { get; set; }
	public Inventory Inventory { get; private set; }

	public Anim AnimationIdle { get; private set; }
	public Anim AnimationRun { get; private set; }
	public Anim AnimationRunStop { get; private set; }
	public Anim AnimationJump { get; private set; }
	public Anim AnimationFall { get; private set; }
	public Anim AnimationAttack { get; private set; }
	public Anim AnimationShoot { get; private set; }
	public Anim AnimationAirAttack { get; private set; }
	public Anim AnimationHurt { get; private set; }

	private float XInput => Handler.Lateral.Strength;
	private float YInput => Handler.Vertical.Strength;
	private bool IsPressingRight => Handler.Right.IsPressed;
	private bool IsPressingLeft => Handler.Left.IsPressed;
	private bool IsPressingUp => Handler.Up.IsPressed;
	private bool IsPressingDown => Handler.Down.IsPressed;
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
		Inventory.Pick(World.Get<WeaponMeleeItem>("K1"));
		Inventory.Pick(World.Get<WeaponMeleeItem>("M1"));
		Inventory.Pick(World.Get<WeaponRangeItem>("G1"));
		Inventory.Equip(1);
	}

	private void ConfigureAnimations() {
		AnimationIdle = _animationPlayer.Anim("Idle");
		AnimationRun = _animationPlayer.Anim("Run");
		AnimationRunStop = _animationPlayer.Anim("RunStop");
		AnimationJump = _animationPlayer.Anim("Jump");
		AnimationFall = _animationPlayer.Anim("Fall");
		AnimationShoot = _animationPlayer.Anim("Shoot");
		AnimationAttack = _animationPlayer.Anim("Attack");
		AnimationAirAttack = _animationPlayer.Anim("AirAttack");
		AnimationHurt = _animationPlayer.Anim("Hurt");

		// Restorer restorer = new MultiRestorer() 
		// 	.Add(CharacterBody2D.CreateRestorer(Properties.Modulate, Properties.Scale2D))
		// 	.Add(_mainSprite.CreateRestorer(Properties.Modulate, Properties.Scale2D));
		// restorer.Save();
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
			.ScaleX(_attackArea1)
			.ScaleX(_attackArea2);
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

		_characterWeaponController = new CharacterWeaponController(new[] { _attackArea1, _attackArea2 }, _weaponSprite);
		_characterWeaponController.Unequip();

		Inventory = new Inventory();
		Inventory.OnEquip += item => {
			if (item is WeaponItem weapon) {
				_characterWeaponController.Equip(weapon);
			}
		};

		Status.OnHealthUpdate += HudScene.UpdateHealth; 
		Status.Configure(this, PlayerConfig.InitialMaxHealth, PlayerConfig.InitialHealth);

		CharacterManager.RegisterPlayerNode(this);
		CharacterManager.PlayerConfigureCollisions(this);
	}

	private void ConfigureCamera() {
		_cameraController.WithMouseButton(MouseButton.Middle).Attach(_camera2D);
		StageManager.ConfigureStageCamera(_camera2D, PlayerDetector);
	}

	private void ConfigureAttackArea() {
		CharacterManager.PlayerConfigureAttackArea(_attackArea1);
		CharacterManager.PlayerConfigureAttackArea(_attackArea2);
		this.OnProcess(delta => {
			if (Status.AvailableHits > 0) {
				CheckAttackArea(_attackArea1);
				CheckAttackArea(_attackArea2);
			}

			void CheckAttackArea(Area2D attackArea) {
				if (attackArea.Monitoring && attackArea.HasOverlappingAreas()) {
					attackArea.GetOverlappingAreas()
						.Select(area2D => World.Get<EnemyItem>(area2D.GetWorldId()))
						.Where(enemy => !enemy.ZombieNode.Status.UnderAttack)
						.OrderBy(enemy => enemy.ZombieNode.DistanceToPlayer()) // Ascending, so first element is the closest to the player
						.Take(Status.AvailableHits)
						.ForEach(enemy => {
							Status.AvailableHits--;
							EventBus.Publish(new PlayerAttackEvent(this, enemy, Inventory.WeaponMeleeEquipped!));
						});
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
		Step1,
		Step2
	}

	private void StartStack() {
		_stateTimer.Restart();
		// Logger.Debug(CurrentState.Key+ ": Attack started");
		_attackState = AttackState.Start;
		Status.AvailableHits = Inventory.WeaponMeleeEquipped.EnemiesPerHit;
	}

	private void StopAttack() {
		// Logger.Debug(CurrentState.Key+ ": Attack ended: GodotStopwatch physics: " + _stateTimer.Elapsed);
		_attackState = AttackState.None;
		// AnimationAttack.Stop();
		Status.AvailableHits = 0;
	}

	public void AnimationCallback_EndAttack1() {
		if (_attackState == AttackState.Step1) {
			// Short means the player didn't attack again, so the attack ends here
			StopAttack();
		} else if (_attackState == AttackState.Step2) {
			// The player pressed attack twice, so the short attack is now a long attack, and this signal call is ignored
			Status.AvailableHits = Inventory.WeaponMeleeEquipped.EnemiesPerHit * 2;
		}
	}

	public void AnimationCallback_EndAttack2() {
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

		OnFree += () => {
			invincibleTween?.Kill();
		};

		PhysicsBody2D? fallingPlatform = null;
		void FallFromPlatform() {
			fallingPlatform = PlatformBody.GetFloorCollider<PhysicsBody2D>()!;
			PlatformManager.RemovePlatformCollision(fallingPlatform);
		}

		void FinishFallFromPlatform() {
			if (fallingPlatform != null) PlatformManager.ConfigurePlatformCollision(fallingPlatform);
		}

		void Shoot() {
			AnimationShoot.PlayFrom(0);
			var bulletPosition = new Vector2(Inventory.WeaponRangeEquipped.BulletStartPosition.X * PlatformBody.FacingRight, Inventory.WeaponRangeEquipped.BulletStartPosition.Y);
			var bulletDirection = new Vector2(PlatformBody.FacingRight, 0);
			Game.NewBullet().ShootFrom(Inventory.WeaponRangeEquipped, CharacterBody2D.ToGlobal(bulletPosition), bulletDirection);
		}

		OnTransition += (args) => _stateTimer.Restart();
		// OnTransition += (args) => Logger.Debug(args.From +" -> "+args.To);

		On(PlayerEvent.Hurt).Set(PlayerState.Hurting);
		On(PlayerEvent.Death).Set(PlayerState.Death);
		On(PlayerEvent.Attack).Then(ctx => {
			if (Inventory.GetCurrent() is WeaponMeleeItem) {
				return ctx.Set(PlatformBody.IsOnFloor() ? PlayerState.MeleeAttack : PlayerState.MeleeAttackAir);
			} else if (Inventory.GetCurrent() is WeaponRangeItem) {
				return ctx.Set(PlatformBody.IsOnFloor() ? PlayerState.RangeAttack : PlayerState.RangeAttackAir);
			} else return ctx.Stay();
		});
		
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
				jumpJustInTime = Jump.WasPressed(PlayerConfig.JumpHelperTime) && CanJump();
			})
			.If(() => Attack.IsJustPressed).Send(PlayerEvent.Attack)
			.If(() => jumpJustInTime).Then(ctx => {
				_jumpHelperMonitor?.Show($"{Jump.PressedTime:0.00} <= {PlayerConfig.JumpHelperTime:0.00} Done!");
				return ctx.Set(PlayerState.Jumping);
			})
			.If(() => XInput == 0).Then((ctx) => {
				_jumpHelperMonitor?.Show($"{Jump.PressedTime:0.00} > {PlayerConfig.JumpHelperTime:0.00} NOPE");
				return ctx.Set(PlayerState.Idle);
			})
			.If(() => true).Set(PlayerState.Running)
			.Build();

		State(PlayerState.Idle)
			.OnInput(InventoryHandler)
			// .OnInputBatch(AttackAndJumpHandler)
			.Enter(() => AnimationIdle.Play())
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
			})
			.If(() => Jump.IsJustPressed && IsPressingDown && IsOnFallingPlatform()).Then(
				context => {
					FallFromPlatform();
					return context.Set(PlayerState.Fall);
				})
			.If(() => Attack.IsJustPressed).Then(ctx => ctx.Send(PlayerEvent.Attack))	
			.If(() => Jump.IsJustPressed && CanJump()).Set(PlayerState.Jumping)
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
						AnimationRunStop.Play();
					}
				} else {
					if (Math.Abs(MotionX) >= PlayerConfig.StopIfSpeedIsLessThan) {
						AnimationRun.Play();
					}
				}
				PlatformBody.Flip(XInput);
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.Friction, 
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
			})
			.If(() => Jump.IsJustPressed && IsPressingDown && IsOnFallingPlatform()).Then(
				context => {
					FallFromPlatform();
					return context.Set(PlayerState.Fall);
				})
			.If(() => Jump.IsJustPressed && Attack.IsJustPressed).Set(PlayerState.Jumping)
			.If(() => Attack.IsJustPressed).Then(ctx => ctx.Send(PlayerEvent.Attack))
			.If(() => Jump.IsJustPressed && CanJump()).Set(PlayerState.Jumping)
			.If(() => XInput == 0 && MotionX == 0 && !AnimationRunStop.IsPlaying()).Set(PlayerState.Idle)
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.FallWithCoyote)
			.Build();

		State(PlayerState.MeleeAttack)
			.Enter(() => {
				AnimationAttack.Play();
				StartStack();
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
				if (Attack.IsJustPressed) {
					if (_attackState == AttackState.Step1) {
						// Promoted short attack to long attack 
						_attackState = AttackState.Step2;
					} else if (_attackState == AttackState.Step2) {
						//
					}
				}
				if (_attackState == AttackState.Start) _attackState = AttackState.Step1;
			})
			.If(() => _attackState != AttackState.None).Stay()
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.Fall)
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => XInput != 0).Set(PlayerState.Running)
			.Build();

		State(PlayerState.RangeAttack)
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
				if (Attack.IsJustPressed && _stateTimer.Elapsed >= Inventory.WeaponRangeEquipped.Config.DelayBetweenShots) {
					Shoot();
				}				
			})
			.If(() => AnimationShoot.IsPlaying()).Stay()
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.Fall)
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => XInput != 0).Set(PlayerState.Running)
			.Build();

		State(PlayerState.RangeAttackAir)
			.Execute(() => {
				ApplyFloorGravity();
				if (!PlatformBody.IsOnFloor()) {
					PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
						PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
				} else {
					PlatformBody.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
				}
				if (Attack.IsJustPressed && _stateTimer.Elapsed >= Inventory.WeaponRangeEquipped.Config.DelayBetweenShots) {
					Shoot();
				}				
			})
			.If(() => _stateTimer.Elapsed < Inventory.WeaponRangeEquipped.Config.DelayBetweenShots).Stay()
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.Fall)
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => XInput != 0).Set(PlayerState.Running)
			.Build();
		
		State(PlayerState.Jumping)
			.OnInput(InventoryHandler)
			.Enter(() => {
				PlatformBody.MotionY = -PlayerConfig.JumpSpeed;
				AnimationJump.Play();
				AnimationFall.Queue();
			})
			.Execute(() => {
				if (Jump.IsJustReleased && MotionY < -PlayerConfig.JumpSpeedMin) {
					PlatformBody.MotionY = -PlayerConfig.JumpSpeedMin;
				}

				PlatformBody.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
			})
			.If(() => Attack.IsJustPressed).Send(PlayerEvent.Attack)
			.If(() => Float.IsJustPressed).Set(PlayerState.Floating)
			.If(() => MotionY >= 0).Set(PlayerState.Fall)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.Build();

		State(PlayerState.MeleeAttackAir)
			.Enter(() => {
				AnimationAttack.Play();
				StartStack();
			})
			.Execute(() => {
				ApplyFloorGravity();
				if (!PlatformBody.IsOnFloor()) {
					PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed,
						PlayerConfig.AirResistance,
						PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
				} else {
					PlatformBody.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
				}
				if (_attackState == AttackState.Start) _attackState = AttackState.Step1;
			})
			.If(() => _attackState != AttackState.None).Stay()
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.Fall)
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => XInput != 0).Set(PlayerState.Running)
			.Build();

		var coyoteTimer = new GodotStopwatch();
		State(PlayerState.FallWithCoyote)
			.OnInput(InventoryHandler)
			.Enter(() => coyoteTimer.Restart().SetAlarm(PlayerConfig.CoyoteJumpTime))
			.Execute(() => {
				if (MotionY > PlayerConfig.StartFallingSpeed) AnimationFall.Play();
				PlatformBody.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
			})
			.If(() => Attack.IsJustPressed).Send(PlayerEvent.Attack)
			.If(() => Jump.IsJustPressed && CanJump()).Set(PlayerState.Jumping)
			.If(() => Float.IsJustPressed).Set(PlayerState.Floating)
			.If(() => coyoteTimer.IsAlarm()).Set(PlayerState.Fall)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.Build();

		OnTransition += (args) => {
			if (args is { From: PlayerState.FallWithCoyote, To: PlayerState.Jumping }) {
				_coyoteMonitor?.Show($"{coyoteTimer.Elapsed:0.00} <= {PlayerConfig.CoyoteJumpTime:0.00} Done!");				
			} else if (Jump.IsJustPressed) {
				_coyoteMonitor?.Show($"{coyoteTimer.Elapsed:0.00} > {PlayerConfig.CoyoteJumpTime:0.00} TOO LATE");
			}
		};
				
		State(PlayerState.Fall)
			.OnInput(InventoryHandler)
			.Execute(() => {
				if (MotionY > PlayerConfig.StartFallingSpeed) AnimationFall.Play();
				PlatformBody.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor);
			})
			.If(() => Attack.IsJustPressed).Send(PlayerEvent.Attack)
			.If(() => Float.IsJustPressed).Set(PlayerState.Floating)
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
			.If(() => Float.IsJustPressed).Set(PlayerState.Fall)
			.Exit(() => CharacterBody2D.MotionMode = CharacterBody2D.MotionModeEnum.Grounded)
			.Build();

		State(PlayerState.Hurting)
			.Enter(() => {
				Status.Invincible = true;
				weaponSpriteVisible = _weaponSprite.Visible;
				AnimationHurt.Play();
			})
			.Execute(() => {
				ApplyAirGravity();
			})
			.If(() => AnimationHurt.IsPlaying()).Stay()
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
