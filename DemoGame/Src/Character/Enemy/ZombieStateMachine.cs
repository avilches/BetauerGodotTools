using Betauer;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Input;
using Betauer.StateMachine;
using Betauer.StateMachine.Sync;
using Betauer.Core.Time;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Controller.Character;
using Veronenger.Managers;

namespace Veronenger.Character.Enemy {
    public enum ZombieEvent {
        Dead,
        Attacked
    }

    public enum ZombieState {
        Idle,
        Run,
        Landing,
        Jump,
        
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
    public partial class ZombieStateMachine : StateMachineNodeSync<ZombieState, ZombieEvent> {
        public ZombieStateMachine() : base(ZombieState.Idle, "Zombie.StateMachine") {
        }

        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
        [Inject] private CharacterManager CharacterManager { get; set; }
        
        [Inject] private PlayerConfig PlayerConfig { get; set; }
        [Inject] private EnemyConfig EnemyConfig { get; set; }
        [Inject] public KinematicPlatformMotion Body { get; set; }
        [Inject] private GodotStopwatch StateTimer  { get; set; }
        [Inject] private ICharacterHandler Handler { get; set; }

        private ZombieController _zombieController;

        private float XInput => Handler.XInput;
        private float YInput => Handler.YInput;
        private IActionHandler Jump => Handler.Jump;
        private IActionHandler Attack => Handler.Attack;
        private IActionHandler Float => Handler.Float;

        private float MotionX => Body.MotionX;
        private float MotionY => Body.MotionY;

        public readonly EnemyStatus Status = new();

        public void ApplyFloorGravity(float factor = 1.0F) {
            Body.ApplyGravity(PlayerConfig.FloorGravity * factor, PlayerConfig.MaxFallingSpeed);
        }

        public void ApplyAirGravity(float factor = 1.0F) {
            Body.ApplyGravity(PlayerConfig.AirGravity * factor, PlayerConfig.MaxFallingSpeed);
        }

        public void Start(string name, ZombieController zombie, IFlipper flippers, Marker2D marker2D) {
            _zombieController = zombie;
            zombie.AddChild(this);

            Body.Configure(name, zombie, flippers, marker2D, MotionConfig.FloorUpDirection);
            zombie.FloorStopOnSlope = true;
            // playerController.FloorBlockOnWall = true;
            zombie.FloorConstantSpeed = true;
            zombie.FloorSnapLength = MotionConfig.SnapLength;

            AddOnExecuteStart((delta, state) => Body.SetDelta(delta));
            AddOnTransition((args) => zombie.Label.Text = args.To.ToString());

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
                .GraphSpeed("Speed", PlayerConfig.JumpSpeed * 2).EndMonitor()
                .Text("Floor", () => Body.GetFloorCollisionInfo()).EndMonitor()
                .Text("Ceiling", () => Body.GetCeilingCollisionInfo()).EndMonitor()
                .Text("Wall", () => Body.GetWallCollisionInfo()).EndMonitor();

            On(ZombieEvent.Attacked).Then(context => IsState(ZombieState.Attacked) ? context.None() : context.Push(ZombieState.Attacked));
            On(ZombieEvent.Dead).Then(context=> context.Set(ZombieState.Destroy));
            
            State(ZombieState.Landing)
                .If(() => XInput == 0).Set(ZombieState.Idle)
                .If(() => true).Set(ZombieState.Run)
                .Build();

            State(ZombieState.Idle)
                .Enter(() => {
                    _zombieController.AnimationIdle.PlayLoop();
                })
                .Execute(() => {
                    ApplyFloorGravity();
                    Body.Stop(EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan);
                })
                .If(() => !Body.IsOnFloor()).Set(ZombieState.FallShort)
                .If(() => Jump.IsJustPressed()).Set(ZombieState.Jump)
                .If(() => XInput != 0).Set(ZombieState.Run)
                .Build();

            State(ZombieState.Run)
                .Enter(() => {
                    _zombieController.AnimationStep.PlayLoop();
                })
                .Execute(() => {
                    Body.Flip(XInput);
                    ApplyFloorGravity();
                    Body.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed, 
                        EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
                })
                .If(() => !Body.IsOnFloor()).Set(ZombieState.FallShort)
                .If(() => Jump.IsJustPressed()).Set(ZombieState.Jump)
                .If(() => XInput == 0 && MotionX == 0).Set(ZombieState.Idle)
                .Build();

            State(ZombieState.Attacked)
                .Enter(() => {
                    Body.FaceTo(CharacterManager.PlayerController.PlayerDetector);
                    Body.MotionY = EnemyConfig.MiniJumpOnAttack.y;
                    Body.MotionX = EnemyConfig.MiniJumpOnAttack.x * (Body.IsToTheLeftOf(CharacterManager.PlayerController.PlayerDetector) ? 1 : -1);
                    _zombieController.PlayAnimationAttacked();
                    StateTimer.Restart().SetAlarm(1f);
                })
                .Execute(() => {
                    ApplyAirGravity();
                    Body.Move();
                })
                .If(() => StateTimer.IsAlarm()).Pop()
                .Build();

            State(ZombieState.Destroy)
                .Enter(() => {
                    _zombieController.DisableAll();

                    if (Body.IsToTheLeftOf(CharacterManager.PlayerController.PlayerDetector)) {
                        _zombieController.AnimationDieLeft.PlayOnce(true);
                    } else {
                        _zombieController.AnimationDieRight.PlayOnce(true);
                    }
                })
                .Execute(() => {
                    if (!_zombieController.AnimationDieRight.Playing && !_zombieController.AnimationDieLeft.Playing) {
                        _zombieController.QueueFree();
                    }
                })
                .Build();

            State(ZombieState.Jump)
                .Enter(() => {
                    Body.MotionY = -PlayerConfig.JumpSpeed;
                    // DebugJump($"Jump start: decelerating to {(-PlayerConfig.JumpSpeed).ToString()}");
                    // _player.AnimationJump.PlayLoop();
                })
                .Execute(() => {
                    ApplyAirGravity();
                    Body.Flip(XInput);
                    Body.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed,
                        EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
                })
                .If(() => MotionY >= 0).Set(ZombieState.FallShort)
                .If(Body.IsOnFloor).Set(ZombieState.Landing)
                .Build();

            State(ZombieState.FallShort)
                .Execute(() => {
                    ApplyAirGravity();
                    Body.Flip(XInput);
                    Body.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed,
                        EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
                })
                .If(Body.IsOnFloor).Set(ZombieState.Landing)
                .Build();

        }

        
        
        public void TriggerAttacked(Attack attack) {
            Status.Attack(attack);
            Enqueue(Status.IsDead() ? ZombieEvent.Dead : ZombieEvent.Attacked);
        }
    }
}