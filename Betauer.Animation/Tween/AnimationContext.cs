using Godot;

namespace Betauer.Animation.Tween {
    public class AnimationContext<TProperty> {
        public readonly Node Target;
        public readonly TProperty InitialValue;
        public readonly float Duration;
        public TProperty Value;

        public AnimationContext(Node target, TProperty initialValue, float duration) {
            Target = target;
            InitialValue = initialValue;
            Duration = duration;
        }

        public AnimationContext(Node target, TProperty initialValue, float duration, TProperty value) :
            this(target, initialValue, duration) {
            Value = value;
        }
    }
}