using System.Collections.Generic;
using Betauer.Animation.Easing;
using Godot;

namespace Betauer.Animation.Tween {
    public interface IPropertyKeyframeTweener {
        public bool IsCompatibleWith(Node node);
        public float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target, float duration);
    }

    public abstract class PropertyKeyframeTweener<TProperty> : PropertyTweener<TProperty>, IPropertyKeyframeTweener {
        public readonly List<AnimationKeyframe<TProperty>> Keyframes = new List<AnimationKeyframe<TProperty>>();

        protected PropertyKeyframeTweener(IProperty<TProperty> property, IEasing? defaultEasing) :
            base(property, defaultEasing) {
        }

        public float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target, float duration) {
            if (!Validate(Keyframes.Count, target, Property)) return 0;
            var initialValue = Property.GetValue(target);
            var from = FromFunction != null ? FromFunction(target) : initialValue;
            var initialFrom = from;
            var startTime = 0f;
            var idx = 0;
            var context = new AnimationContext<TProperty>(target, initialValue, duration);
            foreach (var step in Keyframes) {
                var to = step.GetTo(target, RelativeToFrom ? initialFrom : from);
                var endTime = step.Percent * duration;
                var keyDuration = endTime - startTime;
                var start = initialDelay + startTime;
                if (idx == 0 || (keyDuration > 0 && !Equals(from, to))) {
                    // always run the first keyframe, no matter if it's the 0% or any other, but avoid
                    // keyframes where the from and to are the same (no changed) or the duration is 0
                    if (step.Percent == 0f) {
                        // That means a 0s duration, so, it works like a set variable, no need to Lerp from..to
                        from = to;
                    }
                    RunStep(sceneTreeTween, context, Property, from, to, start, keyDuration, step.Easing);
                }
                if (step.CallbackNode != null) {
                    sceneTreeTween
                        .Parallel()
                        .TweenCallbackAction(() => step.CallbackNode(target))
                        .SetDelay(start);
                }
                from = to;
                startTime = endTime;
                idx++;
            }
            return duration;
        }
        
        public bool IsCompatibleWith(Node node) {
            return Property.IsCompatibleWith(node);
        }
    }
}