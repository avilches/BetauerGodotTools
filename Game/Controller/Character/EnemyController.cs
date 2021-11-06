using System;
using Godot;
using Tools;
using Tools.Bus.Topics;
using Tools.Input;
using Tools.Statemachine;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Player;
using Veronenger.Game.Character.Player.States;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Controller.Character {
    public class EnemyController : CharacterController {
        public PlayerConfig EnemyConfig => (PlayerConfig)CharacterConfig;
        private readonly StateMachine _stateMachine;
        private AnimationPlayer _animationPlayer;
        private Sprite _sprite;
        private Label _label;

        public EnemyController() {
            CharacterConfig = new PlayerConfig();

            // State Machine
            _stateMachine = new StateMachine(EnemyConfig, this);
            // .AddState(new GroundStateIdle(this))
        }

        public override void _EnterTree() {
            _sprite = GetNode<Sprite>("Sprite");
            _animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
            _stateMachine.SetNextState(typeof(GroundStateIdle));
        }

        public override void _Ready() {
            GameManager.Instance.CharacterManager.RegisterEnemy(this);

            PlatformManager.SubscribeFallingPlatformOut(
                new BodyOnArea2DEnterListenerDelegate("Enemy" + Name, this, _OnFallingPlatformOut));

            _animationPlayer.Connect(GodotConstants.GODOT_SIGNAL_animation_finished, this, nameof(OnAnimationFinished));
        }

        public void _OnFallingPlatformOut(BodyOnArea2D evt) => PlatformManager.BodyStopFallFromPlatform(this);


        protected override void PhysicsProcess() {
            _stateMachine.Execute();
            /*
            _label.Text = "Floor: " + IsOnFloor() + "\n" +
                          "Slope: " + IsOnSlope() + "\n" +
                          "Stair: " + IsOnSlopeStairs() + "\n" +
                          "Moving: " + IsOnMovingPlatform() + "\n" +
                          "Falling: " + IsOnFallingPlatform();
            */
        }

        private EventWrapper w = new EventWrapper(null);

        private void TestJumpActions() {
            if (EnemyConfig.DEBUG_INPUT_EVENTS) {
                if (w.IsMotion()) {
                    GD.Print($"Axis {w.Device}[{w.Axis}]:{w.GetStrength()} ({w.AxisValue})");
                } else if (w.IsAnyButton()) {
                    GD.Print($"Button {w.Device}[{w.Button}]:{w.Pressed} ({w.Pressure})");
                } else if (w.IsAnyKey()) {
                    GD.Print($"Key {w.KeyString} [{w.Key}] {w.Pressed}/{w.Echo}");
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

        public void SetNextConfig(System.Collections.Generic.Dictionary<string, object> config) {
            _stateMachine.SetNextConfig(config);
        }

        public void SetNextConfig(string key, object value) {
            _stateMachine.SetNextConfig(key, value);
        }

        public System.Collections.Generic.Dictionary<string, object> GetNextConfig() {
            return _stateMachine.GetNextConfig();
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
        private const string _ATTACK_ANIMATION = "Attack";
        private const string _JUMP_ATTACK_ANIMATION = "JumpAttack";

        public void AnimateJump() => AnimationPlay(_JUMP_ANIMATION);
        public void AnimateIdle() => AnimationPlay(_IDLE_ANIMATION);
        public void AnimateRun() => AnimationPlay(_RUN_ANIMATION);
        public void AnimateFall() => AnimationPlay(_FALL_ANIMATION);
        public void AnimateAttack() => AnimationPlay(_ATTACK_ANIMATION);
        public void AnimateJumpAttack() => AnimationPlay(_JUMP_ATTACK_ANIMATION);
        private string _previousAnimation = null;

        public bool IsAttacking = false;

        private void AnimationPlay(string newAnimation) {
            if (_currentAnimation == newAnimation) return;
            if (IsAttacking) {
                _previousAnimation = newAnimation;
            } else {
                _previousAnimation = _currentAnimation;
                _animationPlayer.Play(newAnimation);
                _currentAnimation = newAnimation;
            }
        }

        public void Attack(bool floor) {
            if (IsAttacking) return;
            if (floor) {
                AnimateAttack();
            } else {
                AnimateJumpAttack();
            }
            IsAttacking = true;
        }

        public void OnAnimationFinished(string animation) {
            var attackingAnimation = animation == _ATTACK_ANIMATION || animation == _JUMP_ATTACK_ANIMATION;
            if (attackingAnimation) {
                IsAttacking = false;
            }
            GD.Print($"IsAttacking {IsAttacking} (finished {animation} is attacking {attackingAnimation})");
            if (_previousAnimation != null) {
                AnimationPlay(_previousAnimation);
            }
        }

        public void DeathZone(Area2D deathArea2D) {
            GD.Print("MUETO!!");
        }
    }
}