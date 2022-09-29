using System;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.OnReady;
using Betauer.StateMachine;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Enemy;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Character {
    public sealed class EnemyZombieController : KinematicBody2D {
        private readonly Logger _logger = LoggerFactory.GetLogger<EnemyZombieController>();

        [OnReady("Sprite")] private Sprite _mainSprite;
        [OnReady("AttackArea")] private Area2D _attackArea;
        [OnReady("DamageArea")] private Area2D _damageArea;
        [OnReady("../Label")] private Label Label;
        [OnReady("Position2D")] private Position2D _position2D;
        [OnReady("Sprite/AnimationPlayer")] private AnimationPlayer _animationPlayer;
        [OnReady("RayCasts/SlopeDetector")] private RayCast2D _slopeDetector;

        [Inject] private EnemyZombieStateMachineNode StateMachineNode { get; set; }  // Transient
        [Inject] private CharacterManager _characterManager { get; set; }

        public ILoopStatus AnimationIdle { get; private set; }
        public IOnceStatus AnimationStep { get; private set; }
        public IOnceStatus AnimationDieRight { get; private set; }
        public IOnceStatus AnimationDieLeft { get; private set; }

        private AnimationStack _animationStack;

        public override void _Ready() {
            _animationStack = new AnimationStack("Zombie.AnimationStack").SetAnimationPlayer(_animationPlayer);
            AnimationIdle = _animationStack.AddLoopAnimation("Idle");
            AnimationStep = _animationStack.AddOnceAnimation("Step");
            AnimationDieRight = _animationStack.AddOnceAnimation("DieRight");
            AnimationDieLeft = _animationStack.AddOnceAnimation("DieLeft");

            var flippers = new FlipperList().AddSprite(_mainSprite).AddNode2D(_attackArea);
            StateMachineNode.Start("Zombie", this, flippers, _slopeDetector, _position2D);

            _characterManager.ConfigureEnemyCollisions(this);
            // CharacterManager.ConfigureEnemyAttackArea2D(_attack);
            _characterManager.ConfigureEnemyDamageArea2D(_damageArea);
        }

        public override void _PhysicsProcess(float delta) {
            // if (_characterManager.PlayerController?.PlayerDetector?.Position != null) {
                // Label.Text = "" + DegreesTo(_characterManager.PlayerController.PlayerDetector);
            // }
            /*
            _label.Text = "Floor: " + IsOnFloor() + "\n" +
                          "Slope: " + IsOnSlope() + "\n" +
                          "Stair: " + IsOnSlopeStairs() + "\n" +
                          "Moving: " + IsOnMovingPlatform() + "\n" +
                          "Falling: " + IsOnFallingPlatform();
            */
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

        public void AttackedByPlayer(PlayerController playerController) {
            StateMachineNode.TriggerAttacked();
        }
    }
}