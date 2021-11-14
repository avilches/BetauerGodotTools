using Godot;
using Tools;
using Tools.Statemachine;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Enemy.States;
using Veronenger.Game.Character.Player;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Controller.Character {
    public class EnemyController : CharacterController {
        public CharacterConfig EnemyConfig => CharacterConfig;
        private readonly StateMachine _stateMachine;
        private AnimationPlayer _animationPlayer;

        public EnemyController() {
            CharacterConfig = new CharacterConfig();

            // State Machine
            _stateMachine = new StateMachine(EnemyConfig, this)
                .AddState(new GroundStateIdle( /*this*/));
        }

        public override void _EnterTree() {
            _sprite = GetNode<Sprite>("Sprite");
            _animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
            _stateMachine.SetNextState(typeof(GroundStateIdle));
        }

        public override void _Ready() {
            GameManager.Instance.CharacterManager.RegisterEnemy(this);
            _animationStack = new AnimationStack(_animationPlayer)
                .AddLoopAnimation(new LoopAnimationIdle("Idle"))
                .AddLoopAnimation(new LoopAnimationRun("Run"))
                .AddOnceAnimation(new AnimationAttack("Attack"));

        }

        protected override void PhysicsProcess() {
            _stateMachine.Execute();
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