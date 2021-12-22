using Tools;
using Tools.Statemachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers;
using Timer = Tools.Timer;

namespace Veronenger.Game.Character.Enemy.States {
    public class ZombieStatePatrolStep : ZombieState {
        public ZombieStatePatrolStep(string name, EnemyZombieController enemyZombie) : base(name, enemyZombie) {
        }

        [Inject] private CharacterManager CharacterManager;

        private Timer _patrolTimer;

        public override void Start(Context context) {
            _patrolTimer ??= new AutoTimer(context.Owner).Start();
            if (context.FromState is ZombieStatePatrolWait) {
                // State come from the wait, do nothing...
            } else {
                // Body.Flip();
                _patrolTimer.SetAlarm(4).Reset();
            }
            EnemyZombie.FaceTo(CharacterManager.PlayerController.PlayerDetector);
            EnemyZombie.AnimationStep.PlayOnce();
        }

        /*
         * AnimationStep + lateral move -> wait(1,2) + stop
         */
        public override NextState Execute(Context context) {
            if (!EnemyZombie.IsOnFloor()) {
                EnemyZombie.AnimationIdle.PlayLoop();
                Body.Fall();
                return context.Current();
            }

            if (_patrolTimer.IsAlarm()) {
                // Stop slowly and go to idle
                if (Body.Motion.x == 0) {
                    return context.Immediate(Idle);
                } else {
                    Body.StopLateralMotionWithFriction(MotionConfig.Friction,
                        MotionConfig.StopIfSpeedIsLessThan);
                    Body.MoveSnapping();
                }
                return context.Current();
            }

            if (!EnemyZombie.AnimationStep.Playing) {
                return context.NextFrame(PatrolWait);
            }

            Body.AddLateralMotion(Body.IsFacingRight ? 1 : -1, MotionConfig.Acceleration,
                MotionConfig.AirResistance, MotionConfig.StopIfSpeedIsLessThan, 0);
            Body.LimitMotion();
            Body.MoveSnapping();
            return context.Current();
        }
    }

    public class ZombieStatePatrolWait : ZombieState {
        public ZombieStatePatrolWait(string name, EnemyZombieController enemyZombie) : base(name, enemyZombie) {
        }

        /*
         * AnimationStep + lateral move -> wait(1,2) + stop
         */
        public override NextState Execute(Context context) {
            if (!EnemyZombie.IsOnFloor()) {
                return context.Immediate(PatrolStep);
            }
            Body.StopLateralMotionWithFriction(MotionConfig.Friction, MotionConfig.StopIfSpeedIsLessThan);
            Body.MoveSnapping();

            return context.NextFrameIfElapsed(0.3f, PatrolStep);
        }
    }
}