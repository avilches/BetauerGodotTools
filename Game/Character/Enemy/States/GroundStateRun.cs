using System;
using Godot;
using Tools;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public class GroundStateRun : GroundState {
        private Random rand = new Random();
        private Clock _clock = new Clock();

        private OnceAnimationStatus _stepStatus;

        public GroundStateRun(EnemyController enemy) : base(enemy) {
        }

        public override void Start() {
            _clock.Start().Finish(rand.Next(5, 8));
            _stepStatus = Enemy.AnimateStep();
        }

        public override void Execute() {
            _clock.Add(Enemy.Delta);
            _stepStatus = Enemy.AnimateStep();

            if (!Enemy.IsOnFloor()) {
                Enemy.AnimateIdle();
                Enemy.ApplyGravity();
                Enemy.LimitMotion();
                Enemy.Slide();
                return;
            }


            if (_clock.IsFinished()) {
                Enemy.StopLateralMotionWithFriction(EnemyConfig.FRICTION, EnemyConfig.STOP_IF_SPEED_IS_LESS_THAN);

                if (Enemy.Motion.x == 0) {
                    Enemy.SetNextState(typeof(GroundStateIdle));
                }
                return;
            }

            Enemy.AddLateralMotion(Enemy.IsFacingRight ? 1: -1 , EnemyConfig.ACCELERATION, EnemyConfig.AIR_RESISTANCE,
                EnemyConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);

            Enemy.LimitMotion();
            Enemy.MoveSnapping();
        }
    }
}