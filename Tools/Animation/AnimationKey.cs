namespace Tools.Animation {
    public abstract class AnimationKey<T> {
        public readonly Easing Easing;
        public readonly CallbackNode CallbackNode;

        internal AnimationKey(Easing easing, CallbackNode callbackNode) {
            Easing = easing;
            CallbackNode = callbackNode;
        }

        public abstract T GetTo(T from);
    }

    /**
     * Step: to + duration
     */
    public abstract class AnimationKeyStep<T> : AnimationKey<T> {
        public readonly float Duration;

        internal AnimationKeyStep(float duration, Easing easing, CallbackNode callbackNode) : base(easing, callbackNode) {
            Duration = duration;
        }
    }

    public class AnimationKeyStepTo<T> : AnimationKeyStep<T> {
        public readonly T To;

        internal AnimationKeyStepTo(T to, float duration, Easing easing, CallbackNode callbackNode) :
            base(duration, easing, callbackNode) {
            To = to;
        }

        public override T GetTo(T from) {
            return To;
        }
    }

    public class AnimationKeyStepOffset<T> : AnimationKeyStep<T> {
        public readonly T Offset;

        internal AnimationKeyStepOffset(T offset, float duration, Easing easing, CallbackNode callbackNode) :
            base(duration, easing, callbackNode) {
            Offset = offset;
        }

        public override T GetTo(T from) {
            return GodotTools.SumVariant(from, Offset);
        }
    }

    /**
     * Percent: to + percent keyframe
     */
    public abstract class AnimationKeyPercent<T> : AnimationKey<T> {
        public readonly float Percent;

        internal AnimationKeyPercent(float percent, Easing easing, CallbackNode callbackNode) : base(easing, callbackNode) {
            Percent = percent;
        }
    }

    public class AnimationKeyPercentTo<T> : AnimationKeyPercent<T> {
        public readonly T To;

        internal AnimationKeyPercentTo(float percent, T to, Easing easing, CallbackNode callbackNode) :
            base(percent, easing, callbackNode) {
            To = to;
        }

        public override T GetTo(T from) {
            return To;
        }
    }

    public class AnimationKeyPercentOffset<T> : AnimationKeyPercent<T> {
        public readonly T Offset;

        internal AnimationKeyPercentOffset(float percent, T offset, Easing easing, CallbackNode callbackNode) :
            base(percent, easing, callbackNode) {
            Offset = offset;
        }

        public override T GetTo(T from) {
            return GodotTools.SumVariant(from, Offset);
        }
    }
}