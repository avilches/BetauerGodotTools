using Godot;
using Tools;
using Tools.Statemachine;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Enemy;
using Veronenger.Game.Character.Enemy.States;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Controller.Character {
    public sealed class EnemyZombieController : Character2DPlatformController {
        private readonly string _name;
        private readonly Logger _logger;
        private readonly StateMachine _stateMachine;

        public LoopAnimationStatus AnimationIdle { get; private set; }
        public OnceAnimationStatus AnimationStep { get; private set; }

        public readonly EnemyConfig EnemyConfig = new EnemyConfig();

        public EnemyZombieController() {
            _name = "Enemy.Zombie:" + GetHashCode().ToString("x8");
            _logger = LoggerFactory.GetLogger(_name);
            _stateMachine = new StateMachine(_name)
                .AddState(new GroundStatePatrolStep(this))
                .AddState(new GroundStatePatrolWait(this))
                .AddState(new GroundStateIdle(this))
                .SetNextState(typeof(GroundStateIdle));
        }

        protected override Platform2DCharacterConfig Platform2DCharacterConfig => EnemyConfig;
        protected override string GetName() => _name;

        protected override void EnterTree() {
            var animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
            var animationStack = new AnimationStack(_name, animationPlayer);
            AnimationIdle = animationStack.AddLoopAnimationAndGetStatus(new LoopAnimationIdle());
            AnimationStep = animationStack.AddOnceAnimationAndGetStatus(new AnimationZombieStep());

        }

        public override void _Ready() {
            base._Ready();
            GameManager.Instance.CharacterManager.ConfigureEnemyCollisions(this);
        }

        protected override void PhysicsProcess() {
            _stateMachine.Execute(Delta);
            /*
            _label.Text = "Floor: " + IsOnFloor() + "\n" +
                          "Slope: " + IsOnSlope() + "\n" +
                          "Stair: " + IsOnSlopeStairs() + "\n" +
                          "Moving: " + IsOnMovingPlatform() + "\n" +
                          "Falling: " + IsOnFallingPlatform();
            */
        }
    }
}