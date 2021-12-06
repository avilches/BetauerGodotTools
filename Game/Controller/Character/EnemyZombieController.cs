using System;
using Godot;
using Tools;
using Tools.Statemachine;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Enemy;
using Veronenger.Game.Character.Enemy.States;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Character {
    public sealed class EnemyZombieController : DiKinematicBody2D {
        private readonly string _name;
        private readonly Logger _logger;
        private readonly StateMachine _stateMachine;
        [OnReady("Sprite")] private Sprite _mainSprite;
        [OnReady("AttackArea")] private Area2D _attackArea;
        [OnReady("DamageArea")] private Area2D _damageArea;
        [OnReady("../Label")] protected Label Label;
        [OnReady("Position2D")] public Position2D _position2D;
        [OnReady("Sprite/AnimationPlayer")] private AnimationPlayer _animationPlayer;

        [Inject] public CharacterManager CharacterManager;

        public ILoopStatus AnimationIdle { get; private set; }
        public IOnceStatus AnimationStep { get; private set; }
        public IOnceStatus AnimationDieRight { get; private set; }
        public IOnceStatus AnimationDieLeft { get; private set; }

        public readonly EnemyConfig EnemyConfig = new EnemyConfig();

        public MotionBody MotionBody;
        public IFlipper _flippers;

        public EnemyZombieController() {
            _name = "Enemy.Zombie:" + GetHashCode().ToString("x8");
            _logger = LoggerFactory.GetLogger(_name);
            _stateMachine = new StateMachine(this, _name)
                .AddState(new ZombieStatePatrolStep(ZombieState.PatrolStep, this))
                .AddState(new ZombieStatePatrolWait(ZombieState.PatrolWait, this))
                .AddState(new ZombieStateIdle(ZombieState.Idle, this))
                .AddState(new ZombieStateAttacked(ZombieState.Attacked, this))
                .SetNextState(ZombieState.Idle);
        }

        public override void Ready() {
            var animationStack = new AnimationStack(_name, _animationPlayer);
            AnimationIdle = animationStack.AddLoopAnimation("Idle");
            AnimationStep = animationStack.AddOnceAnimation("Step");
            AnimationDieRight = animationStack.AddOnceAnimation("DieRight");
            AnimationDieLeft = animationStack.AddOnceAnimation("DieLeft");

            _flippers = new FlipperList().AddSprite(_mainSprite).AddNode2D(_attackArea);
            MotionBody = new MotionBody(this, _flippers, _name, EnemyConfig.MotionConfig);

            CharacterManager.ConfigureEnemyCollisions(this);
            // CharacterManager.ConfigureEnemyAttackArea2D(_attack);
            CharacterManager.ConfigureEnemyDamageArea2D(_damageArea);
        }

        public override void _PhysicsProcess(float delta) {
            MotionBody.StartFrame(delta);
            _stateMachine.Execute(delta);

            if (CharacterManager.PlayerController?._playerDetector?.Position != null) {
                Label.Text = "" + DegreesTo(CharacterManager.PlayerController._playerDetector);
            }
            /*
            _label.Text = "Floor: " + IsOnFloor() + "\n" +
                          "Slope: " + IsOnSlope() + "\n" +
                          "Stair: " + IsOnSlopeStairs() + "\n" +
                          "Moving: " + IsOnMovingPlatform() + "\n" +
                          "Falling: " + IsOnFallingPlatform();
            */
            MotionBody.EndFrame();
        }

        public void DisableAll() {
            CollisionLayer = 0;
            CollisionMask = 0;
            _damageArea.CollisionLayer = 0;
            // _attackArea.CollisionLayer = 0;
            _damageArea.CollisionMask = 0;
            // _attackArea.CollisionMask = 0;
        }

        public void Dispose() {
            _stateMachine.Dispose();
            GetParent().QueueFree();
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
            _stateMachine.SetNextState(ZombieState.Attacked);
        }
    }
}