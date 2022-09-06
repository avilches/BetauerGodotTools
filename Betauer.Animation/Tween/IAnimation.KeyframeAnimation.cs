using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Animation.Easing;
using Godot;

namespace Betauer.Animation.Tween {

    public class KeyframeAnimation : BaseAnimation<KeyframeAnimation> {
        public float Duration { get; protected set; }

        public ICollection<IPropertyKeyframeTweener> TweenList { get; protected set; } =
            new List<IPropertyKeyframeTweener>();

        public static KeyframeAnimation Create(Node target = null) {
            var sequenceBuilder = new KeyframeAnimation().SetDefaultTarget(target);
            return sequenceBuilder;
        }

        public override SceneTreeTween Play(Node? target, float initialDelay = 0) {
            return Play(target, initialDelay, -1);
        }

        public SceneTreeTween Play(Node target, float initialDelay, float duration) {
            duration = duration > 0 ? duration : Duration;
            var (node, sceneTreeTween) = ValidateAndCreateSceneTreeTween(target, duration);
            StartAction?.Invoke(node);
            foreach (var tweener in TweenList) {
                tweener.Start(sceneTreeTween, initialDelay, node, duration);
            }
            ApplySceneTreeTweenConfiguration(sceneTreeTween);
            return sceneTreeTween;
        }
        
        public SceneTreeTween Play(IEnumerable<Node> targets, float delayPerTarget = 0, float initialDelay = 0, float durationPerTarget = -1, float maxDurationAllTargets = -1) {
            durationPerTarget = durationPerTarget > 0 ? durationPerTarget : Duration;
            if (maxDurationAllTargets > 0) {
                var targetCount = targets.Count();
                if (delayPerTarget * targetCount > maxDurationAllTargets) {
                    delayPerTarget = maxDurationAllTargets / targetCount;
                }
            }
            
            var (_, sceneTreeTween) = ValidateAndCreateSceneTreeTween(targets.First(), durationPerTarget);
            targets.ForEach(node => {
                StartAction?.Invoke(node);
                foreach (var tweener in TweenList) {
                    tweener.Start(sceneTreeTween, initialDelay, node, durationPerTarget);
                }
                initialDelay += delayPerTarget;
            });
            ApplySceneTreeTweenConfiguration(sceneTreeTween);
            return sceneTreeTween;
        }

        private (Node, SceneTreeTween) ValidateAndCreateSceneTreeTween(Node? target, float duration) {
            if (TweenList.Count == 0) throw new Exception("Can't start a keyframe animation without animations");
            if (duration <= 0) throw new Exception("Keyframe animation duration should be more than 0");
            return CreateSceneTreeTween(target);
        }

        public override bool IsCompatibleWith(Node node) {
            return TweenList.All(t => t.IsCompatibleWith(node));
        }
        
        protected void AddTweener(IPropertyKeyframeTweener tweener) {
            TweenList.Add(tweener);
        }

        public KeyframeAnimation SetDuration(float duration) {
            Duration = duration;
            return this;
        }

        /*
         * AnimateKeys
         */

        public PropertyKeyframeAbsoluteTweener<TProperty> AnimateKeys<TProperty>(
            Action<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyframeAbsoluteTweener<TProperty>(this, new CallbackProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyframeAbsoluteTweener<TProperty> AnimateKeys<TProperty>(
            Action<Node, TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyframeAbsoluteTweener<TProperty>(this, new NodeCallbackProperty<TProperty>(property),
                    easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyframeAbsoluteTweener<TProperty> AnimateKeys<TProperty>(
            string property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyframeAbsoluteTweener<TProperty>(this, new IndexedProperty<TProperty>(property), easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyframeAbsoluteTweener<TProperty> AnimateKeys<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            var tweener = new PropertyKeyframeAbsoluteTweener<TProperty>(this, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateKeysBy
         */

        public PropertyKeyframeOffsetTweener<TProperty> AnimateKeysBy<TProperty>(
            Action<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyframeOffsetTweener<TProperty>(this, new CallbackProperty<TProperty>(property), easing,
                    false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyframeOffsetTweener<TProperty> AnimateKeysBy<TProperty>(
            Action<Node, TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyframeOffsetTweener<TProperty>(this, new NodeCallbackProperty<TProperty>(property),
                    easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyframeOffsetTweener<TProperty> AnimateKeysBy<TProperty>(
            string property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyframeOffsetTweener<TProperty>(this, new IndexedProperty<TProperty>(property), easing,
                    false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyframeOffsetTweener<TProperty> AnimateKeysBy<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyframeOffsetTweener<TProperty>(this, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeKeys
         */

        public PropertyKeyframeOffsetTweener<TProperty> AnimateRelativeKeys<TProperty>(
            Action<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyframeOffsetTweener<TProperty>(this, new CallbackProperty<TProperty>(property), easing,
                    true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyframeOffsetTweener<TProperty> AnimateRelativeKeys<TProperty>(
            Action<Node, TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyframeOffsetTweener<TProperty>(this, new NodeCallbackProperty<TProperty>(property),
                    easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyframeOffsetTweener<TProperty> AnimateRelativeKeys<TProperty>(
            string property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyframeOffsetTweener<TProperty>(this, new IndexedProperty<TProperty>(property), easing,
                    true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyframeOffsetTweener<TProperty> AnimateRelativeKeys<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            var tweener =
                new PropertyKeyframeOffsetTweener<TProperty>(this, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }
    }
}