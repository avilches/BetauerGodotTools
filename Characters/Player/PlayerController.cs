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
            PlatformManager.SubscribeFallingPlatformOut(this, nameof(_OnFallingPlatformOut));
            PlatformManager.SubscribeSlopeStairsUp(this, nameof(_OnSlopeStairsUpIn), nameof(_OnSlopeStairsUpOut));
            PlatformManager.SubscribeSlopeStairsDown(this, nameof(_OnSlopeStairsDownIn), nameof(_OnSlopeStairsDownOut));
            PlatformManager.SubscribeSlopeStairsEnabler(this, nameof(_OnSlopeStairsEnablerIn));
            PlatformManager.SubscribeSlopeStairsDisabler(this, nameof(_OnSlopeStairsDisablerIn));
        }

        public void EnableSlopeStairs() {
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "Enabling slope stairs");
            PlatformManager.DisableSlopeStairsCoverForBody(this);
            PlatformManager.EnableSlopeStairsForBody(this);
        }

        public void DisableSlopeStairs() {
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "Disabling slope stairs");
            PlatformManager.EnableSlopeStairsCoverForBody(this);
            PlatformManager.DisableSlopeStairsForBody(this);
        }

        public bool IsOnSlopeStairsUp() => _slope_stairs_up;
        public bool IsOnSlopeStairsDown() => _slope_stairs_down;
        private bool _slope_stairs_down;
        private bool _slope_stairs_up;

        public void _OnSlopeStairsUpIn(Node body, Area2D area2D) {
            if (body != this) return;
            _slope_stairs_up = true;
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "_slope_stairs_up " + _slope_stairs_up);
        }

        public void _OnSlopeStairsUpOut(Node body, Area2D area2D) {
            if (body != this) return;
            _slope_stairs_up = false;
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "_slope_stairs_up " + _slope_stairs_up);
        }

        public void _OnSlopeStairsDownIn(Node body, Area2D area2D) {
            if (body != this) return;
            _slope_stairs_down = true;
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "_slope_stairs_down " + _slope_stairs_down);
        }

        public void _OnSlopeStairsDownOut(Node body, Area2D area2D) {
            if (body != this) return;
            _slope_stairs_down = false;
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "_slope_stairs_down " + _slope_stairs_down);
        }

        public void _OnFallingPlatformOut(Node body, Area2D area2D) {
            if (body == this) PlatformManager.BodyStopFallFromPlatform(this);
        }

        public void _OnSlopeStairsEnablerIn(Node body, Area2D area2D) {
            if (body == this) EnableSlopeStairs();
        }

        public void _OnSlopeStairsDisablerIn(Node body, Area2D area2D) {
            if (body == this) DisableSlopeStairs();
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

        public void Flip(float xInput) {
            if (xInput == 0) return;
            Flip(xInput < 0);
        }

        public void Flip(bool left) {
            _sprite.FlipH = left;
        }

        private string _currentAnimation = null;
        private const string _JUMP_ANIMATION = "Jump";
        private const string _IDLE_ANIMATION = "Idle";
        private const string _RUN_ANIMATION = "Run";
        private const string _FALL_ANIMATION = "Fall";

        public void AnimateJump() => AnimationPlay(_JUMP_ANIMATION);
        public void AnimateIdle() => AnimationPlay(_IDLE_ANIMATION);
        public void AnimateRun() => AnimationPlay(_RUN_ANIMATION);
        public void AnimateFall() => AnimationPlay(_FALL_ANIMATION);
        private void AnimationPlay(string newAnimation) {
            if (_currentAnimation == newAnimation) return;
            _animationPlayer.Play(newAnimation);
            _currentAnimation = newAnimation;
        }
    }
}