using System;
using System.Collections.Generic;
using System.Linq;
using Betauer;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Input;

using Betauer.StateMachine.Sync;
using Betauer.Core.Time;
using Godot;
using Veronenger.Character.Handler;
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

        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
        [Inject] private PlatformManager PlatformManager { get; set;}
        
        [Inject] public PlayerConfig PlayerConfig { get; set;}
        [Inject] public KinematicPlatformMotion Body { get; set; }
        [Inject] private GodotStopwatch CoyoteFallingTimer { get; set; }
        [Inject] private Bus Bus { get; set; }
        [Inject] private InputActionCharacterHandler Handler { get; set; }

        private PlayerController _player;

        private float XInput => Handler.XInput;
        private float YInput => Handler.YInput;
        private bool IsPressingRight => Handler.IsPressingRight;
        private bool IsPressingLeft => Handler.IsPressingLeft;
        private bool IsPressingUp => Handler.IsPressingUp;
        private bool IsPressingDown => Handler.IsPressingDown;
        private IActionHandler Jump => Handler.Jump;
        private IActionHandler Attack => Handler.Attack;
        private IActionHandler Float => Handler.Float;
        
        private float MotionX => Body.MotionX;
        private float MotionY => Body.MotionY;


        // private bool IsOnPlatform() => PlatformManager.IsPlatform(Body.GetFloor());
        private bool IsOnFallingPlatform() => Body.IsOnFloor() && PlatformManager.IsFallingPlatform(Body.GetFloorColliders<PhysicsBody2D>().FirstOrDefault());
        // private bool IsMovingPlatform() => PlatformManager.IsMovingPlatform(Body.GetFloor());
        private MonitorText? _coyoteMonitor;
        private MonitorText? _jumpHelperMonitor;
        
        public void Start(string name, PlayerController playerController, IFlipper flippers, List<RayCast2D> floorRaycasts) {
            _player = playerController;
            playerController.AddChild(this);

            Body.Configure(name, playerController, flippers, _player.Marker2D, MotionConfig.FloorUpDirection, floorRaycasts);
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

            PhysicsBody2D? fallingPlatform = null;
            void FallFromPlatform() {
                fallingPlatform = Body.GetFloorCollider<PhysicsBody2D>()!;
                PlatformManager.RemovePlatformCollision(fallingPlatform);
            }

            void FinishFallFromPlatform() {
                 if (fallingPlatform != null) PlatformManager.ConfigurePlatformCollision(fallingPlatform);
            }

            On(PlayerEvent.Death).Then(ctx => ctx.Set(PlayerState.Death));

            var delayedJump = ((InputAction)Jump).Delayed();
            var jumpJustInTime = false;
            State(PlayerState.Landing)
                .Enter(() => {
                    FinishFallFromPlatform();
                    CoyoteFallingTimer.Stop(); // not really needed, but less noise in the debug overlay
                    jumpJustInTime = delayedJump.WasPressed(PlayerConfig.JumpHelperTime);
                })
                .Execute(() => {
                    if (jumpJustInTime) {
                        _jumpHelperMonitor?.Show($"{delayedJump.LastPressed} <= {PlayerConfig.JumpHelperTime.ToString()} Done!");
                    } else {
                        _jumpHelperMonitor?.Show($"{delayedJump.LastPressed} > {PlayerConfig.JumpHelperTime.ToString()} TOO MUCH TIME");
                    }
                })
                .If(() => jumpJustInTime).Set(PlayerState.Jump)
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
                .If(() => Jump.IsJustPressed() && IsPressingDown && IsOnFallingPlatform()).Then(
                    context => {
                        FallFromPlatform();
                        return context.Set(PlayerState.FallShort);
                    })
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
                .If(() => Jump.IsJustPressed() && IsPressingDown && IsOnFallingPlatform()).Then(
                    context => {
                        FallFromPlatform();
                        return context.Set(PlayerState.FallShort);
                    })
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