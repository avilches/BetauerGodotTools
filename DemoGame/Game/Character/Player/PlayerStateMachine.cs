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
        [Inject] public KinematicPlatformMotion PlatformBody { get; set; }

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
        private float MotionX => PlatformBody.MotionX;
        private float MotionY => PlatformBody.MotionY;

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

            PlatformBody.Configure(name, playerController, flippers, _player.FloorRaycasts, _player.SlopeRaycast, _player.Position2D, MotionConfig.SnapToFloorVector, MotionConfig.FloorUpDirection);
            PlatformBody.ConfigureGravity(PlayerConfig.AirGravity, PlayerConfig.MaxFallingSpeed, PlayerConfig.MaxFloorGravity);
            
            AddOnExecuteStart((delta, _) => PlatformBody.StartFrame(delta));
            AddOnExecuteEnd((_) => PlatformBody.EndFrame());
            Bus.Subscribe(Enqueue);
            GroundStates();
            AirStates();

            var debugOverlay = DebugOverlayManager.Overlay(_player).StopFollowing();
            // debugOverlay.Text("JumpHelperTimer", () => JumpHelperTimer.ToString());
            // _jumpHelperState = debugOverlay.Text("JumpHelperState");
            // debugOverlay.Text("FallingTimer", () => FallingTimer.ToString()).Disable();
            // _coyoteJumpState = debugOverlay.Text("CoyoteState");

            debugOverlay
                .Text("State", () => CurrentState.Key.ToString()).EndMonitor()
                .OpenBox()
                    .Vector("Motion", () => PlatformBody.Motion, PlayerConfig.MaxSpeed)
                        .SetChartWidth(100)
                    .EndMonitor()
                    .Graph("MotionX", () => PlatformBody.MotionX, -PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed)
                        .AddSeparator(0)
                        .AddSerie("MotionY").Load(() => PlatformBody.MotionY).EndSerie()
                    .EndMonitor()
                .CloseBox()
                .Graph("Floor", () => PlatformBody.IsOnFloor())
                    .Keep(10)
                    .SetChartHeight(10)
                    .AddSerie("Slope").Load(() => PlatformBody.IsOnSlope()).EndSerie()
                .EndMonitor()
                .GraphSpeed("Speed", PlayerConfig.JumpSpeed*2).EndMonitor()
                .Text("Floor", () => PlatformBody.GetFloorCollisionInfo()).EndMonitor()
                .Text("Ceiling", () => PlatformBody.GetCeilingCollisionInfo()).EndMonitor()
                .Text("Wall", () => PlatformBody.GetWallCollisionInfo());
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
                    if (PlatformBody.IsOnSlope()) {
                        // Stop go down fast when the player lands in a slope
                        PlatformBody.MotionX = 0;
                    }
                })
                .Execute(context => {
                    CheckGroundAttack();

                    if (!PlatformBody.IsOnFloor()) {
                        return context.Set(PlayerState.FallShort);
                    }

                    if (XInput != 0) {
                        return context.Set(PlayerState.Run);
                    }
                    if (PlatformBody.IsOnWall()) {
                        PlatformBody.MotionX = 0;
                    } else {
                        PlatformBody.ApplyLateralFriction(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
                    }

                    if (Jump.IsJustPressed()) {
                        if (IsDown && IsOnFallingPlatform()) {
                            PlatformManager.BodyFallFromPlatform(_player);
                        } else {
                            return context.Set(PlayerState.Jump);
                        }
                    }

                    PlatformBody.ApplyDefaultGravity();
                    PlatformBody.MoveSnapping();

                    return context.None();
                })
                .Build();

            State(PlayerState.Run)
                .Enter(() => {
                    _player.AnimationRun.PlayLoop();
                })
                .Execute(context => {
                    CheckGroundAttack();

                    if (!PlatformBody.IsOnFloor()) {
                        _coyoteJumpEnabled = true;
                        if (PlatformBody.MotionY < 0) PlatformBody.MotionY = 0f;
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
                        PlatformBody.ApplyLateralFriction(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
                    } else {
                        PlatformBody.Flip(XInput);
                        PlatformBody.AddLateralSpeed(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.Friction, 
                            PlayerConfig.StopIfSpeedIsLessThan, 0);
                        if (PlatformBody.IsOnSlope()) {
                            PlatformBody.LimitMotionNormalized(PlayerConfig.MaxSpeed);
                        }
                    }
                    PlatformBody.ApplyDefaultGravity();
                    var pendingInertia = PlatformBody.MoveSnapping();
                    if (PlatformBody.IsOnSlope()) {
                        // Ensure the body can climb up or down slopes. Without this, the player will go down too fast
                        // and go up too slow
                        // And never use the pendingInertia.x when climbing a slope!!!
                        PlatformBody.MotionY = pendingInertia.y;
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

        private bool IsOnPlatform() => PlatformManager.IsPlatform(PlatformBody.GetFloor());
        private bool IsOnFallingPlatform() => PlatformManager.IsFallingPlatform(PlatformBody.GetFloor());
        private bool IsMovingPlatform() => PlatformManager.IsMovingPlatform(PlatformBody.GetFloor());

        private ExecuteContext<PlayerState, PlayerTransition>.Response CheckLanding(ExecuteContext<PlayerState, PlayerTransition> context) {
            if (!PlatformBody.IsOnFloor()) return context.None(); // Still in the air! :)
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
                    PlatformBody.MotionY = -PlayerConfig.JumpSpeed;
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
                        PlatformBody.MotionY = -PlayerConfig.JumpSpeedMin;
                    }

                    PlatformBody.AddLateralSpeed(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
                        PlayerConfig.StopIfSpeedIsLessThan, 0);
                    PlatformBody.Flip(XInput);
                    PlatformBody.ApplyDefaultGravity();
                    // Keep the speed from the move so if the player collides, the player could slide or stop
                    PlatformBody.Motion = PlatformBody.MoveSlide();

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

                    PlatformBody.AddLateralSpeed(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
                        PlayerConfig.StopIfSpeedIsLessThan, 0);
                    PlatformBody.Flip(XInput);
                    PlatformBody.ApplyDefaultGravity();
                    // Keep the speed from the move so if the player collides, the player could slide or stop
                    PlatformBody.Motion = PlatformBody.MoveSlide();

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

                        PlatformBody.Flip(XInput);
                        PlatformBody.AddLateralSpeed(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed,
                            PlayerConfig.AirResistance, PlayerConfig.StopIfSpeedIsLessThan, 0);
                        PlatformBody.ApplyDefaultGravity();
                        // Keep the speed from the move so if the player collides, the player could slide or stop
                        PlatformBody.Motion = PlatformBody.MoveSlide();

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
                    PlatformBody.AddSpeed(XInput, YInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed,
                        PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan, 0);
                    PlatformBody.MoveSlide();
                    return context.None();
                }).Build();

            AddOnTransition(args => Console.WriteLine(args.To));

        }
    }
}