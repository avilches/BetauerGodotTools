using System;
using Tools;
using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public class GroundStateIdle : GroundState {
        private Random rand = new Random();
        private Clock _clock = new Clock();

        public GroundStateIdle(EnemyZombieController enemyZombie) : base(enemyZombie) {
        }

        public override void Start(StateConfig config) {
            EnemyZombie.AnimationIdle.Play();;
            _clock.Start().Finish(1); // rand.Next(2, 4));
        }

        public override NextState Execute(NextState nextState) {
            _clock.Add(EnemyZombie.Delta);

            if (!EnemyZombie.IsOnFloor()) {
                EnemyZombie.ApplyGravity();
                EnemyZombie.LimitMotion();
                EnemyZombie.Slide();
                return nextState.Current();
            }


            if (_clock.IsFinished()) {
                EnemyZombie.IsFacingRight = !EnemyZombie.IsFacingRight;
                return nextState.Immediate(typeof(GroundStateRun));
                // _clock.Start().Finish(rand.Next(1, 4)* 1000);
            }

            if (!EnemyZombie.IsOnMovingPlatform()) {
                // No gravity in moving platforms
                // Gravity in slopes to avoid go down slowly
                EnemyZombie.ApplyGravity();
            }

            EnemyZombie.MoveSnapping();
            return nextState.Current();
        }
    }
}