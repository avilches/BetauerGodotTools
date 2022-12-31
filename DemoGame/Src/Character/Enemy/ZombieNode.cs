using System.Collections.Generic;
using Betauer;
using Betauer.Animation;
using Betauer.Application.Monitor;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Property;
using Betauer.DI;
using Betauer.OnReady;
using Betauer.Tools.Logging;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Managers;

namespace Veronenger.Character.Enemy; 

public interface IEnemy {
	public void AttackedByPlayer(Attack attack);
}
	
	
public partial class ZombieNode : CharacterBody2D, IEnemy {
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

	[OnReady("Sprite2D")] private Sprite2D _mainSprite;
	[OnReady("AttackArea")] private Area2D _attackArea;
	[OnReady("DamageArea")] private Area2D _damageArea;
	[OnReady("Label")] public Label Label;
	[OnReady("HitLabelPosition/HitLabel")] public Label HitLabel;
	[OnReady("Sprite2D/AnimationPlayer")] private AnimationPlayer _animationPlayer;

	[OnReady("Marker2D")] public Marker2D Marker2D;
	[OnReady("RayCasts/Floor")] public RayCast2D FloorRaycast;
	[OnReady("RayCasts/FacePlayerDetector")] public RayCast2D FacePlayerDetector;
	[OnReady("RayCasts/BackPlayerDetector")] public RayCast2D BackPlayerDetector;

	[Inject] private ZombieStateMachine StateMachine { get; set; }  // Transient
	[Inject] private CharacterManager CharacterManager { get; set; }
	[Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

	public ILoopStatus AnimationIdle { get; private set; }
	public ILoopStatus AnimationStep { get; private set; }
	public IOnceStatus AnimationDieRight { get; private set; }
	public IOnceStatus AnimationDieLeft { get; private set; }

	private Tween _sceneTreeTween;
	private AnimationStack _animationStack;
	public IFlipper Flipper;

	public void PlayAnimationAttacked() {
		_sceneTreeTween?.Kill();
		_sceneTreeTween = RedFlash.Play(_mainSprite);
	}

	public override void _Ready() {
		_animationStack = new AnimationStack("Zombie.AnimationStack").SetAnimationPlayer(_animationPlayer);
		AnimationIdle = _animationStack.AddLoopAnimation("Idle");
		AnimationStep = _animationStack.AddLoopAnimation("Step");
		AnimationDieRight = _animationStack.AddOnceAnimation("DieRight");
		AnimationDieLeft = _animationStack.AddOnceAnimation("DieLeft");

		Flipper = new FlipperList()
			.AddSprite(_mainSprite)
			.AddArea2D(_attackArea)
			.AddRayCast2D(FacePlayerDetector)
			.AddRayCast2D(BackPlayerDetector);
		StateMachine.Start("Zombie", this);

		CharacterManager.EnemyConfigureCollisions(this);
		CharacterManager.EnemyConfigureCollisions(FloorRaycast);
		CharacterManager.EnemyConfigurePlayerDetector(FacePlayerDetector);
		CharacterManager.EnemyConfigurePlayerDetector(BackPlayerDetector);
		// CharacterManager.ConfigureEnemyAttackArea2D(_attack);
		CharacterManager.EnemyConfigureDamageArea2D(_damageArea, (playerAttackArea2D) => {
			GD.Print("Attacking");
			// var enemy = enemyDamageArea2DPublisher.GetParent<IEnemy>();
			AttackedByPlayer(new Attack(1f));
		});
	}

	public void DisableAll() {
		CollisionLayer = 0;
		CollisionMask = 0;
		_damageArea.CollisionLayer = 0;
		// _attackArea.CollisionLayer = 0;
		_damageArea.CollisionMask = 0;
		// _attackArea.CollisionMask = 0;
	}

	public override void _PhysicsProcess(double delta) {
		QueueRedraw();
	}

	private Color[] _colors = { Colors.Blue, Colors.Yellow, Colors.Green, Colors.Fuchsia };
	public override void _Draw() {
		DrawRaycast(FacePlayerDetector, Colors.Red);
		DrawRaycast(BackPlayerDetector, Colors.Red);
		DrawRaycast(FloorRaycast, Colors.Blue);
	}

	private void DrawRaycast(RayCast2D rayCast, Color color) {
		var targetPosition = (rayCast.Position + rayCast.TargetPosition) * rayCast.Scale;
		DrawLine(rayCast.Position, targetPosition, color, 3F);
		if (rayCast.IsColliding()) {
			targetPosition = rayCast.GetLocalCollisionPoint();
			DrawLine(rayCast.Position, rayCast.Position + targetPosition, Colors.White, 1F);
		}
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
		StateMachine.TriggerAttacked(attack);
	}
}
