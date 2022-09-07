using System;
using System.Collections.Generic;
using Betauer.Animation.Easing;
using Betauer.Nodes.Property;
using Godot;

namespace Betauer.Animation.Tween {
    public class PropertyStepOffsetBuilder<TProperty> : PropertyStepTweener<TProperty> {
        private readonly SequenceAnimation _animation;

        internal PropertyStepOffsetBuilder(SequenceAnimation animation,
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? defaultEasing, bool relativeToFrom) :
            base(propertyFactory, defaultEasing) {
            _animation = animation;
            RelativeToFrom = relativeToFrom;
        }

        public PropertyStepOffsetBuilder<TProperty> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyStepOffsetBuilder<TProperty> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyStepOffsetBuilder<TProperty> Offset(TProperty offset, float duration,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            return Offset(_ => offset, duration, easing, callbackNode);
        }

        public PropertyStepOffsetBuilder<TProperty> Offset(Func<Node, TProperty> offset, float duration,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationStepOffset<TProperty>(offset, duration, easing, callbackNode);
            Steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyStepOffsetBuilder<TProperty> SetDebugSteps(List<DebugStep<TProperty>> debugSteps) {
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