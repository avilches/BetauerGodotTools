using System;
using System.Collections.Generic;
using Betauer.Animation.Easing;
using Betauer.Nodes.Property;
using Godot;

namespace Betauer.Animation.Tween {
    public class PropertyStepAbsoluteTweener<TProperty> : PropertyStepTweener<TProperty> {
        private readonly SequenceAnimation _animation;

        internal PropertyStepAbsoluteTweener(SequenceAnimation animation, Func<Node, IProperty<TProperty>> propertyFactory, IEasing? defaultEasing) :
            base(propertyFactory, defaultEasing) {
            _animation = animation;
        }

        public PropertyStepAbsoluteTweener<TProperty> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyStepAbsoluteTweener<TProperty> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyStepAbsoluteTweener<TProperty> To(TProperty to, float duration,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            return To(_ => to, duration, easing, callbackNode);
        }

        public PropertyStepAbsoluteTweener<TProperty> To(Func<Node, TProperty> to, float duration,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationStepAbsolute<TProperty>(to, duration, easing, callbackNode);
            Steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyStepAbsoluteTweener<TProperty> SetDebugSteps(List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public SequenceAnimation EndAnimate() {
            if (Steps == null || Steps.Count == 0) { //
                throw new InvalidAnimationException("Animation without steps");
            }
            return _animation;
        }
    }
}