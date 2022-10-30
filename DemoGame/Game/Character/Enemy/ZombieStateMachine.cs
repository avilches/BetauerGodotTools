using Betauer;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.StateMachine;
using Betauer.StateMachine.Sync;
using Betauer.Time;
using Godot;
using Veronenger.Game.Character.Player;
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
        Destroy,
        
        FallShort
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

        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
        [Inject] private MainStateMachine MainStateMachine { get; set; }
        [Inject] private CharacterManager CharacterManager { get; set; }
        [Inject] public KinematicPlatformMotion Body { get; set; }
        [Inject] private EnemyConfig EnemyConfig { get; set; }
        [Inject] private PlayerConfig PlayerConfig { get; set; }
        // State shared between states
        [Inject] private GodotStopwatch PatrolTimer  { get; set; }
        [Inject] private GodotStopwatch StateTimer  { get; set; }

        private ZombieController _zombieController;

        public readonly EnemyStatus Status = new();

        public void Start(string name, ZombieController zombie, IFlipper flippers, RayCast2D slopeRaycast, Position2D position2D) {
            _zombieController = zombie;
            zombie.AddChild(this);

            Body.Configure(name, zombie, flippers, null, slopeRaycast, position2D, MotionConfig.SnapToFloorVector, MotionConfig.FloorUpDirection);
            Body.ConfigureGravity(PlayerConfig.AirGravity, PlayerConfig.MaxFallingSpeed, PlayerConfig.MaxFloorGravity);

            AddOnExecuteStart((delta, state) => Body.StartFrame(delta));
            AddOnExecuteEnd((state) => Body.EndFrame());

            var debugOverlay = DebugOverlayManager.Follow(zombie);

            debugOverlay
                .Text("State", () => CurrentState.Key.ToString()).EndMonitor()
                .OpenBox()
                    .Vector("Motion", () => Body.Motion, PlayerConfig.MaxSpeed).SetChartWidth(100).EndMonitor()
                    .Graph("MotionX", () => Body.MotionX, -PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed).AddSeparator(0)
                        .AddSerie("MotionY").Load(() => Body.MotionY).EndSerie().EndMonitor()
                .CloseBox()
                .Graph("Floor", () => Body.IsOnFloor()).Keep(10).SetChartHeight(10)
                    .AddSerie("Slope").Load(() => Body.IsOnSlope()).EndSerie().EndMonitor()
                .GraphSpeed("Speed", PlayerConfig.JumpSpeed*2).EndMonitor()
                .Text("Floor", () => Body.GetFloorCollisionInfo()).EndMonitor()
                .Text("Ceiling", () => Body.GetCeilingCollisionInfo()).EndMonitor()
                .Text("Wall", () => Body.GetWallCollisionInfo());

            State(ZombieState.Idle)
                .Enter(() => {
                    StateTimer.Restart().SetAlarm(2f);
                    _zombieController.AnimationIdle.PlayLoop();
                })
                .Execute(context => {
                    if (!_zombieController.IsOnFloor()) {
                        // return context.Set(PlayerState.FallShort);
                        return context.None();
                    }
                    Body.Stop(EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan);
                    
                    if (StateTimer.IsAlarm()) {
                        return context.Set(ZombieState.Patrol);
                    }
                    return context.None();
                })
                .Build();
            
            State(ZombieState.Patrol)
                .Enter(() => {
                    StateTimer.Restart().SetAlarm(10f);
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

                    Body.Run(Body.IsFacingRight ? 1 : -1, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed, 
                        EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);

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
                    Body.ApplyDefaultGravity();
                    Body.ApplyLateralFriction(EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan);
                    Body.MoveSnapping();

                    return StateTimer.IsAlarm() ? context.Set(ZombieState.Patrol) : context.None();
                })
                .Build();

            On(ZombieTransition.Attacked,
                context => IsState(ZombieState.Attacked) ? context.None() : context.Push(ZombieState.Attacked));
            State(ZombieState.Attacked)
                .Enter(() => {
                    Body.FaceTo(CharacterManager.PlayerController.PlayerDetector);
                    Body.MotionY = EnemyConfig.MiniJumpOnAttack.y;
                    Body.MotionX = EnemyConfig.MiniJumpOnAttack.x * (Body.IsToTheLeftOf(CharacterManager.PlayerController.PlayerDetector) ? 1 : -1);
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

            State(ZombieState.FallShort)
                .Execute(context => {
                    if (Body.IsOnFloor()) return context.Set(ZombieState.Idle);
                    Body.ApplyDefaultGravity();
                    // Keep the speed from the move so if the player collides, the player could slide or stop
                    Body.Motion = Body.Slide();

                    return context.None();
                })
                .Build();

        }

        public void TriggerAttacked(Attack attack) {
            Status.Attack(attack);
            Enqueue(Status.IsDead() ? ZombieTransition.Dead : ZombieTransition.Attacked);
        }
    }
}