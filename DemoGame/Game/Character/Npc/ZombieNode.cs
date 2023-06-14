using System;
using Betauer.Animation;
using Betauer.Animation.AnimationPlayer;
using Betauer.Application.Monitor;
using Betauer.Application.Persistent;
using Betauer.Bus;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Pool.Lifecycle;
using Betauer.Core.Restorer;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Holder;
using Betauer.Flipper;
using Betauer.FSM.Sync;
using Betauer.Input;
using Betauer.NodePath;
using Betauer.Nodes;
using Betauer.Physics;
using Godot;
using Veronenger.Game.Character.InputActions;
using Veronenger.Game.Character.Player;
using Veronenger.Game.Items;
using Veronenger.Game.Platform;
using Veronenger.Game.Worlds.Platform;

namespace Veronenger.Game.Character.Npc; 

public enum ZombieEvent {
	Hurt,
	Death,
}

public enum ZombieState {
	Idle,
	Run,
	Landing,
	Jump,
	Attacking,
		
	Hurt,
	Death,
	End,
		
	Fall
}

public partial class ZombieNode : NpcNode, IInjectable {
	
	private static readonly SequenceAnimation RedFlash;

	static ZombieNode() {
		const float redFlashTime = 0.05f;
		const float totalTime = 0.2f;
		const int steps = (int)(totalTime / redFlashTime / 2);
		var colorAnimation = SequenceAnimation.Create()
			.AnimateSteps(Properties.Modulate)
			.From(Colors.White);
		for (var i = 0; i < steps; i++) {
			colorAnimation.To(Colors.Red, redFlashTime);
			colorAnimation.To(Colors.White, redFlashTime);
		}
		RedFlash = colorAnimation.EndAnimate();
	}

	[NodePath("Character")] private CharacterBody2D CharacterBody2D;
	[NodePath("Character/Sprites/Body")] private Sprite2D _mainSprite;
	[NodePath("Character/Sprites/AnimationPlayer")] private AnimationPlayer _animationPlayer;
	
	[NodePath("Character/AttackArea")] private Area2D _attackArea;
	[NodePath("Character/HurtArea")] private Area2D _hurtArea;
	[NodePath("Character/Label")] public Label Label;
	[NodePath("Character/HitLabelPosition/HitLabel")] public Label HitLabel;
	[NodePath("Character/Marker2D")] public Marker2D Marker2D;
	[NodePath("Character/RayCasts/FinishFloorLeft")] public RayCast2D FinishFloorLeft;
	[NodePath("Character/RayCasts/FinishFloorRight")] public RayCast2D FinishFloorRight;
	[NodePath("Character/RayCasts/Floor")] public RayCast2D FloorRaycast;
	[NodePath("Character/HealthBarPosition/HealthBar")] public TextureProgressBar HealthBar;
	[NodePath("Character/Sprites/BloodParticles")] public GpuParticles2D BloodParticles;

	[Inject] private MainStateMachine MainStateMachine { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
	[Inject] private EventBus EventBus { get; set; }
	[Inject] private PlayerConfig PlayerConfig { get; set; }
	[Inject("PlatformWorldHolder")] private IHolder<PlatformWorld> PlatformWorld { get; set; } 
	
	// [Inject] private InputActionCharacterHandler Handler { get; set; }
	private NpcController Handler { get; set; } = new NpcController();

	[Inject] private GameObjectRepository GameObjectRepository { get; set; }

	public Anim AnimationIdle { get; private set; }
	public Anim AnimationRun { get; private set; }
	public Anim AnimationAttack { get; private set; }
	public Anim AnimationReset { get; private set; }
	public Anim AnimationHurt { get; private set; }
	public Anim AnimationDead { get; private set; }

	private float XInput => Handler.Lateral.Strength;
	private float YInput => Handler.Vertical.Strength;
	private InputAction Jump => Handler.Jump;
	private InputAction Attack => Handler.Attack;
	private InputAction Float => Handler.Float;
	private float MotionX => PlatformBody.MotionX;
	private float MotionY => PlatformBody.MotionY;

	public KinematicPlatformMotion PlatformBody;
	public LateralState LateralState;

	private readonly FsmSync<ZombieState, ZombieEvent> _fsm = new(ZombieState.Idle, "Zombie.FSM");
	private float _delta;
	private ICharacterAi _zombieAi;
	private BasePool<ILabelEffect> _labelHits;
	private Restorer _restorer; 
	private LazyRaycast2D _lazyRaycastToPlayer;
	private DebugOverlay? _overlay;

	private Vector2 PlayerPos => PlatformWorld.Get().ClosestPlayer(Marker2D.GlobalPosition).Marker2D.GlobalPosition;
	public bool IsFacingToPlayer() => LateralState.IsFacingTo(PlayerPos);
	public bool IsToTheRightOfPlayer() => LateralState.IsToTheRightOf(PlayerPos);
	public int RightOfPlayer() => IsToTheRightOfPlayer() ? 1 : -1;
	public float AngleToPlayer() => Marker2D.GlobalPosition.AngleTo(DirectionToPlayer());
	public override float DistanceToPlayer() => Marker2D.GlobalPosition.DistanceTo(PlayerPos);
	public Vector2 DirectionToPlayer() => Marker2D.GlobalPosition.DirectionTo(PlayerPos);
	public bool CanSeeThePlayer() => IsFacingToPlayer() &&
									 DistanceToPlayer() <= NpcConfig.VisionDistance &&
									 IsPlayerInAngle() &&
									 !_lazyRaycastToPlayer.From(Marker2D).To(PlayerPos).Cast().Collision.IsColliding;
	
	public bool IsPlayerInAngle() => NpcConfig.VisionAngle > 0 && 
									 Mathf.Acos(Mathf.Abs(RightVector.Dot(DirectionToPlayer()))) <= NpcConfig.VisionAngle;

	private Vector2 RightVector => PlatformBody.UpRightNormal.Rotate90Right();

	private Multicast<object, PlayerAttackEvent>.EventConsumer consumer;

	public override Vector2 GlobalPosition {
		get => CharacterBody2D.GlobalPosition;
		set => CharacterBody2D.GlobalPosition = value;
	}

	public override Vector2 Velocity {
		get => PlatformBody.Motion;
		set => PlatformBody.Motion = value;
	}

	public override bool IsFacingRight {
		get => LateralState.IsFacingRight;
		set => LateralState.IsFacingRight = value;
	}

	public void PostInject() {
		ConfigureAnimations();
		ConfigureCharacter();
		ConfigureFsm();
		ConfigureAi();

		// ConfigureOverlays();
		ConfigureOverlayRays();
				
		// Uncomment to discover if all the Ready methods are restoring the data correctly or there is still some property updated
		// this.OnReady(_restorer.Save, true);
		// this.OnReady(_restorer.Restore);
	
		// Uncomment to discover all properties modified during the life of the Node in the scene
		// TreeExiting += _restorer.Restore;

	}
	
	private void ConfigureOverlayRays() { 
		var drawRaycasts = this.OnDraw(canvas => {
			// canvas.DrawRaycast(FacePlayerDetector, Colors.Red);
			// canvas.DrawRaycast(BackPlayerDetector, Colors.Red);
			// canvas.DrawRaycast(FloorRaycast, Colors.Blue);
			canvas.DrawRaycast(FinishFloorRight, Colors.Blue);
			canvas.DrawRaycast(FinishFloorLeft, Colors.Blue);
		});
		drawRaycasts.Disable();

		var drawPlayerInsight = this.OnDraw(canvas => {
			// Same conditions as CanSeeThePlayer
			if (!IsFacingToPlayer() ||
				DistanceToPlayer() > NpcConfig.VisionDistance ||
				!IsPlayerInAngle()) {
				var distance = new Vector2(NpcConfig.VisionDistance, 0);
				var direction = new Vector2(LateralState.FacingRight, 1);
				canvas.DrawLine(Marker2D.GlobalPosition, Marker2D.GlobalPosition + distance.Rotated(-NpcConfig.VisionAngle) * direction, Colors.Gray);
				canvas.DrawLine(Marker2D.GlobalPosition, Marker2D.GlobalPosition + distance.Rotated(+NpcConfig.VisionAngle) * direction, Colors.Gray);
				canvas.DrawLine(Marker2D.GlobalPosition, Marker2D.GlobalPosition + distance * direction, Colors.Gray);
				return;
			}

			var result = _lazyRaycastToPlayer.From(Marker2D).To(PlayerPos).Cast().Collision;
			if (result.IsColliding) {
				canvas.DrawLine(Marker2D.GlobalPosition, result.Position, Colors.Red, 2);
				return;
			}
			canvas.DrawLine(Marker2D.GlobalPosition, PlayerPos, Colors.Lime, 2);
		});
		drawPlayerInsight.Disable();
	}

	private void ConfigureAi() {
		_zombieAi = MeleeAi.Create(Handler, new MeleeAi.Sensor(this, PlatformBody, LateralState, () => PlayerPos, () => _delta));
		_fsm.OnBefore += () => _zombieAi.Execute();
		_fsm.OnBefore += () => Label.Text = _zombieAi.GetState();
		_fsm.OnAfter += () => _zombieAi.EndFrame();
		Ready += _fsm.Reset;
		Ready += _zombieAi.Reset;
	}

	private void ConfigureOverlays() {
		_overlay = DebugOverlayManager.Follow(CharacterBody2D).Title("Zombie");
		AddHurtStates(_overlay);
		AddOverlayStates(_overlay);
		AddOverlayPlayerInfo(_overlay);
		AddOverlayCrossAndDot(_overlay);
		AddOverlayMotion(_overlay);
		AddOverlayCollisions(_overlay);
		Ready += () => _overlay?.Enable();
		TreeExiting += () => _overlay?.Disable();
	}

	private void ConfigureAnimations() {
		AnimationIdle = _animationPlayer.Anim("Idle");
		AnimationRun = _animationPlayer.Anim("Run");
		AnimationAttack = _animationPlayer.Anim("Attack");
		AnimationHurt = _animationPlayer.Anim("Hurt");
		AnimationDead = _animationPlayer.Anim("Dead");
		AnimationReset = _animationPlayer.Anim("RESET");

		Ready += () => {
			_mainSprite.Modulate = Colors.White;
			_mainSprite.Visible = true;
			BloodParticles.Emitting = false;
		};

		var firstCall = true;
		_labelHits = PoolTemplates.Lifecycle<ILabelEffect>(
			() => {
				if (firstCall) {
					firstCall = false;
					return new LabelHit(HitLabel);
				}
				var duplicate = (Label)HitLabel.Duplicate();
				HitLabel.AddSibling(duplicate);
				return new LabelHit(duplicate);
			});
		_labelHits.Fill(1);
	}

	private void ConfigureCharacter() {
		CharacterBody2D.FloorStopOnSlope = true;
		// CharacterBody2D.FloorBlockOnWall = true;
		CharacterBody2D.FloorConstantSpeed = true;
		CharacterBody2D.FloorSnapLength = MotionConfig.SnapLength;
		var flipper = new FlipperList()
			.Sprite2DFlipH(_mainSprite)
			.ScaleX(_attackArea);
		
		LateralState = new LateralState(flipper, () => CharacterBody2D.UpDirection.Rotate90Right(), () => Marker2D.GlobalPosition);
		PlatformBody = new KinematicPlatformMotion(CharacterBody2D, MotionConfig.FloorUpDirection);
		_lazyRaycastToPlayer = new LazyRaycast2D().Config(CollisionLayerConfig.NpcConfigureCollisions);
		_attackArea.SetCollisionNode(this);
		_hurtArea.SetCollisionNode(this);

		Ready += () => {
			CollisionLayerConfig.NpcConfigureCollisions(CharacterBody2D);
			CollisionLayerConfig.NpcConfigureCollisions(FloorRaycast);
			CollisionLayerConfig.NpcConfigureCollisions(FinishFloorRight);
			CollisionLayerConfig.NpcConfigureCollisions(FinishFloorLeft);
			CollisionLayerConfig.EnemyConfigureAttackArea(_attackArea);
			CollisionLayerConfig.EnemyConfigureHurtArea(_hurtArea);
			_lazyRaycastToPlayer.UseDirectSpace(_mainSprite.GetWorld2D().DirectSpaceState);
			UpdateHealthBar();
			consumer = EventBus.Subscribe(OnPlayerAttackEvent).UnsubscribeIf(Predicates.IsInvalid(this));
		};

		TreeExiting += () => consumer.Unsubscribe();
		
		_restorer = new MultiRestorer()
			.Add(CharacterBody2D.CreateRestorer())
			.Add(_hurtArea.CreateRestorer())
			.Add(_attackArea.CreateRestorer())
			.Add(_mainSprite.CreateRestorer());
	}

	private void UpdateHealthBar() {
		if (NpcConfig.HealthBarVisible) {
			HealthBar.MinValue = 0;
			HealthBar.MaxValue = NpcGameObject.MaxHealth;
			HealthBar.Value = NpcGameObject.Health;
		}
		HealthBar.Visible = NpcConfig.HealthBarVisible;
	}

	public override bool CanBeAttacked(WeaponGameObject weapon) {
		if (weapon is WeaponMeleeGameObject) return !NpcGameObject.UnderMeleeAttack;
		if (weapon is WeaponRangeGameObject) return true;
		return true;
	}

	private void OnPlayerAttackEvent(PlayerAttackEvent playerAttackEvent) {
		if (playerAttackEvent.NpcNode != this) return;
		if (playerAttackEvent.Weapon is WeaponMeleeGameObject) {
			NpcGameObject.UnderMeleeAttack = true;
			Kickback(NpcConfig.KickbackMeleeAngle.start, NpcConfig.KickbackMeleeAngle.end, playerAttackEvent.Weapon.Damage * NpcConfig.KickbackMeleeEnergyMultiplier);
		} else if (playerAttackEvent.Weapon is WeaponRangeGameObject) {
			Kickback(NpcConfig.KickbackRangeAngle.start, NpcConfig.KickbackRangeAngle.end, playerAttackEvent.Weapon.Damage * NpcConfig.KickbackRangeEnergyMultiplier);
		}
		NpcGameObject.UpdateHealth(-playerAttackEvent.Weapon.Damage);
		UpdateHealthBar();
		_labelHits.Get().Show(((int)playerAttackEvent.Weapon.Damage).ToString());
		_fsm.Send(NpcGameObject.IsDead() ? ZombieEvent.Death : ZombieEvent.Hurt);
	}

	// TODO: this could be moved to the weapon config?
	private void Kickback(int startAngle, int endAngle, float energy) {
		var angle = Random.Range(startAngle, endAngle);
		energy = Random.Range(Math.Max(0, energy - NpcConfig.KickbackEnergyThreshold), energy + NpcConfig.KickbackEnergyThreshold);
		var dir = Vector2.Right.Rotated(Mathf.DegToRad(angle)) * energy;                                                                           
		PlatformBody.MotionX = IsToTheRightOfPlayer() ? dir.X : -dir.X;
		PlatformBody.MotionY = -dir.Y;
	}

	public void ApplyFloorGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.FloorGravity * factor, PlayerConfig.MaxFallingSpeed, _delta);
	}

	public void ApplyAirGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.AirGravity * factor, PlayerConfig.MaxFallingSpeed, _delta);
	}

	public void AddHurtStates(DebugOverlay overlay) {
		overlay
		.OpenBox()
			.Text("Hurting", () => _hurtArea.Monitoring).EndMonitor()
			.Text("Hurtable", () => _hurtArea.Monitorable).EndMonitor()
		.CloseBox();
	}

	public void AddOverlayStates(DebugOverlay overlay) {
		overlay
		.OpenBox()
			.Text("State", () => _fsm.CurrentState.Key.ToString()).EndMonitor()
			.Text("IA", () => _zombieAi.GetState()).EndMonitor()
			.Text("Animation", () => _animationPlayer.CurrentAnimation).EndMonitor()
			.Text("GameObjectId", () => NpcGameObject.Id.ToString()).EndMonitor()
			.Text("ObjectId", () => GetInstanceId().ToString()).EndMonitor()
		.CloseBox();
	}

	public void AddOverlayPlayerInfo(DebugOverlay overlay) {    
		overlay
			.OpenBox()
			.Text("Pos", () => IsFacingToPlayer()?
											IsToTheRightOfPlayer()?"P <me|":"|me> P":
											IsToTheRightOfPlayer()?"P |me>":"<me| P").EndMonitor()
			.Text("See Player", CanSeeThePlayer).EndMonitor()
		.CloseBox()
		.OpenBox()
			.Angle("Player angle", AngleToPlayer).EndMonitor()
			.Text("Player is", () => IsToTheRightOfPlayer()?"Left":"Right").EndMonitor()
			.Text("FacingPlayer", IsFacingToPlayer).EndMonitor()
			.Text("Distance", () => DistanceToPlayer().ToString()).EndMonitor()
		.CloseBox();
	}

	public void AddOverlayCrossAndDot(DebugOverlay overlay) {    
		overlay
			.OpenBox()
				.Text("Dot", () => RightVector.Dot(DirectionToPlayer()).ToString("0.00")).EndMonitor()
				.Text("Cross", () => RightVector.Cross(DirectionToPlayer()).ToString("0.00")).EndMonitor()
				.Text("Acos(Dot)", () => Mathf.RadToDeg(Mathf.Acos(Math.Abs(RightVector.Dot(DirectionToPlayer())))).ToString("0.00")).EndMonitor()
				.Text("Acos(Cross)", () => Mathf.RadToDeg(Mathf.Acos(Math.Abs(RightVector.Cross(DirectionToPlayer())))).ToString("0.00")).EndMonitor()
			.CloseBox()
			.OpenBox()
				.Text("SameDir", () => RightVector.IsSameDirection(DirectionToPlayer())).EndMonitor()
				.Text("OppDir", () => RightVector.IsOppositeDirection(DirectionToPlayer())).EndMonitor()
				.Text("IsRight", () => RightVector.IsRight(DirectionToPlayer())).EndMonitor()
				.Text("IsLeft", () => RightVector.IsLeft(DirectionToPlayer())).EndMonitor()
			.CloseBox()
			.OpenBox()
				.Text("SameDirA", () => RightVector.IsSameDirectionAngle(DirectionToPlayer())).EndMonitor()
				.Text("OppDirA", () => RightVector.IsOppositeDirectionAngle(DirectionToPlayer())).EndMonitor()
				.Text("IsRightA", () => RightVector.IsRightAngle(DirectionToPlayer())).EndMonitor()
				.Text("IsLeftA", () => RightVector.IsLeftAngle(DirectionToPlayer())).EndMonitor()
			.CloseBox()
			.OpenBox()
			.CloseBox();
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
	
	public void AddOverlayCollisions(DebugOverlay overlay) {    
		overlay
			.Graph("Floor", () => PlatformBody.IsOnFloor()).Keep(10).SetChartHeight(10)
				.AddSerie("Slope").Load(() => PlatformBody.IsOnSlope()).EndSerie()
			.EndMonitor()
			.Text("Floor", () => PlatformBody.GetFloorCollisionInfo()).EndMonitor()
			.Text("Ceiling", () => PlatformBody.GetCeilingCollisionInfo()).EndMonitor()
			.Text("Wall", () => PlatformBody.GetWallCollisionInfo()).EndMonitor();
	}

	public override void _PhysicsProcess(double delta) {
		_delta = (float)delta;
		_fsm.Execute();
	}

	public void ConfigureFsm() {    

		_fsm.On(ZombieEvent.Hurt).Set(ZombieState.Hurt);
		_fsm.On(ZombieEvent.Death).Set(ZombieState.Death);
			
		_fsm.State(ZombieState.Landing)
			.If(() => Attack.IsJustPressed).Set(ZombieState.Attacking)
			.If(() => XInput == 0).Set(ZombieState.Idle)
			.If(() => true).Set(ZombieState.Run)
			.Build();

		_fsm.State(ZombieState.Idle)
			.Enter(() => {
				AnimationIdle.Play();
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.ApplyLateralFriction(NpcConfig.Friction, NpcConfig.StopIfSpeedIsLessThan);
				if (PlatformBody.IsOnWall()) PlatformBody.MotionX = 0;
				PlatformBody.Move();
			})
			.If(() => Jump.IsJustPressed).Set(ZombieState.Jump)
			.If(() => Attack.IsJustPressed).Set(ZombieState.Attacking)
			.If(() => !PlatformBody.IsOnFloor()).Set(ZombieState.Fall)
			.If(() => XInput != 0).Set(ZombieState.Run)
			.Build();

		_fsm.State(ZombieState.Run)
			.Enter(() => {
				AnimationRun.Play();
			})
			.Execute(() => {
				ApplyFloorGravity();
				_animationPlayer.SpeedScale = Math.Abs(XInput);
				PlatformBody.ApplyLateralConstantAcceleration(XInput, NpcConfig.Acceleration, NpcConfig.MaxSpeed, 
					NpcConfig.Friction, NpcConfig.StopIfSpeedIsLessThan, 0, _delta);
				PlatformBody.Move();
				
			})
			.If(() => Jump.IsJustPressed).Set(ZombieState.Jump)
			.If(() => Attack.IsJustPressed).Set(ZombieState.Attacking)
			.If(() => !PlatformBody.IsOnFloor()).Set(ZombieState.Fall)
			.If(() => XInput == 0 && MotionX == 0).Set(ZombieState.Idle)
			.Exit(() => _animationPlayer.SpeedScale = 1)
			.Build();

		_fsm.State(ZombieState.Attacking)
			.Enter(() => {
				AnimationAttack.Play();
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.ApplyLateralConstantAcceleration(XInput, NpcConfig.Acceleration, NpcConfig.MaxSpeed, 
					NpcConfig.Friction, NpcConfig.StopIfSpeedIsLessThan, 0, _delta);
				PlatformBody.Move();
			})
			.If(() => AnimationAttack.IsPlaying()).Stay()
			.If(() => !PlatformBody.IsOnFloor()).Set(ZombieState.Fall)
			.If(() => XInput == 0).Set(ZombieState.Idle)
			.If(() => XInput != 0).Set(ZombieState.Run)
			.Build();

		Tween? redFlash = null;
		_fsm.State(ZombieState.Hurt)
			.Enter(() => {
				AnimationHurt.Play();
				redFlash?.Kill();
				redFlash = RedFlash.Play(_mainSprite, 0); 
			})
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.Move();
			})
			.If(() => !AnimationHurt.IsPlaying() && PlatformBody.IsOnFloor()).Set(ZombieState.Idle)
			.Exit(() => {
				NpcGameObject.UnderMeleeAttack = false;
			})
			.Build();

		_fsm.State(ZombieState.Death)
			.Enter(() => {
				CollisionLayerConfig.DisableAttackAndHurtAreas(_attackArea, _hurtArea);
				HealthBar.Visible = false;
				AnimationDead.Play();
			})
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.ApplyLateralFriction(NpcConfig.Friction, NpcConfig.StopIfSpeedIsLessThan);
				if (PlatformBody.IsOnWall()) PlatformBody.MotionX = 0;
				PlatformBody.Move();
			})
			.If(() => !AnimationDead.IsPlaying()).Set(ZombieState.End)
			.Build();

		_fsm.State(ZombieState.End)
			.Enter(() => GameObjectRepository.Remove(NpcGameObject))
			.If(() => true).Set(ZombieState.Idle)
			.Build();

		_fsm.State(ZombieState.Jump)
			.Enter(() => {
				PlatformBody.MotionY = -PlayerConfig.JumpSpeed;
				// DebugJump($"Jump start: decelerating to {(-PlayerConfig.JumpSpeed).ToString()}");
				// _player.AnimationJump.PlayLoop();
			})
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.ApplyLateralConstantAcceleration(XInput, NpcConfig.Acceleration, NpcConfig.MaxSpeed,
					NpcConfig.Friction, NpcConfig.StopIfSpeedIsLessThan, 0, _delta);
				PlatformBody.Move();
			})
			.If(() => MotionY >= 0).Set(ZombieState.Fall)
			.If(PlatformBody.IsOnFloor).Set(ZombieState.Landing)
			.Build();

		_fsm.State(ZombieState.Fall)
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.ApplyLateralConstantAcceleration(XInput, NpcConfig.Acceleration, NpcConfig.MaxSpeed,
					NpcConfig.Friction, NpcConfig.StopIfSpeedIsLessThan, 0, _delta);
				PlatformBody.Move();
			})
			.If(PlatformBody.IsOnFloor).Set(ZombieState.Landing)
			.Build();
	}

	public bool IsState(ZombieState state) => _fsm.IsState(state);
}
