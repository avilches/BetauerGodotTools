using System.Collections.Generic;
using Betauer.Animation.Easing;
using Godot;

namespace Betauer.Animation.Tween {
    public abstract class PropertyStepTweener<TProperty> : PropertyTweener<TProperty>, ITweener {
        public readonly ICollection<AnimationStep<TProperty>> Steps = new List<AnimationStep<TProperty>>();

        protected PropertyStepTweener(IProperty<TProperty> property, IEasing? defaultEasing) :
            base(property, defaultEasing) {
        }

        public bool IsCompatibleWith(Node node) {
            return Property.IsCompatibleWith(node);
        }

        public float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target) {
            if (!Validate(Steps.Count, target, Property)) return 0;
            TProperty initialValue = Property.GetValue(target);
            var from = FromFunction != null ? FromFunction(target) : initialValue;
            var initialFrom = from;
            var startTime = 0f;
            AnimationContext<TProperty> context = new AnimationContext<TProperty>(target, initialValue, 0);
            foreach (var step in Steps) {
                var to = step.GetTo(target, RelativeToFrom ? initialFrom : from);
                var durationStep = step.Duration;
                var start = initialDelay + startTime;
                if (durationStep > 0 && !from.Equals(to)) {
                    RunStep(sceneTreeTween, context, Property, from, to, start, durationStep, step.Easing);
                }
                if (step.CallbackNode != null) {
                    var callbackTweener = sceneTreeTween
                        .Parallel()
                        .TweenCallbackAction(() => step.CallbackNode(target))
                        .SetDelay(start);
                }
                from = to;
                startTime += durationStep;
            }
            return startTime;
        }
    }
}

