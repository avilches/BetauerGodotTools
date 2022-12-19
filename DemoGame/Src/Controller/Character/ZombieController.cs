using System;
using System.Collections.Generic;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Animation;
using Betauer.Application.Monitor;
using Betauer.Core.Nodes;
using Betauer.DI;
using Betauer.Tools.Logging;
using Betauer.Core.Nodes.Property;
using Betauer.OnReady;
using Veronenger.Character.Enemy;
using Veronenger.Managers;

namespace Veronenger.Controller.Character {
	public interface IEnemy {
		public void AttackedByPlayer(Attack attack);
	}
	
	
	public partial class ZombieController : CharacterBody2D, IEnemy {
		private static readonly KeyframeAnimation RedFlash = KeyframeAnimation.Create()
			.SetDuration(0.3f)
			.AnimateKeys(Properties.Modulate)
			.KeyframeTo(0.00f, Colors.White)
			.KeyframeTo(0.25f, Colors.Red)
			.KeyframeTo(0.50f, Colors.White)
			.KeyframeTo(0.75f, Colors.Red)
			.KeyframeTo(1.00f, Colors.White)
			.EndAnimate();

		private readonly Logger _logger = LoggerFactory.GetLogger<ZombieController>();

		[OnReady("Sprite2D")] private Sprite2D _mainSprite;
		[OnReady("AttackArea")] private Area2D _attackArea;
		[OnReady("DamageArea")] private Area2D _damageArea;
		[OnReady("Label")] public Label Label;
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
		private List<RayCast2D> _rayCasts;
		private AnimationStack _animationStack;
		private readonly FlipperList _flippers = new();
		public bool IsFacingRight => _flippers.IsFacingRight;

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

			// _rayCasts = new List<RayCast2D> { FloorRaycast, FacePlayerDetector, BackPlayerDetector };
			_rayCasts = new List<RayCast2D> { FacePlayerDetector };

			_flippers
				.AddSprite(_mainSprite)
				.AddArea2D(_attackArea)
				.AddRayCast2D(FacePlayerDetector)
				.AddRayCast2D(BackPlayerDetector);
			StateMachine.Start("Zombie", this, _flippers, Marker2D);

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

			DebugOverlayManager.CreateOverlay()
				.Follow(this)
				.RemoveButtons()
				.Offset(new Vector2(0, -100))
				.Text("State", () => StateMachine.CurrentState.Key.ToString()).EndMonitor()
				// .Text("Mouse", () => $"{Position.DistanceTo(GetLocalMousePosition()):F1} {Position.AngleTo(GetLocalMousePosition()):F1}").EndMonitor()
				.Text("Speed",() => StateMachine.Body.Motion.ToString("F")).EndMonitor();
		}

		public void DisableAll() {
			CollisionLayer = 0;
			CollisionMask = 0;
			_damageArea.CollisionLayer = 0;
			// _attackArea.CollisionLayer = 0;
			_damageArea.CollisionMask = 0;
			// _attackArea.CollisionMask = 0;
		}

		public override void _Draw() {
			// if (CharacterManager.PlayerController?._playerDetector?.Position != null) {
			// DrawLine(ToLocal(CharacterManager.PlayerController._playerDetector.GlobalPosition),
			// _position2D.Position, Colors.Blue, 3F);
			// }
			// DrawLine(_position2D.Position, GetLocalMousePosition(), Colors.Blue, 3F);
		}

		public void AttackedByPlayer(Attack attack) {
			StateMachine.TriggerAttacked(attack);
		}
	}
}
