using Godot;
using Tools;
using Tools.Animation;
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
        [OnReady("RichTextLabel")] protected RichTextLabel Label;
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

        public ILoopStatus AnimationIdle { get; private set; }
        public ILoopStatus AnimationRun { get; private set; }
        public ILoopStatus AnimationJump { get; private set; }
        public ILoopStatus AnimationFall { get; private set; }
        public IOnceStatus AnimationAttack { get; private set; }
        public IOnceStatus AnimationJumpAttack { get; private set; }

        public ILoopStatus PulsateTween;
        public ILoopStatus DangerTween;
        public ILoopStatus ResetTween;
        public IOnceStatus SqueezeTween;

        /**
         * The Player needs to know if its body is overlapping the StairsUp and StairsDown.
         */
        public bool IsOnSlopeStairsUp() => _slopeStairsUp.IsOverlapping;

        public bool IsOnSlopeStairsDown() => _slopeStairsDown.IsOverlapping;
        private BodyOnArea2DStatus _slopeStairsDown;
        private BodyOnArea2DStatus _slopeStairsUp;

        private AnimationStack _animationStack;
        private AnimationStack _tweenStack;

        public override void Ready() {
            _animationStack = new AnimationStack(_name, _animationPlayer);
            AnimationIdle = _animationStack.AddLoopAnimation("Idle");
            AnimationRun = _animationStack.AddLoopAnimation("Run");
            AnimationJump = _animationStack.AddLoopAnimation("Jump");
            AnimationFall = _animationStack.AddLoopAnimation("Fall");
            AnimationAttack = _animationStack.AddOnceAnimation("Attack");
            AnimationJumpAttack = _animationStack.AddOnceAnimation("JumpAttack");

            _tweenStack = new AnimationStack(_name, _animationPlayer, new TweenPlayer("Player").NewTween(this));
            PulsateTween = _tweenStack.AddLoopTween("Pulsate", CreatePulsate());
            DangerTween = _tweenStack.AddLoopTween("Danger", CreateDanger());
            ResetTween = _tweenStack.AddLoopTween("Reset", CreateReset());
            SqueezeTween = _tweenStack.AddOnceTween("Squeeze", CreateSqueeze());

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


        public void StopIdle() {
            DangerTween.Stop();
        }

        public void StartIdle() {
            DangerTween.PlayLoop();
        }

        public void StopModulate() {
            PulsateTween.Stop();
        }

        public void StartModulate() {
            PulsateTween.PlayLoop();
        }

        public void StopSqueeze() {
            SqueezeTween.Stop(true);
        }

        public void StartSqueeze() {
            SqueezeTween.PlayOnce(true);
        }


        private TweenSequence CreateReset() {
            var seq = new TweenSequenceBuilder()
                .AnimateColor(_mainSprite, "modulate").From(new Color(1, 1, 1, 0)).To(new Color(1, 1, 1, 1), 1).EndAnimate();
            // seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 1), 0.1f);
            // seq.Parallel().AddProperty(this, "scale", new Vector2(1f, 1f), 0.1f);
            return seq;
        }

        private TweenSequence CreatePulsate() {
            var seq = new TweenSequenceBuilder()
                .KeyframeColor(_mainSprite, "modulate")
                .Duration(0.5f)
                .KeyframeTo(0.25f, new Color(1, 1, 1, 0))
                .KeyframeTo(0.75f, new Color(1, 1, 1, 0.5f))
                .KeyframeTo(1f, new Color(1, 1, 1, 1))
                .EndAnimate()
                .Parallel()
                .AnimateVector2(this, "scale")
                .To(new Vector2(1.4f, 1f), 0.5f)
                .To(new Vector2(1f, 1f), 0.5f)
                .EndAnimate();
            // seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 0), 1f).SetTrans(Tween.TransitionType.Cubic);
            // seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 1), 1f).SetTrans(Tween.TransitionType.Cubic);
            return seq;
        }

        private TweenSequence CreateDanger() {
            var seq = new TweenSequenceBuilder()
                .AnimateColor(_mainSprite, "modulate", Easing.CubicInOut)
                .To(new Color(1, 0, 0, 1), 1)
                .To(new Color(1, 1, 1, 1), 1)
                .EndAnimate();
            return seq;
        }

        private TweenSequence CreateSqueeze() {
            var seq = new TweenSequenceBuilder()
                .AnimateVector2(this, "scale", Easing.SineInOut)
                .To(new Vector2(1.4f, 1f), 0.25f)
                .To(new Vector2(1f, 1f), 0.25f)
                .EndAnimate()
                .SetLoops(2);
            return seq;
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
            // Label.BbcodeText = "Idle:" + AnimationIdle.Playing + " Attack:" + AnimationAttack.Playing + "\n" +
            // _animationStack.GetPlayingLoop()?.Name + " " + _animationStack.GetPlayingOnce()?.Name;
            Label.BbcodeText = _animationStack.GetPlayingLoop()?.Name + " " + _animationStack.GetPlayingOnce()?.Name +
                               "\n" +
                               _tweenStack.GetPlayingLoop()?.Name + " " + _tweenStack.GetPlayingOnce()?.Name;
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