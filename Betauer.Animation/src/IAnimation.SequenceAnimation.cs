using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Easing;
using Betauer.Core.Nodes.Property;
using Godot;

namespace Betauer.Animation {
    public class SequenceAnimation : BaseAnimation<SequenceAnimation> {
        private bool _parallel = false;
        public List<List<ITweener>> TweenList { get; protected set; } = new();

        public static SequenceAnimation Create(Node target = null) {
            var sequenceBuilder = new SequenceAnimation().SetDefaultTarget(target);
            return sequenceBuilder;
        }

        public override Tween Play(Node? target, float initialDelay = 0) {
            if (TweenList.Count == 0) throw new InvalidAnimationException("Can't start a sequence without animations");
            var (realTarget, sceneTreeTween) = CreateSceneTreeTween(target);
            ExecuteTweenList(sceneTreeTween, initialDelay, realTarget);
            AddOnFinishAllEvent(sceneTreeTween);
            ApplySceneTreeTweenConfiguration(sceneTreeTween);
            return sceneTreeTween;
        }

        public Tween Play(IEnumerable<Node> nodes, float delayBetweenNodes = 0, float initialDelay = 0) {
            if (TweenList.Count == 0) throw new Exception("Can't start a sequence without animations");
            var (_, sceneTreeTween) = CreateSceneTreeTween(nodes.First());
            foreach (var node in nodes) {
                ExecuteTweenList(sceneTreeTween, initialDelay, node);
                initialDelay += delayBetweenNodes;
            }
            AddOnFinishAllEvent(sceneTreeTween);
            ApplySceneTreeTweenConfiguration(sceneTreeTween);
            return sceneTreeTween;
        }

        protected float ExecuteTweenList(Tween sceneTreeTween, float initialDelay, Node target) {
            if (StartAction != null) {
                sceneTreeTween.TweenCallback(Callable.From(() => StartAction?.Invoke(target))).SetDelay(initialDelay);
            }
            float accumulatedDelay = 0;
            foreach (var parallelGroup in TweenList) {
                float longestTime = 0;
                foreach (var tweener in parallelGroup) {
                    var tweenTime = tweener.Start(sceneTreeTween, initialDelay + accumulatedDelay, target);
                    longestTime = Math.Max(longestTime, tweenTime);
                }
                accumulatedDelay += longestTime;
            }
            if (FinishAction != null) {
                sceneTreeTween.TweenCallback(Callable.From(() => FinishAction?.Invoke(target))).SetDelay(initialDelay + accumulatedDelay);
            }
            return accumulatedDelay;
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

        public SequenceAnimation Add(SequenceAnimation sequence) {
            AddTweener(new NestedSequenceAsTweener(sequence));
            return this;
        }

        public SequenceAnimation Callback(Action callback, float delay = 0) {
            return Callback((_) => callback(), delay);
        }

        public SequenceAnimation Callback(Action<Node> callback, float delay = 0) {
            AddTweener(new CallbackNodeTweener(delay, callback));
            return this;
        }

        /*
         * AnimateSteps
         */
        public PropertyStepTweenerAbsolute<TProperty> AnimateSteps<TProperty>(
            Action<TProperty> action, IInterpolation? easing = null) {
            return AnimateSteps<TProperty>((_, value) => action(value), easing);
        }

        public PropertyStepTweenerAbsolute<TProperty> AnimateSteps<TProperty>(
            Action<Node, TProperty> action, IInterpolation? easing = null) {
            var tweener =
                new PropertyStepTweenerAbsolute<TProperty>(this, (_) => new NodeCallbackProperty<TProperty>(action), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepTweenerAbsolute<TProperty> AnimateSteps<TProperty>(
            string property, IInterpolation? easing = null) {
            return AnimateSteps((IndexedSingleProperty<TProperty>)property, easing);
        }

        public PropertyStepTweenerAbsolute<TProperty> AnimateSteps<TProperty>(
            IProperty<TProperty> property, IInterpolation? easing = null) {
            return AnimateSteps((_) => property, easing);
        }

        public PropertyStepTweenerAbsolute<TProperty> AnimateSteps<TProperty>(
            Func<Node, IProperty<TProperty>> propertyFactory, IInterpolation? easing = null) {
            var tweener = new PropertyStepTweenerAbsolute<TProperty>(this, propertyFactory, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateStepsBy
         */

        public PropertyStepTweenerOffset<TProperty> AnimateStepsBy<TProperty>(
            Action<TProperty> action, IInterpolation? easing = null) {
            return AnimateStepsBy<TProperty>((_, value) => action(value), easing);
        }

        public PropertyStepTweenerOffset<TProperty> AnimateStepsBy<TProperty>(
            Action<Node, TProperty> action, IInterpolation? easing = null) {
            var tweener = new PropertyStepTweenerOffset<TProperty>(this, 
                (_) => new NodeCallbackProperty<TProperty>(action), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepTweenerOffset<TProperty> AnimateStepsBy<TProperty>(
            string property, IInterpolation? easing = null) {
            return AnimateStepsBy((IndexedSingleProperty<TProperty>)property, easing);
        }

        public PropertyStepTweenerOffset<TProperty> AnimateStepsBy<TProperty>(
            IProperty<TProperty> property, IInterpolation? easing = null) {
            return AnimateStepsBy((_) => property, easing);
        }

        public PropertyStepTweenerOffset<TProperty> AnimateStepsBy<TProperty>(
            Func<Node, IProperty<TProperty>> propertyFactory, IInterpolation? easing = null) {
            var tweener =
                new PropertyStepTweenerOffset<TProperty>(this, propertyFactory, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeSteps
         */

        public PropertyStepTweenerOffset<TProperty> AnimateRelativeSteps<TProperty>(
            Action<TProperty> action, IInterpolation? easing = null) {
            return AnimateRelativeSteps<TProperty>((_, value) => action(value), easing);
        }

        public PropertyStepTweenerOffset<TProperty> AnimateRelativeSteps<TProperty>(
            Action<Node, TProperty> action, IInterpolation? easing = null) {
            var tweener = new PropertyStepTweenerOffset<TProperty>(this, 
                    (_) => new NodeCallbackProperty<TProperty>(action), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyStepTweenerOffset<TProperty> AnimateRelativeSteps<TProperty>(
            string property, IInterpolation? easing = null) {
            return AnimateRelativeSteps((IndexedSingleProperty<TProperty>)property, easing);
        }

        public PropertyStepTweenerOffset<TProperty> AnimateRelativeSteps<TProperty>(
            IProperty<TProperty> property, IInterpolation? easing = null) {
            return AnimateRelativeSteps((_) => property, easing);
        }

        public PropertyStepTweenerOffset<TProperty> AnimateRelativeSteps<TProperty>(
            Func<Node, IProperty<TProperty>> propertyFactory, IInterpolation? easing = null) {
            var tweener = new PropertyStepTweenerOffset<TProperty>(this, propertyFactory, easing, true);
            AddTweener(tweener);
            return tweener;
        }
        
        // TODO: missing tests of:
        // - if loops is 0, it should fail
        // - if no tweens, it should fail
        // - test with loops
        // - test StartAction is only executed once per target when loops > 0
        private class NestedSequenceAsTweener : ITweener {
            private readonly SequenceAnimation _sequenceAnimation;

            public NestedSequenceAsTweener(SequenceAnimation sequenceAnimation) {
                _sequenceAnimation = sequenceAnimation;
            }

            public float Start(Tween sceneTreeTween, float initialDelay, Node target) {
                if (_sequenceAnimation.Loops == 0) throw new InvalidAnimationException("Nested sequence can not have infinite loops (0)");
                if (_sequenceAnimation.TweenList.Count == 0) throw new InvalidAnimationException("Can't start a sequence without animations");
                var accumulatedDelay = 0f;
                for (var loop = 0; loop < _sequenceAnimation.Loops; loop++) {
                    var time = _sequenceAnimation.ExecuteTweenList(sceneTreeTween, initialDelay + accumulatedDelay, target);
                    accumulatedDelay += time;
                }
                _sequenceAnimation.AddOnFinishAllEvent(sceneTreeTween);
                return accumulatedDelay;
            }

            public bool IsCompatibleWith(Node node) {
                return _sequenceAnimation.IsCompatibleWith(node);
            }
        }

    }
}