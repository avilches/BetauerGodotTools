using Godot;
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
                EnemyZombie.IsFacingRight = !EnemyZombie.IsFacingRight;
                _patrolTimer.SetAlarm(4).Reset();
            }
            EnemyZombie.AnimationStep.PlayOnce();
        }

        /*
         * AnimationStep + lateral move -> wait(1,2) + stop
         */
        public override NextState Execute(Context context) {
            _patrolTimer.Add(context.Delta);
            if (!EnemyZombie.IsOnFloor()) {
                EnemyZombie.AnimationIdle.PlayLoop();
                EnemyZombie.ApplyGravity();
                EnemyZombie.LimitMotion();
                EnemyZombie.Slide();
                return context.Current();
            }

            if (_patrolTimer.IsAlarm()) {
                // Stop slowly and go to idle
                if (EnemyZombie.Motion.x == 0) {
                    return context.Immediate(typeof(GroundStateIdle));
                } else {
                    EnemyZombie.StopLateralMotionWithFriction(EnemyConfig.FRICTION,
                        EnemyConfig.STOP_IF_SPEED_IS_LESS_THAN);
                    EnemyZombie.MoveSnapping();
                }
                return context.Current();
            }

            if (!EnemyZombie.AnimationStep.Playing) {
                return context.NextFrame(typeof(GroundStatePatrolWait));
            }

            EnemyZombie.AddLateralMotion(EnemyZombie.IsFacingRight ? 1 : -1, EnemyConfig.ACCELERATION,
                EnemyConfig.AIR_RESISTANCE, EnemyConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
            EnemyZombie.LimitMotion();
            EnemyZombie.MoveSnapping();
            return context.Current();
        }
    }

    public class GroundStatePatrolWait : GroundState {
        public GroundStatePatrolWait(EnemyZombieController enemyZombie) : base(enemyZombie) {
        }

        public override void Start(Context context, StateConfig config) {
            context.StateTimer.SetAlarm(0.3f);
        }

        /*
         * AnimationStep + lateral move -> wait(1,2) + stop
         */
        public override NextState Execute(Context context) {

            if (!EnemyZombie.IsOnFloor()) {
                return context.Immediate(typeof(GroundStatePatrolStep));
            }

            EnemyZombie.StopLateralMotionWithFriction(EnemyConfig.FRICTION, EnemyConfig.STOP_IF_SPEED_IS_LESS_THAN);
            EnemyZombie.MoveSnapping();

            return context.ImmediateIfAlarm(typeof(GroundStatePatrolStep));
        }
    }
}