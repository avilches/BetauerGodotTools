using System;
using System.Collections.Generic;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Animation;
using Betauer.Application.Camera;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.Input;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Property;
using Betauer.OnReady;
using Betauer.Core.Restorer;
using Veronenger.Character.Player;
using Veronenger.Controller.UI.Consoles;
using Veronenger.Managers;

namespace Veronenger.Controller.Character {

    public readonly struct Attack {
        public readonly float Damage;

        public Attack(float damage) {
            Damage = damage;
        }
    }
    
    public sealed class PlayerController : CharacterBody2D {
        [OnReady("Sprite2D")] private Sprite2D _mainSprite;
        [OnReady("AttackArea")] private Area2D _attackArea;
        [OnReady("DamageArea")] private Area2D _damageArea;
        [OnReady("RichTextLabel")] public RichTextLabel Label;
        [OnReady("Detector")] public Area2D PlayerDetector;
        [OnReady("Sprite2D/AnimationPlayer")] private AnimationPlayer _animationPlayer;
        [OnReady("ConsoleButton")] private ConsoleButton _consoleButton;
        [OnReady("Camera2D")] private Camera2D _camera2D;

        [OnReady("Position2D")] public Position2D Position2D;
        [OnReady("SlopeRaycast")] public RayCast2D SlopeRaycast;
        [OnReady("FloorRaycasts")] public List<RayCast2D> FloorRaycasts;

        [Inject] private PlatformManager PlatformManager { get; set; }
        [Inject] private CharacterManager CharacterManager { get; set; }
        [Inject] private SlopeStairsManager SlopeStairsManager { get; set; }
        [Inject] private PlayerStateMachine StateMachine { get; set; } // Transient!
        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

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

            _cameraController = new DragCameraController(_camera2D, MouseButton.Middle, 1.8f, 100f);
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
            StateMachine.Start("Player", this, flippers);
            AddChild(StateMachine);

            CharacterManager.RegisterPlayerController(this);
            CharacterManager.ConfigurePlayerCollisions(this);
            CharacterManager.ConfigurePlayerAttackArea2D(_attackArea,
                (enemyDamageArea2DPublisher, playerAttackArea2D) => {
                    var enemy = enemyDamageArea2DPublisher.GetParent<IEnemy>();
                    enemy.AttackedByPlayer(new Attack(1f));
                    
                });
            // CharacterManager.ConfigurePlayerDamageArea2D(_damageArea);

            SlopeStairsManager.SubscribeSlopeStairsEnabler(this, (area2D) => EnableSlopeStairs());
            SlopeStairsManager.SubscribeSlopeStairsDisabler(this, (area2D) => DisableSlopeStairs());

            PlatformManager.SubscribeFallingPlatformOut(this, (area2D) => {
                PlatformManager.BodyStopFallFromPlatform(this);
            });

            // DebugOverlayManager.Overlay(this)
            //     .Title("Player")
            //     .Text("AnimationStack",() => _animationStack.GetPlayingLoop()?.Name + " " + _animationStack.GetPlayingOnce()?.Name).EndMonitor()
            //     .Text("TweenStack", () => _tweenStack.GetPlayingLoop()?.Name + " " + _tweenStack.GetPlayingOnce()?.Name).EndMonitor()
            //     .Add(new HBoxContainer().NodeBuilder()
            //         .Button("DangerTween.PlayLoop", () => DangerTween.PlayLoop()).End()
            //         .Button("DangerTween.Stop", () => DangerTween.Stop()).End()
            //         .TypedNode)
            //     .Add(new HBoxContainer().NodeBuilder()
            //         .Button("PulsateTween.PlayOnce", () => PulsateTween.PlayOnce()).End()
            //         .Button("PulsateTween.Stop", () => PulsateTween.Stop()).End()
            //         .TypedNode)
            //     .Add(new HBoxContainer().NodeBuilder()
            //         .Button("SqueezeTween.PlayOnce(kill)", () => SqueezeTween.PlayOnce(true)).End()
            //         .Button("SqueezeTween.Stop", () => SqueezeTween.Stop()).End()
            //         .TypedNode);
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
            if (e.IsKeyPressed(Key.Q)) {
                // _camera2D.Zoom -= new Vector2(0.05f, 0.05f);
            } else if (e.IsKeyPressed(Key.W)) {
                // _camera2D.Zoom = new Vector2(1, 1);
            } else if (e.IsKeyPressed(Key.E)) {
                // _camera2D.Zoom += new Vector2(0.05f, 0.05f);
            }
        }

        public void ChangeParent(Node? target) {
            if (target == null) return;
            var parent = GetParent();
            if (target != parent) {
                var tmp = GetGlobalTransform().BasisXform(Position);
                parent.RemoveChild(this);
                target.AddChild(this);
                Position = GetGlobalTransform().BasisXformInv(tmp);
            }
        }

        public override void _Process(double delta) {
            Update();
        }

        public override void _Draw() {
            DrawLine(SlopeRaycast.Position, SlopeRaycast.Position + SlopeRaycast.CastTo, Colors.Blue, 3F);
            foreach (var floorRaycast in FloorRaycasts) {
                DrawLine(floorRaycast.Position, floorRaycast.Position + floorRaycast.CastTo, Colors.Red, 1F);
            }
            // DrawLine(_floorRaycast.Position, GetLocalMousePosition(), Colors.Blue, 3F);
        }

        public bool IsAttacking => AnimationJumpAttack.Playing || AnimationAttack.Playing;

    }
}