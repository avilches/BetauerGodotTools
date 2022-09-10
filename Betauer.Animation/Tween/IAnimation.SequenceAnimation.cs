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
        public PropertyStepTweenerAbsolute<TProperty> AnimateSteps<TProperty>(
            Action<TProperty> action, IEasing? easing = null) {
            return AnimateSteps<TProperty>((_, value) => action(value), easing);
        }

        public PropertyStepTweenerAbsolute<TProperty> AnimateSteps<TProperty>(
            Action<Node, TProperty> action, IEasing? easing = null) {
            var tweener =
                new PropertyStepTweenerAbsolute<TProperty>(this, (_) => new NodeCallbackProperty<TProperty>(action), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepTweenerAbsolute<TProperty> AnimateSteps<TProperty>(
            string property, IEasing? easing = null) {
            return AnimateSteps(IndexedSingleProperty.Create<TProperty>(property), easing);
        }

        public PropertyStepTweenerAbsolute<TProperty> AnimateSteps<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            return AnimateSteps((_) => property, easing);
        }

        public PropertyStepTweenerAbsolute<TProperty> AnimateSteps<TProperty>(
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? easing = null) {
            var tweener = new PropertyStepTweenerAbsolute<TProperty>(this, propertyFactory, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateStepsBy
         */

        public PropertyStepTweenerOffset<TProperty> AnimateStepsBy<TProperty>(
            Action<TProperty> action, IEasing? easing = null) {
            return AnimateStepsBy<TProperty>((_, value) => action(value), easing);
        }

        public PropertyStepTweenerOffset<TProperty> AnimateStepsBy<TProperty>(
            Action<Node, TProperty> action, IEasing? easing = null) {
            var tweener = new PropertyStepTweenerOffset<TProperty>(this, 
                (_) => new NodeCallbackProperty<TProperty>(action), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepTweenerOffset<TProperty> AnimateStepsBy<TProperty>(
            string property, IEasing? easing = null) {
            return AnimateStepsBy(IndexedSingleProperty.Create<TProperty>(property), easing);
        }

        public PropertyStepTweenerOffset<TProperty> AnimateStepsBy<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            return AnimateStepsBy((_) => property, easing);
        }

        public PropertyStepTweenerOffset<TProperty> AnimateStepsBy<TProperty>(
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? easing = null) {
            var tweener =
                new PropertyStepTweenerOffset<TProperty>(this, propertyFactory, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeSteps
         */

        public PropertyStepTweenerOffset<TProperty> AnimateRelativeSteps<TProperty>(
            Action<TProperty> action, IEasing? easing = null) {
            return AnimateRelativeSteps<TProperty>((_, value) => action(value), easing);
        }

        public PropertyStepTweenerOffset<TProperty> AnimateRelativeSteps<TProperty>(
            Action<Node, TProperty> action, IEasing? easing = null) {
            var tweener = new PropertyStepTweenerOffset<TProperty>(this, 
                    (_) => new NodeCallbackProperty<TProperty>(action), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepTweenerOffset<TProperty> AnimateRelativeSteps<TProperty>(
            string property, IEasing? easing = null) {
            return AnimateRelativeSteps(IndexedSingleProperty.Create<TProperty>(property), easing);
        }

        public PropertyStepTweenerOffset<TProperty> AnimateRelativeSteps<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            return AnimateRelativeSteps((_) => property, easing);
        }

        public PropertyStepTweenerOffset<TProperty> AnimateRelativeSteps<TProperty>(
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? easing = null) {
            var tweener = new PropertyStepTweenerOffset<TProperty>(this, propertyFactory, easing, true);
            AddTweener(tweener);
            return tweener;
        }
    }
}