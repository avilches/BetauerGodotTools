using System;
using System.Collections.Generic;
using Betauer.Core.Easing;
using Betauer.Core.Nodes.Property;
using Godot;

namespace Betauer.Animation {
    public class PropertyStepTweenerOffset<[MustBeVariant] TProperty> : PropertyStepTweener<TProperty> {
        private readonly SequenceAnimation _animation;

        internal PropertyStepTweenerOffset(SequenceAnimation animation,
            Func<Node, IProperty<TProperty>> propertyFactory, IEasing? defaultEasing, bool relativeToFrom) :
            base(propertyFactory, defaultEasing) {
            _animation = animation;
            RelativeToFrom = relativeToFrom;
        }

        public PropertyStepTweenerOffset<TProperty> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyStepTweenerOffset<TProperty> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyStepTweenerOffset<TProperty> Offset(TProperty offset, float duration,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            return Offset(_ => offset, duration, easing, callbackNode);
        }

        public PropertyStepTweenerOffset<TProperty> Offset(Func<Node, TProperty> offset, float duration,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationStepOffset<TProperty>(offset, duration, easing, callbackNode);
            Steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyStepTweenerOffset<TProperty> SetDebugSteps(List<DebugStep<TProperty>> debugSteps) {
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