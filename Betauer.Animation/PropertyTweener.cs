using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    public interface ITweener {
        float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target, float duration);
    }

    internal class CallbackTweener : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));
        private readonly Action _callback;
        private readonly float _delay;

        internal CallbackTweener(float delay, Action callback) {
            _delay = delay;
            _callback = callback;
        }

        public float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node ignoredTarget, float ignoredDuration) {
            if (!Object.IsInstanceValid(sceneTreeTween)) {
                Logger.Warning("Can't start a " + nameof(CallbackTweener) + " from a freed tween instance");
                return 0;
            }
            var start = _delay + initialDelay;
            Logger.Info("Adding anonymous callback with " + _delay + "s delay. Scheduled: " + start.ToString("F"));
            var callbackTweener = sceneTreeTween
                .Parallel()
                .TweenCallbackAction(_callback)
                .SetDelay(start);
            return _delay;
        }
    }
    internal class MethodCallbackTweener : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));
        private readonly string _methodName;
        private readonly float _delay;
        private readonly object[]? _binds;
        private readonly Node? _target;

        public MethodCallbackTweener(float delay, Node? target, string methodName, params object[] binds) {
            _target = target;
            _methodName = methodName;
            _delay = delay;
            _binds = binds;
        }

        public float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target, float ignoredDuration) {
            if (!Object.IsInstanceValid(sceneTreeTween)) {
                Logger.Warning("Can't start a " + nameof(MethodCallbackTweener) + " from a freed tween instance");
                return 0;
            }
            if (!Object.IsInstanceValid(target)) {
                Logger.Warning("Can't start a " + nameof(MethodCallbackTweener) + " using a freed target instance");
                return 0;
            }
            var start = _delay + initialDelay;
            Logger.Info("Adding method callback " + _methodName + "with " + _delay + "s delay. Scheduled: " +
                        start.ToString("F"));
            var methodTweener = sceneTreeTween
                .Parallel()
                .TweenCallback(_target ?? target, _methodName, _binds != null && _binds.Length > 0 ? new Godot.Collections.Array(_binds) : null)
                .SetDelay(start);
            return _delay;
        }
    }

    internal class PauseTweener : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));
        private readonly float _delay;

        internal PauseTweener(float delay) {
            _delay = delay;
        }

        public float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node? ignoredDefaultTarget,
            float ignoredDuration) {
            var delayEndTime = _delay + initialDelay;
            Logger.Info("Adding a delay of " + _delay + "s. Scheduled from " + initialDelay.ToString("F") + " to " +
                        delayEndTime.ToString("F"));
            return _delay;
        }
    }

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

    public abstract class PropertyTweener {
        protected static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener));
    }
    
    public abstract class PropertyTweener<TProperty> : PropertyTweener, ITweener {
        protected readonly IProperty<TProperty> Property;
        protected readonly Easing? DefaultEasing;


        protected Func<Node, TProperty>? FromFunction;
        protected bool RelativeToFrom = false;

        protected List<DebugStep<TProperty>>? DebugSteps = null;

        internal PropertyTweener(IProperty<TProperty> property, Easing? defaultEasing) {
            Property = property;
            DefaultEasing = defaultEasing;
        }

        public abstract float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target, float duration);

        protected bool Validate(int count, Node target, IProperty<TProperty> property) {
            if (count == 0) {
                throw new InvalidDataException("Cant' start an animation with 0 steps or keys");
            }
            if (!(property is CallbackProperty<TProperty>)) {
                // Callbacks properties don't need target
                if (target == null) {
                    throw new InvalidDataException("No target defined for the animation");
                }
                if (!Object.IsInstanceValid(target)) {
                    Logger.Warning("Can't start " + GetType() + " using a freed target instance");
                    return false;
                }
            }
            if (property == null) {
                throw new InvalidDataException("No property defined for the animation");
            }
            return true;
        }

        protected void RunStep(SceneTreeTween sceneTreeTween, AnimationContext<TProperty> context,
            IProperty<TProperty> property,
            TProperty from, TProperty to, float start, float duration, Easing? easing) {
            easing ??= DefaultEasing ?? Easing.LinearInOut;
            var end = start + duration;
            var target = context.Target;
            Logger.Info("\"" + target?.Name + "\" " + target?.GetType().Name + "." + property + ": " +
                        from + " to " + to +
                        " Scheduled from " + start.ToString("F") +
                        " to " + end.ToString("F") +
                        " (+" + duration.ToString("F") + ") " + easing.Name);

            if (easing is GodotEasing godotEasing) {
                RunEasingStep(sceneTreeTween, context, property, from, to, start, duration, godotEasing);
            } else if (easing is BezierCurve bezierCurve) {
                RunCurveBezierStep(sceneTreeTween, context, property, from, to, start, duration, easing);
            }
            if (DebugSteps != null) {
                DebugSteps.Add(new DebugStep<TProperty>(target, from, to, start, duration, easing));
            }
        }

        private static void RunCurveBezierStep(SceneTreeTween sceneTreeTween, AnimationContext<TProperty> context,
            IProperty<TProperty> property,
            TProperty from, TProperty to, float start, float duration, Easing bezierCurve) {

            Action<float> action = linearY => {
                var curveY = bezierCurve.GetY(linearY);
                var value = (TProperty)VariantHelper.LerpVariant(from, to, curveY);
                // Logger.Debug(target.Name + "." + property + ": " + typeof(TProperty).Name + " t:" + value + " y:" + value);
                // Console.WriteLine($"Play  From/To: {from}/{to} | Delta:+{(float)x.ElapsedMilliseconds/1000:0.0000} From/To: 0.00/{duration:0.00} (duration: {duration:0.00} Time:{((float)x2.ElapsedMilliseconds)/1000:0.0000} | t:{linearY:0.0000} y:{curveY:0000} Value: {value}");
                // TODO: there are no tests with bezier curves. No need to test the curve, need to test if the value is set
                context.Value = value;
                property.SetValue(context);
            };
            var methodTweener = sceneTreeTween
                .Parallel()
                .TweenInterpolateAction(0f, 1f, duration, action)
                .SetTrans(Tween.TransitionType.Linear)
                .SetEase(Tween.EaseType.InOut)
                .SetDelay(start);
        }

        private static void RunEasingStep(SceneTreeTween sceneTreeTween, AnimationContext<TProperty> context,
            IProperty<TProperty> property,
            TProperty from, TProperty to, float start, float duration, GodotEasing godotEasing) {
            var target = context.Target;
            if (property is IIndexedProperty<TProperty> basicProperty) {
                var propertyTweener = sceneTreeTween
                    .Parallel()
                    .TweenProperty(target, basicProperty.GetIndexedPropertyName(target), to, duration)
                    .From(@from)
                    .SetTrans(godotEasing.TransitionType)
                    .SetEase(godotEasing.EaseType)
                    .SetDelay(start);
            } else {
                Action<TProperty> action = value => {
                    // Logger.Debug(target.Name + "." + property + ": " + typeof(TProperty).Name + " t:" + value + " y:" + value);
                    context.Value = value;
                    property.SetValue(context);
                };
                var methodTweener = sceneTreeTween
                    .Parallel()
                    .TweenInterpolateAction(@from, to, duration, action)
                    .SetTrans(godotEasing.TransitionType)
                    .SetEase(godotEasing.EaseType)
                    .SetDelay(start);
            }
        }
    }

    public class PropertyKeyStepTweener<TProperty> : PropertyTweener<TProperty> {
        protected readonly ICollection<AnimationKeyStep<TProperty>> _steps =
            new List<AnimationKeyStep<TProperty>>();

        public List<AnimationKeyStep<TProperty>> CreateStepList() => new List<AnimationKeyStep<TProperty>>(_steps);

        internal PropertyKeyStepTweener(IProperty<TProperty> property, Easing? defaultEasing) :
            base(property, defaultEasing) {
        }

        public override float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target, float duration) {
            if (!Validate(_steps.Count, target, Property)) return 0;
            // TODO: duration is ignored. It should be % or absolute or nothing
            TProperty initialValue = Property.IsCompatibleWith(target) ? Property.GetValue(target) : default;
            var from = FromFunction != null ? FromFunction(target) : initialValue;
            var initialFrom = from;
            var startTime = 0f;
            AnimationContext<TProperty> context = new AnimationContext<TProperty>(target, initialValue, duration);
            // var totalDuration = _steps.Sum(step => step.Duration);
            foreach (var step in _steps) {
                var to = step.GetTo(target, RelativeToFrom ? initialFrom : from);
                var durationStep = step.Duration;
                // var percentStart = startTime / totalDuration;
                // var percentEnd = (startTime + duration) / totalDuration;
                var start = initialDelay + startTime;
                if (durationStep > 0 && !from.Equals(to) && Property.IsCompatibleWith(target)) {
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

    public class PropertyKeyPercentTweener<TProperty> : PropertyTweener<TProperty> {
        protected readonly ICollection<AnimationKeyPercent<TProperty>> Steps =
            new List<AnimationKeyPercent<TProperty>>();

        public List<AnimationKeyPercent<TProperty>> CreateStepList() =>
            new List<AnimationKeyPercent<TProperty>>(Steps);

        public float AllStepsDuration = 0;

        internal PropertyKeyPercentTweener(IProperty<TProperty> property, Easing? defaultEasing) :
            base(property, defaultEasing) {
        }

        public override float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target, float duration) {
            var allStepsDuration = duration > 0 ? duration : AllStepsDuration;
            if (allStepsDuration <= 0)
                throw new Exception("Keyframe animation duration should be more than 0");

            if (!Validate(Steps.Count, target, Property)) return 0;
            var initialValue = Property.IsCompatibleWith(target) ? Property.GetValue(target) : default;
            var from = FromFunction != null ? FromFunction(target) : initialValue;
            var initialFrom = from;
            var startTime = 0f;
            var percentStart = 0f;
            var i = 0;
            AnimationContext<TProperty> context = new AnimationContext<TProperty>(target, initialValue, duration);
            foreach (var step in Steps) {
                var to = step.GetTo(target, RelativeToFrom ? initialFrom : from);
                var endTime = step.Percent * allStepsDuration;
                var keyDuration = endTime - startTime;
                // var percentEnd = step.Percent;
                var start = initialDelay + startTime;
                if ((i == 0 || (keyDuration > 0 && !from.Equals(to))) && Property.IsCompatibleWith(target)) {
                    // always run the first keyframe, no matter if it's the 0% or any other
                    if (step.Percent == 0f) {
                        // That means a 0s duration, so, it works like a set variable, no need to Lerp from..to
                        from = to;
                    }
                    RunStep(sceneTreeTween, context, Property, from, to, start, keyDuration, step.Easing);
                }
                if (step.CallbackNode != null) {
                    var callbackTweener = sceneTreeTween
                        .Parallel()
                        .TweenCallbackAction(() => step.CallbackNode(target))
                        .SetDelay(start);
                }
                from = to;
                // percentStart = percentEnd;
                startTime = endTime;
                i++;
            }
            return allStepsDuration;
        }
    }

    public class PropertyKeyStepToBuilder<TProperty, TBuilder> : PropertyKeyStepTweener<TProperty>
        where TBuilder : class {
        private readonly AbstractSequenceBuilder<TBuilder> _abstractSequenceBuilder;

        internal PropertyKeyStepToBuilder(AbstractSequenceBuilder<TBuilder> abstractSequenceBuilder,
            IProperty<TProperty> property, Easing? defaultEasing) :
            base(property, defaultEasing) {
            _abstractSequenceBuilder = abstractSequenceBuilder;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> To(TProperty to, float duration,
            Easing? easing = null, Action<Node>? callbackNode = null) {
            return To(_ => to, duration, easing, callbackNode);
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> To(Func<Node, TProperty> to, float duration,
            Easing? easing = null, Action<Node>? callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyStepTo<TProperty>(to, duration, easing, callbackNode);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> SetDebugSteps(List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public TBuilder EndAnimate() {
            if (_steps == null || _steps.Count == 0) {
                throw new InvalidDataException("Animation without steps");
            }
            return _abstractSequenceBuilder as TBuilder;
        }
    }

    public class PropertyKeyStepOffsetBuilder<TProperty, TBuilder> : PropertyKeyStepTweener<TProperty>
        where TBuilder : class {
        private readonly AbstractSequenceBuilder<TBuilder> _abstractSequenceBuilder;

        internal PropertyKeyStepOffsetBuilder(AbstractSequenceBuilder<TBuilder> abstractSequenceBuilder,
            IProperty<TProperty> property, Easing? defaultEasing, bool relativeToFrom) :
            base(property, defaultEasing) {
            _abstractSequenceBuilder = abstractSequenceBuilder;
            RelativeToFrom = relativeToFrom;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> Offset(TProperty offset, float duration,
            Easing? easing = null, Action<Node>? callbackNode = null) {
            return Offset(_ => offset, duration, easing, callbackNode);
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> Offset(Func<Node, TProperty> offset, float duration,
            Easing? easing = null, Action<Node>? callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyStepOffset<TProperty>(offset, duration, easing, callbackNode);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> SetDebugSteps(List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public TBuilder EndAnimate() {
            return _abstractSequenceBuilder as TBuilder;
        }
    }

    public class PropertyKeyPercentToBuilder<TProperty, TBuilder> : PropertyKeyPercentTweener<TProperty>
        where TBuilder : class {
        private readonly AbstractSequenceBuilder<TBuilder> _abstractSequenceBuilder;

        internal PropertyKeyPercentToBuilder(AbstractSequenceBuilder<TBuilder> abstractSequenceBuilder,
            IProperty<TProperty> property, Easing defaultEasing) :
            base(property, defaultEasing) {
            _abstractSequenceBuilder = abstractSequenceBuilder;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> Duration(float duration) {
            AllStepsDuration = duration;
            return this;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> KeyframeTo(float percentage, TProperty to,
            Easing? easing = null, Action<Node>? callbackNode = null) {
            return KeyframeTo(percentage, _ => to, easing, callbackNode);
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> KeyframeTo(float percentage, Func<Node, TProperty> to,
            Easing? easing = null, Action<Node>? callbackNode = null) {
            if (percentage == 0f) {
                From(to);
            }
            var animationStepPropertyTweener =
                new AnimationKeyPercentTo<TProperty>(percentage, to, easing, callbackNode);
            Steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> SetDebugSteps(
            List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public TBuilder EndAnimate() {
            return _abstractSequenceBuilder as TBuilder;
        }
    }

    public class PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> : PropertyKeyPercentTweener<TProperty>
        where TBuilder : class {
        private readonly AbstractSequenceBuilder<TBuilder> _abstractSequenceBuilder;

        internal PropertyKeyPercentOffsetBuilder(AbstractSequenceBuilder<TBuilder> abstractSequenceBuilder,
            IProperty<TProperty> property, Easing? defaultEasing, bool relativeToFrom) :
            base(property, defaultEasing) {
            _abstractSequenceBuilder = abstractSequenceBuilder;
            RelativeToFrom = relativeToFrom;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> Duration(float duration) {
            AllStepsDuration = duration;
            return this;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> KeyframeOffset(float percentage, TProperty offset,
            Easing? easing = null, Action<Node>? callbackNode = null) {
            return KeyframeOffset(percentage, _ => offset, easing, callbackNode);
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> KeyframeOffset(float percentage,
            Func<Node, TProperty> offset,
            Easing? easing = null, Action<Node>? callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyPercentOffset<TProperty>(percentage, offset, easing, callbackNode);
            Steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> SetDebugSteps(
            List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public TBuilder EndAnimate() {
            return _abstractSequenceBuilder as TBuilder;
        }
    }

    public class DebugStep<TProperty> {
        public readonly Node Target;
        public readonly TProperty From;
        public readonly TProperty To;
        public readonly float Start;
        public readonly float Duration;
        public readonly Easing Easing;

        public DebugStep(Node target, TProperty from, TProperty to, float start, float duration, Easing easing) {
            Target = target;
            From = from;
            To = to;
            Start = start;
            Duration = duration;
            Easing = easing;
        }
    }
}