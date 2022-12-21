using System;
using System.Collections.Generic;
using Betauer.Animation.Easing;
using Betauer.Core.Nodes.Property;
using Godot;

namespace Betauer.Animation {
    public interface IPropertyKeyframeTweener {
        public bool IsCompatibleWith(Node node);
        public float Start(Tween sceneTreeTween, float initialDelay, Node target, float duration);
    }

    public abstract class PropertyKeyframeTweener<TProperty> : PropertyTweener<TProperty>, IPropertyKeyframeTweener {
        public readonly List<AnimationKeyframe<TProperty>> Keyframes = new List<AnimationKeyframe<TProperty>>();

        protected PropertyKeyframeTweener(Func<Node, IProperty<TProperty>> propertyFactory, IEasing? defaultEasing) :
            base(propertyFactory, defaultEasing) {
        }

        public float Start(Tween sceneTreeTween, float initialDelay, Node target, float duration) {
            var property = PropertyFactory(target);
            if (!Validate(Keyframes.Count, target, property)) return 0;
            var initialValue = property.GetValue(target);
            var from = FromFunction != null ? FromFunction(target) : initialValue;
            var initialFrom = from;
            var startTime = 0f;
            var idx = 0;
            foreach (var step in Keyframes) {
                var to = step.GetTo(target, RelativeToFrom ? initialFrom : from);
                var endTime = step.Percent * duration;
                var keyDuration = endTime - startTime;
                var start = initialDelay + startTime;

                // always run the first keyframe (idx==0), no matter if it's the 0% or bigger. Why? Because some
                // animations start to move later (50% for example), so from 0% to 50% they stay still. This loop
                // ignore tween where from==to, so a tween of 0s with from=to is generated for the first 0% step
                if (idx == 0 || (keyDuration > 0 && !Equals(from, to))) {
                    if (step.Percent == 0f) {
                        // That means a 0s duration, so, it works like a set variable
                        from = to;
                    }
                    RunStep(sceneTreeTween, target, property, from, to, start, keyDuration, step.Easing);
                }
                if (step.CallbackNode != null) {
                    sceneTreeTween
                        .Parallel()
                        .TweenCallback(Callable.From(() => step.CallbackNode(target)))
                        .SetDelay(start);
                }
                from = to;
                startTime = endTime;
                idx++;
            }
            return duration;
        }
        
        public bool IsCompatibleWith(Node node) {
            return PropertyFactory(node).IsCompatibleWith(node);
        }
    }
}