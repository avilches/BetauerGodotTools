using System;
using Betauer.Characters.Player.States;
using Betauer.Tools.Character;
using Betauer.Tools.Input;
using Betauer.Tools.Platforms;
using Betauer.Tools.Statemachine;
using Godot;

namespace Betauer.Characters.Player {
    public class PlayerController : CharacterController {
        public PlayerConfig PlayerConfig => (PlayerConfig) CharacterConfig;
        private readonly StateMachine _stateMachine;
        public readonly MyPlayerActions PlayerActions;
        private AnimationPlayer _animationPlayer;
        private Sprite _sprite;


        public PlayerController() {
            CharacterConfig = new PlayerConfig();
            PlayerActions = new MyPlayerActions(-1);

            // State Machine
            _stateMachine = new StateMachine(PlayerConfig, this)
                .AddState(new GroundStateIdle(this))
                .AddState(new GroundStateRun(this))
                .AddState(new AirStateFall(this))
                .AddState(new AirStateJump(this));

            // Mapping
            PlayerActions.ConfigureMapping();
        }


        public override void _EnterTree() {
            _sprite = GetNode<Sprite>("Sprite");
            _animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
            _stateMachine.SetNextState(typeof(GroundStateIdle));

            PlatformManager.ConfigureBodyCollisions(this);
            PlatformManager.subscribe_platform_out(this, nameof(subscribe_platform_out));

            // PlatformManager.on_slope_stairs_down_flag(self, "is_on_slope_stairs_down")
            // PlatformManager.on_slope_stairs_up_flag(self, "is_on_slope_stairs_up")
            PlatformManager.subscribe_slope_stairs_up(this, nameof(_slope_stairs_up_in), nameof(_slope_stairs_up_out));
            PlatformManager.subscribe_slope_stairs_down(this, nameof(_slope_stairs_down_in), nameof(_slope_stairs_down_out));
            PlatformManager.subscribe_slope_stairs_enabler(this,
                nameof(_slope_stairs_enabler_in)); // _slope_stairs_enabler_out
            PlatformManager.subscribe_slope_stairs_disabler(this,
                nameof(_slope_stairs_disabler_in)); // _slope_stairs_disabler_out
        }

        public void enable_slope_stairs() {
            // # permite subir una escalera
            // if C.DEBUG_SLOPE_STAIRS: print("stairs_enabler_in ENABLING")
            PlatformManager.body_disable_slope_stairs_cover(this);
            PlatformManager.body_enable_slope_stairs(this);
        }

        public void disable_slope_stairs() {
            // # deja de subir la escalera
            // if C.DEBUG_SLOPE_STAIRS: print("stairs_disabler_in DISABLING")
            PlatformManager.body_enable_slope_stairs_cover(this);
            PlatformManager.body_disable_slope_stairs(this);
        }

        public bool IsOnSlopeStairsUp() => _slope_stairs_up;
        public bool IsOnSlopeStairsDown() => _slope_stairs_down;
        private bool _slope_stairs_down;
        private bool _slope_stairs_up;

        public void _slope_stairs_up_in(Node body, Area2D area2D) {
            if (body == this) {
                _slope_stairs_up = true;
                GD.Print("_slope_stairs_up ", _slope_stairs_up);
            }
        }

        public void _slope_stairs_up_out(Node body, Area2D area2D) {
            if (body == this) {
                _slope_stairs_up = false;
                GD.Print("_slope_stairs_up ", _slope_stairs_up);
            }
        }

        public void _slope_stairs_down_in(Node body, Area2D area2D) {
            if (body == this) {
                _slope_stairs_down = true;
                GD.Print("_slope_stairs_down ", _slope_stairs_down);
            }
        }

        public void _slope_stairs_down_out(Node body, Area2D area2D) {
            if (body == this) {
                _slope_stairs_down = false;
                GD.Print("_slope_stairs_down ", _slope_stairs_down);
            }
        }

        public void subscribe_platform_out() {
            PlatformManager.body_stop_falling_from_platform(this);
        }

        public void _slope_stairs_enabler_in(Node body, Area2D area2D) {
            if (body == this) {
                enable_slope_stairs();
                GD.Print("Enabling slope");
            }
        }

        public void _slope_stairs_disabler_in(Node body, Area2D area2D) {
            if (body == this) {
                disable_slope_stairs();
                GD.Print("Disabling slope");
            }
        }

        protected override void PhysicsProcess() {
            _stateMachine.Execute();
            PlayerActions.ClearJustState();
        }

        private EventWrapper w = new EventWrapper(null);

        public override void _UnhandledInput(InputEvent @event) {
            w.@event = @event;
            if (!PlayerActions.Update(w)) {
                _stateMachine._UnhandledInput(@event);
            }

            TestJumpActions();
        }

        private void TestJumpActions() {
            if (PlayerConfig.DEBUG_INPUT_EVENTS) {
                if (w.IsMotion()) {
                    GD.Print("Axis " + w.Device + "[" + w.Axis + "]:" + w.GetStrength() + " (" + w.AxisValue + ")");
                } else if (w.IsAnyButton()) {
                    GD.Print("Button " + w.Device + "[" + w.Button + "]:" + w.Pressed + " (" + w.Pressure + ")");
                } else if (w.IsAnyKey()) {
                    GD.Print("Key " + w.KeyString + " [" + w.Key + "] " + w.Pressed + "/" + w.Echo);
                } else {
                }
            }

            /**
            * Aqui se comprueba que el JustPressed, Pressed y JustReleased de las acciones del PlayerActions coinciden
            * con las del singleton Input de Godot. Se genera un texto con los 3 resultados y si no coinciden se pinta
            */
            // var mine = PlayerActions.Jump.JustPressed + " " + PlayerActions.Jump.JustReleased + " " +
            // PlayerActions.Jump.Pressed;
            // var godot = Input.IsActionJustPressed("ui_select") + " " + Input.IsActionJustReleased("ui_select") + " " +
            // Input.IsActionPressed("ui_select");
            // if (!mine.Equals(godot)) {
            // GD.Print("Mine:" + mine);
            // GD.Print("Godo:" + godot);
            // }
        }

        public void SetNextState(Type nextStateType, bool immediate = false) {
            _stateMachine.SetNextState(nextStateType, immediate);
        }

        public void Flip(float XInput) {
            if (XInput == 0) return;
            Flip(XInput < 0);
        }

        public void Flip(bool left) {
            _sprite.FlipH = left;
        }

        private string _currentAnimation = null;
        private const string JUMP_ANIMATION = "Jump";
        private const string IDLE_ANIMATION = "Idle";
        private const string RUN_ANIMATION = "Run";
        private const string FALL_ANIMATION = "Fall";

        public void AnimateJump() {
            if (_currentAnimation == JUMP_ANIMATION) return;
            _animationPlayer.Play(JUMP_ANIMATION);
        }

        public void AnimateIdle() {
            if (_currentAnimation == IDLE_ANIMATION) return;
            _animationPlayer.Play(IDLE_ANIMATION);
        }

        public void AnimateRun() {
            if (_currentAnimation == RUN_ANIMATION) return;
            _animationPlayer.Play(RUN_ANIMATION);
        }

        public void AnimateFall() {
            if (_currentAnimation == FALL_ANIMATION) return;
            _animationPlayer.Play(FALL_ANIMATION);
        }
    }
}