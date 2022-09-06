using System;
using System.Collections.Generic;
using Betauer.Animation.Easing;
using Godot;

namespace Betauer.Animation.Tween {
    public class PropertyKeyframeOffsetTweener<TProperty> : PropertyKeyframeTweener<TProperty> {
        private readonly KeyframeAnimation _animation;

        internal PropertyKeyframeOffsetTweener(KeyframeAnimation animation,
            IProperty<TProperty> property, IEasing? defaultEasing, bool relativeToFrom) :
            base(property, defaultEasing) {
            _animation = animation;
            RelativeToFrom = relativeToFrom;
        }
        
        public PropertyKeyframeOffsetTweener<TProperty> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyKeyframeOffsetTweener<TProperty> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyKeyframeOffsetTweener<TProperty> KeyframeOffset(float percentage, TProperty offset,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            return KeyframeOffset(percentage, _ => offset, easing, callbackNode);
        }

        public PropertyKeyframeOffsetTweener<TProperty> KeyframeOffset(float percentage,
            Func<Node, TProperty> offset,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyframeOffset<TProperty>(percentage, offset, easing, callbackNode);
            Keyframes.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyframeOffsetTweener<TProperty> SetDebugSteps(
            List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public KeyframeAnimation EndAnimate() {
            if (Keyframes == null || Keyframes.Count == 0) { //
                throw new Exception("Animation without offset keyframes");
            }
            return _animation;
        }
    }
}