using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Animation.Easing;
using Betauer.Nodes.Property;
using Godot;

namespace Betauer.Animation.Tween {
    public class SequenceAnimation : BaseAnimation<SequenceAnimation> {
        private bool _parallel = false;
        public List<List<ITweener>> TweenList { get; protected set; } = new List<List<ITweener>>();

        public static SequenceAnimation Create(Node target = null) {
            var sequenceBuilder = new SequenceAnimation().SetDefaultTarget(target);
            return sequenceBuilder;
        }

        public override SceneTreeTween Play(Node? target, float initialDelay = 0) {
            if (TweenList.Count == 0) throw new InvalidAnimationException("Can't start a sequence without animations");
            var (realTarget, sceneTreeTween) = CreateSceneTreeTween(target);
            ExecuteTweenList(sceneTreeTween, initialDelay, realTarget);
            ApplySceneTreeTweenConfiguration(sceneTreeTween);
            return sceneTreeTween;
        }

        public SceneTreeTween Play(IEnumerable<Node> nodes, float delayBetweenNodes = 0, float initialDelay = 0) {
            if (TweenList.Count == 0) throw new Exception("Can't start a sequence without animations");
            var (_, sceneTreeTween) = CreateSceneTreeTween(nodes.First());
            nodes.ForEach(node => {
                ExecuteTweenList(sceneTreeTween, initialDelay, node);
                initialDelay += delayBetweenNodes;
            });
            ApplySceneTreeTweenConfiguration(sceneTreeTween);
            return sceneTreeTween;
        }

        private void ExecuteTweenList(SceneTreeTween sceneTreeTween, float initialDelay, Node target) {
            float accumulatedDelay = 0;
            StartAction?.Invoke(target);
            foreach (var parallelGroup in TweenList) {
                float longestTime = 0;
                foreach (var tweener in parallelGroup) {
                    var tweenTime = tweener.Start(sceneTreeTween, initialDelay + accumulatedDelay, target);
                    longestTime = Math.Max(longestTime, tweenTime);
                }
                accumulatedDelay += longestTime;
            }
        }

        public override bool IsCompatibleWith(Node node) {
            return TweenList.All(list => list.All(t=> t.IsCompatibleWith(node)));
        }

        protected void AddTweener(ITweener tweener) {
            if (_parallel) {
                TweenList.Last().Add(tweener);
            } else {
                TweenList.Add(new List<ITweener> { tweener });
            }
        }

        public SequenceAnimation Parallel() {
            _parallel = true;
            return this;
        }

        public SequenceAnimation Chain() {
            _parallel = false;
            return this;
        }

        public SequenceAnimation Pause(float delay) {
            if (delay > 0f) {
                AddTweener(new PauseTweener(delay));
            }
            return this;
        }

        // TODO: create a callback Action<Node> receiving the defaultNode, useful for templates
        public SequenceAnimation Callback(Action callback, float delay = 0) {
            AddTweener(new CallbackTweener(delay, callback));
            return this;
        }

        public SequenceAnimation Callback(Node target, string method, float delay = 0, params object[] binds) {
            AddTweener(new MethodCallbackTweener(delay, target, method, binds));
            return this;
        }

        /*
         * AnimateSteps
         */
        public PropertyStepAbsoluteTweener<TProperty> AnimateSteps<TProperty>(
            Action<TProperty> property, IEasing? easing = null) {
            var tweener = new PropertyStepAbsoluteTweener<TProperty>(this, (_) => new CallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepAbsoluteTweener<TProperty> AnimateSteps<TProperty>(
            Action<Node, TProperty> action, IEasing? easing = null) {
            var tweener =
                new PropertyStepAbsoluteTweener<TProperty>(this, (_) => new NodeCallbackProperty<TProperty>(action), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepAbsoluteTweener<TProperty> AnimateSteps<TProperty>(
            string property, IEasing? easing = null) {
            var tweener = new PropertyStepAbsoluteTweener<TProperty>(this, (_) => IndexedSingleProperty.Create<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepAbsoluteTweener<TProperty> AnimateSteps<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            var tweener = new PropertyStepAbsoluteTweener<TProperty>(this, (_) => property, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepAbsoluteTweener<TProperty> AnimateSteps<TProperty>(
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? easing = null) {
            var tweener = new PropertyStepAbsoluteTweener<TProperty>(this, propertyFactory, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateStepsBy
         */

        public PropertyStepOffsetBuilder<TProperty> AnimateStepsBy<TProperty>(
            Action<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyStepOffsetBuilder<TProperty>(this, (_) => new CallbackProperty<TProperty>(property), easing,
                    false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepOffsetBuilder<TProperty> AnimateStepsBy<TProperty>(
            Action<Node, TProperty> action, IEasing? easing = null) {
            var tweener =
                new PropertyStepOffsetBuilder<TProperty>(this, (_) => new NodeCallbackProperty<TProperty>(action), easing,
                    false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepOffsetBuilder<TProperty> AnimateStepsBy<TProperty>(
            string property, IEasing? easing = null) {
            var tweener =
                new PropertyStepOffsetBuilder<TProperty>(this, (_) => IndexedSingleProperty.Create<TProperty>(property), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepOffsetBuilder<TProperty> AnimateStepsBy<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyStepOffsetBuilder<TProperty>(this, (_) => property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepOffsetBuilder<TProperty> AnimateStepsBy<TProperty>(
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? easing = null) {
            var tweener =
                new PropertyStepOffsetBuilder<TProperty>(this, propertyFactory, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeSteps
         */

        public PropertyStepOffsetBuilder<TProperty> AnimateRelativeSteps<TProperty>(
            Action<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyStepOffsetBuilder<TProperty>(this, (_) => new CallbackProperty<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepOffsetBuilder<TProperty> AnimateRelativeSteps<TProperty>(
            Action<Node, TProperty> action, IEasing? easing = null) {
            var tweener =
                new PropertyStepOffsetBuilder<TProperty>(this, (_) => new NodeCallbackProperty<TProperty>(action), easing,
                    true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepOffsetBuilder<TProperty> AnimateRelativeSteps<TProperty>(
            string property, IEasing? easing = null) {
            var tweener =
                new PropertyStepOffsetBuilder<TProperty>(this, (_) => IndexedSingleProperty.Create<TProperty>(property), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepOffsetBuilder<TProperty> AnimateRelativeSteps<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyStepOffsetBuilder<TProperty>(this, (_) => property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepOffsetBuilder<TProperty> AnimateRelativeSteps<TProperty>(
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? easing = null) {
            var tweener =
                new PropertyStepOffsetBuilder<TProperty>(this, propertyFactory, easing, true);
            AddTweener(tweener);
            return tweener;
        }
    }
}