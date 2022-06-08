using Betauer;
using Betauer.DI;
using Betauer.Input;

using Betauer.StateMachine;
using Godot;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers;
using Timer = Betauer.Timer;

namespace Veronenger.Game.Character.Player {
    [Transient]
    public class PlayerStateMachine {
        private readonly Logger _loggerJumpHelper = LoggerFactory.GetLogger("Player", "JumpHelper");
        private readonly Logger _loggerCoyoteJump = LoggerFactory.GetLogger("Player", "CoyoteJump");
        private readonly Logger _loggerJumpVelocity = LoggerFactory.GetLogger("Player", "JumpVelocity");
        private void DebugJumpHelper(string message) => _loggerJumpHelper.Debug(message);
        private void DebugCoyoteJump(string message) => _loggerCoyoteJump.Debug(message);
        private void DebugJump(string message) => _loggerJumpVelocity.Debug(message);

        public enum Transition {
        }

        public enum State {
            StateIdle,
            StateRun,
            StateFallShort,
            StateFallLong,
            StateJump,
        }

        [Inject] private PlatformManager _platformManager;
        [Inject] private PlayerConfig _playerConfig;
        [Inject] private LateralAction LateralMotion;
        [Inject] private VerticalAction VerticalMotion;
        [Inject] private ActionState Jump;
        [Inject] private ActionState Attack;

        private PlayerController _player;
        private StateMachineNode<State, Transition> _stateMachineNode;

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
        private Timer _fallingJumpTimer;
        private Timer _fallingTimer;


        public void Configure(PlayerController playerController) {
            _player = playerController;
            _fallingJumpTimer = new AutoTimer(playerController).Stop();
            _fallingTimer = new AutoTimer(playerController).Stop();

            _stateMachineNode = new StateMachineNode<State, Transition>(State.StateIdle, "Player", ProcessMode.Physics);
            playerController.AddChild(_stateMachineNode);

            _stateMachineNode.BeforeExecute((delta) => { Body.StartFrame(delta); });

            var builder = _stateMachineNode.CreateBuilder();
            GroundStates(builder);
            AirStates(builder);
            builder.Build();

            _stateMachineNode.AfterExecute((delta) => {
                Body.EndFrame();
            });
        }
        
        public void GroundStates(StateMachineBuilder<StateMachineNode<State, Transition>, State, Transition> builder) {
            bool CheckGroundAttack() {
                if (!Attack.JustPressed) return false;
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

            builder.State(State.StateIdle)
                .Enter(() => { _player.AnimationIdle.PlayLoop(); })
                .Execute(context => {
                    CheckGroundAttack();

                    if (!_player.IsOnFloor()) {
                        return context.Set(State.StateFallShort);
                    }

                    if (XInput != 0) {
                        return context.Set(State.StateRun);
                    }

                    if (Jump.JustPressed) {
                        if (IsDown && Body.IsOnFallingPlatform()) {
                            _platformManager.BodyFallFromPlatform(_player);
                        } else {
                            return context.Set(State.StateJump);
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
                });

            builder.State(State.StateRun)
                .Enter(() => { _player.AnimationRun.PlayLoop(); })
                .Execute(context => {
                    CheckGroundAttack();

                    if (!_player.IsOnFloor()) {
                        _coyoteJumpEnabled = true;
                        return context.Set(State.StateFallShort);
                    }

                    if (XInput == 0 && Motion.x == 0) {
                        return context.Set(State.StateIdle);
                    }

                    if (Jump.JustPressed) {
                        if (IsDown && Body.IsOnFallingPlatform()) {
                            _platformManager.BodyFallFromPlatform(_player);
                        } else {
                            return context.Set(State.StateJump);
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
                });
        }

        public void AirStates(StateMachineBuilder<StateMachineNode<State, Transition>, State, Transition> builder) {
            ExecuteTransition<State, Transition> CheckLanding(ExecuteContext<State, Transition> context) {
                if (!_player.IsOnFloor()) return context.None(); // Still in the air! :)

                _platformManager.BodyStopFallFromPlatform(_player);

                // Check helper jump
                if (!_fallingJumpTimer.Stopped) {
                    _fallingJumpTimer.Stop();
                    if (_fallingJumpTimer.Elapsed <= PlayerConfig.JumpHelperTime) {
                        DebugJumpHelper($"{_fallingJumpTimer.Elapsed} <= {PlayerConfig.JumpHelperTime} Done!");
                        return context.Set(State.StateJump);
                    }
                    DebugJumpHelper(
                        $"{_fallingJumpTimer.Elapsed} <= {PlayerConfig.JumpHelperTime} TOO MUCH TIME");
                }

                // Debug("Just grounded!");
                if (XInput == 0) {
                    if (Body.IsOnSlope()) {
                        // Evita resbalarse hacia abajo al caer sobre un slope
                        Body.SetMotionX(0);
                    }
                    return context.Set(State.StateIdle);
                }
                return context.Set(State.StateRun);
            }

            bool CheckAirAttack() {
                if (!Attack.JustPressed) return false;
                // Attack was pressed
                _player.AnimationJumpAttack.PlayOnce();
                return true;
            }

            bool CheckCoyoteJump() {
                if (!Jump.JustPressed) return false;
                // Jump was pressed
                _fallingJumpTimer.Reset().Start();
                if (_fallingTimer.Elapsed <= PlayerConfig.CoyoteJumpTime) {
                    DebugCoyoteJump($"{_fallingTimer.Elapsed} <= {PlayerConfig.CoyoteJumpTime} Done!");
                    return true;
                }
                DebugCoyoteJump($"{_fallingTimer.Elapsed} > {PlayerConfig.CoyoteJumpTime} TOO LATE");
                return false;
            }

            builder.State(State.StateJump)
                .Enter(() => {
                    Body.SetMotionY(-MotionConfig.JumpForce);
                    DebugJump("Jump start: decelerating to " + -MotionConfig.JumpForce);
                    _player.AnimationJump.PlayLoop();
                })
                .Execute(context => {
                    CheckAirAttack();

                    if (Jump.Released && Motion.y < -MotionConfig.JumpForceMin) {
                        DebugJump("Short jump: decelerating from " + Motion.y + " to " +
                                  -MotionConfig.JumpForceMin);
                        Body.SetMotionY(-MotionConfig.JumpForceMin);
                    }

                    Body.AddLateralMotion(XInput, MotionConfig.Acceleration, MotionConfig.AirResistance,
                        MotionConfig.StopIfSpeedIsLessThan, 0);
                    Body.Flip(XInput);
                    Body.Fall();

                    if (Motion.y >= 0) {
                        return context.Set(State.StateFallShort);
                    }

                    return CheckLanding(context);
                });

            builder.State(State.StateFallShort)
                .Enter(() => {
                    // Only if the state comes from running -> fall, the Coyote jump is enabled
                    // Other cases (State.State comes from idle or jump), the coyote is not enabled
                    _fallingTimer.Reset().Start();
                })
                .Execute(context => {
                    CheckAirAttack();

                    if (_coyoteJumpEnabled && CheckCoyoteJump()) {
                        _coyoteJumpEnabled = false;
                        return context.Set(State.StateJump);
                    }
                    if (Motion.y > MotionConfig.StartFallingSpeed) {
                        return context.Set(State.StateFallLong);
                    }

                    Body.AddLateralMotion(XInput, MotionConfig.Acceleration, MotionConfig.AirResistance,
                        MotionConfig.StopIfSpeedIsLessThan, 0);
                    Body.Flip(XInput);

                    Body.Fall();

                    return CheckLanding(context);
                });

            builder.State(State.StateFallLong)
                .Enter(() => {
                    if (_fallingTimer.Elapsed > PlayerConfig.CoyoteJumpTime) {
                        DebugCoyoteJump(
                            $"Coyote jump will never happen in FallLong state: {_fallingTimer.Elapsed} > {PlayerConfig.CoyoteJumpTime}");
                    }
                    _player.AnimationFall.PlayLoop();
                })
                .Execute(context => {
                        CheckAirAttack();

                        if (CheckCoyoteJump()) {
                            return context.Set(State.StateJump);
                        }

                        Body.AddLateralMotion(XInput, MotionConfig.Acceleration, MotionConfig.AirResistance,
                            MotionConfig.StopIfSpeedIsLessThan, 0);
                        Body.Flip(XInput);

                        Body.Fall();

                        return CheckLanding(context);
                    }
                );
        }
    }
}