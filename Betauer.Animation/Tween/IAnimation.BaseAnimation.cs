using System;
using Godot;

namespace Betauer.Animation.Tween {
    public abstract class BaseAnimation<TBuilder> : IAnimation where TBuilder : class {
        public int Loops { get; protected set; } = 1;
        public Node? DefaultTarget { get; protected set; }
        public Action<Node> StartAction { get; protected set; } // TODO: make it an event

        public float Speed { get; protected set; } = 1.0f;
        public Godot.Tween.TweenProcessMode ProcessMode { get; protected set; } = Godot.Tween.TweenProcessMode.Idle;

        public SceneTreeTween Play() {
            return Play(null);
        }

        public abstract bool IsCompatibleWith(Node node);

        public SceneTreeTween Play(float initialDelay) {
            return Play(null, initialDelay);
        }

        public abstract SceneTreeTween Play(Node? target, float initialDelay = 0);

        public TBuilder SetSpeed(float speed) {
            Speed = speed;
            return this as TBuilder;
        }

        public TBuilder OnStart(Action<Node> onStart) {
            StartAction = onStart;
            return this as TBuilder;
        }

        public TBuilder SetProcessMode(Godot.Tween.TweenProcessMode processMode) {
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
        
        protected (Node, SceneTreeTween) CreateSceneTreeTween(Node? target) {
            var realTarget = target ?? DefaultTarget ??
                throw new InvalidAnimationException("Sequence has no target and Execute() method does not provide a target");
            
            SceneTreeTween? sceneTreeTween = realTarget.CreateTween();
            if (sceneTreeTween == null)
                throw new InvalidAnimationException("Tween created from node " + realTarget.GetType().Name + " \'" + realTarget.Name +
                                    "\' is null: add it to the scene tree.");
            return (realTarget, sceneTreeTween);
        }

        protected void ApplySceneTreeTweenConfiguration(SceneTreeTween sceneTreeTween) {
            sceneTreeTween
                .SetProcessMode(ProcessMode)
                .SetSpeedScale(Speed)
                .SetLoops(Loops);
        }

        
    }
}