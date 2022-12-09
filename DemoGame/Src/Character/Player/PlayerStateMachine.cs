using System;
using Betauer;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Input;

using Betauer.StateMachine;
using Betauer.StateMachine.Sync;
using Betauer.Core.Time;
using Godot;
using Veronenger.Controller.Character;
using Veronenger.Managers;

namespace Veronenger.Character.Player {
    
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

    public enum PlayerEvent {
        Death
    }

    [Service(Lifetime.Transient)]
    public partial class PlayerStateMachine : StateMachineNodeSync<PlayerState, PlayerEvent> {
        public PlayerStateMachine() : base(PlayerState.Idle, "Player.StateMachine", true) {
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
        private bool IsPressingRight => XInput > 0;
        private bool IsPressingLeft => XInput < 0;
        private bool IsPressingUp => YInput < 0;
        private bool IsPressingDown => YInput > 0;
        private float MotionX => Body.MotionX;
        private float MotionY => Body.MotionY;

        [Inject] private GodotStopwatch LastJumpOnAirTimer { get; set; }
        [Inject] private GodotStopwatch CoyoteFallingTimer { get; set; }
        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
        [Inject] private Bus Bus { get; set; }

        // private bool IsOnPlatform() => PlatformManager.IsPlatform(Body.GetFloor());
        // private bool IsOnFallingPlatform() => PlatformManager.IsFallingPlatform(Body.GetFloor());
        // private bool IsMovingPlatform() => PlatformManager.IsMovingPlatform(Body.GetFloor());
        private MonitorText? _coyoteMonitor;
        private MonitorText? _jumpHelperMonitor;

        public void Start(string name, PlayerController playerController, IFlipper flippers) {
            _player = playerController;
            playerController.AddChild(this);

            Body.Configure(name, playerController, flippers, _player.Marker2D, MotionConfig.FloorUpDirection);
            playerController.FloorStopOnSlope = true;
            // playerController.FloorBlockOnWall = true;
            playerController.FloorConstantSpeed = true;
            playerController.FloorSnapLength = MotionConfig.SnapLength;
            
            AddOnExecuteStart((delta, _) => Body.SetDelta(delta));
            AddOnTransition(args => Console.WriteLine(args.To));
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
                .SetMaxSize(1000, 1000)
                .OpenBox()
                    .Vector("Motion", () => Body.Motion, PlayerConfig.MaxSpeed).SetChartWidth(100).EndMonitor()
                    .Graph("MotionX", () => Body.MotionX, -PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed).AddSeparator(0)
                        .AddSerie("MotionY").Load(() => Body.MotionY).EndSerie().EndMonitor()
                .CloseBox()
                .Graph("Floor", () => Body.IsOnFloor()).Keep(10).SetChartHeight(10)
                    .AddSerie("Slope").Load(() => Body.IsOnSlope()).EndSerie().EndMonitor()
                .GraphSpeed("Speed", _player , PlayerConfig.JumpSpeed*2, "000").EndMonitor()
                .Text("Floor", () => Body.GetFloorCollisionInfo()).EndMonitor()
                .Text("Ceiling", () => Body.GetCeilingCollisionInfo()).EndMonitor()
                .Text("Wall", () => Body.GetWallCollisionInfo()).EndMonitor()
                .Disable();
            
            
            // debugOverlay.AddChild(new Consol);
        }

        public void ApplyFloorGravity(float factor = 1.0F) {
            Body.ApplyGravity(PlayerConfig.FloorGravity * factor, PlayerConfig.MaxFallingSpeed);
        }

        public void ApplyAirGravity(float factor = 1.0F) {
            Body.ApplyGravity(PlayerConfig.AirGravity * factor, PlayerConfig.MaxFallingSpeed);
        }

        public void GroundStates() {
            bool CheckGroundAttack() {
                if (!Attack.IsJustPressed()) return false;
                // Attack was pressed
                _player.AnimationAttack.PlayOnce();
                return true;
            }

            On(PlayerEvent.Death).Then(ctx => ctx.Set(PlayerState.Death));

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
                .If(() => LastJumpOnAirTimer.IsRunning).Set(PlayerState.Jump)
                .If(() => XInput == 0).Set(PlayerState.Idle)
                .If(() => true).Set(PlayerState.Run)
                .Build();

            State(PlayerState.Idle)
                .Enter(() => {
                    _player.AnimationIdle.PlayLoop();
                })
                .Execute(() => {
                    CheckGroundAttack();
                    ApplyFloorGravity();
                    Body.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
                })
                .If(() => !Body.IsOnFloor()).Set(PlayerState.FallShort)
                // .If(() => Jump.IsJustPressed() && IsPressingDown && IsOnFallingPlatform()).Then(
                    // context => {
                        // PlatformManager.BodyFallFromPlatform(_player);
                        // return context.Set(PlayerState.FallShort);
                    // })
                .If(() => Jump.IsJustPressed()).Set(PlayerState.Jump)
                .If(() => XInput != 0).Set(PlayerState.Run)
                .Build();
                
            State(PlayerState.Run)
                .Enter(() => {
                    _player.AnimationRun.PlayLoop();
                })
                .Execute(() => {
                    CheckGroundAttack();

                    ApplyFloorGravity();
                    if (_player.IsAttacking) {
                        Body.Stop(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
                    } else {
                        Body.Flip(XInput);
                        Body.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.Friction, 
                            PlayerConfig.StopIfSpeedIsLessThan, 0);
                    }
                })
                .If(() => !Body.IsOnFloor()).Then( 
                    context => {
                        CoyoteFallingTimer.Restart();
                        return context.Set(PlayerState.FallShort);
                    })
                // .If(() => Jump.IsJustPressed() && IsPressingDown && IsOnFallingPlatform()).Then(
                    // context => {
                        // PlatformManager.BodyFallFromPlatform(_player);
                        // return context.Set(PlayerState.FallShort);
                    // })
                .If(() => Jump.IsJustPressed()).Set(PlayerState.Jump)
                .If(() => XInput == 0 && MotionX == 0).Set(PlayerState.Idle)
                .Build();

            State(PlayerState.Death)
                .Enter(() => {
                    Console.WriteLine("MUERTO");
                    Bus.Publish(MainEvent.EndGame);
                })
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
                    _player.AnimationJump.PlayLoop();
                })
                .Execute(() => {
                    CheckAirAttack();

                    if (Jump.IsReleased() && MotionY < -PlayerConfig.JumpSpeedMin) {
                        Body.MotionY = -PlayerConfig.JumpSpeedMin;
                    }

                    Body.Flip(XInput);
                    ApplyAirGravity();
                    Body.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
                        PlayerConfig.StopIfSpeedIsLessThan, 0);
                })
                .If(() => Float.IsPressed()).Set(PlayerState.Float)
                .If(() => Body.IsOnFloor()).Set(PlayerState.Landing)
                .If(() => MotionY >= 0).Set(PlayerState.FallShort)
                .Build();
                

            State(PlayerState.FallShort)
                .Execute(() => {
                    CheckAirAttack();

                    Body.Flip(XInput);
                    ApplyAirGravity();
                    Body.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.AirResistance,
                        PlayerConfig.StopIfSpeedIsLessThan, 0);
                })
                .If(() => Float.IsPressed()).Set(PlayerState.Float)
                .If(CheckCoyoteJump).Set(PlayerState.Jump)
                .If(() => Body.IsOnFloor()).Set(PlayerState.Landing)
                .If(() => MotionY > PlayerConfig.StartFallingSpeed).Set(PlayerState.FallLong)
                .Build();
                
            State(PlayerState.FallLong)
                .Enter(() => {
                    _player.AnimationFall.PlayLoop();
                })
                .Execute(() => {
                    CheckAirAttack();
                    Body.Flip(XInput);
                    ApplyAirGravity();
                    Body.Lateral(XInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed,
                        PlayerConfig.AirResistance, PlayerConfig.StopIfSpeedIsLessThan, 0);
                })
                .If(() => Float.IsPressed()).Set(PlayerState.Float)
                .If(CheckCoyoteJump).Set(PlayerState.Jump)
                .If(() => Body.IsOnFloor()).Set(PlayerState.Landing)
                .Build();

            State(PlayerState.Float)
                .Enter(() => {
                    Body.Body.MotionMode = CharacterBody2D.MotionModeEnum.Floating;
                })
                .Execute(() => {
                    Body.AddSpeed(XInput, YInput, PlayerConfig.Acceleration, PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed,
                        PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan, 0);
                    Body.Move();
                })
                .If(() => Float.IsPressed()).Set(PlayerState.FallShort)
                .Exit(() => {
                    Body.Body.MotionMode = CharacterBody2D.MotionModeEnum.Grounded;
                })
                .Build();

        }
    }
}