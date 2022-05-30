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
        private StateMachineNode<string, string> _stateMachineNode;

        private KinematicPlatformMotionBody Body => _enemyZombieController.KinematicPlatformMotionBody;
        private MotionConfig MotionConfig => _enemyZombieController.EnemyConfig.MotionConfig;

        // State sharad between states
        private Timer _patrolTimer;
        private Timer _stateTimer;

        public void Configure(EnemyZombieController enemyZombie) {
            _enemyZombieController = enemyZombie;
            _stateMachineNode = new StateMachineNode<string, string>(Idle, "Zombie", ProcessMode.Idle);
            _patrolTimer = new AutoTimer(enemyZombie);
            _stateTimer = new AutoTimer(enemyZombie);
            enemyZombie.AddChild(_stateMachineNode);

            _stateMachineNode.BeforeExecute((delta) => {
                Body.StartFrame(delta);
            });

            var builder = _stateMachineNode.CreateBuilder();
            AddStates(builder);
            builder.Build();
            _stateMachineNode.AfterExecute((delta) => {
                Body.EndFrame();
            });

        }

        public void TriggerAttacked() {
            _stateMachineNode.Trigger(Attacked);
        }

        private void AddStates(StateMachineBuilder<StateMachineNode<string, string>, string, string> builder) {
            builder.On(Attacked, context => context.Set(Attacked));
            builder.State(Idle)
                .Enter(() => {
                    _stateTimer.Reset().Start().SetAlarm(2f);
                    _enemyZombieController.AnimationIdle.PlayLoop();
                })
                .Execute(context => {
                    if (!_enemyZombieController.IsOnFloor()) {
                        Body.Fall();
                        return context.None();
                    }

                    if (!Body.IsOnMovingPlatform()) {
                        // No gravity in moving platforms
                        // Gravity in slopes to avoid go down slowly
                        Body.ApplyGravity();
                    }

                    Body.MoveSnapping();
                    if (_stateTimer.IsAlarm()) {
                        _patrolTimer.SetAlarm(4).Reset().Start();
                        return context.Set(PatrolStep);
                    }
                    return context.None();
                });
            
            builder.State(PatrolStep)
                .Enter(() => {
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
                        return context.None();
                    }

                    if (_patrolTimer.IsAlarm() && !_enemyZombieController.AnimationStep.Playing) {
                        // Stop slowly and go to idle
                        if (Body.Motion.x == 0) {
                            return context.Set(Idle);
                        } else {
                            Body.StopLateralMotionWithFriction(MotionConfig.Friction,
                                MotionConfig.StopIfSpeedIsLessThan);
                            Body.MoveSnapping();
                        }
                        return context.None();
                    }

                    if (!_enemyZombieController.AnimationStep.Playing) {
                        return context.Set(PatrolWait);
                    }

                    Body.AddLateralMotion(Body.IsFacingRight ? 1 : -1, MotionConfig.Acceleration,
                        MotionConfig.AirResistance, MotionConfig.StopIfSpeedIsLessThan, 0);
                    Body.LimitMotion();
                    Body.MoveSnapping();
                    return context.None();
                });

            builder.State(PatrolWait)
                .Enter(() => {
                    _stateTimer.Reset().Start().SetAlarm(0.3f);
                })
                .Execute(context => {
                    if (!_enemyZombieController.IsOnFloor()) {
                        return context.Set(PatrolStep);
                    }
                    Body.StopLateralMotionWithFriction(MotionConfig.Friction, MotionConfig.StopIfSpeedIsLessThan);
                    Body.MoveSnapping();

                    return _stateTimer.IsAlarm() ? context.Set(PatrolStep) : context.None();
                });

            builder.State(Attacked)
                .Enter(() => {
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
                    return context.None();
                });
        }
    }
}