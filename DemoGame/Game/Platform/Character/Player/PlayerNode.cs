using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Animation.AnimationPlayer;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Camera;
using Betauer.Camera.Control;
using Betauer.Core;
using Betauer.Core.Nodes.Events;
using Betauer.Core.Restorer;
using Betauer.Core.Time;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Flipper;
using Betauer.FSM.Sync;
using Betauer.Input;
using Betauer.NodePath;
using Betauer.Physics;
using Godot;
using Veronenger.Game.Platform.Character.Npc;
using Veronenger.Game.Platform.Items;
using Veronenger.Game.Platform.World;

namespace Veronenger.Game.Platform.Character.Player;

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
	Idle
}

[InputEvents(Shortcut = false, UnhandledKey = false)]
[Notifications(Process = false, PhysicsProcess = false)]
public partial class PlayerNode : Node, IInjectable, INodeGameObject {

	public override partial void _Input(InputEvent @event);
	public override partial void _UnhandledInput(InputEvent @event);

	public override partial void _Notification(int what);

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
	[NodePath("Character/Marker2D")] public Marker2D Marker2D;
	[NodePath("Character/CanJump")] public RayCast2D RaycastCanJump;
	[NodePath("Character/FloorRaycasts")] public List<RayCast2D> FloorRaycasts;

	[Inject] private IMain Main { get; set; }
	[Inject] private PlatformConfig PlatformConfig { get; set; }
	[Inject] private ITransient<StageCameraController> StageCameraControllerFactory { get; set; }
	[Inject] private CameraContainer CameraContainer { get; set; }
	
	[Inject] private ITemporal<PlatformGameView> PlatformGameView { get; set; }
	[Inject] private PlatformWorld PlatformWorld => (PlatformWorld)PlatformGameView.Get().GetWorld(); 
	
	[Inject] private SceneTree SceneTree { get; set; }
	[Inject] private PlatformBus PlatformBus { get; set; }
	[Inject] private PlatformQuery PlatformQuery { get; set; }
	[Inject] private InputActionsContainer PlayerActionsContainer { get; set; }
	[Inject] private PlayerConfig PlayerConfig { get; set; }

	private readonly FsmNodeSync<PlayerState, PlayerEvent> _fsm = new(PlayerState.Idle, "Player.FSM", true);

	public StageCameraController StageCameraController { get; private set; }
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
										  PlatformConfig.IsFallingPlatform(PlatformBody
											  .GetFloorColliders<PhysicsBody2D>().FirstOrDefault());
	// private readonly LazyRaycast2D _lazyRaycast2DDrop = new();

	// private bool IsMovingPlatform() => PlatformManager.IsMovingPlatform(Body.GetFloor());
	private MonitorText? _coyoteMonitor;
	private MonitorText? _jumpHelperMonitor;
	private CameraController _cameraController;

	private CharacterWeaponController _characterWeaponController;
	private AttackState _attackState = AttackState.None;
	private readonly GodotStopwatch _stateTimer = new(false, true);

	public GameObject GameObject { get; set; }
	public PlayerGameObject PlayerGameObject => (PlayerGameObject)GameObject;

	private readonly MultiRestorer _restorer = new ();

	public void PostInject() {
		ConfigureAnimations();
		ConfigureCharacter(); // collisions are reset to 0 here
		ConfigureInventory();
		ConfigureHud();
		ConfigureCamera();
		ConfigureAttackArea();
		ConfigurePlayerHurtArea();
		ConfigureFsm();
		ConfigureInputActions();
		ConfigureOverlay();
		
		// Uncomment to discover if all the Ready methods are restoring the data correctly or there is still some property updated
		// this.OnReady(_restorer.Save, true);
		// this.OnReady(_restorer.Restore);
	
		// Uncomment to discover all properties modified during the life of the Node in the scene
		// TreeExiting += _restorer.Restore;
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

		PlatformBody = new KinematicPlatformMotion(CharacterBody2D, MotionConfig.FloorUpDirection, FloorRaycasts);
		LateralState = new LateralState(flipper, () => MotionConfig.FloorUpDirection.Rotate90Right(), () => GlobalPosition);
		Ready += () => {
			LateralState.IsFacingRight = true;
			_mainSprite.Visible = true;
			_mainSprite.Modulate = Colors.White;
		};
		
		// OnAfter += () => {
		// 	Label.Text = _animationStack.GetPlayingOnce() != null
		// 		? _animationStack.GetPlayingOnce().Name
		// 		: _animationStack.GetPlayingLoop().Name;
		// 	Label.Text += "(" + _attackState + ")";
		// };

		_characterWeaponController = new CharacterWeaponController(new[] { _attackArea1, _attackArea2 }, _weaponSprite);
		Ready += _characterWeaponController.Unequip;

		_restorer.Add(_mainSprite);
		_restorer.Add(_weaponSprite);
		_restorer.Add(CharacterBody2D);
		_restorer.Add(PlayerDetector);
		
		Ready += () => CollisionLayerConfig.PlayerConfigureCollisions(this);

		CollisionLayerConfig.PlayerPickableArea(this, OnPick);

		// _lazyRaycast2DDrop.GetDirectSpaceFrom(Marker2D);
		// _lazyRaycast2DDrop.Config(CollisionLayerManager.PlayerConfigureRaycastDrop);
	}

	private void OnPick(Area2D area2D) {
		var pickable = area2D.GetCollisionNode<PickableItemNode>();
		pickable.PlayerPicksUp(() => Marker2D.GlobalPosition, () => PickUp(pickable.PickableGameObject));
	}

	private void ConfigureInventory() {
		Inventory = new Inventory(this);
		Inventory.OnChange += e => {
			if (e.Type is PlayerInventoryEventType.Equip) {
				if (e.PickableGameObject is WeaponGameObject weapon) {
					_characterWeaponController.Equip(weapon);
				}
			} else if (e.Type is PlayerInventoryEventType.Unequip) {
				_characterWeaponController.Unequip();
			}
			PlatformBus.Publish(e);
		};
		Ready += () => {
			// Needs to be delayed until HudScene is loaded and ready
			PlatformBus.Publish(new PlayerInventoryChangeEvent(Inventory, PlayerInventoryEventType.Refresh, null!));
		};
	}

	private void ConfigureHud() {
		Ready += () => {
			// Needs to be delayed PlayerGameObject is linked with LinkNode
			PlayerGameObject.OnHealthUpdate += (phe) => PlatformBus.Publish(phe);
			PlayerGameObject.TriggerRefresh();
		};
	}

	private void ConfigureCamera() {
		StageCameraController = StageCameraControllerFactory.Create();
		StageCameraController.AddTarget(PlayerDetector);
		TreeExiting += () => {
			StageCameraController.ClearState();
			StopFollowingCamera();
		};
	}

	private void PickUp(PickableGameObject pickable) {
		// pickable objects are not removed from the gamerepository because they belong to the user in the inventory,
		// so they need to be saved. Unlink just make the node (the object in the ground) available for other uses.
		var node = pickable.UnlinkNode();
		PlatformWorld.Release(node!);
		Inventory.Pick(pickable);
	}

	public void SetCamera(Camera2D camera) {
		_cameraController = CameraContainer.Camera(camera).Follow(CharacterBody2D);
		// Uncomment to enable stages
		// StageCameraController.CurrentCamera = camera; 
	}
	
	public void StopFollowingCamera() {
		StageCameraController.CurrentCamera = null;
	}

	private void ConfigurePlayerHurtArea() {
		Ready += () => CollisionLayerConfig.PlayerConfigureHurtArea(_hurtArea);
		_restorer.Add(_hurtArea);
		OnProcess += (delta) => {
			if (PlayerGameObject is { UnderAttack: false, Invincible: false } &&
				_hurtArea.Monitoring &&
				_hurtArea.HasOverlappingAreas()) {
				var attacker = _hurtArea.GetOverlappingAreas()
					.Select(area2D => area2D.GetCollisionNode<NpcNode>())
					.MinBy(enemy => enemy.DistanceToPlayer());
				PlayerGameObject.UnderAttack = true;
				PlayerGameObject.UpdateHealth(-attacker.NpcConfig.Attack);
				if (PlayerGameObject.IsDead()) {
					_fsm.Send(PlayerEvent.Death, 10000);
				} else {
					_fsm.Send(PlayerEvent.Hurt, 10000);
				}
			}
		};
	}

	private void ConfigureAttackArea() {
		Ready += () => {
			CollisionLayerConfig.PlayerConfigureAttackArea(_attackArea1);
			CollisionLayerConfig.PlayerConfigureAttackArea(_attackArea2);
		};
		_restorer.Add(_attackArea1);
		_restorer.Add(_attackArea2);
		
		OnProcess += delta => {
			if (PlayerGameObject.AvailableHits > 0) {
				CheckAttackArea(_attackArea1);
				CheckAttackArea(_attackArea2);
			}
			
			// Monitoring flag of every area is changed during the melee attack animations 
			void CheckAttackArea(Area2D attackArea) {
				if (attackArea.Monitoring && attackArea.HasOverlappingAreas()) {
					attackArea.GetOverlappingAreas()
						.Select(area2D => area2D.GetCollisionNode<NpcNode>())
						.Where(enemy => enemy.CanBeAttacked(Inventory.WeaponMeleeEquipped))
						.OrderBy(enemy => enemy.DistanceToPlayer()) // Ascending, so first element is the closest to the player
						.Take(PlayerGameObject.AvailableHits)
						.ForEach(enemy => {
							PlayerGameObject.AvailableHits--;
							PlatformBus.Publish(new PlayerAttackEvent(this, enemy, Inventory.WeaponMeleeEquipped));
						});
				}
			}
		};
	}

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
		PlayerGameObject.AvailableHits = Inventory.WeaponMeleeEquipped!.EnemiesPerHit;
	}

	private void StopAttack() {
		// Logger.Debug(CurrentState.Key+ ": Attack ended: GodotStopwatch physics: " + _stateTimer.Elapsed);
		_attackState = AttackState.None;
		// AnimationAttack.Stop();
		PlayerGameObject.AvailableHits = 0;
	}

	public void AnimationCallback_EndAttack1() {
		if (_attackState == AttackState.Step1) {
			// Short means the player didn't attack again, so the attack ends here
			StopAttack();
		} else if (_attackState == AttackState.Step2) {
			// The player pressed attack twice, so the short attack is now a long attack, and this signal call is ignored
			PlayerGameObject.AvailableHits = Inventory.WeaponMeleeEquipped!.EnemiesPerHit * 2;
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
		PlatformWorld.PlayerDrop(item, Marker2D.GlobalPosition, dropVelocity);
		Inventory.Drop();
	}

	private void ConfigureFsm() {
		AddChild(_fsm);

		Tween? invincibleTween = null;

		void RestoreInvincibleEffect() {
			invincibleTween?.Kill();
			PlayerGameObject.Invincible = false;
			_mainSprite.Visible = true;
		}
		
		void StartInvincibleEffect() {
			const float flashTime = 0.025f;
			invincibleTween?.Kill();
			invincibleTween = CreateTween();
			invincibleTween
				.TweenCallback(Callable.From(() => _mainSprite.Visible = !_mainSprite.Visible))
				.SetDelay(flashTime);
			invincibleTween.SetLoops((int)(PlayerConfig.HurtInvincibleTime / flashTime));
			invincibleTween.Finished += RestoreInvincibleEffect;
		}

		PhysicsBody2D? fallingPlatform = null;
		void FallFromPlatform() {
			fallingPlatform = PlatformBody.GetFloorCollider<PhysicsBody2D>()!;
			PlatformConfig.RemovePlatformCollision(fallingPlatform);
		}

		void FinishFallFromPlatform() {
			if (fallingPlatform != null) PlatformConfig.ConfigurePlatformCollision(fallingPlatform);
		}

		var xInputEnterState = 0f;
		_fsm.OnTransition += (args) => {
			_stateTimer.Restart();
			xInputEnterState = XInput;
		};
		
		var shootTimer = new GodotStopwatch().Start();

		bool PlayerCanMeleeAttack() => Inventory.WeaponEquipped is WeaponMeleeGameObject;
		bool PlayerCanShoot() => Inventory.WeaponEquipped is WeaponRangeGameObject weaponRangeItem && 
								 shootTimer.Elapsed >= weaponRangeItem.DelayBetweenShots;

		_fsm.On(PlayerEvent.Idle).Set(PlayerState.Idle);
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
		
		TreeExiting += () => {
			RestoreInvincibleEffect();
			FinishFallFromPlatform();
			_fsm.Send(PlayerEvent.Idle);
		};

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

		var jumpJustInTime = false;
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
				if (e.IsKeyPressed(Key.V)) PlatformWorld.InstantiateNewZombie();
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
				PlayerGameObject.Reload(weapon);
				return;
			} 
			shootTimer.Restart();
			var bulletPosition = weapon.Config.ProjectileStartPosition * new Vector2(LateralState.FacingRight, 1);
			var bulletDirection = new Vector2(LateralState.FacingRight, 0);
			var hits = 0;
			var bullet = PlatformWorld.NewBullet();
			Inventory.UpdateWeaponRangeAmmo(weapon, -1);
			bullet.ShootFrom(weapon, CharacterBody2D.ToGlobal(bulletPosition), bulletDirection,
				CollisionLayerConfig.PlayerConfigureBulletRaycast,
				collision => {
					if (!collision.Collider.HasCollisionNode()) {
						return ProjectileTrail.Behaviour.Stop; // Something solid was hit
					}
					var npc = collision.Collider.GetCollisionNode<NpcNode>();
					if (npc.CanBeAttacked(weapon)) {
						hits++;
						PlatformBus.Publish(new PlayerAttackEvent(this, npc, weapon));
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

		var weaponSpriteVisible = false;
		_fsm.State(PlayerState.Hurting)
			.Enter(() => {
				PlayerGameObject.Invincible = true;
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
				PlayerGameObject.UnderAttack = false;
				_weaponSprite.Visible = weaponSpriteVisible;
				StartInvincibleEffect();
			})
			.Build();

		_fsm.State(PlayerState.Death)
			.Enter(() => {
				Console.WriteLine("MUERTO");
				Main.Send(MainEvent.EndGame);                                                                     
			})
			.If(() => true).Set(PlayerState.Idle)
			.Build();
	}
}
