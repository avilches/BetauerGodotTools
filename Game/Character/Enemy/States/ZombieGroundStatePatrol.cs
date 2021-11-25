using Tools;
using Tools.Statemachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers;
using Timer = Tools.Timer;

namespace Veronenger.Game.Character.Enemy.States {
    public class GroundStatePatrolStep : GroundState {
        public GroundStatePatrolStep(EnemyZombieController enemyZombie) : base(enemyZombie) {
        }

        [Inject] private CharacterManager CharacterManager;

        private Timer _patrolTimer = new Timer().Start();

        public override void Start(Context context, StateConfig config) {
            if (context.FromState is GroundStatePatrolWait) {
                // State come from the wait, do nothing...
            } else {
                // Body.Flip();
                _patrolTimer.SetAlarm(4).Reset();
            }
            EnemyZombie.FaceTo(CharacterManager.PlayerController._playerDetector);
            EnemyZombie.AnimationStep.PlayOnce();
        }

        /*
         * AnimationStep + lateral move -> wait(1,2) + stop
         */
        public override NextState Execute(Context context) {
            _patrolTimer.Update(context.Delta);
            if (!EnemyZombie.IsOnFloor()) {
                EnemyZombie.AnimationIdle.PlayLoop();
                Body.Fall();
                return context.Current();
            }

            if (_patrolTimer.IsAlarm()) {
                // Stop slowly and go to idle
                if (Body.Motion.x == 0) {
                    return context.Immediate(typeof(GroundStateIdle));
                } else {
                    Body.StopLateralMotionWithFriction(MotionConfig.Friction,
                        MotionConfig.StopIfSpeedIsLessThan);
                    Body.MoveSnapping();
                }
                return context.Current();
            }

            if (!EnemyZombie.AnimationStep.Playing) {
                return context.NextFrame(typeof(GroundStatePatrolWait));
            }

            Body.AddLateralMotion(Body.IsFacingRight ? 1 : -1, MotionConfig.Acceleration,
                MotionConfig.AirResistance, MotionConfig.StopIfSpeedIsLessThan, 0);
            Body.LimitMotion();
            Body.MoveSnapping();
            return context.Current();
        }
    }

    public class GroundStatePatrolWait : GroundState {
        public GroundStatePatrolWait(EnemyZombieController enemyZombie) : base(enemyZombie) {
        }

        /*
         * AnimationStep + lateral move -> wait(1,2) + stop
         */
        public override NextState Execute(Context context) {
            if (!EnemyZombie.IsOnFloor()) {
                return context.Immediate(typeof(GroundStatePatrolStep));
            }
            Body.StopLateralMotionWithFriction(MotionConfig.Friction, MotionConfig.StopIfSpeedIsLessThan);
            Body.MoveSnapping();

            return context.NextFrameIfElapsed(0.3f, typeof(GroundStatePatrolStep));
        }
    }
}