using Betauer;
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
        private Logger _loggerJumpVelocity;
        private void DebugJump(string message) => _loggerJumpVelocity.Debug(message);

        public enum Transition {
        }

        public enum State {
            Idle,
            Run,
            FallShort,
            FallLong,
            Jump,
        }
        
        public PlayerStateMachineNode() : base(State.Idle, "Player", ProcessMode.Physics) {
        }

        [Inject] private GameManager _gameManager { get; set;}
        [Inject] private PlatformManager _platformManager { get; set;}
        [Inject] private PlayerConfig _playerConfig { get; set;}
        private AxisAction LateralMotion => Left.AxisAction;
        private AxisAction VerticalMotion => Up.AxisAction;
        [Inject] private InputAction Left { get; set;}
        [Inject] private InputAction Up { get; set;}
        [Inject] private InputAction Jump { get; set;}
        [Inject] private InputAction Attack { get; set;}

        private PlayerController _player;
        private KinematicPlatformMotionBody Body => _player.KinematicPlatformMotionBody;

        // Input from the player
        private float XInput => LateralMotion.Strength;
        private float YInput => VerticalMotion.Strength;
        private bool IsRight => XInput > 0;
        private bool IsLeft => XInput < 0;
        private bool IsUp => YInput < 0;
        private bool IsDown => YInput > 0;
        private Vector2 Motion => _player.KinematicPlatformMotionBody.Motion;
        private MotionConfig MotionConfig => _playerConfig.MotionConfig;

        // State sharad between states
        private bool _coyoteJumpEnabled = false;
        [Inject] private GodotStopwatch JumpHelperTimer { get; set; }
        [Inject] private GodotStopwatch FallingTimer { get; set; }

        private string _coyoteJumpState = "";
        private string _jumpHelperState = "";

        public void Configure(PlayerController playerController, string name) {
            _loggerJumpVelocity = LoggerFactory.GetLogger("JumpVelocity", name);
            _player = playerController;

            playerController.AddChild(this);

            var events = new StateMachineEvents<State>();
            events.ExecuteStart += (delta, state) => Body.StartFrame(delta);
            events.ExecuteEnd += (state) => Body.EndFrame();
            AddListener(events);
            GroundStates();
            AirStates();

            _gameManager.DebugOverlay.Add("JumpHelperTimer").Do(() => JumpHelperTimer.ToString()).Bind(this);
            _gameManager.DebugOverlay.Add("JumpHelperState").Do(() => _jumpHelperState).Bind(this);
            _gameManager.DebugOverlay.Add("FallingTimer").Do(() => FallingTimer.ToString()).Bind(this);
            _gameManager.DebugOverlay.Add("CoyoteState").Do(() => _coyoteJumpState).Bind(this);
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
                            _platformManager.BodyFallFromPlatform(_player);
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
                        return context.Set(State.FallShort);
                    }

                    if (XInput == 0 && Motion.x == 0) {
                        return context.Set(State.Idle);
                    }

                    if (Jump.IsJustPressed()) {
                        if (IsDown && Body.IsOnFallingPlatform()) {
                            _platformManager.BodyFallFromPlatform(_player);
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

            _platformManager.BodyStopFallFromPlatform(_player);

            // Check helper jump
            if (JumpHelperTimer.IsRunning) {
                JumpHelperTimer.Stop();
                if (JumpHelperTimer.Elapsed <= PlayerConfig.JumpHelperTime) {
                    _jumpHelperState = $"{JumpHelperTimer.Elapsed} <= {PlayerConfig.JumpHelperTime} Done!";
                    return context.Set(State.Jump);
                }
                _jumpHelperState = $"{JumpHelperTimer.Elapsed} <= {PlayerConfig.JumpHelperTime} TOO MUCH TIME";
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
                JumpHelperTimer.Restart(); // Alarm(PlayerConfig.JumpHelperTime);
                if (FallingTimer.IsRunning) {
                    if (FallingTimer.Elapsed <= PlayerConfig.CoyoteJumpTime) {
                        // if (!_fallingTimer.IsAlarm()) {
                        _coyoteJumpState = $"{FallingTimer.Elapsed} <= {PlayerConfig.CoyoteJumpTime} Done!";
                        return true;
                    }
                    _coyoteJumpState = $"{FallingTimer.Elapsed} > {PlayerConfig.CoyoteJumpTime} TOO LATE";
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
                    // Only if the state comes from running -> fall, the Coyote jump is enabled
                    // Other cases (State.State comes from idle or jump), the coyote is not enabled
                    FallingTimer.Restart(); // Alarm(PlayerConfig.CoyoteJumpTime);
                })
                .Execute(context => {
                    CheckAirAttack();

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