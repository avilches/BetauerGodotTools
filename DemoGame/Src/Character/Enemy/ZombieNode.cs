using System;
using Betauer.Animation;
using Betauer.Animation.AnimationPlayer;
using Betauer.Application.Monitor;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Pool;
using Betauer.Core.Restorer;
using Betauer.DI;
using Betauer.Flipper;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.OnReady;
using Betauer.StateMachine.Sync;
using Betauer.Tools.Logging;
using Godot;
using Pcg;
using Veronenger.Character.InputActions;
using Veronenger.Character.Player;
using Veronenger.Items;
using Veronenger.Managers;

namespace Veronenger.Character.Enemy; 

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


public partial class ZombieNode : StateMachineNodeSync<ZombieState, ZombieEvent> {
	public ZombieNode() : base(ZombieState.Idle, "Zombie.StateMachine", true) {
	}

	private static readonly PcgRandom Random = new();

	private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(ZombieNode));

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

	[OnReady("Character")] private CharacterBody2D CharacterBody2D;
	[OnReady("Character/Sprites/Body")] private Sprite2D _mainSprite;
	[OnReady("Character/Sprites/AnimationPlayer")] private AnimationPlayer _animationPlayer;
	
	[OnReady("Character/AttackArea")] private Area2D _attackArea;
	[OnReady("Character/HurtArea")] private Area2D _hurtArea;
	[OnReady("Character/Label")] public Label Label;
	[OnReady("Character/HitLabelPosition/HitLabel")] public Label HitLabel;
	[OnReady("Character/Marker2D")] public Marker2D Marker2D;
	[OnReady("Character/RayCasts/FinishFloorLeft")] public RayCast2D FinishFloorLeft;
	[OnReady("Character/RayCasts/FinishFloorRight")] public RayCast2D FinishFloorRight;
	[OnReady("Character/RayCasts/Floor")] public RayCast2D FloorRaycast;
	[OnReady("Character/HealthBarPosition/HealthBar")] public TextureProgressBar HealthBar;

	[Inject] private CharacterManager CharacterManager { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

	[Inject] public World World { get; set; }
	[Inject] private EventBus EventBus { get; set; }
	[Inject] private PlayerConfig PlayerConfig { get; set; }
	[Inject] public EnemyConfig EnemyConfig { get; set; }
	
	//
	// [Inject] private InputActionCharacterHandler Handler { get; set; }
	private NpcController Handler { get; set; } = new NpcController();

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
	public Vector2? InitialPosition { get; set; }
	public EnemyStatus Status { get; private set; }

	private ICharacterAI _zombieAi;
	private MiniPoolBusy<ILabelEffect> _labelHits;
	private Restorer _restorer; 
	private EnemyItem _enemyItem;
	private LazyRaycast2D _lazyRaycastToPlayer;

	private Vector2 PlayerPos => CharacterManager.PlayerNode.Marker2D.GlobalPosition;
	public bool IsFacingToPlayer() => PlatformBody.IsFacingTo(PlayerPos);
	public bool IsToTheRightOfPlayer() => PlatformBody.IsToTheRightOf(PlayerPos);
	public int RightOfPlayer() => IsToTheRightOfPlayer() ? 1 : -1;
	public float AngleToPlayer() => PlatformBody.AngleTo(PlayerPos);
	public float DistanceToPlayer() => PlatformBody.DistanceTo(PlayerPos);
	public Vector2 DirectionToPlayer() => PlatformBody.DirectionTo(PlayerPos);
	public bool CanSeeThePlayer() => IsFacingToPlayer() &&
									 DistanceToPlayer() <= EnemyConfig.VisionDistance &&
									 IsPlayerInAngle() &&
									 !_lazyRaycastToPlayer.From(Marker2D).To(PlayerPos).Cast().Collision.IsColliding;
	
	public bool IsPlayerInAngle() => EnemyConfig.VisionAngle > 0 && 
									 Mathf.Acos(Mathf.Abs(PlatformBody.LookRightDirection.Dot(DirectionToPlayer()))) <= EnemyConfig.VisionAngle;

	
	public override void _Ready() {
		if (!Get("visible").As<bool>()) {                                                                                                          
			QueueFree();
			return;
		}
		CreateAnimations();
		ConfigureCharacter();
		ConfigureStateMachine();

		// AI
		_zombieAi = MeleeAI.Create(Handler, new MeleeAI.Sensor(this, PlatformBody, () => PlayerPos));
		OnBefore += () =>_zombieAi.Execute();
		OnBefore += () => Label.Text = _zombieAi.GetState();
		OnAfter += () => _zombieAi.EndFrame();

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
				DistanceToPlayer() > EnemyConfig.VisionDistance ||
				!IsPlayerInAngle()) {
				var distance = new Vector2(EnemyConfig.VisionDistance, 0);
				var direction = new Vector2(PlatformBody.FacingRight, 1);
				canvas.DrawLine(Marker2D.GlobalPosition, Marker2D.GlobalPosition + distance.Rotated(-EnemyConfig.VisionAngle) * direction, Colors.Gray);
				canvas.DrawLine(Marker2D.GlobalPosition, Marker2D.GlobalPosition + distance.Rotated(+EnemyConfig.VisionAngle) * direction, Colors.Gray);
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

		// var overlay = DebugOverlayManager.Follow(CharacterBody2D).Title("Zombie");
		// AddHurtStates(overlay);
		// AddOverlayStates(overlay);
		// AddOverlayCrossAndDot(overlay);
		// AddOverlayMotion(overlay);
		// AddOverlayCollisions(overlay);
	}

	private void ConfigureCharacter() {
		if (InitialPosition.HasValue) CharacterBody2D.GlobalPosition = InitialPosition.Value;
		CharacterBody2D.FloorStopOnSlope = true;
		// CharacterBody2D.FloorBlockOnWall = true;
		CharacterBody2D.FloorConstantSpeed = true;
		CharacterBody2D.FloorSnapLength = MotionConfig.SnapLength;
		var flipper = new FlipperList()
			.Sprite2DFlipH(_mainSprite)
			.ScaleX(_attackArea);
		flipper.IsFacingRight = flipper.IsFacingRight;

		PlatformBody = new KinematicPlatformMotion(CharacterBody2D, flipper, Marker2D, MotionConfig.FloorUpDirection);
		OnBefore += () => PlatformBody.SetDelta(Delta);
		
		CharacterManager.EnemyConfigureCollisions(CharacterBody2D);
		CharacterManager.EnemyConfigureCollisions(FloorRaycast);
		CharacterManager.EnemyConfigureCollisions(FinishFloorRight);
		CharacterManager.EnemyConfigureCollisions(FinishFloorLeft);
		
		_enemyItem = World.CreateEnemy(this);
		_lazyRaycastToPlayer = new LazyRaycast2D(_mainSprite)
			.Config(ray => CharacterManager.EnemyConfigureCollisions(ray));

		CharacterManager.EnemyConfigureAttackArea(_attackArea);
		_attackArea.GetNode<CollisionShape2D>("Body").Disabled = false;
		_attackArea.GetNode<CollisionShape2D>("Weapon").Disabled = true;
		_attackArea.SetWorldId(_enemyItem);

		CharacterManager.EnemyConfigureHurtArea(_hurtArea);
		_hurtArea.SetWorldId(_enemyItem);
		EventBus.Subscribe(OnPlayerAttackEvent).UnsubscribeIf(Predicates.IsInvalid(this));
		
		_restorer = new MultiRestorer()
			.Add(CharacterBody2D.CreateCollisionRestorer())
			.Add(_hurtArea.CreateCollisionRestorer())
			.Add(_attackArea.CreateCollisionRestorer())
			.Add(_mainSprite.CreateRestorer(Properties.Modulate, Properties.Scale2D));
		_restorer.Save();

		Status = new EnemyStatus(EnemyConfig.InitialMaxHealth, EnemyConfig.InitialHealth);
		UpdateHealthBar();
	}

	private void UpdateHealthBar() {
		if (EnemyConfig.HealthBarVisible) {
			HealthBar.MinValue = 0;
			HealthBar.MaxValue = Status.MaxHealth;
			HealthBar.Value = Status.Health;
		}
		HealthBar.Visible = EnemyConfig.HealthBarVisible;
	}

	public bool CanBeAttacked(WeaponItem weapon) {
		if (weapon is WeaponMeleeItem) return !Status.UnderMeleeAttack;
		if (weapon is WeaponRangeItem) return true;
		return true;
	}

	private void OnPlayerAttackEvent(PlayerAttackEvent playerAttackEvent) {
		if (playerAttackEvent.Enemy.Id != _enemyItem.Id) return;
		if (playerAttackEvent.Weapon is WeaponMeleeItem) {
			Status.UnderMeleeAttack = true;
			Kickback(30, 80, playerAttackEvent.Weapon.Damage * 15);
		} else if (playerAttackEvent.Weapon is WeaponRangeItem) {
			Kickback(0, 25, playerAttackEvent.Weapon.Damage * 10);
		}
		Status.UpdateHealth(-playerAttackEvent.Weapon.Damage);
		UpdateHealthBar();
		_labelHits.Get().Show(((int)playerAttackEvent.Weapon.Damage).ToString());
		Send(Status.IsDead() ? ZombieEvent.Death : ZombieEvent.Hurt);
	}

	private void CreateAnimations() {
		AnimationIdle = _animationPlayer.Anim("Idle");
		AnimationRun = _animationPlayer.Anim("Run");
		AnimationAttack = _animationPlayer.Anim("Attack");
		AnimationHurt = _animationPlayer.Anim("Hurt");
		AnimationDead = _animationPlayer.Anim("Dead");

		AnimationReset = _animationPlayer.Anim("RESET");

		var firstCall = true;
		_labelHits = new MiniPoolBusy<ILabelEffect>(
			() => {
				if (firstCall) {
					firstCall = false;
					return new LabelHit(HitLabel);
				}
				var duplicate = (Label)HitLabel.Duplicate();
				HitLabel.AddSibling(duplicate);
				return new LabelHit(duplicate);
			}, 1, false);
	}

	public void Recycle() {
		// AnimationReset.PlayOnce();
		// _restorer.Restore();
		World.Remove(_enemyItem);
		EnableAttackAndHurtAreas();
		QueueFree();
	}

	public void DisableCollisions() {
		CharacterBody2D.CollisionLayer = 0;
		CharacterBody2D.CollisionMask = 0;
	}

	public void EnableAttackAndHurtAreas(bool enabled = true) {
		_attackArea.Monitoring = enabled;
		_attackArea.Monitorable = enabled;
		_hurtArea.EnableAllShapes(enabled);
	}

	public void ApplyFloorGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.FloorGravity * factor, PlayerConfig.MaxFallingSpeed);
	}

	public void ApplyAirGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.AirGravity * factor, PlayerConfig.MaxFallingSpeed);
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
				.Text("State", () => CurrentState.Key.ToString()).EndMonitor()
				.Text("IA", () => _zombieAi.GetState()).EndMonitor()
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
				.Text("Dot", () => PlatformBody.LookRightDirection.Dot(DirectionToPlayer()).ToString("0.00")).EndMonitor()
				.Text("Cross", () => PlatformBody.LookRightDirection.Cross(DirectionToPlayer()).ToString("0.00")).EndMonitor()
				.Text("Acos(Dot)", () => Mathf.RadToDeg(Mathf.Acos(Math.Abs(PlatformBody.LookRightDirection.Dot(DirectionToPlayer())))).ToString("0.00")).EndMonitor()
				.Text("Acos(Cross)", () => Mathf.RadToDeg(Mathf.Acos(Math.Abs(PlatformBody.LookRightDirection.Cross(DirectionToPlayer())))).ToString("0.00")).EndMonitor()
			.CloseBox()
			.OpenBox()
				.Text("SameDir", () => PlatformBody.LookRightDirection.IsSameDirection(DirectionToPlayer())).EndMonitor()
				.Text("OppDir", () => PlatformBody.LookRightDirection.IsOppositeDirection(DirectionToPlayer())).EndMonitor()
				.Text("IsRight", () => PlatformBody.LookRightDirection.IsRight(DirectionToPlayer())).EndMonitor()
				.Text("IsLeft", () => PlatformBody.LookRightDirection.IsLeft(DirectionToPlayer())).EndMonitor()
			.CloseBox()
			.OpenBox()
				.Text("SameDirA", () => PlatformBody.LookRightDirection.IsSameDirectionAngle(DirectionToPlayer())).EndMonitor()
				.Text("OppDirA", () => PlatformBody.LookRightDirection.IsOppositeDirectionAngle(DirectionToPlayer())).EndMonitor()
				.Text("IsRightA", () => PlatformBody.LookRightDirection.IsRightAngle(DirectionToPlayer())).EndMonitor()
				.Text("IsLeftA", () => PlatformBody.LookRightDirection.IsLeftAngle(DirectionToPlayer())).EndMonitor()
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

	private void Kickback(int startAngle, int endAngle, float energy) {
		var angle = Random.Range(startAngle, endAngle);
		energy = Random.Range(Math.Max(0, energy-10), energy+10);
		var dir = Vector2.Right.Rotated(Mathf.DegToRad(angle)) * energy;
		PlatformBody.MotionX = IsToTheRightOfPlayer() ? dir.X : -dir.X;
		PlatformBody.MotionY = -dir.Y;
	}

	public void ConfigureStateMachine() {    

		On(ZombieEvent.Hurt).Set(ZombieState.Hurt);
		On(ZombieEvent.Death).Set(ZombieState.Death);
			
		State(ZombieState.Landing)
			.If(() => Attack.IsJustPressed).Set(ZombieState.Attacking)
			.If(() => XInput == 0).Set(ZombieState.Idle)
			.If(() => true).Set(ZombieState.Run)
			.Build();

		State(ZombieState.Idle)
			.Enter(() => {
				AnimationIdle.Play();
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.Stop(EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan);
			})
			.If(() => Jump.IsJustPressed).Set(ZombieState.Jump)
			.If(() => Attack.IsJustPressed).Set(ZombieState.Attacking)
			.If(() => !PlatformBody.IsOnFloor()).Set(ZombieState.Fall)
			.If(() => XInput != 0).Set(ZombieState.Run)
			.Build();

		State(ZombieState.Run)
			.Enter(() => {
				AnimationRun.Play();
			})
			.Execute(() => {
				ApplyFloorGravity();
				_animationPlayer.SpeedScale = Math.Abs(XInput);
				PlatformBody.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed, 
					EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
				
			})
			.If(() => Jump.IsJustPressed).Set(ZombieState.Jump)
			.If(() => Attack.IsJustPressed).Set(ZombieState.Attacking)
			.If(() => !PlatformBody.IsOnFloor()).Set(ZombieState.Fall)
			.If(() => XInput == 0 && MotionX == 0).Set(ZombieState.Idle)
			.Exit(() => _animationPlayer.SpeedScale = 1)
			.Build();

		State(ZombieState.Attacking)
			.Enter(() => {
				AnimationAttack.Play();
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed, 
					EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
			})
			.If(() => AnimationAttack.IsPlaying()).Stay()
			.If(() => !PlatformBody.IsOnFloor()).Set(ZombieState.Fall)
			.If(() => XInput == 0).Set(ZombieState.Idle)
			.If(() => XInput != 0).Set(ZombieState.Run)
			.Build();

		Tween? redFlash = null;
		State(ZombieState.Hurt)
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
				Status.UnderMeleeAttack = false;
			})
			.Build();

		State(ZombieState.Death)
			.Enter(() => {
				EnableAttackAndHurtAreas(false);
				HealthBar.Visible = false;
				AnimationDead.Play();
			})
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.Stop(EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan);
			})
			.If(() => !AnimationDead.IsPlaying()).Set(ZombieState.End)
			.Build();

		State(ZombieState.End).Enter(Recycle).Build();

		State(ZombieState.Jump)
			.Enter(() => {
				PlatformBody.MotionY = -PlayerConfig.JumpSpeed;
				// DebugJump($"Jump start: decelerating to {(-PlayerConfig.JumpSpeed).ToString()}");
				// _player.AnimationJump.PlayLoop();
			})
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed,
					EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
			})
			.If(() => MotionY >= 0).Set(ZombieState.Fall)
			.If(PlatformBody.IsOnFloor).Set(ZombieState.Landing)
			.Build();

		State(ZombieState.Fall)
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed,
					EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
			})
			.If(PlatformBody.IsOnFloor).Set(ZombieState.Landing)
			.Build();
	}
}
