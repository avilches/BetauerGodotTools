using System;
using Tools;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public class GroundStateRun : GroundState {
        private Random rand = new Random();
        private Clock _clock = new Clock();

        public GroundStateRun(EnemyController enemy) : base(enemy) {
        }

        public override void Start() {
            Enemy.AnimateRun();
            _clock.Start().Finish(rand.Next(1, 2));
        }

        public override void Execute() {
            _clock.Add(Enemy.Delta);

            if (!Enemy.IsOnFloor()) {
                Enemy.ApplyGravity();
                Enemy.LimitMotion();
                Enemy.Slide();
                return;
            }

            if (_clock.IsFinished()) {
                Enemy.IsFacingRight = !Enemy.IsFacingRight;
                Enemy.StopLateralMotionWithFriction(EnemyConfig.FRICTION, EnemyConfig.STOP_IF_SPEED_IS_LESS_THAN);
                Enemy.SetNextState(typeof(GroundStateIdle));
                return;
            }

            Enemy.AddLateralMotion(1, EnemyConfig.ACCELERATION, EnemyConfig.AIR_RESISTANCE,
                EnemyConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);

            Enemy.LimitMotion();
            Enemy.MoveSnapping();
        }
    }
}