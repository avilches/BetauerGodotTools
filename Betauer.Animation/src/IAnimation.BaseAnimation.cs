using System;
using Godot;

namespace Betauer.Animation {
    public abstract class BaseAnimation<TBuilder> : IAnimation where TBuilder : class {
        public int Loops { get; protected set; } = 1;
        public Node? DefaultTarget { get; protected set; }
        public Action<Node>? StartAction { get; protected set; }
        public Action<Node>? FinishAction { get; protected set; }
        public Action? FinishAllAction { get; protected set; }

        public float ScaleSpeed { get; protected set; } = 1.0f;
        public Tween.TweenProcessMode ProcessMode { get; protected set; } = Godot.Tween.TweenProcessMode.Idle;

        public Tween Play() {
            return Play(null);
        }

        public abstract bool IsCompatibleWith(Node node);

        public Tween Play(float initialDelay) {
            return Play(null, initialDelay);
        }

        public abstract Tween Play(Node? target, float initialDelay = 0);

        public TBuilder SetSpeed(float speed) {
            ScaleSpeed = speed;
            return this as TBuilder;
        }

        public TBuilder OnStart(Action<Node> onStart) {
            StartAction = onStart;
            return this as TBuilder;
        }

        public TBuilder OnFinish(Action<Node> onFinish) {
            throw new Exception("Missing tests");
            FinishAction = onFinish;
            return this as TBuilder;
        }

        public TBuilder OnFinishAll(Action onFinishAll) {
            throw new Exception("Missing tests");
            FinishAllAction = onFinishAll;
            return this as TBuilder;
        }

        public TBuilder SetProcessMode(Tween.TweenProcessMode processMode) {
            ProcessMode = processMode;
            return this as TBuilder;
        }

        public TBuilder SetInfiniteLoops() {
            Loops = 0;
            return this as TBuilder;
        }

        public TBuilder SetLoops(int maxLoops) {
            Loops = maxLoops;
            return this as TBuilder;
        }

        public TBuilder SetDefaultTarget(Node defaultTarget) {
            DefaultTarget = defaultTarget;
            return this as TBuilder;
        }
        
        
        protected (Node, Tween) CreateSceneTreeTween(Node? target) {
            var realTarget = target ?? DefaultTarget ??
                throw new InvalidAnimationException("Sequence has no target and Execute() method does not provide a target");
            
            Tween? sceneTreeTween = realTarget.CreateTween();
            if (sceneTreeTween == null)
                throw new InvalidAnimationException("Tween created from node " + realTarget.GetType().Name + " \'" + realTarget.Name +
                                    "\' is null: add it to the scene tree.");
            return (realTarget, sceneTreeTween);
        }

        protected void ApplySceneTreeTweenConfiguration(Tween sceneTreeTween) {
            sceneTreeTween
                .SetProcessMode(ProcessMode)
                .SetSpeedScale(ScaleSpeed)
                .SetLoops(Loops);
        }

        protected void AddOnFinishAllEvent(Tween sceneTreeTween) {
            if (FinishAllAction != null) sceneTreeTween.Finished += FinishAllAction;
        }
    }
}