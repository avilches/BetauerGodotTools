using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Animation.Easing;
using Betauer.Nodes.Property;
using Godot;

namespace Betauer.Animation.Tween {

    public class KeyframeAnimation : BaseAnimation<KeyframeAnimation> {
        public float Duration { get; protected set; }

        public ICollection<IPropertyKeyframeTweener> TweenList { get; protected set; } =
            new List<IPropertyKeyframeTweener>();

        public static KeyframeAnimation Create(Node? target = null) {
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
            if (TweenList.Count == 0) throw new InvalidAnimationException("Can't start a keyframe animation without animations");
            if (duration <= 0) throw new InvalidAnimationException("Keyframe animation duration should be more than 0");
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

        public PropertyKeyframeTweenerAbsolute<TProperty> AnimateKeys<TProperty>(
            Action<TProperty> action, IEasing? easing = null) {
            return AnimateKeys<TProperty>((_, value) => action(value), easing);
        }


        public PropertyKeyframeTweenerAbsolute<TProperty> AnimateKeys<TProperty>(
            Action<Node, TProperty> action, IEasing? easing = null) {
            var tweener = new PropertyKeyframeTweenerAbsolute<TProperty>(this, 
                (_) => new NodeCallbackProperty<TProperty>(action), easing);
            AddTweener(tweener);
            return tweener;
        }


        public PropertyKeyframeTweenerAbsolute<TProperty> AnimateKeys<TProperty>(
            string property, IEasing? easing = null) {
            return AnimateKeys(IndexedSingleProperty.Create<TProperty>(property), easing);
        }

        public PropertyKeyframeTweenerAbsolute<TProperty> AnimateKeys<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            return AnimateKeys((_) => property, easing);
        }

        public PropertyKeyframeTweenerAbsolute<TProperty> AnimateKeys<TProperty>(
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? easing = null) {
            var tweener = new PropertyKeyframeTweenerAbsolute<TProperty>(this, propertyFactory, easing);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateKeysBy
         */

        public PropertyKeyframeTweenerOffset<TProperty> AnimateKeysBy<TProperty>(
            Action<TProperty> action, IEasing? easing = null) {
            return AnimateKeysBy<TProperty>((_, value) => action(value), easing);
        }

        public PropertyKeyframeTweenerOffset<TProperty> AnimateKeysBy<TProperty>(
            Action<Node, TProperty> action, IEasing? easing = null) {
            var tweener = new PropertyKeyframeTweenerOffset<TProperty>(this, 
                    (_) => new NodeCallbackProperty<TProperty>(action), easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyframeTweenerOffset<TProperty> AnimateKeysBy<TProperty>(
            string property, IEasing? easing = null) {
            return AnimateKeysBy(IndexedSingleProperty.Create<TProperty>(property), easing);
        }

        public PropertyKeyframeTweenerOffset<TProperty> AnimateKeysBy<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            return AnimateKeysBy((_) => property, easing);
        }

        public PropertyKeyframeTweenerOffset<TProperty> AnimateKeysBy<TProperty>(
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? easing = null) {
            var tweener = new PropertyKeyframeTweenerOffset<TProperty>(this, propertyFactory, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        /*
         * AnimateRelativeKeys
         */

        public PropertyKeyframeTweenerOffset<TProperty> AnimateRelativeKeys<TProperty>(
            Action<TProperty> action, IEasing? easing = null) {
            return AnimateRelativeKeys<TProperty>((_, value) => action(value), easing);
        }

        public PropertyKeyframeTweenerOffset<TProperty> AnimateRelativeKeys<TProperty>(
            Action<Node, TProperty> action, IEasing? easing = null) {
            var tweener = new PropertyKeyframeTweenerOffset<TProperty>(this, 
                (_) => new NodeCallbackProperty<TProperty>(action), easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyframeTweenerOffset<TProperty> AnimateRelativeKeys<TProperty>(
            string property, IEasing? easing = null) {
            return AnimateRelativeKeys(IndexedSingleProperty.Create<TProperty>(property), easing);
        }

        public PropertyKeyframeTweenerOffset<TProperty> AnimateRelativeKeys<TProperty>(
            IProperty<TProperty> property, IEasing? easing = null) {
            return AnimateRelativeKeys((_) => property, easing);
        }
        
        public PropertyKeyframeTweenerOffset<TProperty> AnimateRelativeKeys<TProperty>(
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? easing = null) {
            var tweener = new PropertyKeyframeTweenerOffset<TProperty>(this, propertyFactory, easing, true);
            AddTweener(tweener);
            return tweener;
        }
    }
}