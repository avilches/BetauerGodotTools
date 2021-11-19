using Godot;
using Tools;
using Tools.Bus.Topics;
using Tools.Input;
using Tools.Statemachine;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Player;
using Veronenger.Game.Character.Player.States;
using Veronenger.Game.Managers.Autoload;
using Timer = Tools.Timer;

namespace Veronenger.Game.Controller.Character {
    public class PlayerController : CharacterController {
        public readonly PlayerConfig PlayerConfig = new PlayerConfig();
        public readonly MyPlayerActions PlayerActions;
        private AnimationStack _animationStack;
        private string _name;
        private Area2D _attack;
        private StateMachine _stateMachine;
        private Logger _logger = LoggerFactory.GetLogger("Player:");
        private Logger _loggerInput = LoggerFactory.GetLogger("Player:");

        public readonly Timer FallingJumpTimer = new Timer().Stop();
        public readonly Timer FallingTimer = new Timer().Stop();

        public PlayerController() {
            PlayerActions = new MyPlayerActions(-1); // TODO: deviceId -1... manage add/remove controllers
            PlayerActions.ConfigureMapping();
        }

        public LoopAnimationStatus AnimationIdle { get; private set; }
        public LoopAnimationStatus AnimationRun { get; private set; }
        public LoopAnimationStatus AnimationJump { get; private set; }
        public LoopAnimationStatus AnimationFall { get; private set; }
        public OnceAnimationStatus AnimationAttack { get; private set; }
        public OnceAnimationStatus AnimationJumpAttack { get; private set; }

        protected override CharacterConfig CreateCharacterConfig() {
            return PlayerConfig;
        }

        protected override Logger GetLogger() {
            return _logger;
        }

        public override void _EnterTree() {
            _name = "Player:" + GetHashCode().ToString("x8");
            _logger = LoggerFactory.GetLogger(_name);
            _loggerInput = LoggerFactory.GetLogger(_name, "Input");
            base._EnterTree();
            var animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
            var animationStack = new AnimationStack(_name, animationPlayer);
            AnimationIdle = animationStack.AddLoopAnimationAndGetStatus(new LoopAnimationIdle());
            AnimationRun = animationStack.AddLoopAnimationAndGetStatus(new LoopAnimationRun());
            AnimationJump = animationStack.AddLoopAnimationAndGetStatus(new LoopAnimationJump());
            AnimationFall = animationStack.AddLoopAnimationAndGetStatus(new LoopAnimationFall());
            AnimationAttack = animationStack.AddOnceAnimationAndGetStatus(new AnimationAttack());
            AnimationJumpAttack = animationStack.AddOnceAnimationAndGetStatus(new AnimationJumpAttack());
            _stateMachine = new StateMachine(_name)
                .AddState(new GroundStateIdle(this))
                .AddState(new GroundStateRun(this))
                .AddState(new AirStateFallShort(this))
                .AddState(new AirStateFallLong(this))
                .AddState(new AirStateJump(this))
                .SetNextState(typeof(GroundStateIdle));
            _attack = GetNode<Area2D>("AttackArea");
        }

        /**
         * The Player needs to know if its body is overlapping the StairsUp and StairsDown.
         */
        public bool IsOnSlopeStairsUp() => _slopeStairsUp.IsOverlapping;

        public bool IsOnSlopeStairsDown() => _slopeStairsDown.IsOverlapping;
        private BodyOnArea2DStatus _slopeStairsDown;
        private BodyOnArea2DStatus _slopeStairsUp;

        public override void _Ready() {
            base._Ready();
            GameManager.Instance.RegisterPlayerController(this);
            CharacterManager.ConfigurePlayerCollisions(this);
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
            SlopeStairsManager.DisableSlopeStairsCoverForBody(this);
            SlopeStairsManager.EnableSlopeStairsForBody(this);
        }

        public void DisableSlopeStairs() {
            SlopeStairsManager.EnableSlopeStairsCoverForBody(this);
            SlopeStairsManager.DisableSlopeStairsForBody(this);
        }


        public void _OnFallingPlatformExit(BodyOnArea2D evt) => PlatformManager.BodyStopFallFromPlatform(this);

        public void _OnSlopeStairsEnablerEnter(BodyOnArea2D evt) => EnableSlopeStairs();

        public void _OnSlopeStairsDisablerEnter(BodyOnArea2D evt) => DisableSlopeStairs();

        protected override void PhysicsProcess() {
            FallingJumpTimer.Update(Delta);
            FallingTimer.Update(Delta);

            _stateMachine.Execute(Delta);

            PlayerActions.ClearJustStates();
            /*
                Label.Text = "Floor: " + IsOnFloor() + "\n" +
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
                    _loggerInput.Debug($"Axis {w.Device}[{w.Axis}]:{w.GetStrength()} ({w.AxisValue})");
                } else if (w.IsAnyButton()) {
                    _loggerInput.Debug($"Button {w.Device}[{w.Button}]:{w.Pressed} ({w.Pressure})");
                } else if (w.IsAnyKey()) {
                    _loggerInput.Debug($"Key \"{w.KeyString}\" #{w.Key} Pressed:{w.Pressed}/Echo:{w.Echo}");
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
            // _logger.Debug("Mine :" + mine);
            // _logger.Debug("Godot:" + godot);
            // }
        }

        public bool IsAttacking => AnimationJumpAttack.Playing || AnimationAttack.Playing;

        public void DeathZone(Area2D deathArea2D) {
            _logger.Debug("MUETO!!");
        }
    }
}