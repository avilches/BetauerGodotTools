using System;
using Tools;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public class GroundStateRun : GroundState {
        private Random rand = new Random();
        private Clock _state = new Clock();
        private Clock _waitBetweenSteps = new Clock();

        public GroundStateRun(EnemyZombieController enemyZombie) : base(enemyZombie) {
        }

        public override void Start() {
            _state.Start().Finish(rand.Next(5, 8)).Stop();
            EnemyZombie.AnimationStep.Play();
        }

        /*
         * AnimationStep + lateral move -> wait(1,2) + stop
         */
        public override void Execute() {
            _state.Add(EnemyZombie.Delta);
            _waitBetweenSteps.Add(EnemyZombie.Delta);

            if (!EnemyZombie.IsOnFloor()) {
                EnemyZombie.AnimationIdle.Play();
                EnemyZombie.ApplyGravity();
                EnemyZombie.LimitMotion();
                EnemyZombie.Slide();
                return;
            }

            if (_state.IsFinished()) {
                // Stop slowly and go to idle
                if (EnemyZombie.Motion.x == 0) {
                    EnemyZombie.SetNextState(typeof(GroundStateIdle));
                } else {
                    StopMovement();
                }
                return;
            }

            if (EnemyZombie.AnimationStep.Playing) {
                EnemyZombie.AddLateralMotion(EnemyZombie.IsFacingRight ? 1 : -1, EnemyConfig.ACCELERATION,
                    EnemyConfig.AIR_RESISTANCE, EnemyConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
                EnemyZombie.LimitMotion();
                EnemyZombie.MoveSnapping();
            } else {
                // No playing the "Step" animation anymore -> stop enemy
                StopMovement();
                if (_waitBetweenSteps.Stopped) {
                    // Start step wait
                    _waitBetweenSteps.Start().Finish(rand.Next(200, 800) / 1000f);
                } else if (_waitBetweenSteps.IsFinished()) {
                    // Step wait is finished
                    _waitBetweenSteps.Stop();
                    EnemyZombie.AnimationStep.Play();
                }
            }
        }

        void StopMovement() {
            EnemyZombie.StopLateralMotionWithFriction(EnemyConfig.FRICTION, EnemyConfig.STOP_IF_SPEED_IS_LESS_THAN);
            EnemyZombie.LimitMotion();
            EnemyZombie.MoveSnapping();
        }
    }
}