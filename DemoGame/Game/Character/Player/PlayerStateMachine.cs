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
        Landing,
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
        private bool IsPressingUp => YInput < 0;
        private bool IsPressingDown => YInput > 0;
        private float MotionX => Body.MotionX;
        private float MotionY => Body.MotionY;

        [Inject] private GodotStopwatch LastJumpOnAirTimer { get; set; }
        [Inject] private GodotStopwatch CoyoteFallingTimer { get; set; }
        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
        [Inject] private Bus Bus { get; set; }

        private bool IsOnPlatform() => PlatformManager.IsPlatform(Body.GetFloor());
        private bool IsOnFallingPlatform() => PlatformManager.IsFallingPlatform(Body.GetFloor());
        private bool IsMovingPlatform() => PlatformManager.IsMovingPlatform(Body.GetFloor());
        private MonitorText? _coyoteMonitor;
        private MonitorText? _jumpHelperMonitor;

        public void Start(string name, PlayerController playerController, IFlipper flippers) {
            _player = playerController;

            Body.Configure(name, playerController, flippers, _player.FloorRaycasts, _player.SlopeRaycast, _player.Position2D, MotionConfig.SnapToFloorVector, MotionConfig.FloorUpDirection);
            Body.ConfigureGravity(PlayerConfig.AirGravity, PlayerConfig.MaxFallingSpeed, PlayerConfig.MaxFloorGravity);
            
            AddOnExecuteStart((delta, _) => Body.SetDelta(delta));
            Bus.Subscribe(Enqueue);
            GroundStates();
            AirStates();

            var debugOverlay = DebugOverlayManager.Overlay(_player);
            debugOverlay.Text("JumpHelperTimer", () => LastJumpOnAirTimer.ToString());
            _jumpHelperMonitor = debugOverlay.Text("JumpHelper");
            debugOverlay.Text("CoyoteFallingTimer", () => CoyoteFallingTimer.ToString());
            _coyoteMonitor = debugOverlay.Text("Coyote");

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
                    if (IsPressingUp) {
                        _player.EnableSlopeStairs();
                    } else {
                        _player.DisableSlopeStairs();
                    }
                } else if (_player.IsOnSlopeStairsUp()) {
                    if (IsPressingDown) {
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
                .Execute(() => {
                    CheckGroundAttack();
                    Body.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
                })
                .Condition(() => !Body.IsOnFloor(),context => context.Set(PlayerState.FallShort))
                .Condition(() => Jump.IsJustPressed() && IsPressingDown && IsOnFallingPlatform(),
                    context => {
                        PlatformManager.BodyFallFromPlatform(_player);
                        return context.Set(PlayerState.FallShort);
                    })
                .Condition(() => Jump.IsJustPressed(), context => context.Set(PlayerState.Jump))
                .Condition(() => XInput != 0, context => context.Set(PlayerState.Run))
                .Build();
                
            State(PlayerState.Run)
                .Enter(() => {
                    _player.AnimationRun.PlayLoop();
                })
                .Execute(() => {
                    CheckGroundAttack();

                    // Suelo + no salto + movimiento/inercia
                    EnableSlopeStairs();

                    if (_player.IsAttacking) {
                        Body.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
                    } else {
                        Body.Flip(XInput);
                        Body.Run(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.Friction, 
                            PlayerConfig.StopIfSpeedIsLessThan, 0);
                    }
                })
                .Condition(() => !Body.IsOnFloor(), 
                    context => {
                        CoyoteFallingTimer.Restart();
                        return context.Set(PlayerState.FallShort);
                    })
                .Condition(() => Jump.IsJustPressed() && IsPressingDown && IsOnFallingPlatform(),
                    context => {
                        PlatformManager.BodyFallFromPlatform(_player);
                        return context.Set(PlayerState.FallShort);
                    })
                .Condition(() => Jump.IsJustPressed(), context => context.Set(PlayerState.Jump))
                .Condition(() => XInput == 0 && MotionX == 0, context => context.Set(PlayerState.Idle))
                .Build();

            On(PlayerTransition.Death, ctx => ctx.Set(PlayerState.Death));
            State(PlayerState.Death)
                .Enter(() => {
                    Console.WriteLine("MUERTO");
                    Bus.Publish(MainTransition.EndGame);
                })
                .Build();


            State(PlayerState.Landing)
                .Enter(() => {
                    CoyoteFallingTimer.Stop(); // not really needed, but less noise in the debug overlay
                    PlatformManager.BodyStopFallFromPlatform(_player);
                })
                .Execute(() => {
                    if (LastJumpOnAirTimer.IsRunning) {
                        if (LastJumpOnAirTimer.Elapsed <= PlayerConfig.JumpHelperTime) {
                            _jumpHelperMonitor?.Show($"{LastJumpOnAirTimer.Elapsed.ToString()} <= {PlayerConfig.JumpHelperTime.ToString()} Done!");
                        } else {
                            // The timer acts like a flag: if running, the player can jump, if stopped, the player can't
                            LastJumpOnAirTimer.Stop();
                            _jumpHelperMonitor?.Show($"{LastJumpOnAirTimer.Elapsed.ToString()} > {PlayerConfig.JumpHelperTime.ToString()} TOO MUCH TIME");
                        }
                    }
                })
                .Condition(() => LastJumpOnAirTimer.IsRunning, context => context.Set(PlayerState.Jump))
                .Condition(() => XInput == 0, context => context.Set(PlayerState.Idle))
                .Condition(() => true, context => context.Set(PlayerState.Run))
                .Build();

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
                LastJumpOnAirTimer.Restart();
                if (!CoyoteFallingTimer.IsRunning) return false;
                
                CoyoteFallingTimer.Stop();
                if (CoyoteFallingTimer.Elapsed <= PlayerConfig.CoyoteJumpTime) {
                    _coyoteMonitor?.Show($"{CoyoteFallingTimer.Elapsed.ToString()} <= {PlayerConfig.CoyoteJumpTime.ToString()} Done!");
                    return true;
                }
                _coyoteMonitor?.Show($"{CoyoteFallingTimer.Elapsed.ToString()} > {PlayerConfig.CoyoteJumpTime.ToString()} TOO LATE");
                return false;
            }

            State(PlayerState.Jump)
                .Enter(() => {
                    Body.MotionY = -PlayerConfig.JumpSpeed;
                    DebugJump($"Jump start: decelerating to {(-PlayerConfig.JumpSpeed).ToString()}");
                    _player.AnimationJump.PlayLoop();
                })
                .Execute(() => {
                    CheckAirAttack();

                    if (Jump.IsReleased() && MotionY < -PlayerConfig.JumpSpeedMin) {
                        DebugJump($"Short jump: decelerating from {MotionY.ToString()} to {(-PlayerConfig.JumpSpeedMin).ToString()}");
                        Body.MotionY = -PlayerConfig.JumpSpeedMin;
                    }

                    Body.Flip(XInput);
                    Body.FallLateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
                        PlayerConfig.StopIfSpeedIsLessThan, 0);
                })
                .Condition(() => Float.IsPressed(), context => context.Set(PlayerState.Float))
                .Condition(() => Body.IsOnFloor(), context => context.Set(PlayerState.Landing))
                .Condition(() => MotionY >= 0, context => context.Set(PlayerState.FallShort))
                .Build();
                

            State(PlayerState.FallShort)
                .Execute(() => {
                    CheckAirAttack();

                    Body.Flip(XInput);
                    Body.FallLateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
                        PlayerConfig.StopIfSpeedIsLessThan, 0);
                })
                .Condition(() => Float.IsPressed(), context => context.Set(PlayerState.Float))
                .Condition(CheckCoyoteJump, context => context.Set(PlayerState.Jump))
                .Condition(() => Body.IsOnFloor(), context => context.Set(PlayerState.Landing))
                .Condition(() => MotionY > PlayerConfig.StartFallingSpeed, context => context.Set(PlayerState.FallLong))
                .Build();
                
            State(PlayerState.FallLong)
                .Enter(() => {
                    _player.AnimationFall.PlayLoop();
                })
                .Execute(() => {
                    CheckAirAttack();
                    Body.Flip(XInput);
                    Body.FallLateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed,
                        PlayerConfig.AirResistance, PlayerConfig.StopIfSpeedIsLessThan, 0);
                })
                .Condition(() => Float.IsPressed(), context => context.Set(PlayerState.Float))
                .Condition(CheckCoyoteJump, context => context.Set(PlayerState.Jump))
                .Condition(() => Body.IsOnFloor(), context => context.Set(PlayerState.Landing))
                .Build();

            State(PlayerState.Float)
                .Execute(() => {
                    Body.AddSpeed(XInput, YInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed,
                        PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan, 0);
                    Body.Slide();
                })
                .Condition(() => Float.IsPressed(), context => context.Set(PlayerState.FallShort))
                .Build();

            AddOnTransition(args => Console.WriteLine(args.To));

        }
    }
}