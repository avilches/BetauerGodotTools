using Godot;
using Tools;
using Tools.Statemachine;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Enemy;
using Veronenger.Game.Character.Enemy.States;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Controller.Character {
    public class EnemyController : CharacterController {
        public readonly EnemyConfig EnemyConfig = new EnemyConfig();

        public EnemyController() {
        }

        protected override StateMachine CreateStateMachine() {
            return new StateMachine(EnemyConfig, this)
                .AddState(new GroundStateRun( this))
                .AddState(new GroundStateIdle( this));
        }

        protected override CharacterConfig CreateCharacterConfig() {
            return EnemyConfig;
        }

        protected override AnimationStack CreateAnimationStack(AnimationPlayer animationPlayer) {
            return new AnimationStack(animationPlayer)
                .AddLoopAnimation(new LoopAnimationIdle())
                .AddOnceAnimation(new AnimationZombieStep())
                .AddOnceAnimation(new AnimationAttack());
        }

        public override void _EnterTree() {
            base._EnterTree();
            StateMachine.SetNextState(typeof(GroundStateIdle));
        }

        public override void _Ready() {
            base._Ready();
            GameManager.Instance.CharacterManager.ConfigureEnemyCollisions(this);
        }

        protected override void PhysicsProcess() {
            StateMachine.Execute();
            /*
            _label.Text = "Floor: " + IsOnFloor() + "\n" +
                          "Slope: " + IsOnSlope() + "\n" +
                          "Stair: " + IsOnSlopeStairs() + "\n" +
                          "Moving: " + IsOnMovingPlatform() + "\n" +
                          "Falling: " + IsOnFallingPlatform();
            */
        }
        public void AnimateIdle() => AnimationStack.PlayLoop(typeof(LoopAnimationIdle));
        public OnceAnimationStatus AnimateStep() => AnimationStack.PlayOnce(typeof(AnimationZombieStep));
        public void AnimateAttack() => AnimationStack.PlayOnce(typeof(AnimationAttack));

    }
}