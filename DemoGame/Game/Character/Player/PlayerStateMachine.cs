using System;
using Betauer;
using Betauer.Application.Monitor;
using Betauer.Bus;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Input;

using Betauer.StateMachine;
using Betauer.StateMachine.Sync;
using Betauer.Time;
using Godot;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Character.Player {
    
    public enum PlayerState {
        Idle,
        Run,
        FallShort,
        FallLong,
        Jump,
        Death,
            
        Float,
    }

    public enum PlayerTransition {
        Death
    }

    [Service(Lifetime.Transient)]
    public class PlayerStateMachine : StateMachineNodeSync<PlayerState, PlayerTransition> {
        private static readonly Logger LoggerJumpVelocity = LoggerFactory.GetLogger("JumpVelocity");
        private void DebugJump(string message) => LoggerJumpVelocity.Debug(message);

        public PlayerStateMachine() : base(PlayerState.Idle, "Player.StateMachine", ProcessMode.Physics) {
        }

        [Inject] private PlatformManager PlatformManager { get; set;}
        [Inject] public PlayerConfig PlayerConfig { get; set;}
        [Inject] private InputAction Left { get; set;}
        [Inject] private InputAction Up { get; set;}
        [Inject] private InputAction Jump { get; set;}
        [Inject] private InputAction Attack { get; set;}
        [Inject] private InputAction Float { get; set;}
        [Inject] public KinematicPlatformMotion Body { get; set; }

        private PlayerController _player;

        // Input from the player
        private AxisAction LateralMotion => Left.AxisAction;
        private AxisAction VerticalMotion => Up.AxisAction;
        private float XInput => LateralMotion.Strength;
        private float YInput => VerticalMotion.Strength;
        private bool IsRight => XInput > 0;
        private bool IsLeft => XInput < 0;
        private bool IsUp => YInput < 0;
        private bool IsDown => YInput > 0;
        private float MotionX => Body.MotionX;
        private float MotionY => Body.MotionY;

        // State sharad between states
        private bool _coyoteJumpEnabled = false;
        [Inject] private GodotStopwatch JumpHelperTimer { get; set; }
        [Inject] private GodotStopwatch FallingTimer { get; set; }
        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
        [Inject] private Bus Bus { get; set; }

        private MonitorText? _coyoteJumpState;
        private MonitorText? _jumpHelperState;

        public void Start(string name, PlayerController playerController, IFlipper flippers) {
            _player = playerController;

            Body.Configure(name, playerController, flippers, _player.FloorRaycasts, _player.SlopeRaycast, _player.Position2D, MotionConfig.SnapToFloorVector, MotionConfig.FloorUpDirection);
            Body.ConfigureGravity(PlayerConfig.AirGravity, PlayerConfig.MaxFallingSpeed, PlayerConfig.MaxFloorGravity);
            
            AddOnExecuteStart((delta, _) => Body.StartFrame(delta));
            AddOnExecuteEnd((_) => Body.EndFrame());
            Bus.Subscribe(Enqueue);
            GroundStates();
            AirStates();

            var debugOverlay = DebugOverlayManager.Overlay(_player);
            // debugOverlay.Text("JumpHelperTimer", () => JumpHelperTimer.ToString());
            // _jumpHelperState = debugOverlay.Text("JumpHelperState");
            // debugOverlay.Text("FallingTimer", () => FallingTimer.ToString()).Disable();
            // _coyoteJumpState = debugOverlay.Text("CoyoteState");

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
        }

        public void GroundStates() {
            bool CheckGroundAttack() {
                if (!Attack.IsJustPressed()) return false;
                // Attack was pressed
                _player.AnimationAttack.PlayOnce();
                return true;
            }

            void EnableSlopeStairs() {
                if (_player.IsOnSlopeStairsDown()) {
                    if (IsUp) {
                        _player.EnableSlopeStairs();
                    } else {
                        _player.DisableSlopeStairs();
                    }
                } else if (_player.IsOnSlopeStairsUp()) {
                    if (IsDown) {
                        _player.EnableSlopeStairs();
                    } else {
                        _player.DisableSlopeStairs();
                    }
                }
            }

            State(PlayerState.Idle)
                .Enter(() => {
                    _player.AnimationIdle.PlayLoop();
                    if (Body.IsOnSlope()) {
                        // Stop go down fast when the player lands in a slope
                        Body.MotionX = 0;
                    }
                })
                .Execute(context => {
                    CheckGroundAttack();

                    if (!Body.IsOnFloor()) {
                        return context.Set(PlayerState.FallShort);
                    }

                    if (XInput != 0) {
                        return context.Set(PlayerState.Run);
                    }
                    if (Body.IsOnWall()) {
                        Body.MotionX = 0;
                    } else {
                        Body.ApplyLateralFriction(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
                    }

                    if (Jump.IsJustPressed()) {
                        if (IsDown && IsOnFallingPlatform()) {
                            PlatformManager.BodyFallFromPlatform(_player);
                        } else {
                            return context.Set(PlayerState.Jump);
                        }
                    }

                    Body.ApplyDefaultGravity();
                    Body.MoveSnapping();

                    return context.None();
                })
                .Build();

            State(PlayerState.Run)
                .Enter(() => {
                    _player.AnimationRun.PlayLoop();
                })
                .Execute(context => {
                    CheckGroundAttack();

                    if (!Body.IsOnFloor()) {
                        _coyoteJumpEnabled = true;
                        if (Body.MotionY < 0) Body.MotionY = 0f;
                        return context.Set(PlayerState.FallShort);
                    }

                    if (XInput == 0 && MotionX == 0) {
                        return context.Set(PlayerState.Idle);
                    }

                    if (Jump.IsJustPressed()) {
                        if (IsDown && IsOnFallingPlatform()) {
                            PlatformManager.BodyFallFromPlatform(_player);
                        } else {
                            return context.Set(PlayerState.Jump);
                        }
                    }

                    // Suelo + no salto + movimiento/inercia
                    EnableSlopeStairs();

                    if (_player.IsAttacking) {
                        Body.ApplyLateralFriction(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
                    } else {
                        Body.Flip(XInput);
                        Body.AddLateralSpeed(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.Friction, 
                            PlayerConfig.StopIfSpeedIsLessThan, 0);
                        if (Body.IsOnSlope()) {
                            Body.LimitMotionNormalized(PlayerConfig.MaxSpeed);
                        }
                    }
                    Body.ApplyDefaultGravity();
                    var pendingInertia = Body.MoveSnapping();
                    if (Body.IsOnSlope()) {
                        // Ensure the body can climb up or down slopes. Without this, the player will go down too fast
                        // and go up too slow
                        // And never use the pendingInertia.x when climbing a slope!!!
                        Body.MotionY = pendingInertia.y;
                    }
                    

                    return context.None();
                })
                .Build();

            On(PlayerTransition.Death, ctx => ctx.Set(PlayerState.Death));
            State(PlayerState.Death)
                .Enter(() => {
                    Console.WriteLine("MUERTO");
                    Bus.Publish(MainTransition.EndGame);
                })
                .Build();


        }

        private bool IsOnPlatform() => PlatformManager.IsPlatform(Body.GetFloor());
        private bool IsOnFallingPlatform() => PlatformManager.IsFallingPlatform(Body.GetFloor());
        private bool IsMovingPlatform() => PlatformManager.IsMovingPlatform(Body.GetFloor());

        private ExecuteContext<PlayerState, PlayerTransition>.Response CheckLanding(ExecuteContext<PlayerState, PlayerTransition> context) {
            if (!Body.IsOnFloor()) return context.None(); // Still in the air! :)
            // IsJustLanded() is true here
            
            PlatformManager.BodyStopFallFromPlatform(_player);

            // Check helper jump
            if (JumpHelperTimer.IsRunning) {
                JumpHelperTimer.Stop();
                if (JumpHelperTimer.Elapsed <= PlayerConfig.JumpHelperTime) {
                    _jumpHelperState?.Show($"{JumpHelperTimer.Elapsed.ToString()} <= {PlayerConfig.JumpHelperTime.ToString()} Done!");
                    return context.Set(PlayerState.Jump);
                }
                _jumpHelperState?.Show($"{JumpHelperTimer.Elapsed.ToString()} <= {PlayerConfig.JumpHelperTime.ToString()} TOO MUCH TIME");
            }

            return context.Set(XInput == 0 ? PlayerState.Idle : PlayerState.Run);
        }

        public void AirStates() {

            bool CheckAirAttack() {
                if (!Attack.IsJustPressed()) return false;
                // Attack was pressed
                _player.AnimationJumpAttack.PlayOnce();
                return true;
            }

            bool CheckCoyoteJump() {
                if (!Jump.IsJustPressed()) return false;
                // Jump was pressed
                JumpHelperTimer.Restart();
                if (FallingTimer.IsRunning) {
                    if (FallingTimer.Elapsed <= PlayerConfig.CoyoteJumpTime) {
                        _coyoteJumpState?.Show($"{FallingTimer.Elapsed.ToString()} <= {PlayerConfig.CoyoteJumpTime.ToString()} Done!");
                        return true;
                    }
                    _coyoteJumpState?.Show($"{FallingTimer.Elapsed.ToString()} > {PlayerConfig.CoyoteJumpTime.ToString()} TOO LATE");
                }
                return false;
            }

            State(PlayerState.Jump)
                .Enter(() => {
                    Body.MotionY = -PlayerConfig.JumpSpeed;
                    DebugJump($"Jump start: decelerating to {(-PlayerConfig.JumpSpeed).ToString()}");
                    _player.AnimationJump.PlayLoop();
                })
                .Execute(context => {
                    if (Float.IsPressed()) {
                        return context.Set(PlayerState.Float);
                    }
                    CheckAirAttack();

                    if (Jump.IsReleased() && MotionY < -PlayerConfig.JumpSpeedMin) {
                        DebugJump($"Short jump: decelerating from {MotionY.ToString()} to {(-PlayerConfig.JumpSpeedMin).ToString()}");
                        Body.MotionY = -PlayerConfig.JumpSpeedMin;
                    }

                    Body.AddLateralSpeed(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
                        PlayerConfig.StopIfSpeedIsLessThan, 0);
                    Body.Flip(XInput);
                    Body.ApplyDefaultGravity();
                    // Keep the speed from the move so if the player collides, the player could slide or stop
                    Body.Motion = Body.MoveSlide();

                    if (MotionY >= 0) {
                        return context.Set(PlayerState.FallShort);
                    }

                    return CheckLanding(context);
                })
                .Build();
                

            State(PlayerState.FallShort)
                .Enter(() => {
                    FallingTimer.Restart();
                })
                .Execute(context => {
                    CheckAirAttack();
                    if (Float.IsJustPressed()) {
                        return context.Set(PlayerState.Float);
                    }

                    // The flag _coyoteJumpEnabled is only enabled from Running -> fallShort
                    // Other cases (from idle or jump), the coyote is not enabled 
                    if (_coyoteJumpEnabled && CheckCoyoteJump()) {
                        _coyoteJumpEnabled = false;
                        return context.Set(PlayerState.Jump);
                    }
                    if (MotionY > PlayerConfig.StartFallingSpeed) {
                        return context.Set(PlayerState.FallLong);
                    }

                    Body.AddLateralSpeed(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
                        PlayerConfig.StopIfSpeedIsLessThan, 0);
                    Body.Flip(XInput);
                    Body.ApplyDefaultGravity();
                    // Keep the speed from the move so if the player collides, the player could slide or stop
                    Body.Motion = Body.MoveSlide();

                    return CheckLanding(context);
                })
                .Build();
                
            State(PlayerState.FallLong)
                .Enter(() => {
                    FallingTimer.Stop();
                    if (FallingTimer.Elapsed > PlayerConfig.CoyoteJumpTime) {
                        _coyoteJumpState?.Show($"Coyote jump will never happen in FallLong state: {FallingTimer.Elapsed} > {PlayerConfig.CoyoteJumpTime}");
                    }
                    _player.AnimationFall.PlayLoop();
                })
                .Execute(context => {
                        CheckAirAttack();
                        if (Float.IsPressed()) {
                            return context.Set(PlayerState.Float);
                        }

                        if (CheckCoyoteJump()) {
                            return context.Set(PlayerState.Jump);
                        }

                        Body.Flip(XInput);
                        Body.AddLateralSpeed(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed,
                            PlayerConfig.AirResistance, PlayerConfig.StopIfSpeedIsLessThan, 0);
                        Body.ApplyDefaultGravity();
                        // Keep the speed from the move so if the player collides, the player could slide or stop
                        Body.Motion = Body.MoveSlide();

                        return CheckLanding(context);
                    }
                )
                .Build();

            State(PlayerState.Float)
                .Enter(() => {
                })
                .Execute(context => {
                    if (Float.IsJustPressed()) {
                        return context.Set(PlayerState.FallShort);
                    }
                    Body.AddSpeed(XInput, YInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed,
                        PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan, 0);
                    Body.MoveSlide();
                    return context.None();
                }).Build();

            AddOnTransition(args => Console.WriteLine(args.To));

        }
    }
}