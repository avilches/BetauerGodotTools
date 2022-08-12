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
        private readonly string _name;
        private readonly Logger _logger;

        [OnReady("Sprite")] private Sprite _mainSprite;
        [OnReady("AttackArea")] private Area2D _attackArea;
        [OnReady("DamageArea")] private Area2D _damageArea;
        [OnReady("../Label")] private Label Label;
        [OnReady("Position2D")] public Position2D _position2D;
        [OnReady("Sprite/AnimationPlayer")] private AnimationPlayer _animationPlayer;

        [Inject] private EnemyZombieStateMachine _stateMachine { get; set; }
        [Inject] private CharacterManager _characterManager { get; set; }
        [Inject] public KinematicPlatformMotionBody KinematicPlatformMotionBody { get; set; }

        public ILoopStatus AnimationIdle { get; private set; }
        public IOnceStatus AnimationStep { get; private set; }
        public IOnceStatus AnimationDieRight { get; private set; }
        public IOnceStatus AnimationDieLeft { get; private set; }

        public readonly EnemyConfig EnemyConfig = new EnemyConfig();

        public IFlipper _flippers;

        public EnemyZombieController() {
            _name = "Enemy.Zombie:" + GetHashCode().ToString("x8");
            _logger = LoggerFactory.GetLogger(_name);
        }

        private AnimationStack _animationStack;

        public override void _Ready() {
            _animationStack = new AnimationStack(_name, _animationPlayer);
            AnimationIdle = _animationStack.AddLoopAnimation("Idle");
            AnimationStep = _animationStack.AddOnceAnimation("Step");
            AnimationDieRight = _animationStack.AddOnceAnimation("DieRight");
            AnimationDieLeft = _animationStack.AddOnceAnimation("DieLeft");

            _flippers = new FlipperList().AddSprite(_mainSprite).AddNode2D(_attackArea);
            KinematicPlatformMotionBody.Configure(this, _flippers, _name, EnemyConfig.MotionConfig);

            _stateMachine.Configure(this, _name);

            _characterManager.ConfigureEnemyCollisions(this);
            // CharacterManager.ConfigureEnemyAttackArea2D(_attack);
            _characterManager.ConfigureEnemyDamageArea2D(_damageArea);
        }

        public override void _PhysicsProcess(float delta) {
            if (_characterManager.PlayerController?.PlayerDetector?.Position != null) {
                Label.Text = "" + DegreesTo(_characterManager.PlayerController.PlayerDetector);
            }
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


        public bool IsFacingRight => _flippers.IsFacingRight;


        /**
         * node is | I'm facing  | flip?
         * right   | right       | no
         * right   | left        | yes
         * left    | right       | yes
         * left    | left        | no
         *
         */
        public void FaceTo(Node2D node2D) {
            if (IsToTheRightOf(node2D) != _flippers.IsFacingRight) {
                _flippers.Flip();
            }
        }

        public bool IsToTheLeftOf(Node2D node2D) {
            return Math.Abs(DegreesTo(node2D) - 180f) < 0.1f;
        }

        public bool IsToTheRightOf(Node2D node2D) {
            return DegreesTo(node2D) == 0f;
        }

        public float DegreesTo(Node2D node2D) {
            return Mathf.Rad2Deg(ToLocal(node2D.GlobalPosition).AngleTo(_position2D.Position));
        }

        public override void _Draw() {
            // if (CharacterManager.PlayerController?._playerDetector?.Position != null) {
            // DrawLine(ToLocal(CharacterManager.PlayerController._playerDetector.GlobalPosition),
            // _position2D.Position, Colors.Blue, 3F);
            // }
            // DrawLine(_position2D.Position, GetLocalMousePosition(), Colors.Blue, 3F);
        }

        public void AttackedByPlayer(PlayerController playerController) {
            _stateMachine.TriggerAttacked();
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                _animationStack?.Free(); // It's a GodotObject, it needs to be freed manually
            }
        }
    }
}