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
    public class PlayerController : CharacterController {
        public PlayerConfig PlayerConfig => (PlayerConfig)CharacterConfig;
        private readonly StateMachine _stateMachine;
        public readonly MyPlayerActions PlayerActions;
        private AnimationPlayer _animationPlayer;
        private Sprite _sprite;
        private Label _label;

        private Area2D _attack;


        public PlayerController() {
            CharacterConfig = new PlayerConfig();
            PlayerActions = new MyPlayerActions(-1); // TODO: deviceId -1... manage add/remove controllers

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
            _label = GetNode<Label>("Label");
            _attack = GetNode<Area2D>("Attack");
        }

        public override void _Ready() {
            GameManager.Instance.RegisterPlayerController(this);

            CharacterManager.RegisterPlayerWeapon(_attack);

            PlatformManager.SubscribeFallingPlatformOut(
                new BodyOnArea2DListenerDelegate(Name, this, this, _OnFallingPlatformExit));

            _slopeStairsUp = SlopeStairsManager.CreateSlopeStairsUpStatusListener(Name, this);
            _slopeStairsDown = SlopeStairsManager.CreateSlopeStairsDownStatusListener(Name, this);
            SlopeStairsManager.SubscribeSlopeStairsEnabler(
                new BodyOnArea2DListenerDelegate(Name, this, this, _OnSlopeStairsEnablerEnter));
            SlopeStairsManager.SubscribeSlopeStairsDisabler(
                new BodyOnArea2DListenerDelegate(Name, this, this, _OnSlopeStairsDisablerEnter));

            _animationPlayer.Connect(GodotConstants.GODOT_SIGNAL_animation_finished, this, nameof(OnAnimationFinished));
        }

        public void EnableSlopeStairs() {
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "Enabling slope stairs");
            SlopeStairsManager.DisableSlopeStairsCoverForBody(this);
            SlopeStairsManager.EnableSlopeStairsForBody(this);
        }

        public void DisableSlopeStairs() {
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "Disabling slope stairs");
            SlopeStairsManager.EnableSlopeStairsCoverForBody(this);
            SlopeStairsManager.DisableSlopeStairsForBody(this);
        }

        public bool IsOnSlopeStairsUp() => _slopeStairsUp.IsOverlapping;
        public bool IsOnSlopeStairsDown() => _slopeStairsDown.IsOverlapping;
        private BodyOnArea2DStatus _slopeStairsDown;
        private BodyOnArea2DStatus _slopeStairsUp;

        public void _OnFallingPlatformExit(BodyOnArea2D evt) => PlatformManager.BodyStopFallFromPlatform(this);

        public void _OnSlopeStairsEnablerEnter(BodyOnArea2D evt) => EnableSlopeStairs();

        public void _OnSlopeStairsDisablerEnter(BodyOnArea2D evt) => DisableSlopeStairs();

        protected override void PhysicsProcess() {
            _stateMachine.Execute();
            PlayerActions.ClearJustState();
            /*
                _label.Text = "Floor: " + IsOnFloor() + "\n" +
                              "Slope: " + IsOnSlope() + "\n" +
                              "Stair: " + IsOnSlopeStairs() + "\n" +
                              "Moving: " + IsOnMovingPlatform() + "\n" +
                              "Falling: " + IsOnFallingPlatform();
                */
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
        private const string JUMP_ANIMATION = "Jump";
        private const string IDLE_ANIMATION = "Idle";
        private const string RUN_ANIMATION = "Run";
        private const string FALL_ANIMATION = "Fall";
        private const string ATTACK_ANIMATION = "Attack";
        private const string JUMP_ATTACK_ANIMATION = "JumpAttack";

        public void AnimateJump() => AnimationPlay(JUMP_ANIMATION);
        public void AnimateIdle() => AnimationPlay(IDLE_ANIMATION);
        public void AnimateRun() => AnimationPlay(RUN_ANIMATION);
        public void AnimateFall() => AnimationPlay(FALL_ANIMATION);
        public void AnimateAttack() => AnimationPlay(ATTACK_ANIMATION);
        public void AnimateJumpAttack() => AnimationPlay(JUMP_ATTACK_ANIMATION);
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
            var attackingAnimation = animation == ATTACK_ANIMATION || animation == JUMP_ATTACK_ANIMATION;
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