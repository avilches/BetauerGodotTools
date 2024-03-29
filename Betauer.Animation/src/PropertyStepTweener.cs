using System;
using System.Collections.Generic;
using Betauer.Core.Easing;
using Betauer.Core.Nodes.Property;
using Godot;

namespace Betauer.Animation {
    public abstract class PropertyStepTweener<[MustBeVariant] TProperty> : PropertyTweener<TProperty>, ITweener {
        public readonly ICollection<AnimationStep<TProperty>> Steps = new List<AnimationStep<TProperty>>();

        protected PropertyStepTweener(Func<Node, IProperty<TProperty>> propertyFactory, IInterpolation? defaultEasing) :
            base(propertyFactory, defaultEasing) {
        }

        public bool IsCompatibleWith(Node node) {
            return PropertyFactory(node).IsCompatibleWith(node);
        }

        public float Start(Tween sceneTreeTween, float initialDelay, Node target) {
            var property = PropertyFactory(target);
            if (!Validate(Steps.Count, target, property)) return 0;
            TProperty initialValue = property.GetValue(target);
            var from = FromFunction != null ? FromFunction(target) : initialValue;
            var initialFrom = from;
            var startTime = 0f;
            foreach (var step in Steps) {
                var to = step.GetTo(target, RelativeToFrom ? initialFrom : from);
                var durationStep = step.Duration;
                var start = initialDelay + startTime;
                if (durationStep > 0 && !from.Equals(to)) {
                    RunStep(sceneTreeTween, target, property, from, to, start, durationStep, step.Easing);
                }
                if (step.CallbackNode != null) {
                    var callbackTweener = sceneTreeTween
                        .Parallel()
                        .TweenCallback(Callable.From(() => step.CallbackNode(target)))
                        .SetDelay(start);
                }
                from = to;
                startTime += durationStep;
            }
            return startTime;
        }
    }
}

