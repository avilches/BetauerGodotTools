using System.Linq;
using Betauer;
using Betauer.Animation;
using Betauer.Application.Monitor;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Pool;
using Betauer.Core.Restorer;
using Betauer.Core.Time;
using Betauer.DI;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.OnReady;
using Betauer.StateMachine.Sync;
using Betauer.Tools.Logging;
using Godot;
using Veronenger.Character.Handler;
using Veronenger.Character.Player;
using Veronenger.Managers;

namespace Veronenger.Character.Enemy; 

public enum ZombieEvent {
	Dead,
	Hurt
}

public enum ZombieState {
	Idle,
	Run,
	Landing,
	Jump,
	Attacking,
		
	Hurt,
	Destroy,
		
	Fall
}

public partial class ZombieNode : StateMachineNodeSync<ZombieState, ZombieEvent> {
	public ZombieNode() : base(ZombieState.Idle, "Zombie.StateMachine", true) {
	}

	private static readonly KeyframeAnimation RedFlash = KeyframeAnimation.Create()
		.SetDuration(0.3f)
		.AnimateKeys(Properties.Modulate)
		.KeyframeTo(0.00f, Colors.White)
		.KeyframeTo(0.25f, Colors.Red)
		.KeyframeTo(0.50f, Colors.White)
		.KeyframeTo(0.75f, Colors.Red)
		.KeyframeTo(1.00f, Colors.White)
		.EndAnimate();

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
	[OnReady("Character/RayCasts/FacePlayerDetector")] public RayCast2D FacePlayerDetector;
	[OnReady("Character/RayCasts/BackPlayerDetector")] public RayCast2D BackPlayerDetector;

	[Inject] private CharacterManager CharacterManager { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

	[Inject] private EventBus EventBus { get; set; }
	[Inject] private PlayerConfig PlayerConfig { get; set; }
	[Inject] private EnemyConfig EnemyConfig { get; set; }
	[Inject] private ICharacterHandler Handler { get; set; }

	public ILoopStatus AnimationIdle { get; private set; }
	public ILoopStatus AnimationRun { get; private set; }
	public IOnceStatus AnimationAttack { get; private set; }
	public IOnceStatus AnimationReset { get; private set; }
	public IOnceStatus AnimationHurt { get; private set; }
	public IOnceStatus AnimationDead { get; private set; }

	private Tween _sceneTreeTween;
	private AnimationStack _animationStack;

	private float XInput => Handler.Directional.XInput;
	private float YInput => Handler.Directional.YInput;
	private IAction Jump => Handler.Jump;
	private IAction Attack => Handler.Attack;
	private IAction Float => Handler.Float;
	private float MotionX => PlatformBody.MotionX;
	private float MotionY => PlatformBody.MotionY;

	public KinematicPlatformMotion PlatformBody;
	public Vector2? InitialPosition { get; set; }
	public EnemyStatus Status { get; private set; }

	private ICharacterAI _zombieAi;
	private readonly GodotStopwatch _stateTimer = new();
	private MiniPool<ILabelEffect> _labelHits;
	private Restorer _restorer; 
	private bool _hitLabelUsed = false;

	public void PlayAnimationHurt() {
		_sceneTreeTween?.Kill();
		_sceneTreeTween = RedFlash.Play(_mainSprite);
	}

	public override void _Ready() {
		if (!Get("visible").As<bool>()) {
			QueueFree();
			return;
		}
		CreateAnimations();
		ConfigureCharacter();
		ConfigureStateMachine();

		// AI
		_zombieAi = MeleeAI.Create(Handler, new MeleeAI.Sensor(this, PlatformBody, () => CharacterManager.PlayerNode.Marker2D.GlobalPosition));
		OnBeforeExecute += _zombieAi.Execute;
		OnBeforeExecute += () => Label.Text = _zombieAi.GetState();
		OnAfterExecute += _zombieAi.EndFrame;

		var drawEvent = this.OnDraw(canvas => {
			canvas.DrawRaycast(FacePlayerDetector, Colors.Red);
			canvas.DrawRaycast(BackPlayerDetector, Colors.Red);
			canvas.DrawRaycast(FloorRaycast, Colors.Blue);
			canvas.DrawRaycast(FinishFloorRight, Colors.Blue);
			canvas.DrawRaycast(FinishFloorLeft, Colors.Blue);
		});
		drawEvent.Disable();

		var overlay = DebugOverlayManager.Follow(CharacterBody2D).Title("Zombie");
		AddOverlayStates(overlay);
		AddOverlayMotion(overlay);
		AddOverlayCollisions(overlay);
	}

	private void ConfigureCharacter() {
		if (InitialPosition.HasValue) CharacterBody2D.GlobalPosition = InitialPosition.Value;
		CharacterBody2D.FloorStopOnSlope = true;
		// CharacterBody2D.FloorBlockOnWall = true;
		CharacterBody2D.FloorConstantSpeed = true;
		CharacterBody2D.FloorSnapLength = MotionConfig.SnapLength;
		var flipper = new FlipperList()
			.Sprite2DFlipH(_mainSprite)
			.ScaleX(_attackArea)
			.ScaleX(FacePlayerDetector)
			.ScaleX(BackPlayerDetector);
		flipper.IsFacingRight = flipper.IsFacingRight;

		PlatformBody = new KinematicPlatformMotion(CharacterBody2D, flipper, Marker2D, MotionConfig.FloorUpDirection);
		OnBeforeExecute += () => PlatformBody.SetDelta(Delta);
		
		CharacterManager.EnemyConfigureCollisions(CharacterBody2D);
		CharacterManager.EnemyConfigureCollisions(FloorRaycast);
		CharacterManager.EnemyConfigureCollisions(FinishFloorRight);
		CharacterManager.EnemyConfigureCollisions(FinishFloorLeft);
		CharacterManager.EnemyConfigurePlayerDetector(FacePlayerDetector);
		CharacterManager.EnemyConfigurePlayerDetector(BackPlayerDetector);
		
		CharacterManager.EnemyConfigureAttackArea(_attackArea);
		_attackArea.GetNode<CollisionShape2D>("Body").Disabled = false;
		_attackArea.GetNode<CollisionShape2D>("Weapon").Disabled = true;

		CharacterManager.EnemyConfigureHurtArea(_hurtArea);
		_hurtArea.SetMeta("EnemyNodeHashCode", GetHashCode());
		EventBus.Subscribe(OnPlayerAttack).UnsubscribeIf(Predicates.IsInvalid(this));
		
		_restorer = new MultiRestorer()
			.Add(CharacterBody2D.CreateCollisionRestorer())
			.Add(_hurtArea.CreateCollisionRestorer())
			.Add(_attackArea.CreateCollisionRestorer())
			.Add(_mainSprite.CreateRestorer(Properties.Modulate, Properties.Scale2D));
		_restorer.Save();

		Status = new EnemyStatus(32);

	}

	private void OnPlayerAttack(PlayerAttack playerAttack) {
		GD.Print("Enemy: i'm attacked by player "+ GetHashCode()+": "+Status.Health);
		if (playerAttack.EnemyAttackArea.GetMeta("EnemyNodeHashCode").AsInt32() != GetHashCode()) return;
		Status.Attack(playerAttack.Weapon.Damage);
		GD.Print("Enemy: i'm attacked by player "+ GetHashCode()+": "+Status.Health);
		_labelHits.Get().Show(((int)playerAttack.Weapon.Damage).ToString());
		Enqueue(Status.IsDead() ? ZombieEvent.Dead : ZombieEvent.Hurt);
	}

	private void CreateAnimations() {
		_animationStack = new AnimationStack("Zombie.AnimationStack").SetAnimationPlayer(_animationPlayer);
		AnimationIdle = _animationStack.AddLoopAnimation("Idle");
		AnimationRun = _animationStack.AddLoopAnimation("Run");
		AnimationAttack = _animationStack.AddOnceAnimation("Attack");
		AnimationHurt = _animationStack.AddOnceAnimation("Hurt");
		AnimationDead = _animationStack.AddOnceAnimation("Dead");

		AnimationReset = _animationStack.AddOnceAnimation("RESET");

		HitLabel.Visible = false; // just in case...
		_labelHits = MiniPool<ILabelEffect>.Create()
			.Factory(() => {
				if (_hitLabelUsed) {
					var duplicate = (Label)HitLabel.Duplicate();
					HitLabel.AddSibling(duplicate);
					return new LabelHit(duplicate);
				}
				_hitLabelUsed = true;
				return new LabelHit(HitLabel);
			})
			.BusyIf(l => l.Busy)
			.InvalidIf(l => IsInstanceValid(l.Owner))
			.InitialSize(1)
			.MaxSize(10)
			.Build();
		
	}

	public void Recycle() {
		// AnimationAttack.PlayOnce();
		// _restorer.Restore();
		QueueFree();
	}

	public void DisableAll() {
		CharacterBody2D.CollisionLayer = 0;
		CharacterBody2D.CollisionMask = 0;
		_hurtArea.CollisionLayer = 0;
		_hurtArea.CollisionMask = 0;
		_attackArea.CollisionLayer = 0;
		_attackArea.CollisionMask = 0;
	}

	public void ApplyFloorGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.FloorGravity * factor, PlayerConfig.MaxFallingSpeed);
	}

	public void ApplyAirGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.AirGravity * factor, PlayerConfig.MaxFallingSpeed);
	}

	public void AddOverlayStates(DebugOverlay overlay) {    
		overlay
			.OpenBox()
				.Text("State", () => CurrentState.Key.ToString()).EndMonitor()
				.Text("IA", () => _zombieAi.GetState()).EndMonitor()
				.Text("Pos", () => {
					var playerMark = CharacterManager.PlayerNode.Marker2D;
					return PlatformBody.IsFacingTo(playerMark)?
						PlatformBody.IsToTheRightOf(playerMark)?"P <me|":"|me> P":
						PlatformBody.IsToTheRightOf(playerMark)?"P |me>":"<me| P";
				}).EndMonitor()
			.CloseBox()
			.OpenBox()
			.Angle("Player angle", () => PlatformBody.AngleTo(CharacterManager.PlayerNode.Marker2D)).EndMonitor()
			.Text("Player is", () => PlatformBody.IsToTheRightOf(CharacterManager.PlayerNode.Marker2D)?"Left":"Right").EndMonitor()
			.Text("FacingPlayer", () => PlatformBody.IsFacingTo(CharacterManager.PlayerNode.Marker2D)).EndMonitor()
			.Text("Distance", () => DistanceToPlayer().ToString()).EndMonitor()
			.CloseBox();
			
	}

	public float DistanceToPlayer() {
		return Marker2D.GlobalPosition.DistanceTo(CharacterManager.PlayerNode.Marker2D.GlobalPosition);
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
	
	public void ConfigureStateMachine() {    

		On(ZombieEvent.Hurt).Then(context => context.Set(ZombieState.Hurt));
		On(ZombieEvent.Dead).Then(context=> context.Set(ZombieState.Destroy));
			
		State(ZombieState.Landing)
			.If(() => Attack.IsJustPressed()).Set(ZombieState.Attacking)
			.If(() => XInput == 0).Set(ZombieState.Idle)
			.If(() => true).Set(ZombieState.Run)
			.Build();

		State(ZombieState.Idle)
			.Enter(() => {
				AnimationIdle.PlayLoop();
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.Stop(EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan);
			})
			.If(() => Jump.IsJustPressed()).Set(ZombieState.Jump)
			.If(() => Attack.IsJustPressed()).Set(ZombieState.Attacking)
			.If(() => !PlatformBody.IsOnFloor()).Set(ZombieState.Fall)
			.If(() => XInput != 0).Set(ZombieState.Run)
			.Build();

		State(ZombieState.Run)
			.Enter(() => {
				AnimationRun.PlayLoop();
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed, 
					EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
			})
			.If(() => Jump.IsJustPressed()).Set(ZombieState.Jump)
			.If(() => Attack.IsJustPressed()).Set(ZombieState.Attacking)
			.If(() => !PlatformBody.IsOnFloor()).Set(ZombieState.Fall)
			.If(() => XInput == 0 && MotionX == 0).Set(ZombieState.Idle)
			.Build();

		State(ZombieState.Attacking)
			.Enter(() => {
				AnimationAttack.PlayOnce(true);
			})
			.Execute(() => {
				ApplyFloorGravity();
				PlatformBody.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed, 
					EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
			})
			.If(() => !AnimationAttack.Playing && !PlatformBody.IsOnFloor()).Set(ZombieState.Fall)
			.If(() => !AnimationAttack.Playing && XInput == 0).Set(ZombieState.Idle)
			.If(() => !AnimationAttack.Playing && XInput != 0).Set(ZombieState.Run)
			.Build();

		
		Tween? knockbackTween = null;
		State(ZombieState.Hurt)
			.Enter(() => {
				PlatformBody.MotionX = EnemyConfig.Knockback.x * (PlatformBody.IsToTheRightOf(CharacterManager.PlayerNode.Marker2D) ? 1 : -1);
				AnimationHurt.PlayOnce(true);
				knockbackTween?.Kill();
				knockbackTween = CreateTween();
				knockbackTween.TweenMethod(Callable.From<float>(v => PlatformBody.MotionX = v), PlatformBody.MotionX, 0, 0.1f).SetTrans(Tween.TransitionType.Cubic);
			})
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.Move();
			})
			.If(() => !AnimationHurt.Playing).Set(ZombieState.Idle)
			.Build();

		State(ZombieState.Destroy)
			.Enter(() => {
				// DisableAll();
				AnimationDead.PlayOnce(true);
			})
			.Execute(() => {
				if (!AnimationDead.Playing) Recycle();
			})
			.Build();

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
			.If(PlatformBody.IsOnFloor).Set(ZombieState.Landing)
			.If(() => MotionY >= 0).Set(ZombieState.Fall)
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
