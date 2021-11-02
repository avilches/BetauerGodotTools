using System;
using Game.Characters.Player.States;
using Game.Tools;
using Game.Tools.Character;
using Game.Tools.Events;
using Game.Tools.Input;
using Game.Tools.Statemachine;
using Godot;

namespace Game.Characters.Player {
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
            PlayerActions = new MyPlayerActions(-1);  // TODO: deviceId -1... manage add/remove controllers

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

            PlatformManager.SubscribeFallingPlatformOut(new Area2DEnterListenerDelegate(this, _OnFallingPlatformOut));
            PlatformManager.SubscribeSlopeStairsUp(new Area2DEnterListenerDelegate(this, _OnSlopeStairsUpIn),
                new Area2DEnterListenerDelegate(this, _OnSlopeStairsUpOut));
            PlatformManager.SubscribeSlopeStairsDown(new Area2DEnterListenerDelegate(this, _OnSlopeStairsDownIn),
                new Area2DEnterListenerDelegate(this, _OnSlopeStairsDownOut));
            PlatformManager.SubscribeSlopeStairsEnabler(new Area2DEnterListenerDelegate(this, _OnSlopeStairsEnablerIn));
            PlatformManager.SubscribeSlopeStairsDisabler(
                new Area2DEnterListenerDelegate(this, _OnSlopeStairsDisablerIn));

            _animationPlayer.Connect(GodotConstants.GODOT_SIGNAL_animation_finished, this, nameof(OnAnimationFinished));
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

        public bool IsOnSlopeStairsUp() => _slopeStairsUp;
        public bool IsOnSlopeStairsDown() => _slopeStairsDown;
        private bool _slopeStairsDown;
        private bool _slopeStairsUp;

        public void _OnSlopeStairsUpIn(BodyOnArea2D evt) {
            _slopeStairsUp = true;
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "_slope_stairs_up " + _slopeStairsUp);
        }

        public void _OnSlopeStairsUpOut(BodyOnArea2D evt) {
            _slopeStairsUp = false;
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "_slope_stairs_up " + _slopeStairsUp);
        }

        public void _OnSlopeStairsDownIn(BodyOnArea2D evt) {
            _slopeStairsDown = true;
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "_slope_stairs_down " + _slopeStairsDown);
        }

        public void _OnSlopeStairsDownOut(BodyOnArea2D evt) {
            _slopeStairsDown = false;
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "_slope_stairs_down " + _slopeStairsDown);
        }

        public void _OnFallingPlatformOut(BodyOnArea2D evt) => PlatformManager.BodyStopFallFromPlatform(this);

        public void _OnSlopeStairsEnablerIn(BodyOnArea2D evt) => EnableSlopeStairs();

        public void _OnSlopeStairsDisablerIn(BodyOnArea2D evt) => DisableSlopeStairs();

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

            GD.Print("IsAttacking " + IsAttacking + " (finished " + animation + " is attacking " + attackingAnimation +
                     ")");
            if (_previousAnimation != null) {
                AnimationPlay(_previousAnimation);
            }
        }

        public void DeathZone(Area2D deathArea2D) {
            GD.Print("MUETO!!");
        }
    }
}