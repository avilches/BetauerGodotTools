using System;
using System.Collections.Generic;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Tween;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.Tools.Logging;
using Betauer.Nodes.Property;
using Betauer.OnReady;
using Veronenger.Character.Enemy;
using Veronenger.Managers;

namespace Veronenger.Controller.Character {
    public interface IEnemy {
        public void AttackedByPlayer(Attack attack);
    }
    
    
    public sealed class ZombieController : KinematicBody2D, IEnemy {
        private static readonly KeyframeAnimation RedFlash = KeyframeAnimation.Create()
            .SetDuration(0.3f)
            .AnimateKeys(Properties.Modulate)
            .KeyframeTo(0.00f, Colors.White)
            .KeyframeTo(0.25f, Colors.Red)
            .KeyframeTo(0.50f, Colors.White)
            .KeyframeTo(0.75f, Colors.Red)
            .KeyframeTo(1.00f, Colors.White)
            .EndAnimate();

        [OnReady("Sprite")] private Sprite _mainSprite;
        [OnReady("AttackArea")] private Area2D _attackArea;
        [OnReady("DamageArea")] private Area2D _damageArea;
        [OnReady("Label")] public Label Label;
        [OnReady("Sprite/AnimationPlayer")] private AnimationPlayer _animationPlayer;

        [OnReady("Position2D")] public Position2D Position2D;
        [OnReady("SlopeRaycast")] public RayCast2D SlopeRaycast;
        [OnReady("FloorRaycasts")] public List<RayCast2D> FloorRaycasts;

        [Inject] private ZombieStateMachine StateMachine { get; set; }  // Transient
        [Inject] private CharacterManager CharacterManager { get; set; }
        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }

        public ILoopStatus AnimationIdle { get; private set; }
        public ILoopStatus AnimationStep { get; private set; }
        public IOnceStatus AnimationDieRight { get; private set; }
        public IOnceStatus AnimationDieLeft { get; private set; }

        private SceneTreeTween _sceneTreeTween;

        public void PlayAnimationAttacked() {
            _sceneTreeTween?.Kill();
            _sceneTreeTween = RedFlash.Play(_mainSprite);
        }

        private AnimationStack _animationStack;

        public override void _Ready() {
            _animationStack = new AnimationStack("Zombie.AnimationStack").SetAnimationPlayer(_animationPlayer);
            AnimationIdle = _animationStack.AddLoopAnimation("Idle");
            AnimationStep = _animationStack.AddLoopAnimation("Step");
            AnimationDieRight = _animationStack.AddOnceAnimation("DieRight");
            AnimationDieLeft = _animationStack.AddOnceAnimation("DieLeft");
            
            var flippers = new FlipperList().AddSprite(_mainSprite).AddNode2D(_attackArea).AddNode2D(_damageArea);
            StateMachine.Start("Zombie", this, flippers, SlopeRaycast, Position2D);

            CharacterManager.ConfigureEnemyCollisions(this);
            CharacterManager.ConfigureEnemyCollisions(SlopeRaycast);
            FloorRaycasts.ForEach(r => CharacterManager.ConfigureEnemyCollisions(r));
            CharacterManager.ConfigureEnemyDamageArea2D(_damageArea);
            CharacterManager.ConfigureEnemyAttackArea2D(_attackArea);

            DebugOverlayManager.CreateOverlay()
                .Follow(this)
                .RemoveButtons()
                .Offset(new Vector2(0, -100))
                .Text("State", () => StateMachine.CurrentState.Key.ToString()).EndMonitor()
                // .Text("Mouse", () => $"{Position.DistanceTo(GetLocalMousePosition()):F1} {Position.AngleTo(GetLocalMousePosition()):F1}").EndMonitor()
                .Text("Speed",() => StateMachine.Body.Motion.ToString("F")).EndMonitor();
        }

        public void DisableAll() {
            CollisionLayer = 0;
            CollisionMask = 0;
            _damageArea.CollisionLayer = 0;
            _attackArea.CollisionLayer = 0;
            _damageArea.CollisionMask = 0;
            _attackArea.CollisionMask = 0;
        }

        public override void _Draw() {
            // if (CharacterManager.PlayerController?._playerDetector?.Position != null) {
            // DrawLine(ToLocal(CharacterManager.PlayerController._playerDetector.GlobalPosition),
            // _position2D.Position, Colors.Blue, 3F);
            // }
            // DrawLine(_position2D.Position, GetLocalMousePosition(), Colors.Blue, 3F);
        }

        public void AttackedByPlayer(Attack attack) {
            StateMachine.TriggerAttacked(attack);
        }
    }
}