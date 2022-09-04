using System;
using Betauer.Animation.Easing;
using Godot;

namespace Betauer.Animation.Tween {
    public abstract class AnimationKey<TProperty> {
        public readonly IEasing? Easing;
        public readonly Action<Node>? CallbackNode;

        internal AnimationKey(IEasing? easing, Action<Node>? callbackNode) {
            Easing = easing;
            CallbackNode = callbackNode;
        }

        public abstract TProperty GetTo(Node target, TProperty from);
    }

    /**
     * Step: to + duration
     */
    public abstract class AnimationKeyStep<TProperty> : AnimationKey<TProperty> {
        public readonly float Duration;

        internal AnimationKeyStep(float duration, IEasing? easing, Action<Node>? callbackNode) : base(easing, callbackNode) {
            Duration = duration;
        }
    }

    public class AnimationKeyStepTo<TProperty> : AnimationKeyStep<TProperty> {
        public readonly Func<Node, TProperty> To;

        internal AnimationKeyStepTo(Func<Node, TProperty> to, float duration, IEasing? easing, Action<Node>? callbackNode) :
            base(duration, easing, callbackNode) {
            To = to;
        }

        public override TProperty GetTo(Node target, TProperty from) {
            return To(target);
        }
    }

    public class AnimationKeyStepOffset<TProperty> : AnimationKeyStep<TProperty> {
        public readonly Func<Node, TProperty> Offset;

        internal AnimationKeyStepOffset(Func<Node, TProperty> offset, float duration, IEasing? easing, Action<Node>? callbackNode) :
            base(duration, easing, callbackNode) {
            Offset = offset;
        }

        public override TProperty GetTo(Node target, TProperty from) {
            return VariantHelper.SumVariant(from, Offset(target));
        }
    }

    /**
     * Percent: to + percent keyframe
     */
    public abstract class AnimationKeyPercent<TProperty> : AnimationKey<TProperty> {
        public readonly float Percent;

        internal AnimationKeyPercent(float percent, IEasing? easing, Action<Node>? callbackNode) : base(easing, callbackNode) {
            Percent = percent;
        }
    }

    public class AnimationKeyPercentTo<TProperty> : AnimationKeyPercent<TProperty> {
        public readonly Func<Node, TProperty> To;

        internal AnimationKeyPercentTo(float percent, Func<Node, TProperty> to, IEasing? easing, Action<Node>? callbackNode) :
            base(percent, easing, callbackNode) {
            To = to;
        }

        public override TProperty GetTo(Node target, TProperty from) {
            return To(target);
        }
    }

    public class AnimationKeyPercentOffset<TProperty> : AnimationKeyPercent<TProperty> {
        public readonly Func<Node, TProperty> Offset;

        internal AnimationKeyPercentOffset(float percent, Func<Node, TProperty> offset, IEasing? easing, Action<Node>? callbackNode) :
            base(percent, easing, callbackNode) {
            Offset = offset;
        }

        public override TProperty GetTo(Node target, TProperty from) {
            return VariantHelper.SumVariant(from, Offset(target));
        }
    }
}