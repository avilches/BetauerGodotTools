using Godot;
using Tools;
using Tools.Bus.Topics;
using Tools.Input;
using Tools.Statemachine;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Player;
using Veronenger.Game.Character.Player.States;
using Veronenger.Game.Managers;
using Timer = Tools.Timer;

namespace Veronenger.Game.Controller.Character {
    public sealed class PlayerController : DiKinematicBody2D {
        private readonly string _name;
        private readonly Logger _logger;
        private readonly Logger _loggerInput;
        private readonly StateMachine _stateMachine;
        [OnReady("Sprite")] private Sprite _mainSprite;
        [OnReady("AttackArea")] private Area2D _attackArea;
        [OnReady("DamageArea")] private Area2D _damageArea;
        [OnReady("../Label")] protected Label Label;
        [OnReady("Detector")] public Area2D _playerDetector;
        [OnReady("Sprite/AnimationPlayer")] private AnimationPlayer _animationPlayer;

        public readonly PlayerConfig PlayerConfig = new PlayerConfig();
        public readonly MyPlayerActions PlayerActions;
        public readonly Timer FallingJumpTimer;
        public readonly Timer FallingTimer;

        [Inject] public PlatformManager PlatformManager;
        [Inject] public CharacterManager CharacterManager;
        [Inject] public SlopeStairsManager SlopeStairsManager;

        public MotionBody MotionBody;
        public IFlipper _flippers;

        public PlayerController() {
            FallingJumpTimer = new AutoTimer(this).Stop();
            FallingTimer = new AutoTimer(this).Stop();
            _name = "Player:" + GetHashCode().ToString("x8");
            _logger = LoggerFactory.GetLogger(_name);
            _loggerInput = LoggerFactory.GetLogger("Player:" + GetHashCode().ToString("x8"), "Input");
            PlayerActions = new MyPlayerActions(-1); // TODO: deviceId -1... manage add/remove controllers
            PlayerActions.ConfigureMapping();
            _stateMachine = new StateMachine(this, _name)
                .AddState(new GroundStateIdle(PlayerState.StateIdle, this))
                .AddState(new GroundStateRun(PlayerState.StateRun, this))
                .AddState(new AirStateFallShort(PlayerState.StateFallShort, this))
                .AddState(new AirStateFallLong(PlayerState.StateFallLong, this))
                .AddState(new AirStateJump(PlayerState.StateJump, this))
                .SetNextState(PlayerState.StateIdle);
        }

        public LoopAnimation AnimationIdle { get; private set; }
        public LoopAnimation AnimationRun { get; private set; }
        public LoopAnimation AnimationJump { get; private set; }
        public LoopAnimation AnimationFall { get; private set; }
        public OnceAnimation AnimationAttack { get; private set; }
        public OnceAnimation AnimationJumpAttack { get; private set; }

        /**
         * The Player needs to know if its body is overlapping the StairsUp and StairsDown.
         */
        public bool IsOnSlopeStairsUp() => _slopeStairsUp.IsOverlapping;

        public bool IsOnSlopeStairsDown() => _slopeStairsDown.IsOverlapping;
        private BodyOnArea2DStatus _slopeStairsDown;
        private BodyOnArea2DStatus _slopeStairsUp;

        public override void Ready() {
            var animationStack = new AnimationStack(_name, _animationPlayer);
            AnimationIdle = animationStack.AddLoopAnimation("Idle");
            AnimationRun = animationStack.AddLoopAnimation("Run");
            AnimationJump = animationStack.AddLoopAnimation("Jump");
            AnimationFall = animationStack.AddLoopAnimation("Fall");
            AnimationAttack = animationStack.AddOnceAnimation("Attack");
            AnimationJumpAttack = animationStack.AddOnceAnimation("JumpAttack");

            _flippers = new FlipperList().AddSprite(_mainSprite).AddNode2D(_attackArea);
            MotionBody = new MotionBody(this, _flippers, _name, PlayerConfig.MotionConfig);
            CharacterManager.RegisterPlayerController(this);
            CharacterManager.ConfigurePlayerCollisions(this);
            CharacterManager.ConfigurePlayerAttackArea2D(_attackArea, _OnPlayerAttackedEnemy);
            // CharacterManager.ConfigurePlayerDamageArea2D(_damageArea);

            _slopeStairsUp = SlopeStairsManager.CreateSlopeStairsUpStatusListener(Name, this);
            _slopeStairsDown = SlopeStairsManager.CreateSlopeStairsDownStatusListener(Name, this);

            SlopeStairsManager.SubscribeSlopeStairsEnabler(
                new BodyOnArea2DListenerDelegate(Name, this, this, _OnSlopeStairsEnablerEnter));
            SlopeStairsManager.SubscribeSlopeStairsDisabler(
                new BodyOnArea2DListenerDelegate(Name, this, this, _OnSlopeStairsDisablerEnter));

            PlatformManager.SubscribeFallingPlatformOut(
                new BodyOnArea2DListenerDelegate(Name, this, this, _OnFallingPlatformExit));
        }

        private void _OnPlayerAttackedEnemy(Area2DOnArea2D @event) {
            // LoggerFactory.GetLogger(GetType()).RemoveDuplicates = false;
            // LoggerFactory.GetLogger(GetType()).Debug("Collision from Origin:"+originParent.Name+"."+originParent.Name+" / Detected:"+@event.Detected.GetParent().Name+"."+@event.Detected.Name);
            var originParent = @event.Origin.GetParent();
            if (originParent is EnemyZombieController zombieController) {
                zombieController.AttackedByPlayer(this);
            }
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


        public override void _PhysicsProcess(float Delta) {
            // Update();
            // Label.Text = Position.DistanceTo(GetLocalMousePosition())+" "+Position.AngleTo(GetLocalMousePosition());
            MotionBody.StartFrame(Delta);
            _stateMachine.Execute(Delta);
            PlayerActions.ClearJustStates();
            /*
                Label.Text = "Floor: " + IsOnFloor() + "\n" +
                              "Slope: " + IsOnSlope() + "\n" +
                              "Stair: " + IsOnSlopeStairs() + "\n" +
                              "Moving: " + IsOnMovingPlatform() + "\n" +
                              "Falling: " + IsOnFallingPlatform();
                */
            MotionBody.EndFrame();
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
            if (_loggerInput.IsEnabled(TraceLevel.Debug)) {
                if (w.IsMotion()) {
                    _loggerInput.Debug($"Axis {w.Device}[{w.Axis}]:{w.GetStrength()} ({w.AxisValue})");
                } else if (w.IsAnyButton()) {
                    _loggerInput.Debug($"Button {w.Device}[{w.Button}]:{w.Pressed} ({w.Pressure})");
                } else if (w.IsAnyKey()) {
                    _loggerInput.Debug($"Key \"{w.KeyString}\" #{w.Key} Pressed:{w.Pressed}/Echo:{w.Echo}");
                }
                /*
                 * Aqui se comprueba que el JustPressed, Pressed y JustReleased del SALTO SOLO de PlayerActions coinciden
                 * con las del singleton Input de Godot. Se genera un texto con los 3 resultados y si no coinciden se pinta
                 */
                /*
                 var mine = PlayerActions.Jump.JustPressed + " " + PlayerActions.Jump.JustReleased + " " +
                            PlayerActions.Jump.Pressed;
                 var godot = Input.IsActionJustPressed("ui_select") + " " + Input.IsActionJustReleased("ui_select") +
                             " " +
                             Input.IsActionPressed("ui_select");
                 if (!mine.Equals(godot)) {
                     _logger.Debug("INPUT MISMATCH: Mine : " + mine);
                     _logger.Debug("INPUT MISTMATCH Godot: " + godot);
                 }
                 */
            }
        }

        public override void _Draw() {
            // DrawLine(MotionBody.FloorDetector.Position, MotionBody.FloorDetector.Position + MotionBody.FloorDetector.CastTo, Colors.Red, 3F);
            // DrawLine(_playerDetector.Position, GetLocalMousePosition(), Colors.Blue, 3F);
        }
        public bool IsAttacking => AnimationJumpAttack.Playing || AnimationAttack.Playing;

        public void DeathZone(Area2D deathArea2D) {
            _logger.Debug("MUETO!!");
        }
    }
}