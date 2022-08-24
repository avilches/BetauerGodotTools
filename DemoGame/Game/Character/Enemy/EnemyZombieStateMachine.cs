using Betauer;
using Betauer.DI;
using Betauer.StateMachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Character.Enemy {
    [Service(Lifetime.Transient)]
    public class EnemyZombieStateMachine : StateMachineNode<EnemyZombieStateMachine.State,EnemyZombieStateMachine.Transition> {
        public enum Transition {
            Attacked
        }

        public enum State {
            Destroy,
            Idle,
            PatrolStep,
            PatrolWait,
        }

        public EnemyZombieStateMachine() : base(State.Idle) {
        }

        [Inject] private CharacterManager CharacterManager { get; set; }

        private EnemyZombieController _enemyZombieController;

        private KinematicPlatformMotionBody Body => _enemyZombieController.KinematicPlatformMotionBody;
        private MotionConfig MotionConfig => _enemyZombieController.EnemyConfig.MotionConfig;

        // State sharad between states
        private Timer _patrolTimer;
        private Timer _stateTimer;

        public void Configure(EnemyZombieController enemyZombie, string name) {
            _enemyZombieController = enemyZombie;
            _patrolTimer = new AutoTimer(enemyZombie);
            _stateTimer = new AutoTimer(enemyZombie);
            enemyZombie.AddChild(this);

            var events = new StateMachineEvents<State>();
            events.ExecuteStart += (delta, state) => Body.StartFrame(delta);
            events.ExecuteEnd += (state) => Body.EndFrame();
            AddListener(events);

            On(Transition.Attacked, context => context.Set(State.Destroy));
            CreateState(State.Idle)
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
                        return context.Set(State.PatrolStep);
                    }
                    return context.None();
                })
                .Build();
            
            CreateState(State.PatrolStep)
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
                            return context.Set(State.Idle);
                        } else {
                            Body.StopLateralMotionWithFriction(MotionConfig.Friction,
                                MotionConfig.StopIfSpeedIsLessThan);
                            Body.MoveSnapping();
                        }
                        return context.None();
                    }

                    if (!_enemyZombieController.AnimationStep.Playing) {
                        return context.Set(State.PatrolWait);
                    }

                    Body.AddLateralMotion(Body.IsFacingRight ? 1 : -1, MotionConfig.Acceleration,
                        MotionConfig.AirResistance, MotionConfig.StopIfSpeedIsLessThan, 0);
                    Body.LimitMotion();
                    Body.MoveSnapping();
                    return context.None();
                })
                .Build();

            CreateState(State.PatrolWait)
                .Enter(() => {
                    _stateTimer.Reset().Start().SetAlarm(0.3f);
                })
                .Execute(context => {
                    if (!_enemyZombieController.IsOnFloor()) {
                        return context.Set(State.PatrolStep);
                    }
                    Body.StopLateralMotionWithFriction(MotionConfig.Friction, MotionConfig.StopIfSpeedIsLessThan);
                    Body.MoveSnapping();

                    return _stateTimer.IsAlarm() ? context.Set(State.PatrolStep) : context.None();
                })
                .Build();

            CreateState(State.Destroy)
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
                })
                .Build();
        }

        public void TriggerAttacked() {
            Enqueue(Transition.Attacked);
        }
    }
}