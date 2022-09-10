using System;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Animation.Tween;
using Betauer.Application;
using Betauer.Application.Monitor;
using Betauer.Application.Screen;
using Betauer.Bus;
using Betauer.DI;
using Betauer.Input;
using Betauer.Nodes.Property;
using Betauer.Nodes.Property.Callback;
using Betauer.OnReady;
using Betauer.Restorer;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Player;
using Veronenger.Game.Controller.UI.Consoles;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Character {
    public sealed class PlayerController : KinematicBody2D {
        private readonly string _name;
        private readonly Logger _logger;
        [OnReady("Sprite")] private Sprite _mainSprite;
        [OnReady("AttackArea")] private Area2D _attackArea;
        [OnReady("DamageArea")] private Area2D _damageArea;
        [OnReady("RichTextLabel")] private RichTextLabel Label;
        [OnReady("Detector")] public Area2D PlayerDetector;
        [OnReady("Sprite/AnimationPlayer")] private AnimationPlayer _animationPlayer;
        [OnReady("ConsoleButton")] private ConsoleButton _consoleButton;

        private IFlipper _flippers;

        [Inject] private SceneTree _sceneTree { get; set; }
        [Inject] private GameManager _gameManager { get; set; }
        [Inject] private PlatformManager _platformManager { get; set; }
        [Inject] private CharacterManager _characterManager { get; set; }
        [Inject] private SlopeStairsManager _slopeStairsManager { get; set; }
        [Inject] private ScreenSettingsManager _screenSettingsManager { get; set; }
        [Inject] private PlayerStateMachineNode StateMachineNode { get; set; }
        [Inject] private PlayerConfig _playerConfig { get; set; }
        [Inject] public KinematicPlatformMotionBody KinematicPlatformMotionBody { get; set; }
        [Inject] private DebugOverlay DebugOverlay { get; set; }

        public PlayerController() {
            _name = "Player:" + GetHashCode().ToString("x8");
            _logger = LoggerFactory.GetLogger(typeof(PlayerController));
        }

        public ILoopStatus AnimationIdle { get; private set; }
        public ILoopStatus AnimationRun { get; private set; }
        public ILoopStatus AnimationJump { get; private set; }
        public ILoopStatus AnimationFall { get; private set; }
        public IOnceStatus AnimationAttack { get; private set; }
        public IOnceStatus AnimationJumpAttack { get; private set; }

        public IOnceStatus PulsateTween;
        public ILoopStatus DangerTween;
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
        private Restorer _restorer;

        public override void _Ready() {
            _animationStack = new AnimationStack(_name, this).SetAnimationPlayer(_animationPlayer);
            AnimationIdle = _animationStack.AddLoopAnimation("Idle");
            AnimationRun = _animationStack.AddLoopAnimation("Run");
            AnimationJump = _animationStack.AddLoopAnimation("Jump");
            AnimationFall = _animationStack.AddLoopAnimation("Fall");
            AnimationAttack = _animationStack.AddOnceAnimation("Attack");
            AnimationJumpAttack = _animationStack.AddOnceAnimation("JumpAttack");

            _tweenStack = new AnimationStack(_name, this);
            _restorer = this.CreateRestorer(Properties.Modulate, Properties.Scale2D)
                .Add(_mainSprite.CreateRestorer(Properties.Modulate, Properties.Scale2D));
                
            _restorer.Save();
            Action restorePlayer = () => _restorer.Restore();
            PulsateTween = _tweenStack.AddOnceTween("Pulsate", CreateMoveLeft()).OnEnd(restorePlayer);
            DangerTween = _tweenStack.AddLoopTween("Danger", CreateDanger()).OnEnd(restorePlayer);
            SqueezeTween = _tweenStack.AddOnceTween("Squeeze", CreateSqueeze()).OnEnd(restorePlayer);

            _flippers = new FlipperList().AddSprite(_mainSprite).AddNode2D(_attackArea);
            KinematicPlatformMotionBody.Configure(this, _flippers, _name, _playerConfig.MotionConfig);

            StateMachineNode.Configure(this, _name);

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

            DebugOverlay.Create().WithPrefix("Player")
                .Bind(this)
                .Show(() => StateMachineNode.CurrentState.Key.ToString());

            
            DebugOverlay.Create().Bind(this).Show(() => Position.DistanceTo(GetLocalMousePosition())+" "+Position.AngleTo(GetLocalMousePosition()));
            DebugOverlay.Create().Bind(this).Show(() => "Idle " + AnimationIdle.Playing + ":"+_animationStack.GetPlayingLoop()?.Name +
                                                        " Attack" + AnimationAttack.Playing + ": " + _animationStack.GetPlayingOnce()?.Name);
            DebugOverlay.Create().Bind(this).Show(() =>
                _animationStack.GetPlayingLoop()?.Name + " " + _animationStack.GetPlayingOnce()?.Name);
            DebugOverlay.Create().Bind(this).Show(() =>
                _tweenStack.GetPlayingLoop()?.Name + " " + _tweenStack.GetPlayingOnce()?.Name);
            DebugOverlay.Create().Bind(this).Show(() =>
                "Floor: " + IsOnFloor() + "\n" +
                "SlopeStairsUp: " + IsOnSlopeStairsUp() + "\n" +
                "SlopeStairsDown: " + IsOnSlopeStairsDown());

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
            PulsateTween.PlayOnce();
        }

        public void StopSqueeze() {
            SqueezeTween.Stop();
        }

        public void StartSqueeze() {
            SqueezeTween.PlayOnce(true);
        }

        private IAnimation CreateReset() {
            var seq = SequenceAnimation.Create(_mainSprite)
                .AnimateSteps(Properties.Modulate)
                .From(new Color(1, 1, 1, 0))
                .To(new Color(1, 1, 1, 1), 1)
                .EndAnimate();
            // seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 1), 0.1f);
            // seq.Parallel().AddProperty(this, "scale", new Vector2(1f, 1f), 0.1f);
            return seq;
        }

        private IAnimation CreateMoveLeft() {
            var seq = KeyframeAnimation.Create(_mainSprite)
                .SetDuration(2f)
                .AnimateKeys(Properties.Modulate)
                .KeyframeTo(0.25f, new Color(1, 1, 1, 0))
                .KeyframeTo(0.75f, new Color(1, 1, 1, 0.5f))
                .KeyframeTo(1f, new Color(1, 1, 1, 1))
                .EndAnimate()
                .AnimateKeys<Vector2>(Properties.Scale2D)
                .KeyframeTo(0.5f, new Vector2(1.4f, 1f))
                .KeyframeTo(1f, new Vector2(1f, 1f))
                .EndAnimate();
            // seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 0), 1f).SetTrans(Tween.TransitionType.Cubic);
            // seq.AddProperty(_mainSprite, "modulate", new Color(1, 1, 1, 1), 1f).SetTrans(Tween.TransitionType.Cubic);
            return seq;
        }

        private IAnimation CreateDanger() {
            var seq = SequenceAnimation.Create(_mainSprite)
                .AnimateSteps<Color>(Properties.Modulate, Easings.CubicInOut)
                .To(new Color(1, 0, 0, 1), 1)
                .To(new Color(1, 1, 1, 1), 1)
                .EndAnimate();
            return seq;
        }

        private IAnimation CreateSqueeze() {
            var seq = SequenceAnimation.Create(this)
                .AnimateSteps(Properties.Scale2D, Easings.SineInOut)
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


        [Inject] private InputAction UiStart { get; set; }

        public override void _Input(InputEvent e) {
            // var action = InputActionsContainer.FindAction(e);
            // if (action != null) {
                // _logger.Debug(
                    // $"{action.Name} | JustPressed:{action.JustPressed()} Pressed:{action.Pressed()} Released:{action.Released()} {action.Strength()}");
            // }
            if (e is InputEventJoypadButton button) {
                _consoleButton.SetButton((JoystickList)button.ButtonIndex, button.Pressed);
                if (button.Pressed == false) {
                    Templates.FadeOut.Play(_consoleButton, 0, 0.6f);
                } else {
                    _consoleButton.Modulate = Colors.White;
                }
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