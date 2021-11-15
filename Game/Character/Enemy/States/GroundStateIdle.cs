using System;
using Tools;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public class GroundStateIdle : GroundState {
        private Random rand = new Random();
        private Clock _clock = new Clock();

        public GroundStateIdle(EnemyController enemy) : base(enemy) {
        }

        public override void Start() {
            Enemy.AnimateIdle();
            _clock.Start().Finish(rand.Next(2, 4));
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
                Enemy.SetNextState(typeof(GroundStateRun));
                // _clock.Start().Finish(rand.Next(1, 4)* 1000);
            }

            if (!Enemy.IsOnMovingPlatform()) {
                // No gravity in moving platforms
                // Gravity in slopes to avoid go down slowly
                Enemy.ApplyGravity();
            }

            Enemy.MoveSnapping();
        }
    }
}