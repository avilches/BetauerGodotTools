using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Animation.Easing;
using Godot;

namespace Betauer.Animation {
    public interface ISequence {
        public ICollection<ICollection<ITweener>> TweenList { get; }
        public Node? DefaultTarget { get; }
        public float Speed { get; }
        public float Duration { get; }
        public Tween.TweenProcessMode ProcessMode { get; }
        public SceneTreeTween Execute(float initialDelay = 0, Node? target = null, float duration = -1);
        public Action<Node>? StartAction { get; }
    }

    public class Sequence : ISequence {
        public int Loops { get; protected set; } = 1;
        public ICollection<ICollection<ITweener>> TweenList { get; protected set; } = new List<ICollection<ITweener>>();
        public Node? DefaultTarget { get; protected set; }
        public float Duration { get; protected set; }
        public Action<Node> StartAction { get; protected set; }

        public float Speed { get; protected set; } = 1.0f;
        public Tween.TweenProcessMode ProcessMode { get; protected set; } = Tween.TweenProcessMode.Idle;
        protected bool _parallel = false;

        internal Sequence() {
        }

        public SceneTreeTween Execute(float initialDelay = 0, Node? target = null, float duration = -1) {
            float accumulatedDelay = 0;
                if (TweenList.Count == 0) throw new Exception("Can't start an animation with 0 steps or keys");
            var realTarget = target ?? DefaultTarget ?? throw new Exception("Sequence has no target and Execute() method does not provide a target"); 
            StartAction?.Invoke(realTarget);
            var sceneTreeTween = realTarget.CreateTween();
            foreach (var parallelGroup in TweenList) {
                float longestTime = 0;
                foreach (var tweener in parallelGroup) {
                    var tweenTime = tweener.Start(sceneTreeTween, initialDelay + accumulatedDelay, realTarget, 
                        duration > 0 ? duration : Duration);
                    longestTime = Math.Max(longestTime, tweenTime);
                }
                accumulatedDelay += longestTime;
            }
            return sceneTreeTween;
        }

        public Sequence Parallel() {
            _parallel = true;
            return this;
        }

        public Sequence Pause(float delay) {
            if (delay > 0f) {
                AddTweener(new PauseTweener(delay));
            }
            return this;
        }

        // TODO: create a callback receiving the defaultNode, useful for templates
        public Sequence Callback(Action callback, float delay = 0) {
            AddTweener(new CallbackTweener(delay, callback));
            return this;
        }

        public Sequence Callback(Node target, string method, float delay = 0, params object[] binds) {
            AddTweener(new MethodCallbackTweener(delay, target, method, binds));
            return this;
        }

        public Sequence SetSpeed(float speed) {
            Speed = speed;
            return this;
        }

        public Sequence SetDuration(float duration) {
            // TODO: id duration is defined, every single duration should be fit on it
            Duration = duration;
            return this;
        }

        public Sequence OnStart(Action<Node> onStart) {
            StartAction = onStart;
            return this;
        }

        public Sequence SetProcessMode(Tween.TweenProcessMode processMode) {
            ProcessMode = processMode;
            return this;
        }

        protected void AddTweener(ITweener tweener) {
            if (_parallel) {
                TweenList.Last().Add(tweener);
                _parallel = false;
            } else {
                TweenList.Add(new List<ITweener> { tweener });
            }
        }

        public Sequence SetInfiniteLoops() {
            Loops = 0;
            return this;
        }

        public Sequence SetLoops(int maxLoops) {
            Loops = maxLoops;
            return this;
        }

        public Sequence SetDefaultTarget(Node defaultTarget) {
            DefaultTarget = defaultTarget;
            return this;
        }

        /*
         * AnimateSteps
         */

        public PropertyKeyStepToBuilder<TProperty> AnimateSteps<TProperty>(
            Action<TProperty> property, IEasing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty>(this, new CallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty> AnimateSteps<TProperty>(
            Action<Node, TProperty> property, IEasing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty>(this, new NodeCallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty> AnimateSteps<TProperty>(
            string property, IEasing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty>(this, new IndexedProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepToBuilder<TProperty> AnimateSteps<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty>(this, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateStepsBy
         */

        public PropertyKeyStepOffsetBuilder<TProperty> AnimateStepsBy<TProperty>(
            Action<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty>(this, new CallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty> AnimateStepsBy<TProperty>(
            Action<Node, TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty>(this, new NodeCallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty> AnimateStepsBy<TProperty>(
            string property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty>(this, new IndexedProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty> AnimateStepsBy<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty>(this, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeSteps
         */

        public PropertyKeyStepOffsetBuilder<TProperty> AnimateRelativeSteps<TProperty>(
            Action<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty>(this, new CallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty> AnimateRelativeSteps<TProperty>(
            Action<Node, TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty>(this, new NodeCallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty> AnimateRelativeSteps<TProperty>(
            string property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty>(this, new IndexedProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty> AnimateRelativeSteps<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyStepOffsetBuilder<TProperty>(this, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateKeys
         */

        public PropertyKeyPercentToBuilder<TProperty> AnimateKeys<TProperty>(
            Action<TProperty> property, IEasing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty>(this, new CallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyPercentToBuilder<TProperty> AnimateKeys<TProperty>(
            Action<Node, TProperty> property, IEasing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty>(this, new NodeCallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyPercentToBuilder<TProperty> AnimateKeys<TProperty>(
            string property, IEasing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty>(this, new IndexedProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentToBuilder<TProperty> AnimateKeys<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty>(this, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateKeysBy
         */

        public PropertyKeyPercentOffsetBuilder<TProperty> AnimateKeysBy<TProperty>(
            Action<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty>(this, new CallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty> AnimateKeysBy<TProperty>(
            Action<Node, TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty>(this, new NodeCallbackProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty> AnimateKeysBy<TProperty>(
            string property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty>(this, new IndexedProperty<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty> AnimateKeysBy<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty>(this, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeKeys
         */

        public PropertyKeyPercentOffsetBuilder<TProperty> AnimateRelativeKeys<TProperty>(
            Action<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty>(this, new CallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty> AnimateRelativeKeys<TProperty>(
            Action<Node, TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty>(this, new NodeCallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty> AnimateRelativeKeys<TProperty>(
            string property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty>(this, new IndexedProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty> AnimateRelativeKeys<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyPercentOffsetBuilder<TProperty>(this, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public static Sequence Create(Node target = null) {
            var sequenceBuilder = new Sequence().SetDefaultTarget(target);
            return sequenceBuilder;
        }

        public SceneTreeTween Play(Node node, float initialDelay = 0, float duration = -1) {
            return Play(Loops, node, initialDelay, duration);
        }

        public SceneTreeTween PlayForever(Node node = null, float initialDelay = 0, float duration = -1) {
            return Play(0, node, initialDelay, duration);
        }

        public SceneTreeTween Play(int loops, Node node, float initialDelay = 0, float duration = -1) {
            return Execute(initialDelay, node, duration).SetLoops(loops);
        }
    }
}