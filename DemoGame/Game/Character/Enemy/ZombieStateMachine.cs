using Betauer;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.StateMachine;
using Betauer.StateMachine.Sync;
using Betauer.Time;
using Godot;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Character.Enemy {
    public enum ZombieTransition {
        Dead,
        Attacked
    }

    public enum ZombieState {
        Idle,
        Patrol,
        Chase,
        
        Attacked,
        Destroy
    }

    public class EnemyStatus {
        public float Health = 50;

        public void Attack(Attack attack) {
            Health -= attack.Damage;
        }

        public bool IsDead() {
            return Health <= 0f;
        }
    }

    [Service(Lifetime.Transient)]
    public class ZombieStateMachine : StateMachineNodeSync<ZombieState, ZombieTransition> {
        public ZombieStateMachine() : base(ZombieState.Idle, "Zombie.StateMachine") {
        }

        [Inject] private MainStateMachine MainStateMachine { get; set; }
        [Inject] private CharacterManager CharacterManager { get; set; }
        [Inject] public KinematicPlatformMotionBody Body { get; set; }
        [Inject] private EnemyConfig EnemyConfig { get; set; }
        // State shared between states
        [Inject] private GodotStopwatch PatrolTimer  { get; set; }
        [Inject] private GodotStopwatch StateTimer  { get; set; }

        private ZombieController _zombieController;
        private MotionConfig MotionConfig => EnemyConfig.MotionConfig;

        public readonly EnemyStatus Status = new();

        public void Start(string name, ZombieController zombie, IFlipper flippers, RayCast2D floorDetector, Position2D position2D) {
            _zombieController = zombie;
            zombie.AddChild(this);

            Body.Configure(name, zombie, flippers, floorDetector, position2D, EnemyConfig.MotionConfig.Configure);

            AddOnExecuteStart((delta, state) => Body.StartFrame(delta));
            AddOnExecuteEnd((state) => Body.EndFrame());

            State(ZombieState.Idle)
                .Enter(() => {
                    StateTimer.Restart().SetAlarm(2f);
                    _zombieController.AnimationIdle.PlayLoop();
                })
                .Execute(context => {
                    if (!_zombieController.IsOnFloor()) {
                        // return context.Set(PlayerState.FallShort);
                        Body.Fall();
                        return context.None();
                    }

                    if (!Body.IsOnMovingPlatform()) {
                        // No gravity in moving platforms
                        // Gravity in slopes to avoid go down slowly
                        Body.ApplyDefaultGravity();
                    }

                    Body.MoveSnapping();
                    if (StateTimer.IsAlarm()) {
                        return context.Set(ZombieState.Patrol);
                    }
                    return context.None();
                })
                .Build();
            
            State(ZombieState.Patrol)
                .Enter(() => {
                    StateTimer.Restart().SetAlarm(2f);
                    _zombieController.AnimationStep.PlayLoop();
                })
                /*
                 * AnimationStep + lateral move -> wait(1,2) + stop
                 */
                .Execute(context => {
                    if (!_zombieController.IsOnFloor()) {
                        _zombieController.AnimationIdle.PlayLoop();
                        Body.Fall();
                        return context.None();
                        // return context.Set(PlayerState.FallShort);
                    }

                    Body.AddLateralSpeed(Body.IsFacingRight ? 1 : -1, MotionConfig.Acceleration, MotionConfig.MaxSpeed, 
                        MotionConfig.Friction, MotionConfig.StopIfSpeedIsLessThan, 0);
                    Body.MoveSnapping();
                    if (StateTimer.IsAlarm()) {
                        Body.Flip();
                        StateTimer.Restart().SetAlarm(2f);
                    }
                    return context.None();
                })
                .Build();

            State(ZombieState.Chase)
                .Enter(() => {
                    StateTimer.Restart().SetAlarm(1f);
                })
                .Execute(context => {
                    if (!_zombieController.IsOnFloor()) {
                        return context.Set(ZombieState.Patrol);
                    }
                    Body.StopLateralSpeedWithFriction(MotionConfig.Friction, MotionConfig.StopIfSpeedIsLessThan);
                    Body.MoveSnapping();

                    return StateTimer.IsAlarm() ? context.Set(ZombieState.Patrol) : context.None();
                })
                .Build();

            On(ZombieTransition.Attacked,
                context => IsState(ZombieState.Attacked) ? context.None() : context.Push(ZombieState.Attacked));
            State(ZombieState.Attacked)
                .Enter(() => {
                    Body.FaceTo(CharacterManager.PlayerController.PlayerDetector);
                    Body.SpeedY = EnemyConfig.MiniJumpOnAttack.y;
                    Body.SpeedX = EnemyConfig.MiniJumpOnAttack.x * (Body.IsToTheLeftOf(CharacterManager.PlayerController.PlayerDetector) ? 1 : -1);
                    _zombieController.PlayAnimationAttacked();
                    StateTimer.Restart().SetAlarm(1f);
                })
                .Execute(context => {
                    Body.ApplyDefaultGravity();
                    Body.Slide();
                    if (StateTimer.IsAlarm())
                        return context.Pop();
                    return context.None();
                })
                .Build();

            On(ZombieTransition.Dead, context => context.Set(ZombieState.Destroy));
            State(ZombieState.Destroy)
                .Enter(() => {
                    _zombieController.DisableAll();

                    if (Body.IsToTheLeftOf(CharacterManager.PlayerController.PlayerDetector)) {
                        _zombieController.AnimationDieLeft.PlayOnce(true);
                    } else {
                        _zombieController.AnimationDieRight.PlayOnce(true);
                    }
                })
                .Execute(context => {
                    if (!_zombieController.AnimationDieRight.Playing && !_zombieController.AnimationDieLeft.Playing) {
                        _zombieController.QueueFree();
                    }
                    return context.None();
                })
                .Build();
            AddOnTransition((args) => zombie.Label.Text = args.To.ToString());

        }

        public void TriggerAttacked(Attack attack) {
            Status.Attack(attack);
            Enqueue(Status.IsDead() ? ZombieTransition.Dead : ZombieTransition.Attacked);
        }
    }
}