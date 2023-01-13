using Betauer;
using Betauer.Animation;
using Betauer.Application.Monitor;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Property;
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
	Attacked
}

public enum ZombieState {
	Idle,
	Run,
	Landing,
	Jump,
		
	Attacked,
	Destroy,
		
	Fall
}

public class EnemyStatus {
	public float Health = 50;
	public float MaxHealth = 50;
	public float HealthPercent => Health / MaxHealth;

	public void Attack(Attack attack) {
		Health -= attack.Damage;
	}

	public bool IsDead() => Health <= 0f;
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

	private readonly Logger _logger = LoggerFactory.GetLogger<ZombieNode>();

	[OnReady("Character")] private CharacterBody2D CharacterBody2D;
	[OnReady("Character/Sprites/Body")] private Sprite2D _mainSprite;
	[OnReady("Character/Sprites/AnimationPlayer")] private AnimationPlayer _animationPlayer;
	
	[OnReady("Character/AttackArea")] private Area2D _attackArea;
	[OnReady("Character/DamageArea")] private Area2D _damageArea;
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

	[Inject] private PlayerConfig PlayerConfig { get; set; }
	[Inject] private EnemyConfig EnemyConfig { get; set; }
	[Inject] private ICharacterHandler Handler { get; set; }

	public ILoopStatus AnimationIdle { get; private set; }
	public ILoopStatus AnimationStep { get; private set; }
	public IOnceStatus AnimationDieRight { get; private set; }
	public IOnceStatus AnimationDieLeft { get; private set; }

	private Tween _sceneTreeTween;
	private AnimationStack _animationStack;

	private float XInput => Handler.Directional.XInput;
	private float YInput => Handler.Directional.YInput;
	private IAction Jump => Handler.Jump;
	private IAction Attack => Handler.Attack;
	private IAction Float => Handler.Float;
	private float MotionX => PlatformBody.MotionX;
	private float MotionY => PlatformBody.MotionY;

	public Vector2? InitialPosition { get; set; }

	public KinematicPlatformMotion PlatformBody;
	public readonly EnemyStatus Status = new();
	private readonly GodotStopwatch _stateTimer = new();

	public void PlayAnimationAttacked() {
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

		CreateStates();

		// AI
		_zombieAi = MeleeAI.Create(Handler, new MeleeAI.Sensor(this, PlatformBody, () => CharacterManager.PlayerNode.Marker2D.GlobalPosition));
		OnBeforeExecute += _zombieAi.Execute;
		OnBeforeExecute += () => Label.Text = _zombieAi.GetState();
		OnAfterExecute += _zombieAi.EndFrame;

		this.OnDraw(canvas => {
			canvas.DrawRaycast(FacePlayerDetector, Colors.Red);
			canvas.DrawRaycast(BackPlayerDetector, Colors.Red);
			canvas.DrawRaycast(FloorRaycast, Colors.Blue);
			canvas.DrawRaycast(FinishFloorRight, Colors.Blue);
			canvas.DrawRaycast(FinishFloorLeft, Colors.Blue);
		});

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
		// CharacterManager.ConfigureEnemyAttackArea2D(_attack);
		CharacterManager.EnemyConfigureDamageArea2D(_damageArea, (playerAttackArea2D) => {
			GD.Print("Enemy attacked!");
			// var enemy = enemyDamageArea2DPublisher.GetParent<IEnemy>();
			AttackedByPlayer(new Attack(1f));
		});
	}

	private void CreateAnimations() {
		_animationStack = new AnimationStack("Zombie.AnimationStack").SetAnimationPlayer(_animationPlayer);
		AnimationIdle = _animationStack.AddLoopAnimation("Idle");
		AnimationStep = _animationStack.AddLoopAnimation("Step");
		AnimationDieRight = _animationStack.AddOnceAnimation("DieRight");
		AnimationDieLeft = _animationStack.AddOnceAnimation("DieLeft");
	}

	public void DisableAll() {
		CharacterBody2D.CollisionLayer = 0;
		CharacterBody2D.CollisionMask = 0;
		_damageArea.CollisionLayer = 0;
		// _attackArea.CollisionLayer = 0;
		_damageArea.CollisionMask = 0;
		// _attackArea.CollisionMask = 0;
	}

	public void ResetHit() {
		HitLabel.Visible = false;
		HitLabel.Text = "";
	}
	
	private Tween? _tweenHit;
	public void ShowHit(int hit) {
		HitLabel.Text = hit.ToString();
		HitLabel.Visible = true;
		HitLabel.Modulate = Colors.White;
		HitLabel.Position = Vector2.Zero;
		_tweenHit?.Kill();
		_tweenHit = GetTree().CreateTween();
		_tweenHit.Parallel().TweenProperty(HitLabel, "position:y", -15, 1.2f).SetDelay(0.1);
		_tweenHit.Parallel().TweenProperty(HitLabel, "modulate:a", 0, 1.2f).SetDelay(0.1);
	}

	public void AttackedByPlayer(Attack attack) {
		ShowHit((int)attack.Damage);
		TriggerAttacked(attack);
	}

	public void ApplyFloorGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.FloorGravity * factor, PlayerConfig.MaxFallingSpeed);
	}

	public void ApplyAirGravity(float factor = 1.0F) {
		PlatformBody.ApplyGravity(PlayerConfig.AirGravity * factor, PlayerConfig.MaxFallingSpeed);
	}

	private ICharacterAI _zombieAi;

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
	
	public void CreateStates() {    

		On(ZombieEvent.Attacked).Then(context => IsState(ZombieState.Attacked) ? context.None() : context.Push(ZombieState.Attacked));
		On(ZombieEvent.Dead).Then(context=> context.Set(ZombieState.Destroy));
			
		State(ZombieState.Landing)
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
			.If(() => !PlatformBody.IsOnFloor()).Set(ZombieState.Fall)
			.If(() => XInput != 0).Set(ZombieState.Run)
			.Build();

		State(ZombieState.Run)
			.Enter(() => {
				AnimationStep.PlayLoop();
			})
			.Execute(() => {
				PlatformBody.Flip(XInput);
				ApplyFloorGravity();
				PlatformBody.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed, 
					EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
			})
			.If(() => Jump.IsJustPressed()).Set(ZombieState.Jump)
			.If(() => !PlatformBody.IsOnFloor()).Set(ZombieState.Fall)
			.If(() => XInput == 0 && MotionX == 0).Set(ZombieState.Idle)
			.Build();

		State(ZombieState.Attacked)
			.Enter(() => {
				PlatformBody.MotionY = EnemyConfig.MiniJumpOnAttack.y;
				PlatformBody.MotionX = EnemyConfig.MiniJumpOnAttack.x * (PlatformBody.IsToTheRightOf(CharacterManager.PlayerNode.Marker2D) ? 1 : -1);
				PlayAnimationAttacked();
				_stateTimer.Restart().SetAlarm(0.3f);
			})
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.Move();
			})
			.If(() => _stateTimer.IsAlarm()).Pop()
			.Build();

		State(ZombieState.Destroy)
			.Enter(() => {
				DisableAll();

				if (PlatformBody.IsToTheRightOf(CharacterManager.PlayerNode.Marker2D)) {
					AnimationDieRight.PlayOnce(true);
				} else {
					AnimationDieLeft.PlayOnce(true);
				}
			})
			.Execute(() => {
				if (!AnimationDieRight.Playing && !AnimationDieLeft.Playing) {
					QueueFree();
				}
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
				PlatformBody.Flip(XInput);
				PlatformBody.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed,
					EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
			})
			.If(PlatformBody.IsOnFloor).Set(ZombieState.Landing)
			.If(() => MotionY >= 0).Set(ZombieState.Fall)
			.Build();

		State(ZombieState.Fall)
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.Flip(XInput);
				PlatformBody.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed,
					EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
			})
			.If(PlatformBody.IsOnFloor).Set(ZombieState.Landing)
			.Build();
	}

	public void TriggerAttacked(Attack attack) {
		Status.Attack(attack);
		Enqueue(Status.IsDead() ? ZombieEvent.Dead : ZombieEvent.Attacked);
	}
	
}
