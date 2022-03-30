using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Bus.Topics;
using Betauer.DI;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Player;
using Veronenger.Game.Managers;
using Timer = Betauer.Timer;

namespace Veronenger.Game.Controller.Character {
    public sealed class PlayerController : DiKinematicBody2D {
        private readonly string _name;
        private readonly Logger _logger;
        [OnReady("Sprite")] private Sprite _mainSprite;
        [OnReady("AttackArea")] private Area2D _attackArea;
        [OnReady("DamageArea")] private Area2D _damageArea;
        [OnReady("RichTextLabel")] private RichTextLabel Label;
        [OnReady("Detector")] public Area2D PlayerDetector;
        [OnReady("Sprite/AnimationPlayer")] private AnimationPlayer _animationPlayer;

        private SceneTree _sceneTree;
        private IFlipper _flippers;

        [Inject] private GameManager _gameManager;
        [Inject] private PlatformManager _platformManager;
        [Inject] private CharacterManager _characterManager;
        [Inject] private SlopeStairsManager _slopeStairsManager;
        [Inject] private InputManager _inputManager;
        [Inject] private ScreenManager _screenManager;
        [Inject] private PlayerStateMachine _stateMachine;
        [Inject] private PlayerConfig _playerConfig;
        [Inject] public KinematicPlatformMotionBody KinematicPlatformMotionBody;

        public PlayerController() {
            _name = "Player:" + GetHashCode().ToString("x8");
            _logger = LoggerFactory.GetLogger(_name);
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
            _sceneTree = GetTree();

            _animationStack = new AnimationStack(_name, _animationPlayer);
            AnimationIdle = _animationStack.AddLoopAnimation("Idle");
            AnimationRun = _animationStack.AddLoopAnimation("Run");
            AnimationJump = _animationStack.AddLoopAnimation("Jump");
            AnimationFall = _animationStack.AddLoopAnimation("Fall");
            AnimationAttack = _animationStack.AddOnceAnimation("Attack");
            AnimationJumpAttack = _animationStack.AddOnceAnimation("JumpAttack");

            _tweenStack = new AnimationStack(_name, _animationPlayer, new SingleSequencePlayer().WithParent(this));
            PulsateTween = _tweenStack.AddLoopTween("Pulsate", CreatePulsate());
            DangerTween = _tweenStack.AddLoopTween("Danger", CreateDanger());
            ResetTween = _tweenStack.AddLoopTween("Reset", CreateReset());
            SqueezeTween = _tweenStack.AddOnceTween("Squeeze", CreateSqueeze());

            _flippers = new FlipperList().AddSprite(_mainSprite).AddNode2D(_attackArea);
            KinematicPlatformMotionBody.Configure(this, _flippers, _name, _playerConfig.MotionConfig);

            _stateMachine.Configure(this);

            _characterManager.RegisterPlayerController(this);
            _characterManager.ConfigurePlayerCollisions(this);
            _characterManager.ConfigurePlayerAttackArea2D(_attackArea, _OnPlayerAttackedEnemy);
            // CharacterManager.ConfigurePlayerDamageArea2D(_damageArea);

            _slopeStairsUp = _slopeStairsManager.CreateSlopeStairsUpStatusListener(Name, this);
            _slopeStairsDown = _slopeStairsManager.CreateSlopeStairsDownStatusListener(Name, this);

            _slopeStairsManager.SubscribeSlopeStairsEnabler(
                new BodyOnArea2DListenerAction(Name, this, this, _OnSlopeStairsEnablerEnter));
            _slopeStairsManager.SubscribeSlopeStairsDisabler(
                new BodyOnArea2DListenerAction(Name, this, this, _OnSlopeStairsDisablerEnter));

            _platformManager.SubscribeFallingPlatformOut(
                new BodyOnArea2DListenerAction(Name, this, this, _OnFallingPlatformExit));
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


        private ISequence CreateReset() {
            var seq = SequenceBuilder.Create()
                .AnimateSteps<Color>(_mainSprite, Property.Modulate)
                .From(new Color(1, 1, 1, 0))
                .To(new Color(1, 1, 1, 1), 1)
                .EndAnimate();
            // seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 1), 0.1f);
            // seq.Parallel().AddProperty(this, "scale", new Vector2(1f, 1f), 0.1f);
            return seq;
        }

        private ISequence CreatePulsate() {
            var seq = SequenceBuilder.Create()
                .AnimateKeys<Color>(_mainSprite, Property.Modulate)
                .Duration(0.5f)
                .KeyframeTo(0.25f, new Color(1, 1, 1, 0))
                .KeyframeTo(0.75f, new Color(1, 1, 1, 0.5f))
                .KeyframeTo(1f, new Color(1, 1, 1, 1))
                .EndAnimate()
                .Parallel()
                .AnimateSteps<Vector2>(this, Property.Scale2D)
                .To(new Vector2(1.4f, 1f), 0.5f)
                .To(new Vector2(1f, 1f), 0.5f)
                .EndAnimate();
            // seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 0), 1f).SetTrans(Tween.TransitionType.Cubic);
            // seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 1), 1f).SetTrans(Tween.TransitionType.Cubic);
            return seq;
        }

        private ISequence CreateDanger() {
            var seq = SequenceBuilder.Create()
                .AnimateSteps<Color>(_mainSprite, Property.Modulate, Easing.CubicInOut)
                .To(new Color(1, 0, 0, 1), 1)
                .To(new Color(1, 1, 1, 1), 1)
                .EndAnimate();
            return seq;
        }

        private ISequence CreateSqueeze() {
            var seq = SequenceBuilder.Create()
                .AnimateSteps<Vector2>(this, Property.Scale2D, Easing.SineInOut)
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
            _slopeStairsManager.DisableSlopeStairsCoverForBody(this);
            _slopeStairsManager.EnableSlopeStairsForBody(this);
        }

        public void DisableSlopeStairs() {
            _slopeStairsManager.EnableSlopeStairsCoverForBody(this);
            _slopeStairsManager.DisableSlopeStairsForBody(this);
        }


        public void _OnFallingPlatformExit(BodyOnArea2D evt) => _platformManager.BodyStopFallFromPlatform(this);

        public void _OnSlopeStairsEnablerEnter(BodyOnArea2D evt) => EnableSlopeStairs();

        public void _OnSlopeStairsDisablerEnter(BodyOnArea2D evt) => DisableSlopeStairs();


        // public override void _PhysicsProcess(float delta) {
        public override void _Process(float delta) {
            // Update();
            // Label.Text = Position.DistanceTo(GetLocalMousePosition())+" "+Position.AngleTo(GetLocalMousePosition());
            // Label.BbcodeText = "Idle:" + AnimationIdle.Playing + " Attack:" + AnimationAttack.Playing + "\n" +
            // _animationStack.GetPlayingLoop()?.Name + " " + _animationStack.GetPlayingOnce()?.Name;
            Label.BbcodeText = _animationStack.GetPlayingLoop()?.Name + " " + _animationStack.GetPlayingOnce()?.Name +
                               "\n" +
                               _tweenStack.GetPlayingLoop()?.Name + " " + _tweenStack.GetPlayingOnce()?.Name;
            /*
                Label.Text = "Floor: " + IsOnFloor() + "\n" +
                              "Slope: " + IsOnSlope() + "\n" +
                              "Stair: " + IsOnSlopeStairs() + "\n" +
                              "Moving: " + IsOnMovingPlatform() + "\n" +
                              "Falling: " + IsOnFallingPlatform();
                */
        }


        public override void _Input(InputEvent @event) {
            if (!_gameManager.IsGaming()) return;
            if (_inputManager.UiStart.IsEventPressed(@event)) {
                _gameManager.ShowPauseMenu();
            } else if (_inputManager.PixelPerfect.IsEventPressed(@event)) {
                _screenManager.SetPixelPerfect(!_screenManager.Settings.PixelPerfect);
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

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                _tweenStack?.Free(); // It's a GodotObject, it needs to be freed manually
                _animationStack?.Free(); // It's a GodotObject, it needs to be freed manually
            }
        }
    }
}