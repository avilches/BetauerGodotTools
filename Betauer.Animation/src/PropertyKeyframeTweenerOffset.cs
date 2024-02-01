using System;
using System.Collections.Generic;
using Betauer.Core.Easing;
using Betauer.Core.Nodes.Property;
using Godot;

namespace Betauer.Animation {
    public class PropertyKeyframeTweenerOffset<[MustBeVariant] TProperty> : PropertyKeyframeTweener<TProperty> {
        private readonly KeyframeAnimation _animation;

        internal PropertyKeyframeTweenerOffset(KeyframeAnimation animation,
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? defaultEasing, bool relativeToFrom) :
            base(propertyFactory, defaultEasing) {
            _animation = animation;
            RelativeToFrom = relativeToFrom;
        }
        
        public PropertyKeyframeTweenerOffset<TProperty> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyKeyframeTweenerOffset<TProperty> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyKeyframeTweenerOffset<TProperty> KeyframeOffset(float percentage, TProperty offset,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            return KeyframeOffset(percentage, _ => offset, easing, callbackNode);
        }

        public PropertyKeyframeTweenerOffset<TProperty> KeyframeOffset(float percentage,
            Func<Node, TProperty> offset,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyframeOffset<TProperty>(percentage, offset, easing, callbackNode);
            Keyframes.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyframeTweenerOffset<TProperty> SetDebugSteps(
            List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public KeyframeAnimation EndAnimate() {
            if (Keyframes == null || Keyframes.Count == 0) { //
                throw new InvalidAnimationException("Animation without offset keyframes");
            }
            return _animation;
        }
    }
}