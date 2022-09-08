using System;
using System.Runtime.InteropServices;
using Betauer;
using Betauer.DI;
using Betauer.StateMachine;
using Betauer.Time;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Character.Enemy {
    [Service(Lifetime.Transient)]
    public class EnemyZombieStateMachineNode : StateMachineNode<EnemyZombieStateMachineNode.State,EnemyZombieStateMachineNode.Transition> {
        public enum Transition {
            Attacked
        }

        public enum State {
            Destroy,
            Idle,
            PatrolStep,
            PatrolWait,
        }

        public EnemyZombieStateMachineNode() : base(State.Idle) {
        }

        [Inject] private CharacterManager CharacterManager { get; set; }

        private EnemyZombieController _enemyZombieController;

        private KinematicPlatformMotionBody Body => _enemyZombieController.KinematicPlatformMotionBody;
        private MotionConfig MotionConfig => _enemyZombieController.EnemyConfig.MotionConfig;

        // State sharad between states
        [Inject] private GodotStopwatch PatrolTimer  { get; set; }
        [Inject] private GodotStopwatch StateTimer  { get; set; }

        public void Configure(EnemyZombieController enemyZombie, string name) {
            _enemyZombieController = enemyZombie;
            enemyZombie.AddChild(this);

            var events = new StateMachineEvents<State>();
            events.ExecuteStart += (delta, state) => Body.StartFrame(delta);
            events.ExecuteEnd += (state) => Body.EndFrame();
            AddListener(events);

            On(Transition.Attacked, context => context.Set(State.Destroy));
            CreateState(State.Idle)
                .Enter(() => {
                    // Console.WriteLine(DateTime.Now+": Idle, waiting 2 seconds...");
                    StateTimer.Restart().SetAlarm(2f);
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
                    if (StateTimer.IsAlarm()) {
                        // Console.WriteLine(DateTime.Now+": Alarm! Idle 2 seconds done! Go to PatrolStep for 4 seconds...");
                        PatrolTimer.Restart().SetAlarm(4);
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

                    if (PatrolTimer.IsAlarm() && !_enemyZombieController.AnimationStep.Playing) {
                        // Console.WriteLine(DateTime.Now+": Alarm! Patrol for 4 seconds done! Go to idle");
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
                    // Console.WriteLine(DateTime.Now+": Set PatrolWait for 1 second...");
                    StateTimer.Restart().SetAlarm(1f);
                })
                .Execute(context => {
                    if (!_enemyZombieController.IsOnFloor()) {
                        return context.Set(State.PatrolStep);
                    }
                    Body.StopLateralMotionWithFriction(MotionConfig.Friction, MotionConfig.StopIfSpeedIsLessThan);
                    Body.MoveSnapping();

                    // if (_stateTimer.IsAlarm()) {
                        // Console.WriteLine(DateTime.Now+": Alarm! PatrolWait for 1 second done! Go to PatrolStep");
                    // }
                    return StateTimer.IsAlarm() ? context.Set(State.PatrolStep) : context.None();
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