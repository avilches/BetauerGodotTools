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

        public PlayerStateMachine() : base(PlayerState.Idle, "Player.StateMachine") {
        }

        [Inject] private PlatformManager PlatformManager { get; set;}
        [Inject] private PlayerConfig PlayerConfig { get; set;}
        [Inject] private InputAction Left { get; set;}
        [Inject] private InputAction Up { get; set;}
        [Inject] private InputAction Jump { get; set;}
        [Inject] private InputAction Attack { get; set;}
        [Inject] private InputAction Float { get; set;}
        [Inject] private KinematicPlatformMotionBody PlatformBody { get; set; }
        [Inject] private KinematicTopDownMotionBody TopDownBody { get; set; }

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
        private float SpeedX => PlatformBody.SpeedX;
        private float SpeedY => PlatformBody.SpeedY;
        private MotionConfig MotionConfig => PlayerConfig.MotionConfig;

        // State sharad between states
        private bool _coyoteJumpEnabled = false;
        [Inject] private GodotStopwatch JumpHelperTimer { get; set; }
        [Inject] private GodotStopwatch FallingTimer { get; set; }
        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
        [Inject] private Bus Bus { get; set; }

        private Monitor _coyoteJumpState;
        private Monitor _jumpHelperState;

        public void Start(string name, PlayerController playerController, IFlipper flippers, RayCast2D slopeDetector, Position2D position2D) {
            _player = playerController;

            PlatformBody.Configure(name, playerController, flippers, slopeDetector, position2D, MotionConfig.Configure);
            TopDownBody.Configure(name, playerController, position2D, MotionConfig.Configure);
            TopDownBody.DefaultSlideOnSlopes = true;
            
            AddOnExecuteStart((delta, _) => PlatformBody.StartFrame(delta));
            AddOnExecuteStart((delta, _) => TopDownBody.StartFrame(delta));
            AddOnExecuteEnd((_) => PlatformBody.EndFrame());
            AddOnExecuteEnd((_) => TopDownBody.EndFrame());
            Bus.Subscribe(Enqueue);
            GroundStates();
            AirStates();

            var debugOverlay = DebugOverlayManager.Overlay(_player);
            debugOverlay.Text("JumpHelperTimer", () => JumpHelperTimer.ToString());
            _jumpHelperState = debugOverlay.Text("JumpHelperState");
            debugOverlay.Text("FallingTimer", () => FallingTimer.ToString());
            _coyoteJumpState = debugOverlay.Text("CoyoteState");

            debugOverlay.Text("Speed", () => PlatformBody.Speed.ToString("F3"));
            debugOverlay.Text("State", () => CurrentState.Key.ToString());
            debugOverlay.Graph("SpeedY", () => PlatformBody.SpeedY, -PlayerConfig.JumpForce, PlayerConfig.JumpForce).Keep(10).SetColor(Colors.Fuchsia);
            debugOverlay.Graph("SpeedX", () => PlatformBody.SpeedX, -PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed).Keep(10).SetColor(Colors.Aquamarine);

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
                })
                .Execute(context => {
                    CheckGroundAttack();

                    if (!_player.IsOnFloor()) {
                        return context.Set(PlayerState.FallShort);
                    }

                    if (XInput != 0) {
                        return context.Set(PlayerState.Run);
                    }
                    PlatformBody.StopLateralSpeedWithFriction(MotionConfig.Friction, MotionConfig.StopIfSpeedIsLessThan);

                    if (Jump.IsJustPressed()) {
                        if (IsDown && PlatformBody.IsOnFallingPlatform()) {
                            PlatformManager.BodyFallFromPlatform(_player);
                        } else {
                            return context.Set(PlayerState.Jump);
                        }
                    }

                    // Suelo + no salto + sin movimiento

                    if (!PlatformBody.IsOnMovingPlatform()) {
                        // No gravity in moving platforms
                        // Gravity in slopes to avoid go down slowly
                        PlatformBody.ApplyDefaultGravity();
                    }
                    PlatformBody.MoveSnapping();

                    return context.None();
                })
                .Build();

            State(PlayerState.Run)
                .Enter(() => { _player.AnimationRun.PlayLoop(); })
                .Execute(context => {
                    CheckGroundAttack();

                    if (!_player.IsOnFloor()) {
                        _coyoteJumpEnabled = true;
                        PlatformBody.SpeedY = 0f;
                        return context.Set(PlayerState.FallShort);
                    }

                    if (XInput == 0 && SpeedX == 0) {
                        return context.Set(PlayerState.Idle);
                    }

                    if (Jump.IsJustPressed()) {
                        if (IsDown && PlatformBody.IsOnFallingPlatform()) {
                            PlatformManager.BodyFallFromPlatform(_player);
                        } else {
                            return context.Set(PlayerState.Jump);
                        }
                    }

                    // Suelo + no salto + movimiento/inercia
                    EnableSlopeStairs();

                    if (_player.IsAttacking) {
                        PlatformBody.StopLateralSpeedWithFriction(MotionConfig.Friction,
                            MotionConfig.StopIfSpeedIsLessThan);
                    } else {
                        PlatformBody.AddLateralSpeed(XInput, MotionConfig.Acceleration, MotionConfig.MaxSpeed, MotionConfig.Friction, 
                            MotionConfig.StopIfSpeedIsLessThan, 0);
                        PlatformBody.Flip(XInput);
                    }

                    PlatformBody.MoveSnapping();

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

        private ExecuteContext<PlayerState, PlayerTransition>.Response CheckLanding(ExecuteContext<PlayerState, PlayerTransition> context) {
            if (!_player.IsOnFloor()) return context.None(); // Still in the air! :)

            PlatformManager.BodyStopFallFromPlatform(_player);

            // Check helper jump
            if (JumpHelperTimer.IsRunning) {
                JumpHelperTimer.Stop();
                if (JumpHelperTimer.Elapsed <= PlayerConfig.JumpHelperTime) {
                    _jumpHelperState.SetText($"{JumpHelperTimer.Elapsed.ToString()} <= {PlayerConfig.JumpHelperTime.ToString()} Done!");
                    return context.Set(PlayerState.Jump);
                }
                _jumpHelperState.SetText($"{JumpHelperTimer.Elapsed.ToString()} <= {PlayerConfig.JumpHelperTime.ToString()} TOO MUCH TIME");
            }

            // Debug("Just grounded!");
            if (XInput == 0) {
                if (PlatformBody.IsOnSlope()) {
                    // Evita resbalarse hacia abajo al caer sobre un slope
                    PlatformBody.SpeedX = 0;
                }
                return context.Set(PlayerState.Idle);
            }
            return context.Set(PlayerState.Run);
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
                        _coyoteJumpState.SetText($"{FallingTimer.Elapsed.ToString()} <= {PlayerConfig.CoyoteJumpTime.ToString()} Done!");
                        return true;
                    }
                    _coyoteJumpState.SetText($"{FallingTimer.Elapsed.ToString()} > {PlayerConfig.CoyoteJumpTime.ToString()} TOO LATE");
                }
                return false;
            }

            State(PlayerState.Jump)
                .Enter(() => {
                    PlatformBody.SpeedY = -MotionConfig.JumpForce;
                    DebugJump($"Jump start: decelerating to {(-MotionConfig.JumpForce).ToString()}");
                    _player.AnimationJump.PlayLoop();
                })
                .Execute(context => {
                    if (Float.IsPressed()) {
                        return context.Set(PlayerState.Float);
                    }
                    CheckAirAttack();

                    if (Jump.IsReleased() && SpeedY < -MotionConfig.JumpForceMin) {
                        DebugJump($"Short jump: decelerating from {SpeedY.ToString()} to {(-MotionConfig.JumpForceMin).ToString()}");
                        PlatformBody.SpeedY = -MotionConfig.JumpForceMin;
                    }

                    PlatformBody.AddLateralSpeed(XInput, MotionConfig.Acceleration, MotionConfig.MaxSpeed, MotionConfig.AirResistance,
                        MotionConfig.StopIfSpeedIsLessThan, 0);
                    PlatformBody.Flip(XInput);
                    PlatformBody.Fall();

                    if (SpeedY >= 0) {
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
                    if (Float.IsPressed()) {
                        return context.Set(PlayerState.Float);
                    }

                    // The flag _coyoteJumpEnabled is only enabled from Running -> fallShort
                    // Other cases (from idle or jump), the coyote is not enabled 
                    if (_coyoteJumpEnabled && CheckCoyoteJump()) {
                        _coyoteJumpEnabled = false;
                        return context.Set(PlayerState.Jump);
                    }
                    if (SpeedY > MotionConfig.StartFallingSpeed) {
                        return context.Set(PlayerState.FallLong);
                    }

                    PlatformBody.AddLateralSpeed(XInput, MotionConfig.Acceleration, MotionConfig.MaxSpeed, MotionConfig.AirResistance,
                        MotionConfig.StopIfSpeedIsLessThan, 0);
                    PlatformBody.Flip(XInput);

                    PlatformBody.Fall();

                    return CheckLanding(context);
                })
                .Build();
                

            State(PlayerState.FallLong)
                .Enter(() => {
                    FallingTimer.Stop();
                    // if (_fallingTimer.Elapsed > PlayerConfig.CoyoteJumpTime) {
                        // _coyoteJumpState = $"Coyote jump will never happen in FallLong state: {_fallingTimer.Elapsed} > {PlayerConfig.CoyoteJumpTime}";
                    // }
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

                        PlatformBody.AddLateralSpeed(XInput, MotionConfig.Acceleration, MotionConfig.MaxSpeed,
                            MotionConfig.AirResistance, MotionConfig.StopIfSpeedIsLessThan, 0);
                        PlatformBody.Flip(XInput);

                        PlatformBody.Fall();

                        return CheckLanding(context);
                    }
                )
                .Build();

            State(PlayerState.Float)
                .Enter(() => {
                    TopDownBody.Speed = PlatformBody.Speed;
                })
                .Execute(context => {
                    if (Float.IsReleased()) {
                        PlatformBody.Speed = TopDownBody.Speed;
                        return context.Set(PlayerState.FallShort);
                    }
                    TopDownBody.AddSpeed(XInput, YInput, MotionConfig.Acceleration, MotionConfig.MaxSpeed, MotionConfig.MaxSpeed,
                        MotionConfig.Friction, MotionConfig.StopIfSpeedIsLessThan, 0);
                    TopDownBody.Slide();
                    return context.None();
                }).Build();
            
            AddOnTransition(args => _player.Label.Text = args.To.ToString());

        }
    }
}