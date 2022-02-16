using Betauer;
using Betauer.DI;
using Betauer.StateMachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Character.Enemy {
    [Transient]
    public class EnemyZombieStateMachine {
        public const string Attacked = nameof(Attacked);
        public const string Idle = nameof(Idle);
        public const string PatrolStep = nameof(PatrolStep);
        public const string PatrolWait = nameof(PatrolWait);

        [Inject] private CharacterManager CharacterManager;

        private EnemyZombieController _enemyZombieController;
        private StateMachineNode _stateMachineNode;

        private MotionBody Body => _enemyZombieController.MotionBody;
        private MotionConfig MotionConfig => _enemyZombieController.EnemyConfig.MotionConfig;

        // State sharad between states
        private Timer _patrolTimer;

        public void Configure(EnemyZombieController enemyZombie) {
            _enemyZombieController = enemyZombie;
            _stateMachineNode = new StateMachineNode("Zombie", StateMachineNode.ProcessMode.Idle);
            _patrolTimer = new AutoTimer(enemyZombie);
            enemyZombie.AddChild(_stateMachineNode);

            _stateMachineNode.BeforeExecute((delta) => {
                Body.StartFrame(delta);
            });

            var builder = _stateMachineNode.CreateBuilder();
            AddStates(builder);
            builder.Build();
            _stateMachineNode.SetNextState(Idle);
            _stateMachineNode.AfterExecute((delta) => {
                Body.EndFrame();
            });

        }

        public void SetNextState(string attacked) {
            _stateMachineNode.SetNextState(attacked);
        }

        private void AddStates(StateMachineBuilder<StateMachineNode> builder) {
            builder.CreateState(PatrolStep)
                .Enter(context => {
                    _patrolTimer.Start();
                    if (context.FromState.Name is PatrolWait) {
                        // State come from the wait, do nothing...
                    } else {
                        // Body.Flip();
                        _patrolTimer.SetAlarm(4).Reset();
                    }
                    _enemyZombieController.FaceTo(CharacterManager.PlayerController.PlayerDetector);
                    _enemyZombieController.AnimationStep.PlayOnce();
                })
                /*
                 * AnimationStep + lateral move -> wait(1,2) + stop
                 */
                .Execute(context => {
                    if (!_enemyZombieController.IsOnFloor()) {
                        _enemyZombieController.AnimationIdle.PlayLoop();
                        Body.Fall();
                        return context.Current();
                    }

                    if (_patrolTimer.IsAlarm()) {
                        // Stop slowly and go to idle
                        if (Body.Motion.x == 0) {
                            return Context.Immediate(Idle);
                        } else {
                            Body.StopLateralMotionWithFriction(MotionConfig.Friction,
                                MotionConfig.StopIfSpeedIsLessThan);
                            Body.MoveSnapping();
                        }
                        return context.Current();
                    }

                    if (!_enemyZombieController.AnimationStep.Playing) {
                        return Context.NextFrame(PatrolWait);
                    }

                    Body.AddLateralMotion(Body.IsFacingRight ? 1 : -1, MotionConfig.Acceleration,
                        MotionConfig.AirResistance, MotionConfig.StopIfSpeedIsLessThan, 0);
                    Body.LimitMotion();
                    Body.MoveSnapping();
                    return context.Current();
                });

            builder.CreateState(PatrolWait)
                .Enter(context => { })
                .Execute(context => {
                    if (!_enemyZombieController.IsOnFloor()) {
                        return Context.Immediate(PatrolStep);
                    }
                    Body.StopLateralMotionWithFriction(MotionConfig.Friction, MotionConfig.StopIfSpeedIsLessThan);
                    Body.MoveSnapping();

                    return context.NextFrameIfElapsed(0.3f, PatrolStep);
                })
                ;

            builder.CreateState(Idle)
                .Enter(context => { _enemyZombieController.AnimationIdle.PlayLoop(); })
                .Execute(context => {
                    if (!_enemyZombieController.IsOnFloor()) {
                        Body.Fall();
                        return context.Current();
                    }

                    if (!Body.IsOnMovingPlatform()) {
                        // No gravity in moving platforms
                        // Gravity in slopes to avoid go down slowly
                        Body.ApplyGravity();
                    }

                    Body.MoveSnapping();
                    return context.ImmediateIfElapsed(2, PatrolStep);
                })
                ;

            builder.CreateState(Attacked)
                .Enter(context => {
                    _enemyZombieController.DisableAll();

                    if (_enemyZombieController.IsToTheLeftOf(CharacterManager.PlayerController.PlayerDetector)) {
                        _enemyZombieController.AnimationDieLeft.PlayOnce(true);
                    } else {
                        _enemyZombieController.AnimationDieRight.PlayOnce(true);
                    }
                })
                .Execute(context => {
                    if (!_enemyZombieController.AnimationDieRight.Playing && !_enemyZombieController.AnimationDieLeft.Playing) {
                        _enemyZombieController.QueueFree();
                    }
                    return context.Current();
                })
                ;

        }
    }
}