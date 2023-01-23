using System;
using System.Diagnostics;
using Betauer.Animation;
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
using Veronenger.Character.Handler;
using Veronenger.Character.Items;
using Veronenger.Character.Player;
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

	[Inject] private CharacterManager CharacterManager { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

	[Inject] public World World { get; set; }
	[Inject] private EventBus EventBus { get; set; }
	[Inject] private PlayerConfig PlayerConfig { get; set; }
	[Inject] public EnemyConfig EnemyConfig { get; set; }
	[Inject] private ICharacterHandler Handler { get; set; }

	public ILoopStatus AnimationIdle { get; private set; }
	public ILoopStatus AnimationRun { get; private set; }
	public IOnceStatus AnimationAttack { get; private set; }
	public IOnceStatus AnimationReset { get; private set; }
	public IOnceStatus AnimationHurt { get; private set; }
	public IOnceStatus AnimationDead { get; private set; }

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
	private MiniPool<ILabelEffect> _labelHits;
	private Restorer _restorer; 

	private EnemyItem _enemyItem;

	private Vector2 PlayerPos => CharacterManager.PlayerNode.Marker2D.GlobalPosition;
	public bool IsFacingToPlayer() => PlatformBody.IsFacingTo(PlayerPos);
	public bool IsToTheRightOfPlayer() => PlatformBody.IsToTheRightOf(PlayerPos);
	public float AngleToPlayer() => PlatformBody.AngleTo(PlayerPos);
	public float DistanceToPlayer() => PlatformBody.DistanceTo(PlayerPos);
	public Vector2 DirectionToPlayer() => PlatformBody.DirectionTo(PlayerPos);
	public bool CanSeeThePlayer() => IsFacingToPlayer() &&
									 DistanceToPlayer() <= EnemyConfig.VisionDistance &&
									 Mathf.Acos(Mathf.Abs(PlatformBody.LookRightDirection.Dot(DirectionToPlayer()))) <= EnemyConfig.VisionAngle &&
									 Marker2D.RaycastTo(PlayerPos, ray => CharacterManager.EnemyConfigureCollisions(ray)).Count == 0;

	
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
		OnBeforeExecute += _zombieAi.Execute;
		OnBeforeExecute += () => Label.Text = _zombieAi.GetState();
		OnAfterExecute += _zombieAi.EndFrame;

		var drawRaycasts = this.OnDraw(canvas => {
			// canvas.DrawRaycast(FacePlayerDetector, Colors.Red);
			// canvas.DrawRaycast(BackPlayerDetector, Colors.Red);
			// canvas.DrawRaycast(FloorRaycast, Colors.Blue);
			canvas.DrawRaycast(FinishFloorRight, Colors.Blue);
			canvas.DrawRaycast(FinishFloorLeft, Colors.Blue);
		});
		// drawRaycasts.Disable();

		var drawPlayerInsight = this.OnDraw(canvas => {
			// Same conditions as CanSeeThePlayer
			if (!IsFacingToPlayer() ||
			    DistanceToPlayer() > EnemyConfig.VisionDistance ||
			    Mathf.Acos(Mathf.Abs(PlatformBody.LookRightDirection.Dot(DirectionToPlayer()))) > EnemyConfig.VisionAngle) {
				var distance = new Vector2(EnemyConfig.VisionDistance, 0);
				var direction = new Vector2(PlatformBody.FacingRight, 1);
				canvas.DrawLine(Marker2D.GlobalPosition, Marker2D.GlobalPosition + distance.Rotated(-EnemyConfig.VisionAngle) * direction, Colors.Gray);
				canvas.DrawLine(Marker2D.GlobalPosition, Marker2D.GlobalPosition + distance.Rotated(+EnemyConfig.VisionAngle) * direction, Colors.Gray);
				canvas.DrawLine(Marker2D.GlobalPosition, Marker2D.GlobalPosition + distance * direction, Colors.Gray);
				return;
			}

			var result = Marker2D.RaycastTo(PlayerPos, ray => CharacterManager.EnemyConfigureCollisions(ray));
			if (result.Count > 0) {
				canvas.DrawLine(Marker2D.GlobalPosition, result["position"].AsVector2(), Colors.Red, 2);
				return;
			}
			canvas.DrawLine(Marker2D.GlobalPosition, PlayerPos, Colors.Lime, 2);
		});
		// drawPlayerInsight.Disable();

		var overlay = DebugOverlayManager.Follow(CharacterBody2D).Title("Zombie");
		AddOverlayStates(overlay);
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
		OnBeforeExecute += () => PlatformBody.SetDelta(Delta);
		
		CharacterManager.EnemyConfigureCollisions(CharacterBody2D);
		CharacterManager.EnemyConfigureCollisions(FloorRaycast);
		CharacterManager.EnemyConfigureCollisions(FinishFloorRight);
		CharacterManager.EnemyConfigureCollisions(FinishFloorLeft);
		
		_enemyItem = World.CreateEnemy(this);

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
	}

	private void OnPlayerAttackEvent(PlayerAttackEvent playerAttackEvent) {
		if (playerAttackEvent.Enemy.Id != _enemyItem.Id) return;
		Debug.Assert(Status.UnderAttack == false, "Status.UnderAttack == false");
		Status.UnderAttack = true;
		Status.Hurt(playerAttackEvent.Weapon.Damage);
		_labelHits.Get().Show(((int)playerAttackEvent.Weapon.Damage).ToString());
		Send(Status.IsDead() ? ZombieEvent.Death : ZombieEvent.Hurt);
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
		var hitLabelUsed = false;
		_labelHits = MiniPool<ILabelEffect>.Create()
			.Factory(() => {
				if (hitLabelUsed) {
					var duplicate = (Label)HitLabel.Duplicate();
					HitLabel.AddSibling(duplicate);
					return new LabelHit(duplicate);
				}
				hitLabelUsed = true;
				return new LabelHit(HitLabel);
			})
			.BusyIf(l => l.Busy)
			.InvalidIf(l => IsInstanceValid(l.Owner))
			.InitialSize(1)
			.MaxSize(10)
			.Build();
		
	}

	public void Recycle() {
		// AnimationReset.PlayOnce();
		// _restorer.Restore();
		World.Remove(_enemyItem);
		QueueFree();
	}

	public void DisableCollisions() {
		CharacterBody2D.CollisionLayer = 0;
		CharacterBody2D.CollisionMask = 0;
	}

	public void DisableAttack() {
		_attackArea.Monitoring = false;
		_attackArea.Monitorable = false;
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
	
	public void ConfigureStateMachine() {    

		On(ZombieEvent.Hurt).Set(ZombieState.Hurt);
		On(ZombieEvent.Death).Set(ZombieState.Death);
			
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
				_animationPlayer.PlaybackSpeed = Math.Abs(XInput);
				PlatformBody.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed, 
					EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
				
			})
			.If(() => Jump.IsJustPressed()).Set(ZombieState.Jump)
			.If(() => Attack.IsJustPressed()).Set(ZombieState.Attacking)
			.If(() => !PlatformBody.IsOnFloor()).Set(ZombieState.Fall)
			.If(() => XInput == 0 && MotionX == 0).Set(ZombieState.Idle)
			.Exit(() => _animationPlayer.PlaybackSpeed = 1)
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

		Tween? redFlash = null;
		Tween? knockbackTween = null;
		State(ZombieState.Hurt)
			.Enter(() => {
				PlatformBody.MotionX = EnemyConfig.HurtKnockback.x * (IsToTheRightOfPlayer() ? 1 : -1);
				PlatformBody.MotionY = EnemyConfig.HurtKnockback.y;
				AnimationHurt.PlayOnce(true);
				knockbackTween?.Kill();
				knockbackTween = CreateTween();
				knockbackTween.TweenMethod(Callable.From<float>(v => PlatformBody.MotionX = v), PlatformBody.MotionX, 0, EnemyConfig.HurtKnockbackTime).SetTrans(Tween.TransitionType.Cubic);
				redFlash?.Kill();
				redFlash = RedFlash.Play(_mainSprite, 0); 
			})
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.Move();
			})
			.If(() => !AnimationHurt.Playing).Set(ZombieState.Idle)
			.Exit(() => {
				Status.UnderAttack = false;
			})
			.Build();

		State(ZombieState.Death)
			.Enter(() => {
				DisableAttack();
				AnimationDead.PlayOnce(true);
			})
			.Execute(() => {
				ApplyAirGravity();
				PlatformBody.Move();
			})
			.If(() => !AnimationDead.Playing).Set(ZombieState.End)
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
