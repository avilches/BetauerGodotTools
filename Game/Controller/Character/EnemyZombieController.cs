using Godot;
using Tools;
using Tools.Statemachine;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Enemy;
using Veronenger.Game.Character.Enemy.States;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Controller.Character {
    public class EnemyZombieController : CharacterController {
        public readonly EnemyConfig EnemyConfig = new EnemyConfig();
        private StateMachine _stateMachine;
        private AnimationStack _animationStack;
        private string _name;
        private Logger _logger = LoggerFactory.GetLogger("Enemy.Zombie:");

        public LoopAnimationStatus AnimationIdle { get; private set; }
        public OnceAnimationStatus AnimationStep { get; private set; }

        protected override CharacterConfig CreateCharacterConfig() {
            // TODO: rename
            return EnemyConfig;
        }

        protected override Logger GetLogger() {
            return _logger;
        }

        public override void _EnterTree() {
            _name = "Enemy.Zombie:" + GetHashCode().ToString("x8");
            base._EnterTree();
            _logger = LoggerFactory.GetLogger(_name);
            var animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
            var animationStack = new AnimationStack(_name, animationPlayer);
            AnimationIdle = animationStack.AddLoopAnimationAndGetStatus(new LoopAnimationIdle());
            AnimationStep = animationStack.AddOnceAnimationAndGetStatus(new AnimationZombieStep());

            _stateMachine = new StateMachine(_name)
                .AddState(new GroundStatePatrolStep(this))
                .AddState(new GroundStatePatrolWait(this))
                .AddState(new GroundStateIdle(this))
                .SetNextState(typeof(GroundStateIdle));
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