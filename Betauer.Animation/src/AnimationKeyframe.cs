using System;
using Betauer.Core;
using Betauer.Core.Easing;
using Godot;

namespace Betauer.Animation {
    /**
     * Percent: to + percent keyframe
     */
    public abstract class AnimationKeyframe<[MustBeVariant] TProperty> : AnimationItem<TProperty> {
        public readonly float Percent;

        internal AnimationKeyframe(float percent, IInterpolation? easing, Action<Node>? callbackNode) : base(easing, callbackNode) {
            Percent = percent;
        }
    }

    public class AnimationKeyframeAbsolute<[MustBeVariant] TProperty> : AnimationKeyframe<TProperty> {
        public readonly Func<Node, TProperty> To;

        internal AnimationKeyframeAbsolute(float percent, Func<Node, TProperty> to, IInterpolation? easing, Action<Node>? callbackNode) : base(percent, easing, callbackNode) {
            To = to;
        }

        public override TProperty GetTo(Node target, TProperty from) {
            return To(target);
        }
    }

    public class AnimationKeyframeOffset<[MustBeVariant] TProperty> : AnimationKeyframe<TProperty> {
        public readonly Func<Node, TProperty> Offset;

        internal AnimationKeyframeOffset(float percent, Func<Node, TProperty> offset, IInterpolation? easing, Action<Node>? callbackNode) : base(percent, easing, callbackNode) {
            Offset = offset;
        }

        public override TProperty GetTo(Node target, TProperty from) {
            return VariantHelper.Add(from, Offset(target)).As<TProperty>();
        }
    }
}