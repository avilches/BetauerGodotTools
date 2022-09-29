using Betauer;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Input;

using Betauer.StateMachine;
using Betauer.Time;
using Godot;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Character.Player {
    [Service(Lifetime.Transient)]
    public class PlayerStateMachineNode : StateMachineNode<PlayerStateMachineNode.State, PlayerStateMachineNode.Transition> {
        private static readonly Logger LoggerJumpVelocity = LoggerFactory.GetLogger("JumpVelocity");
        private void DebugJump(string message) => LoggerJumpVelocity.Debug(message);

        public enum Transition {
        }

        public enum State {
            Idle,
            Run,
            FallShort,
            FallLong,
            Jump,
        }
        
        public PlayerStateMachineNode() : base(State.Idle, "Player.StateMachine", ProcessMode.Physics) {
        }

        [Inject] private PlatformManager PlatformManager { get; set;}
        [Inject] private PlayerConfig PlayerConfig { get; set;}
        [Inject] private InputAction Left { get; set;}
        [Inject] private InputAction Up { get; set;}
        [Inject] private InputAction Jump { get; set;}
        [Inject] private InputAction Attack { get; set;}
        [Inject] private KinematicPlatformMotionBody Body { get; set; }

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
        private Vector2 Motion => Body.Motion;
        private MotionConfig MotionConfig => PlayerConfig.MotionConfig;

        // State sharad between states
        private bool _coyoteJumpEnabled = false;
        [Inject] private GodotStopwatch JumpHelperTimer { get; set; }
        [Inject] private GodotStopwatch FallingTimer { get; set; }
        [Inject] private DebugOverlay DebugOverlay { get; set; }

        private Monitor _coyoteJumpState;
        private Monitor _jumpHelperState;

        public void Start(string name, PlayerController playerController, IFlipper flippers, RayCast2D slopeDetector, Position2D position2D) {
            _player = playerController;

            Body.Configure(name, playerController, flippers, PlayerConfig.MotionConfig, slopeDetector, position2D);

            playerController.AddChild(this);

            var events = new StateMachineEvents<State>();
            events.ExecuteStart += (delta, state) => Body.StartFrame(delta);
            events.ExecuteEnd += (state) => Body.EndFrame();
            AddListener(events);
            GroundStates();
            AirStates();

            DebugOverlay.CreateMonitor().WithPrefix("JumpHelperTimer").Bind(this).Show(() => JumpHelperTimer.ToString());
            _jumpHelperState = DebugOverlay.CreateMonitor().WithPrefix("JumpHelperState").Bind(this);
            DebugOverlay.CreateMonitor().WithPrefix("FallingTimer").Bind(this).Show(() => FallingTimer.ToString());
            _coyoteJumpState = DebugOverlay.CreateMonitor().WithPrefix("CoyoteState").Bind(this);
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

            CreateState(State.Idle)
                .Enter(() => {
                    _player.AnimationIdle.PlayLoop();
                })
                .Execute(context => {
                    CheckGroundAttack();

                    if (!_player.IsOnFloor()) {
                        return context.Set(State.FallShort);
                    }

                    if (XInput != 0) {
                        return context.Set(State.Run);
                    }

                    if (Jump.IsJustPressed()) {
                        if (IsDown && Body.IsOnFallingPlatform()) {
                            PlatformManager.BodyFallFromPlatform(_player);
                        } else {
                            return context.Set(State.Jump);
                        }
                    }

                    // Suelo + no salto + sin movimiento

                    if (!Body.IsOnMovingPlatform()) {
                        // No gravity in moving platforms
                        // Gravity in slopes to avoid go down slowly
                        Body.ApplyGravity();
                    }
                    Body.MoveSnapping();

                    return context.None();
                })
                .Build();

            CreateState(State.Run)
                .Enter(() => { _player.AnimationRun.PlayLoop(); })
                .Execute(context => {
                    CheckGroundAttack();

                    if (!_player.IsOnFloor()) {
                        _coyoteJumpEnabled = true;
                        Body.SetMotionY(0f);
                        return context.Set(State.FallShort);
                    }

                    if (XInput == 0 && Motion.x == 0) {
                        return context.Set(State.Idle);
                    }

                    if (Jump.IsJustPressed()) {
                        if (IsDown && Body.IsOnFallingPlatform()) {
                            PlatformManager.BodyFallFromPlatform(_player);
                        } else {
                            return context.Set(State.Jump);
                        }
                    }

                    // Suelo + no salto + movimiento/inercia
                    EnableSlopeStairs();

                    if (_player.IsAttacking) {
                        Body.StopLateralMotionWithFriction(MotionConfig.Friction,
                            MotionConfig.StopIfSpeedIsLessThan);
                    } else {
                        Body.AddLateralMotion(XInput, MotionConfig.Acceleration, MotionConfig.Friction,
                            MotionConfig.StopIfSpeedIsLessThan, 0);
                        Body.LimitMotion();
                        Body.Flip(XInput);
                    }

                    Body.MoveSnapping();

                    return context.None();
                })
                .Build();
                
        }

        private ExecuteTransition<State, Transition> CheckLanding(ExecuteContext<State, Transition> context) {
            if (!_player.IsOnFloor()) return context.None(); // Still in the air! :)

            PlatformManager.BodyStopFallFromPlatform(_player);

            // Check helper jump
            if (JumpHelperTimer.IsRunning) {
                JumpHelperTimer.Stop();
                if (JumpHelperTimer.Elapsed <= PlayerConfig.JumpHelperTime) {
                    _jumpHelperState.SetText($"{JumpHelperTimer.Elapsed.ToString()} <= {PlayerConfig.JumpHelperTime.ToString()} Done!");
                    return context.Set(State.Jump);
                }
                _jumpHelperState.SetText($"{JumpHelperTimer.Elapsed.ToString()} <= {PlayerConfig.JumpHelperTime.ToString()} TOO MUCH TIME");
            }

            // Debug("Just grounded!");
            if (XInput == 0) {
                if (Body.IsOnSlope()) {
                    // Evita resbalarse hacia abajo al caer sobre un slope
                    Body.SetMotionX(0);
                }
                return context.Set(State.Idle);
            }
            return context.Set(State.Run);
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

            CreateState(State.Jump)
                .Enter(() => {
                    Body.SetMotionY(-MotionConfig.JumpForce);
                    DebugJump("Jump start: decelerating to " + -MotionConfig.JumpForce);
                    _player.AnimationJump.PlayLoop();
                })
                .Execute(context => {
                    CheckAirAttack();

                    if (Jump.IsReleased() && Motion.y < -MotionConfig.JumpForceMin) {
                        DebugJump("Short jump: decelerating from " + Motion.y + " to " +
                                  -MotionConfig.JumpForceMin);
                        Body.SetMotionY(-MotionConfig.JumpForceMin);
                    }

                    Body.AddLateralMotion(XInput, MotionConfig.Acceleration, MotionConfig.AirResistance,
                        MotionConfig.StopIfSpeedIsLessThan, 0);
                    Body.Flip(XInput);
                    Body.Fall();

                    if (Motion.y >= 0) {
                        return context.Set(State.FallShort);
                    }

                    return CheckLanding(context);
                })
                .Build();
                

            CreateState(State.FallShort)
                .Enter(() => {
                    FallingTimer.Restart();
                })
                .Execute(context => {
                    CheckAirAttack();

                    // The flag _coyoteJumpEnabled is only enabled from Running -> fallShort
                    // Other cases (from idle or jump), the coyote is not enabled 
                    if (_coyoteJumpEnabled && CheckCoyoteJump()) {
                        _coyoteJumpEnabled = false;
                        return context.Set(State.Jump);
                    }
                    if (Motion.y > MotionConfig.StartFallingSpeed) {
                        return context.Set(State.FallLong);
                    }

                    Body.AddLateralMotion(XInput, MotionConfig.Acceleration, MotionConfig.AirResistance,
                        MotionConfig.StopIfSpeedIsLessThan, 0);
                    Body.Flip(XInput);

                    Body.Fall();

                    return CheckLanding(context);
                })
                .Build();
                

            CreateState(State.FallLong)
                .Enter(() => {
                    FallingTimer.Stop();
                    // if (_fallingTimer.Elapsed > PlayerConfig.CoyoteJumpTime) {
                        // _coyoteJumpState = $"Coyote jump will never happen in FallLong state: {_fallingTimer.Elapsed} > {PlayerConfig.CoyoteJumpTime}";
                    // }
                    _player.AnimationFall.PlayLoop();
                })
                .Execute(context => {
                        CheckAirAttack();

                        if (CheckCoyoteJump()) {
                            return context.Set(State.Jump);
                        }

                        Body.AddLateralMotion(XInput, MotionConfig.Acceleration, MotionConfig.AirResistance,
                            MotionConfig.StopIfSpeedIsLessThan, 0);
                        Body.Flip(XInput);

                        Body.Fall();

                        return CheckLanding(context);
                    }
                )
                .Build();
                
        }
    }
}