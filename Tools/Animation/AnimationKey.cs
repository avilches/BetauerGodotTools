namespace Tools.Animation {
    public abstract class AnimationKey<T> {
        public readonly Easing Easing;
        public readonly TweenCallback Callback;

        internal AnimationKey(Easing easing, TweenCallback callback) {
            Easing = easing;
            Callback = callback;
        }

        public abstract T GetTo(T from);
    }

    /**
     * Step: to + duration
     */
    public abstract class AnimationKeyStep<T> : AnimationKey<T> {
        public readonly float Duration;

        internal AnimationKeyStep(float duration, Easing easing, TweenCallback callback) : base(easing, callback) {
            Duration = duration;
        }
    }

    public class AnimationKeyStepTo<T> : AnimationKeyStep<T> {
        public readonly T To;

        internal AnimationKeyStepTo(T to, float duration, Easing easing, TweenCallback callback) :
            base(duration, easing, callback) {
            To = to;
        }

        public override T GetTo(T from) {
            return To;
        }
    }

    public class AnimationKeyStepOffset<T> : AnimationKeyStep<T> {
        public readonly T Offset;

        internal AnimationKeyStepOffset(T offset, float duration, Easing easing, TweenCallback callback) :
            base(duration, easing, callback) {
            Offset = offset;
        }

        public override T GetTo(T from) {
            return (T)GodotTools.SumVariant(from, Offset);
        }
    }

    /**
     * Percent: to + percent keyframe
     */
    public abstract class AnimationKeyPercent<T> : AnimationKey<T> {
        public readonly float Percent;

        internal AnimationKeyPercent(float percent, Easing easing, TweenCallback callback) : base(easing, callback) {
            Percent = percent;
        }
    }

    public class AnimationKeyPercentTo<T> : AnimationKeyPercent<T> {
        public readonly T To;

        internal AnimationKeyPercentTo(float percent, T to, Easing easing, TweenCallback callback) :
            base(percent, easing, callback) {
            To = to;
        }

        public override T GetTo(T from) {
            return To;
        }
    }

    public class AnimationKeyPercentOffset<T> : AnimationKeyPercent<T> {
        public readonly T Offset;

        internal AnimationKeyPercentOffset(float percent, T offset, Easing easing, TweenCallback callback) :
            base(percent, easing, callback) {
            Offset = offset;
        }

        public override T GetTo(T from) {
            return (T)GodotTools.SumVariant(from, Offset);
        }
    }
}