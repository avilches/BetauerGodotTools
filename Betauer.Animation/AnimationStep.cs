using System;
using Betauer.Animation.Easing;
using Betauer.Core;
using Godot;

namespace Betauer.Animation {
    /**
     * Step: to + duration
     */
    public abstract class AnimationStep<[MustBeVariant] TProperty> : AnimationItem<TProperty> {
        public readonly float Duration;

        protected AnimationStep(float duration, IEasing? easing, Action<Node>? callbackNode) : base(easing, callbackNode) {
            Duration = duration;
        }
    }

    public class AnimationStepAbsolute<[MustBeVariant] TProperty> : AnimationStep<TProperty> {
        public readonly Func<Node, TProperty> To;

        internal AnimationStepAbsolute(Func<Node, TProperty> to, float duration, IEasing? easing, Action<Node>? callbackNode) : base(duration, easing, callbackNode) {
            To = to;
        }

        public override TProperty GetTo(Node target, TProperty from) {
            return To(target);
        }
    }

    public class AnimationStepOffset<[MustBeVariant] TProperty> : AnimationStep<TProperty> {
        public readonly Func<Node, TProperty> Offset;

        internal AnimationStepOffset(Func<Node, TProperty> offset, float duration, IEasing? easing, Action<Node>? callbackNode) : base(duration, easing, callbackNode) {
            Offset = offset;
        }

        public override TProperty GetTo(Node target, TProperty from) {
            return VariantHelper.Add(from, Offset(target)).As<TProperty>();
        }
    }
}