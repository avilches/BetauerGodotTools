using Betauer.Core.Easing;
using Godot;

namespace Betauer.Animation {
    public class DebugStep<[MustBeVariant] TProperty> {
        public readonly Node Target;
        public readonly TProperty From;
        public readonly TProperty To;
        public readonly float Start;
        public readonly float Duration;
        public readonly IEasing Easing;

        public DebugStep(Node target, TProperty from, TProperty to, float start, float duration, IEasing easing) {
            Target = target;
            From = from;
            To = to;
            Start = start;
            Duration = duration;
            Easing = easing;
        }
    }
}