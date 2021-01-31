using System;
using Betauer.Characters.Player.States;
using Betauer.Tools.Character;
using Betauer.Tools.Input;
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

            // TestJumpActions();
        }

        private void TestJumpActions() {

            if (w.IsMotion()) {
                GD.Print("Axis " + w.Device + "[" + w.Axis+ "]:" + w.GetStrength()+" ("+w.AxisValue+")");
            } else if (w.IsAnyButton()) {
                GD.Print("Button " + w.Device + "[" + w.Button + "]:" + w.Pressed +" ("+w.Pressure+")");
            } else if (w.IsAnyKey()) {
                GD.Print("Key "+w.KeyString + " [" + w.Key + "] " + w.Pressed + "/" + w.Echo);
            } else {

            }

            /**
         * Aqui se comprueba que el JustPressed, Pressed y JustReleased de las acciones del PlayerActions coinciden
         * con las del singleton Input de Godot. Se genera un texto con los 3 resultados y si no coinciden se pinta
         */
            var mine = PlayerActions.Jump.JustPressed + " " + PlayerActions.Jump.JustReleased + " " +
                       PlayerActions.Jump.Pressed;
            var godot = Input.IsActionJustPressed("ui_select") + " " + Input.IsActionJustReleased("ui_select") + " " +
                        Input.IsActionPressed("ui_select");
            if (!mine.Equals(godot)) {
                GD.Print("Mine:" + mine);
                GD.Print("Godo:" + godot);
            }
        }

        public void ChangeStateTo(Type newStateType) {
            _stateMachine.ChangeStateTo(newStateType);
        }

        public void SetNextState(Type nextStateType) {
            _stateMachine.SetNextState(nextStateType);
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