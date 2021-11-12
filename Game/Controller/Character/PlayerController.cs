using Godot;
using Tools;
using Tools.Bus.Topics;
using Tools.Input;
using Tools.Statemachine;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Player;
using Veronenger.Game.Character.Player.Animations;
using Veronenger.Game.Character.Player.States;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Controller.Character {
    public class PlayerController : CharacterController {
        public PlayerConfig PlayerConfig => (PlayerConfig)CharacterConfig;
        public readonly MyPlayerActions PlayerActions;
        private AnimationPlayer _animationPlayer;
        private Area2D _attack;

        public readonly Clock FallingJumpClock = new Clock().Disable();
        public readonly Clock FallingClock = new Clock().Disable();



        public PlayerController() {
            CharacterConfig = new PlayerConfig();
            PlayerActions = new MyPlayerActions(-1); // TODO: deviceId -1... manage add/remove controllers

            // State Machines

            _stateMachine = new StateMachine(PlayerConfig, this)
                .AddState(new GroundStateIdle(this))
                .AddState(new GroundStateRun(this))
                .AddState(new AirStateFallShort(this))
                .AddState(new AirStateFallLong(this))
                .AddState(new AirStateJump(this));

            // Mapping
            PlayerActions.ConfigureMapping();
        }

        public override void _EnterTree() {
            _sprite = GetNode<Sprite>("Sprite");
            _animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
            _stateMachine.SetNextState(typeof(GroundStateIdle));
            _label = GetNode<Label>("Label");
            _attack = GetNode<Area2D>("AttackArea");

            _animationStack = new AnimationStack(_animationPlayer)
                .AddLoopAnimation(new AnimationIdle("Idle"))
                .AddLoopAnimation(new AnimationRun("Run"))
                .AddLoopAnimation(new AnimationJump("Jump"))
                .AddLoopAnimation(new AnimationFall("Fall"))
                .AddOnceAnimation(new AnimationAttack("Attack", this))
                .AddOnceAnimation(new AnimationJumpAttack("JumpAttack", this));

        }

        /**
         * The Player needs to know if its body is overlapping the StairsUp and StairsDown.
         */
        public bool IsOnSlopeStairsUp() => _slopeStairsUp.IsOverlapping;
        public bool IsOnSlopeStairsDown() => _slopeStairsDown.IsOverlapping;
        private BodyOnArea2DStatus _slopeStairsDown;
        private BodyOnArea2DStatus _slopeStairsUp;

        public override void _Ready() {
            GameManager.Instance.RegisterPlayerController(this);

            CharacterManager.ConfigurePlayerAreaAttack(_attack);

            _slopeStairsUp = SlopeStairsManager.CreateSlopeStairsUpStatusListener(Name, this);
            _slopeStairsDown = SlopeStairsManager.CreateSlopeStairsDownStatusListener(Name, this);

            SlopeStairsManager.SubscribeSlopeStairsEnabler(
                new BodyOnArea2DListenerDelegate(Name, this, this, _OnSlopeStairsEnablerEnter));
            SlopeStairsManager.SubscribeSlopeStairsDisabler(
                new BodyOnArea2DListenerDelegate(Name, this, this, _OnSlopeStairsDisablerEnter));

            PlatformManager.SubscribeFallingPlatformOut(
                new BodyOnArea2DListenerDelegate(Name, this, this, _OnFallingPlatformExit));
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


        public void _OnFallingPlatformExit(BodyOnArea2D evt) => PlatformManager.BodyStopFallFromPlatform(this);

        public void _OnSlopeStairsEnablerEnter(BodyOnArea2D evt) => EnableSlopeStairs();

        public void _OnSlopeStairsDisablerEnter(BodyOnArea2D evt) => DisableSlopeStairs();

        protected override void PhysicsProcess() {
            FallingJumpClock.Add(Delta);
            FallingClock.Add(Delta);
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

        public void AnimateJump() => _animationStack.PlayLoop(typeof(AnimationJump));
        public void AnimateIdle() => _animationStack.PlayLoop(typeof(AnimationIdle));
        public void AnimateRun() => _animationStack.PlayLoop(typeof(AnimationRun));
        public void AnimateFall() => _animationStack.PlayLoop(typeof(AnimationFall));
        public void AnimateAttack() => _animationStack.PlayOnce(typeof(AnimationAttack));
        public void AnimateJumpAttack() => _animationStack.PlayOnce(typeof(AnimationJumpAttack));

        public bool IsAttacking = false;

        public void DeathZone(Area2D deathArea2D) {
            GD.Print("MUETO!!");
        }
    }
}