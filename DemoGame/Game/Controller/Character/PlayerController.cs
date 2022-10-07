using System;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Animation.Tween;
using Betauer.Application.Camera;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.Nodes.Property;
using Betauer.OnReady;
using Betauer.Restorer;
using Veronenger.Game.Character;
using Veronenger.Game.Character.Player;
using Veronenger.Game.Controller.UI.Consoles;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Character {
    public sealed class PlayerController : KinematicBody2D {
        private readonly Logger _logger = LoggerFactory.GetLogger<PlayerController>();
        [OnReady("Sprite")] private Sprite _mainSprite;
        [OnReady("AttackArea")] private Area2D _attackArea;
        [OnReady("DamageArea")] private Area2D _damageArea;
        [OnReady("RichTextLabel")] private RichTextLabel Label;
        [OnReady("Position2D")] private Position2D _position2D;
        [OnReady("Detector")] public Area2D PlayerDetector;
        [OnReady("Sprite/AnimationPlayer")] private AnimationPlayer _animationPlayer;
        [OnReady("ConsoleButton")] private ConsoleButton _consoleButton;
        [OnReady("Camera2D")] private Camera2D _camera2D;
        [OnReady("RayCasts/SlopeDetector")] private RayCast2D _slopeDetector;

        [Inject] private PlatformManager PlatformManager { get; set; }
        [Inject] private CharacterManager CharacterManager { get; set; }
        [Inject] private SlopeStairsManager SlopeStairsManager { get; set; }
        [Inject] private PlayerStateMachine StateMachine { get; set; } // Transient!
        [Inject] private DebugOverlay DebugOverlay { get; set; }

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
        public bool IsOnSlopeStairsUp() => SlopeStairsManager.UpOverlap(this);
        public bool IsOnSlopeStairsDown() => SlopeStairsManager.DownOverlap(this);
        private DragCameraController _cameraController;
        private AnimationStack _animationStack;
        private AnimationStack _tweenStack;
        private Restorer _restorer;

        public override void _Ready() {
            _animationStack = new AnimationStack("Player.AnimationStack").SetAnimationPlayer(_animationPlayer);
            AnimationIdle = _animationStack.AddLoopAnimation("Idle");
            AnimationRun = _animationStack.AddLoopAnimation("Run");
            AnimationJump = _animationStack.AddLoopAnimation("Jump");
            AnimationFall = _animationStack.AddLoopAnimation("Fall");
            AnimationAttack = _animationStack.AddOnceAnimation("Attack");
            AnimationJumpAttack = _animationStack.AddOnceAnimation("JumpAttack");

            _cameraController = new DragCameraController(_camera2D, ButtonList.Middle, 1.8f, 100f);
            this.OnInput((e) => _cameraController.DragCamera(e));

            _tweenStack = new AnimationStack("Player.AnimationStack");
            _restorer = this.CreateRestorer(Properties.Modulate, Properties.Scale2D)
                .Add(_mainSprite.CreateRestorer(Properties.Modulate, Properties.Scale2D));
                
            _restorer.Save();
            Action restorePlayer = () => _restorer.Restore();
            PulsateTween = _tweenStack.AddOnceTween("Pulsate", CreateMoveLeft()).OnEnd(restorePlayer);
            DangerTween = _tweenStack.AddLoopTween("Danger", CreateDanger()).OnEnd(restorePlayer);
            SqueezeTween = _tweenStack.AddOnceTween("Squeeze", CreateSqueeze()).OnEnd(restorePlayer);

            var flippers = new FlipperList().AddSprite(_mainSprite).AddNode2D(_attackArea);
            StateMachine.Start("Player", this, flippers, _slopeDetector, _position2D);
            AddChild(StateMachine);

            CharacterManager.RegisterPlayerController(this);
            CharacterManager.ConfigurePlayerCollisions(this);
            CharacterManager.ConfigurePlayerAttackArea2D(_attackArea, _OnPlayerAttackedEnemy);
            // CharacterManager.ConfigurePlayerDamageArea2D(_damageArea);

            SlopeStairsManager.SubscribeSlopeStairsEnabler(this, (area2D) => EnableSlopeStairs());
            SlopeStairsManager.SubscribeSlopeStairsDisabler(this, (area2D) => DisableSlopeStairs());

            PlatformManager.SubscribeFallingPlatformOut(this, (area2D) => {
                PlatformManager.BodyStopFallFromPlatform(this);
            });

            DebugOverlay.CreateMonitor().WithPrefix("Player")
                .Bind(this)
                .Show(() => StateMachine.CurrentState.Key.ToString());

            // DebugOverlay.Create().Bind(this).Show(() => Position.DistanceTo(GetLocalMousePosition())+" "+Position.AngleTo(GetLocalMousePosition()));
            // DebugOverlay.Create().Bind(this).Show(() => "Idle " + AnimationIdle.Playing + ":"+_animationStack.GetPlayingLoop()?.Name +
                                                        // " Attack" + AnimationAttack.Playing + ": " + _animationStack.GetPlayingOnce()?.Name);
            // DebugOverlay.Create().Bind(this).Show(() =>
                // _animationStack.GetPlayingLoop()?.Name + " " + _animationStack.GetPlayingOnce()?.Name);
            // DebugOverlay.Create().Bind(this).Show(() =>
                // _tweenStack.GetPlayingLoop()?.Name + " " + _tweenStack.GetPlayingOnce()?.Name);
            // DebugOverlay.Create().Bind(this).Show(() =>
                // "Floor: " + IsOnFloor() + "\n" +
                // "SlopeStairsUp: " + IsOnSlopeStairsUp() + "\n" +
                // "SlopeStairsDown: " + IsOnSlopeStairsDown());

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

        private void _OnPlayerAttackedEnemy(Area2D @event) {
            // LoggerFactory.GetLogger(GetType()).RemoveDuplicates = false;
            // LoggerFactory.GetLogger(GetType()).Debug("Collision from Origin:"+originParent.Name+"."+originParent.Name+" / Detected:"+@event.Detected.GetParent().Name+"."+@event.Detected.Name);
            var originParent = @event.GetParent();
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

        public override void _Input(InputEvent e) {
            if (e.IsAnyButton()) {
                _consoleButton.SetButton(e.GetButton(), e.IsPressed());
                if (e.IsPressed()) {
                    _consoleButton.Modulate = Colors.White;
                } else {
                    Templates.FadeOut.Play(_consoleButton, 0, 0.6f);
                }
            }
            if (e.IsLeftDoubleClick()) _camera2D.Position = Vector2.Zero;
                if (e.IsKeyPressed(KeyList.Q)) {
                    _camera2D.Zoom -= new Vector2(0.05f, 0.05f);
                }
                if (e.IsKeyPressed(KeyList.W)) {
                    _camera2D.Zoom = new Vector2(1, 1);
                }
                if (e.IsKeyPressed(KeyList.E)) {
                    _camera2D.Zoom += new Vector2(0.05f, 0.05f);
                }
        }

        public override void _Draw() {
            // DrawLine(MotionBody.FloorDetector.Position, MotionBody.FloorDetector.Position + MotionBody.FloorDetector.CastTo, Colors.Red, 3F);
            // DrawLine(_playerDetector.Position, GetLocalMousePosition(), Colors.Blue, 3F);
        }

        public bool IsAttacking => AnimationJumpAttack.Playing || AnimationAttack.Playing;

    }
}