using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Animation.AnimationPlayer;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Camera;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Time;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Flipper;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.NodePath;
using Betauer.FSM.Sync;
using Betauer.Physics;
using Godot;
using Veronenger.Character.Npc;
using Veronenger.Config;
using Veronenger.Managers;
using Veronenger.Persistent;
using Veronenger.Transient;
using Veronenger.UI;
using Veronenger.Worlds;

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

public partial class PlayerNode : Node, IInjectable, INodeWithGameObject {

	public Vector2 GlobalPosition {
		get => CharacterBody2D.GlobalPosition;
		set => CharacterBody2D.GlobalPosition = value;
	}

	[NodePath("Character")] public CharacterBody2D CharacterBody2D;
	[NodePath("Character/Sprites/Weapon")] private Sprite2D _weaponSprite;
	[NodePath("Character/Sprites/Body")] private Sprite2D _mainSprite;
	
	[NodePath("Character/Sprites/AnimationPlayer")] private AnimationPlayer _animationPlayer;
	[NodePath("Character/AttackArea1")] private Area2D _attackArea1;
	[NodePath("Character/AttackArea2")] private Area2D _attackArea2;
	[NodePath("Character/HurtArea")] private Area2D _hurtArea;
	[NodePath("Character/RichTextLabel")] public RichTextLabel Label;
	[NodePath("Character/Detector")] public Area2D PlayerDetector;
	[NodePath("Character/Camera2D")] public Camera2D Camera2D;
	[NodePath("Character/Marker2D")] public Marker2D Marker2D;
	[NodePath("Character/CanJump")] public RayCast2D RaycastCanJump;
	[NodePath("Character/FloorRaycasts")] public List<RayCast2D> FloorRaycasts;

	[Inject] private IFactory<Game> Game { get; set; }
	[Inject] private PlatformManager PlatformManager { get; set; }
	
	[Inject] private StageManager StageManager { get; set; }
	private CameraStageLimiter _cameraStageLimiter;

	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private EventBus EventBus { get; set; }
	[Inject] private InputActionsContainer PlayerActionsContainer { get; set; }
	[Inject] private HUD HudScene { get; set; }

	private readonly FsmNodeSync<PlayerState, PlayerEvent> _fsm = new(PlayerState.Idle, "Player.FSM", true);
	
	public KinematicPlatformMotion PlatformBody { get; private set; }
	public LateralState LateralState { get; private set; }
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
	
	// private bool IsOnPlatform() => PlatformManager.IsPlatform(Body.GetFloor());
	private bool IsOnFallingPlatform() => PlatformBody.IsOnFloor() &&
										  PlatformManager.IsFallingPlatform(PlatformBody
											  .GetFloorColliders<PhysicsBody2D>().FirstOrDefault());
	// private readonly LazyRaycast2D _lazyRaycast2DDrop = new();

	// private bool IsMovingPlatform() => PlatformManager.IsMovingPlatform(Body.GetFloor());
	private MonitorText? _coyoteMonitor;
	private MonitorText? _jumpHelperMonitor;

	// private readonly DragCameraController _cameraController = new();
	private CharacterWeaponController _characterWeaponController;
	private AttackState _attackState = AttackState.None;
	private readonly GodotStopwatch _stateTimer = new(false, true);

	private PlayerStatus Status => PlayerGameObject.Status;
	private PlayerConfig PlayerConfig => PlayerGameObject.Config;

	public GameObject GameObject { get; set; }

	private PlayerGameObject PlayerGameObject => (PlayerGameObject)GameObject;

	public void PostInject() {
		AddChild(_fsm);
		ConfigureAnimations();
		ConfigureOverlay();
		ConfigureCharacter(); // collisions are reset to 0 here
		ConfigureInventory();
		ConfigureStatus();
		ConfigureCamera();
		ConfigureAttackArea();
		ConfigurePlayerHurtArea();
		ConfigureFsm();
		ConfigureInputActions();
		
		var drawEvent = this.OnDraw(canvas => {
			foreach (var floorRaycast in FloorRaycasts) canvas.DrawRaycast(floorRaycast, Colors.Red);
			canvas.DrawRaycast(RaycastCanJump, Colors.Red);
		});
		drawEvent.Disable();

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

		PlatformBody = new KinematicPlatformMotion(CharacterBody2D, MotionConfig.FloorUpDirection, FloorRaycasts);
		LateralState = new LateralState(flipper, () => MotionConfig.FloorUpDirection.Rotate90Right(), () => GlobalPosition);
		// OnAfter += () => {
		// 	Label.Text = _animationStack.GetPlayingOnce() != null
		// 		? _animationStack.GetPlayingOnce().Name
		// 		: _animationStack.GetPlayingLoop().Name;
		// 	Label.Text += "(" + _attackState + ")";
		// };

		_characterWeaponController = new CharacterWeaponController(new[] { _attackArea1, _attackArea2 }, _weaponSprite);
		_characterWeaponController.Unequip();

		CollisionLayerManager.PlayerConfigureCollisions(this);
		CollisionLayerManager.PlayerPickableArea(this, area2D => {
			var pickable = area2D.GetNodeFromMeta<PickableItemNode>();
			pickable.PlayerPicksUp(() => Marker2D.GlobalPosition, () => PickUp(pickable.PickableGameObject));
		});

		// _lazyRaycast2DDrop.GetDirectSpaceFrom(Marker2D);
		// _lazyRaycast2DDrop.Config(CollisionLayerManager.PlayerConfigureRaycastDrop);
	}

	private void ConfigureInventory() {
		Inventory = new Inventory();
		Inventory.OnEquip += item => {
			if (item is WeaponGameObject weapon) {
				_characterWeaponController.Equip(weapon);
			}
		};
		Inventory.OnUnequip += item => { _characterWeaponController.Unequip(); };
		Ready += () => {
			// Needs to be delayed until HudScene is loaded and ready
			Inventory.OnUpdateInventory += (e) => HudScene.UpdateInventory(this, e);
			Inventory.OnSlotAmountUpdate += (e) => HudScene.UpdateAmount(this, e);
			Inventory.TriggerRefresh();
		};
	}

	public void ConfigureStatus() {
		Ready += () => {
			// Needs to be delayed until HudScene is loaded and ready
			Status.OnHealthUpdate += (phe) => HudScene.UpdateHealth(this, phe);
			Status.SetHealth(Status.MaxHealth);
		};
	}

	private void PickUp(PickableGameObject pickable) {
		pickable.UnlinkNode();
		Inventory.Pick(pickable);
	}

	public void SetViewport(SubViewport subViewport) {
		Camera2D.CustomViewport = subViewport;
		Camera2D.MakeCurrent();
	}

	private void ConfigureCamera() {
		// _cameraController.WithMouseButton(MouseButton.Middle).Attach(_camera2D);
		_cameraStageLimiter = StageManager.ConfigureStageCamera(Camera2D, PlayerDetector);
		TreeExiting += () => _cameraStageLimiter?.ClearState();
	}

	private void ConfigurePlayerHurtArea() {
		CollisionLayerManager.PlayerConfigureHurtArea(_hurtArea);
		this.OnProcess(delta => {
			if (Status is { UnderAttack: false, Invincible: false } &&
				_hurtArea.Monitoring &&
				_hurtArea.HasOverlappingAreas()) {
				var attacker = _hurtArea.GetOverlappingAreas()
					.Select(area2D => area2D.GetNodeFromMeta<NpcNode>())
					.MinBy(enemy => enemy.DistanceToPlayer());
				Status.UnderAttack = true;
				Status.UpdateHealth(-attacker.NpcConfig.Attack);
				if (Status.IsDead()) {
					_fsm.Send(PlayerEvent.Death, 10000);
				} else {
					_fsm.Send(PlayerEvent.Hurt, 10000);
				}
			}
		});
	}

	private void ConfigureAttackArea() {
		CollisionLayerManager.PlayerConfigureAttackArea(_attackArea1);
		CollisionLayerManager.PlayerConfigureAttackArea(_attackArea2);
		this.OnProcess(delta => {
			if (Status.AvailableHits > 0) {
				CheckAttackArea(_attackArea1);
				CheckAttackArea(_attackArea2);
			}
			
			void CheckAttackArea(Area2D attackArea) {
				if (attackArea.Monitoring && attackArea.HasOverlappingAreas()) {
					attackArea.GetOverlappingAreas()
						.Select(area2D => area2D.GetNodeFromMeta<NpcNode>())
						.Where(enemy => enemy.CanBeAttacked(Inventory.WeaponMeleeEquipped))
						.OrderBy(enemy => enemy.DistanceToPlayer()) // Ascending, so first element is the closest to the player
						.Take(Status.AvailableHits)
						.ForEach(enemy => {
							Status.AvailableHits--;
							EventBus.Publish(new PlayerAttackEvent(this, enemy, Inventory.WeaponMeleeEquipped));
						});
				}
			}
		});
	}

	// public new void _Input(InputEvent e) {
		// base._Input(e);
		// if (e.IsLeftDoubleClick()) _camera2D.Position = Vector2.Zero;
		// if (e.IsKeyPressed(Key.Q)) {
			// _camera2D.Zoom -= new Vector2(0.05f, 0.05f);
		// } else if (e.IsKeyPressed(Key.W)) {
			// _camera2D.Zoom = new Vector2(1, 1);
		// } else if (e.IsKeyPressed(Key.E)) {
			// _camera2D.Zoom += new Vector2(0.05f, 0.05f);
		// }
	// }

	public bool CanJump() => !RaycastCanJump.IsColliding(); 

	public void ApplyFloorGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.FloorGravity * factor, PlayerConfig.MaxFallingSpeed, (float)_fsm.Delta);
	}

	public void ApplyAirGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.AirGravity * factor, PlayerConfig.MaxFallingSpeed, (float)_fsm.Delta);
	}

	public enum AttackState {
		None,
		Start,
		Step1,
		Step2
	}

	private void StartMeleeAttack() {
		_stateTimer.Restart();
		// Logger.Debug(CurrentState.Key+ ": Attack started");
		_attackState = AttackState.Start;
		Status.AvailableHits = Inventory.WeaponMeleeEquipped!.EnemiesPerHit;
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
			Status.AvailableHits = Inventory.WeaponMeleeEquipped!.EnemiesPerHit * 2;
		}
	}

	public void AnimationCallback_EndAttack2() {
		StopAttack();
	}

	private void DropItem() {
		var item = Inventory.GetCurrent();
		if (item == null) return;
		// var minDistanceToDrop = new Vector2(PlatformBody.FacingRight * 50, 0);
		// var collision = _lazyRaycast2DDrop.From(Marker2D).To(Marker2D.GlobalPosition + minDistanceToDrop).Cast().Collision;
		// if (collision.IsColliding) return;
		// var dropVelocity = new Vector2(MotionX + (PlatformBody.FacingRight * PlayerConfig.DropLateralSpeed), MotionY);
		var dropVelocity = new Vector2(LateralState.FacingRight * Math.Max(Math.Abs(MotionX), PlayerConfig.DropLateralSpeed), MotionY);
		Game.Get().WorldScene.PlayerDrop(item, Marker2D.GlobalPosition, dropVelocity);
		Inventory.Drop();
	}

	public void ConfigureFsm() {
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

		TreeExiting += () => {
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

		// this.OnEveryProcess(0.5f, Game.InstantiateZombie);

		var xInputEnterState = 0f;
		_fsm.OnTransition += (args) => {
			_stateTimer.Restart();
			xInputEnterState = XInput;
		};
		// OnTransition += (args) => Logger.Debug(args.From +" -> "+args.To);

		
		var shootTimer = new GodotStopwatch().Start();

		bool PlayerCanMeleeAttack() => Inventory.WeaponEquipped is WeaponMeleeGameObject;

		bool PlayerCanShoot() => Inventory.WeaponEquipped is WeaponRangeGameObject weaponRangeItem && 
								 shootTimer.Elapsed >= weaponRangeItem.DelayBetweenShots;

		_fsm.On(PlayerEvent.Hurt).Set(PlayerState.Hurting);
		_fsm.On(PlayerEvent.Death).Set(PlayerState.Death);
		_fsm.On(PlayerEvent.Attack).Then(ctx => {
			if (PlayerCanMeleeAttack()) {
				return ctx.Set(PlatformBody.IsOnFloor() ? PlayerState.MeleeAttack : PlayerState.MeleeAttackAir);
			}
			if (PlayerCanShoot()) {
				return ctx.Set(PlatformBody.IsOnFloor() ? PlayerState.RangeAttack : PlayerState.RangeAttackAir);
			}
			return ctx.Stay();
		});
		
		var jumpJustInTime = false;
		var weaponSpriteVisible = false;

		void InventoryHandler(InputEvent e) {
			if (NextItem.IsEventJustPressed(e)) {
				Inventory.EquipNextItem();
				GetViewport().SetInputAsHandled();
			} else if (PrevItem.IsEventJustPressed(e)) {
				Inventory.EquipPrevItem();
				GetViewport().SetInputAsHandled();
			} else if (Drop.IsEventJustPressed(e)) {
				GetViewport().SetInputAsHandled();
				DropItem();
			}
		}

		_fsm.State(PlayerState.Landing)
			.OnInput(InventoryHandler)
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

		_fsm.State(PlayerState.Idle)
			.OnInput(InventoryHandler)
			.OnInput(e => {
				if (e.IsKeyPressed(Key.V)) Game.Get().WorldScene.InstantiateNewZombie();
			})
			.Enter(() => {
				if (AnimationShoot.IsPlaying()) AnimationIdle.Queue();
				else AnimationIdle.Play();
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.ApplyLateralFriction(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
				if (PlatformBody.IsOnWall()) PlatformBody.MotionX = 0;
				PlatformBody.Move();
			})
			.If(() => Jump.IsJustPressed && IsPressingDown && IsOnFallingPlatform()).Then(
				context => {
					FallFromPlatform();
					return context.Set(PlayerState.Fall);
				})
			.If(() => Attack.IsJustPressed).Send(PlayerEvent.Attack)	
			.If(() => Jump.IsJustPressed && CanJump()).Set(PlayerState.Jumping)
			.If(() => XInput != 0).Set(PlayerState.Running)
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.Fall)
			.Build();

		_fsm.State(PlayerState.Running)
			.OnInput(InventoryHandler)
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
				LateralState.Flip(XInput);
				PlatformBody.ApplyLateralConstantAcceleration(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.Friction, 
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor, (float)_fsm.Delta);
				PlatformBody.Move();
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

		_fsm.State(PlayerState.MeleeAttack)
			.Enter(() => {
				AnimationAttack.Play();
				StartMeleeAttack();
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.ApplyLateralFriction(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
				if (PlatformBody.IsOnWall()) PlatformBody.MotionX = 0;
				PlatformBody.Move();
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

		_fsm.State(PlayerState.MeleeAttackAir)
			.Enter(() => {
				AnimationAttack.Play();
				StartMeleeAttack();
			})
			.Execute(() => {
				ApplyFloorGravity();
				if (!PlatformBody.IsOnFloor()) {
					PlatformBody.ApplyLateralConstantAcceleration(xInputEnterState, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed,
						PlayerConfig.AirResistance,
						PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor, (float)_fsm.Delta);
				} else {
					PlatformBody.ApplyLateralFriction(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
				}
				PlatformBody.Move();
				if (_attackState == AttackState.Start) _attackState = AttackState.Step1;
			})
			.If(() => _attackState != AttackState.None).Stay()
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.Fall)
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => XInput != 0).Set(PlayerState.Running)
			.Build();

		void Shoot() {
			var weapon = Inventory.WeaponRangeEquipped;
			AnimationShoot.PlayFrom(0);
			if (weapon!.Ammo <= 0) {
				// no ammo, reload?
				Status.Reload(weapon);
				return;
			} 
			shootTimer.Restart();
			var bulletPosition = weapon.Config.ProjectileStartPosition * new Vector2(LateralState.FacingRight, 1);
			var bulletDirection = new Vector2(LateralState.FacingRight, 0);
			var hits = 0;
			var bullet = Game.Get().WorldScene.NewBullet();
			Inventory.UpdateWeaponRangeAmmo(weapon, -1);
			bullet.ShootFrom(weapon, CharacterBody2D.ToGlobal(bulletPosition), bulletDirection,
				CollisionLayerManager.PlayerConfigureBullet,
				collision => {
					if (!collision.Collider.HasMetaNodeId()) {
						return ProjectileTrail.Behaviour.Stop; // Something solid was hit
					}
					var npc = collision.Collider.GetNodeFromMeta<NpcNode>();
					if (npc.CanBeAttacked(weapon)) {
						hits++;
						EventBus.Publish(new PlayerAttackEvent(this, npc, weapon));
					}
					return hits < weapon.EnemiesPerHit ? ProjectileTrail.Behaviour.Continue : ProjectileTrail.Behaviour.Stop;
				}
			);
		}

		bool IsPlayerShooting() => Inventory.WeaponRangeEquipped is { Auto: true } ? Attack.IsPressed : Attack.IsJustPressed;

		_fsm.State(PlayerState.RangeAttack)
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.ApplyLateralFriction(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
				PlatformBody.Move();
				if (IsPlayerShooting()) Shoot();
			})
			.If(IsPlayerShooting).Stay()
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.Fall)
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => XInput != 0).Set(PlayerState.Running)
			.Build();

		_fsm.State(PlayerState.RangeAttackAir)
			.Execute(() => {
				ApplyFloorGravity();
				if (!PlatformBody.IsOnFloor()) {
					PlatformBody.ApplyLateralConstantAcceleration(xInputEnterState, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
						PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor, (float)_fsm.Delta);
				} else {
					PlatformBody.ApplyLateralFriction(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
				}
				PlatformBody.Move();
				if (IsPlayerShooting()) Shoot();
			})
			.If(IsPlayerShooting).Stay()
			.If(() => !PlatformBody.IsOnFloor()).Set(PlayerState.Fall)
			.If(() => XInput == 0).Set(PlayerState.Idle)
			.If(() => XInput != 0).Set(PlayerState.Running)
			.Build();
		
		_fsm.State(PlayerState.Jumping)
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

				LateralState.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.ApplyLateralConstantAcceleration(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor, (float)_fsm.Delta);
				PlatformBody.Move();
			})
			.If(() => Attack.IsJustPressed).Send(PlayerEvent.Attack)
			.If(() => Float.IsJustPressed).Set(PlayerState.Floating)
			.If(() => MotionY >= 0).Set(PlayerState.Fall)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.Build();

		var coyoteTimer = new GodotStopwatch();
		_fsm.State(PlayerState.FallWithCoyote)
			.OnInput(InventoryHandler)
			.Enter(() => coyoteTimer.Restart().SetAlarm(PlayerConfig.CoyoteJumpTime))
			.Execute(() => {
				if (MotionY > PlayerConfig.StartFallingSpeed) AnimationFall.Play();
				LateralState.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.ApplyLateralConstantAcceleration(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor, (float)_fsm.Delta);
				PlatformBody.Move();
			})
			.If(() => Attack.IsJustPressed).Send(PlayerEvent.Attack)
			.If(() => Jump.IsJustPressed && CanJump()).Set(PlayerState.Jumping)
			.If(() => Float.IsJustPressed).Set(PlayerState.Floating)
			.If(() => coyoteTimer.IsAlarm()).Set(PlayerState.Fall)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.Build();

		_fsm.OnTransition += (args) => {
			if (args is { From: PlayerState.FallWithCoyote, To: PlayerState.Jumping }) {
				_coyoteMonitor?.Show($"{coyoteTimer.Elapsed:0.00} <= {PlayerConfig.CoyoteJumpTime:0.00} Done!");				
			} else if (Jump.IsJustPressed) {
				_coyoteMonitor?.Show($"{coyoteTimer.Elapsed:0.00} > {PlayerConfig.CoyoteJumpTime:0.00} TOO LATE");
			}
		};
				
		_fsm.State(PlayerState.Fall)
			.OnInput(InventoryHandler)
			.Execute(() => {
				if (MotionY > PlayerConfig.StartFallingSpeed) AnimationFall.Play();
				LateralState.Flip(XInput);
				ApplyAirGravity();
				PlatformBody.ApplyLateralConstantAcceleration(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
					PlayerConfig.StopIfSpeedIsLessThan, PlayerConfig.ChangeDirectionFactor, (float)_fsm.Delta);
				PlatformBody.Move();
			})
			.If(() => Attack.IsJustPressed).Send(PlayerEvent.Attack)
			.If(() => Float.IsJustPressed).Set(PlayerState.Floating)
			.If(() => PlatformBody.IsOnFloor()).Set(PlayerState.Landing)
			.Build();

		_fsm.State(PlayerState.Floating)
			.OnInput(InventoryHandler)
			.Enter(() => CharacterBody2D.MotionMode = CharacterBody2D.MotionModeEnum.Floating)
			.Execute(() => {
				PlatformBody.ApplyConstantAcceleration(XInput, YInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed,
					PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan, 0, (float)_fsm.Delta);
				PlatformBody.Move();
			})
			.If(() => Float.IsJustPressed).Set(PlayerState.Fall)
			.Exit(() => CharacterBody2D.MotionMode = CharacterBody2D.MotionModeEnum.Grounded)
			.Build();

		_fsm.State(PlayerState.Hurting)
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

		_fsm.State(PlayerState.Death)
			.Enter(() => {
				Console.WriteLine("MUERTO");
				EventBus.Publish(MainEvent.EndGame);                                                                     
			})
			.Build();
	}
}
