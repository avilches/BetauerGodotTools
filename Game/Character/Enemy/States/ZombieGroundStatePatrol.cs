using Tools.Statemachine;
using Veronenger.Game.Controller.Character;
using Timer = Tools.Timer;

namespace Veronenger.Game.Character.Enemy.States {
    public class GroundStatePatrolStep : GroundState {
        public GroundStatePatrolStep(EnemyZombieController enemyZombie) : base(enemyZombie) {
        }

        private Timer _patrolTimer = new Timer().Start();

        public override void Start(Context context, StateConfig config) {
            if (context.FromState is GroundStatePatrolWait) {
                // State come from the wait, do nothing...
            } else {
                BOdy.Flip();
                _patrolTimer.SetAlarm(4).Reset();
            }
            EnemyZombie.AnimationStep.PlayOnce();
        }

        /*
         * AnimationStep + lateral move -> wait(1,2) + stop
         */
        public override NextState Execute(Context context) {
            _patrolTimer.Update(context.Delta);
            if (!EnemyZombie.IsOnFloor()) {
                EnemyZombie.AnimationIdle.PlayLoop();
                BOdy.ApplyGravity();
                BOdy.LimitMotion();
                BOdy.Slide();
                return context.Current();
            }

            if (_patrolTimer.IsAlarm()) {
                // Stop slowly and go to idle
                if (BOdy.Motion.x == 0) {
                    return context.Immediate(typeof(GroundStateIdle));
                } else {
                    BOdy.StopLateralMotionWithFriction(MotionConfig.Friction,
                        MotionConfig.StopIfSpeedIsLessThan);
                    BOdy.MoveSnapping();
                }
                return context.Current();
            }

            if (!EnemyZombie.AnimationStep.Playing) {
                return context.NextFrame(typeof(GroundStatePatrolWait));
            }

            BOdy.AddLateralMotion(BOdy.IsFacingRight ? 1 : -1, MotionConfig.Acceleration,
                MotionConfig.AirResistance, MotionConfig.StopIfSpeedIsLessThan, 0);
            BOdy.LimitMotion();
            BOdy.MoveSnapping();
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
            BOdy.StopLateralMotionWithFriction(MotionConfig.Friction, MotionConfig.StopIfSpeedIsLessThan);
            BOdy.MoveSnapping();

            return context.NextFrameIfElapsed(0.3f, typeof(GroundStatePatrolStep));
        }
    }
}