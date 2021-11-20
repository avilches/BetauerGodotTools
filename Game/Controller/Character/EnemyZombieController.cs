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

        [Inject] public CharacterManager CharacterManager;

        public LoopAnimationStatus AnimationIdle { get; private set; }
        public OnceAnimationStatus AnimationStep { get; private set; }

        public readonly EnemyConfig EnemyConfig = new EnemyConfig();

        public readonly MotionBody MotionBody;

        public EnemyZombieController() {
            _name = "Enemy.Zombie:" + GetHashCode().ToString("x8");
            _logger = LoggerFactory.GetLogger(_name);
            _stateMachine = new StateMachine(_name)
                .AddState(new GroundStatePatrolStep(this))
                .AddState(new GroundStatePatrolWait(this))
                .AddState(new GroundStateIdle(this))
                .SetNextState(typeof(GroundStateIdle));
            MotionBody = new MotionBody( this, _name, EnemyConfig.MotionConfig);
        }

        public override void _EnterTree() {
            MotionBody.EnterTree();
            var animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
            var animationStack = new AnimationStack(_name, animationPlayer);
            AnimationIdle = animationStack.AddLoopAnimationAndGetStatus(new LoopAnimationIdle());
            AnimationStep = animationStack.AddOnceAnimationAndGetStatus(new AnimationZombieStep());
        }

        public override void _Ready() {
            CharacterManager.ConfigureEnemyCollisions(this);
        }

        public override void _PhysicsProcess(float delta) {
            MotionBody.StartFrame(delta);
            _stateMachine.Execute(delta);
            /*
            _label.Text = "Floor: " + IsOnFloor() + "\n" +
                          "Slope: " + IsOnSlope() + "\n" +
                          "Stair: " + IsOnSlopeStairs() + "\n" +
                          "Moving: " + IsOnMovingPlatform() + "\n" +
                          "Falling: " + IsOnFallingPlatform();
            */
            MotionBody.EndFrame();
        }
    }
}