using System;
using System.Collections.Generic;
using Betauer.Animation.Easing;
using Betauer.Nodes.Property;
using Godot;

namespace Betauer.Animation.Tween {
    public class PropertyKeyframeAbsoluteTweener<TProperty> : PropertyKeyframeTweener<TProperty> {
        private readonly KeyframeAnimation _animation;

        internal PropertyKeyframeAbsoluteTweener(KeyframeAnimation animation,
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? defaultEasing) :
            base(propertyFactory, defaultEasing) {
            _animation = animation;
        }

        public PropertyKeyframeAbsoluteTweener<TProperty> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyKeyframeAbsoluteTweener<TProperty> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyKeyframeAbsoluteTweener<TProperty> KeyframeTo(float percentage, TProperty to,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            return KeyframeTo(percentage, _ => to, easing, callbackNode);
        }

        public PropertyKeyframeAbsoluteTweener<TProperty> KeyframeTo(float percentage, Func<Node, TProperty> to,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            if (percentage == 0f) {
                From(to);
            }
            var animationStepPropertyTweener =
                new AnimationKeyframeAbsolute<TProperty>(percentage, to, easing, callbackNode);
            Keyframes.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyframeAbsoluteTweener<TProperty> SetDebugSteps(
            List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public KeyframeAnimation EndAnimate() {
            if (Keyframes == null || Keyframes.Count == 0) {//
                throw new InvalidAnimationException("Animation without absolute keyframes");
            }
            return _animation;
        }
    }
}