using System;
using System.Collections.Generic;
using System.IO;
using Betauer.Animation.Easing;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation.Tween {
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
            Logger.Info("Adding a delay of " + _delay + "s. Scheduled from " + initialDelay.ToString("F") + "s to " +
                        delayEndTime.ToString("F")+"s");
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
        protected readonly IEasing? DefaultEasing;


        protected Func<Node, TProperty>? FromFunction;
        protected bool RelativeToFrom = false;

        protected List<DebugStep<TProperty>>? DebugSteps = null;

        internal PropertyTweener(IProperty<TProperty> property, IEasing? defaultEasing) {
            Property = property;
            DefaultEasing = defaultEasing;
        }

        public abstract float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target, float duration);

        protected bool Validate(int count, Node target, IProperty<TProperty> property) {
            if (count == 0) throw new Exception("Can't start an animation with 0 steps or keys");
            if (!(property is CallbackProperty<TProperty>)) {
                // Callbacks properties don't need target
                if (target == null) {
                    throw new Exception("No target defined for the animation");
                }
                if (!Object.IsInstanceValid(target)) {
                    Logger.Warning($"Can't start {GetType()} using a freed target instance");
                    return false;
                }
                if (!Property.IsCompatibleWith(target)) {
                    throw new Exception($"Property {Property.GetType()} is not compatible with target type {target.GetType().Name}");
                }
            }
            if (property == null) {
                throw new InvalidDataException("No property defined for the animation");
            }
            return true;
        }

        protected Tweener RunStep(SceneTreeTween sceneTreeTween, AnimationContext<TProperty> context,
            IProperty<TProperty> property,
            TProperty from, TProperty to, float start, float duration, IEasing? easing) {
            easing ??= DefaultEasing ?? Easings.Linear;
            var end = start + duration;
            var target = context.Target;
            Logger.Info("\"" + target?.Name + "\" " + target?.GetType().Name + ":" + property.GetPropertyName(target) + " Interpolate(" +
                        from + ", " + to +
                        ") Scheduled from " + start.ToString("F") +
                        "s to " + end.ToString("F") +
                        "s (+" + duration.ToString("F") + "s) " + easing.Name);

            DebugSteps?.Add(new DebugStep<TProperty>(target, from, to, start, duration, easing));

            if (easing is BezierCurve bezierCurve) {
                return RunCurveBezierStep(sceneTreeTween, context, property, from, to, start, duration, bezierCurve);
            }
            return RunEasingStep(sceneTreeTween, context, property, from, to, start, duration, (GodotEasing)easing);
        }

        private static Tweener RunCurveBezierStep(SceneTreeTween sceneTreeTween, AnimationContext<TProperty> context,
            IProperty<TProperty> property,
            TProperty from, TProperty to, float start, float duration, BezierCurve bezierCurve) {

            Action<float> action = linearT => {
                var curveY = bezierCurve.GetY(linearT);
                var value = (TProperty)VariantHelper.LerpVariant(from, to, curveY);
                // Logger.Info("\"" + context.Target.Name + "\" " + context.Target.GetType().Name + "." + property.GetPropertyName(context.Target) + ": "
                            // + " Bezier(" + linearT + ")=" + curveY + " value:" + value);
                // Console.WriteLine($"Play  From/To: {from}/{to} | Delta:+{(float)x.ElapsedMilliseconds/1000:0.0000} From/To: 0.00/{duration:0.00} (duration: {duration:0.00} Time:{((float)x2.ElapsedMilliseconds)/1000:0.0000} | t:{linearY:0.0000} y:{curveY:0000} Value: {value}");
                // TODO: there are no tests with bezier curves. No need to test the curve, need to test if the value is set
                context.Value = value;
                property.SetValue(context);
            };
            return sceneTreeTween
                .Parallel()
                .TweenInterpolateAction(0f, 1f, duration, action)
                .SetTrans(Godot.Tween.TransitionType.Linear)
                .SetEase(Godot.Tween.EaseType.InOut)
                .SetDelay(start);
        }

        private static Tweener RunEasingStep(SceneTreeTween sceneTreeTween, AnimationContext<TProperty> context,
            IProperty<TProperty> property,
            TProperty from, TProperty to, float start, float duration, GodotEasing godotEasing) {
            var target = context.Target;
            if (property is IIndexedProperty<TProperty> basicProperty) {
                return sceneTreeTween
                    .Parallel()
                    .TweenProperty(target, basicProperty.GetIndexedPropertyName(target), to, duration)
                    .From(@from)
                    .SetTrans(godotEasing.TransitionType)
                    .SetEase(godotEasing.EaseType)
                    .SetDelay(start);
            } else {
                Action<TProperty> action = value => {
                    // Logger.Info("\"" + context.Target.Name + "\" " + context.Target.GetType().Name + "." + property.GetPropertyName(target) + ": " 
                                // + " value:" + value+"");
                    context.Value = value;
                    property.SetValue(context);
                };
                return sceneTreeTween
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

        internal PropertyKeyStepTweener(IProperty<TProperty> property, IEasing? defaultEasing) :
            base(property, defaultEasing) {
        }

        public override float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target, float duration) {
            if (!Validate(_steps.Count, target, Property)) return 0;
            // TODO: duration is ignored. It should be % or absolute or nothing
            TProperty initialValue = Property.GetValue(target);
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

    public class PropertyKeyPercentTweener<TProperty> : PropertyTweener<TProperty> {
        protected readonly ICollection<AnimationKeyPercent<TProperty>> Steps =
            new List<AnimationKeyPercent<TProperty>>();

        public List<AnimationKeyPercent<TProperty>> CreateStepList() =>
            new List<AnimationKeyPercent<TProperty>>(Steps);

        public float AllStepsDuration = 0;

        internal PropertyKeyPercentTweener(IProperty<TProperty> property, IEasing? defaultEasing) :
            base(property, defaultEasing) {
        }

        public override float Start(SceneTreeTween sceneTreeTween, float initialDelay, Node target, float duration) {
            var allStepsDuration = duration > 0 ? duration : AllStepsDuration;
            if (allStepsDuration <= 0)
                throw new Exception("Keyframe animation duration should be more than 0");

            if (!Validate(Steps.Count, target, Property)) return 0;
            var initialValue = Property.GetValue(target);
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
                if ((i == 0 || (keyDuration > 0 && !from.Equals(to)))) {
                    // always run the first keyframe, no matter if it's the 0% or any other
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
                // percentStart = percentEnd;
                startTime = endTime;
                i++;
            }
            return allStepsDuration;
        }
    }

    public class PropertyKeyStepToBuilder<TProperty> : PropertyKeyStepTweener<TProperty> {
    private readonly Sequence _sequence;

        internal PropertyKeyStepToBuilder(Sequence sequence, IProperty<TProperty> property, IEasing? defaultEasing) :
            base(property, defaultEasing) {
            _sequence = sequence;
        }

        public PropertyKeyStepToBuilder<TProperty> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyKeyStepToBuilder<TProperty> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyKeyStepToBuilder<TProperty> To(TProperty to, float duration,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            return To(_ => to, duration, easing, callbackNode);
        }

        public PropertyKeyStepToBuilder<TProperty> To(Func<Node, TProperty> to, float duration,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyStepTo<TProperty>(to, duration, easing, callbackNode);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyStepToBuilder<TProperty> SetDebugSteps(List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public Sequence EndAnimate() {
            if (_steps == null || _steps.Count == 0) {
                throw new Exception("Animation without steps");
            }
            return _sequence;
        }
    }

    public class PropertyKeyStepOffsetBuilder<TProperty> : PropertyKeyStepTweener<TProperty> {
        private readonly Sequence _abstractSequenceBuilder;

        internal PropertyKeyStepOffsetBuilder(Sequence abstractSequenceBuilder,
            IProperty<TProperty> property, IEasing? defaultEasing, bool relativeToFrom) :
            base(property, defaultEasing) {
            _abstractSequenceBuilder = abstractSequenceBuilder;
            RelativeToFrom = relativeToFrom;
        }

        public PropertyKeyStepOffsetBuilder<TProperty> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyKeyStepOffsetBuilder<TProperty> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyKeyStepOffsetBuilder<TProperty> Offset(TProperty offset, float duration,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            return Offset(_ => offset, duration, easing, callbackNode);
        }

        public PropertyKeyStepOffsetBuilder<TProperty> Offset(Func<Node, TProperty> offset, float duration,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyStepOffset<TProperty>(offset, duration, easing, callbackNode);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyStepOffsetBuilder<TProperty> SetDebugSteps(List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public Sequence EndAnimate() {
            return _abstractSequenceBuilder;
        }
    }

    public class PropertyKeyPercentToBuilder<TProperty> : PropertyKeyPercentTweener<TProperty>
        {
        private readonly Sequence _abstractSequenceBuilder;

        internal PropertyKeyPercentToBuilder(Sequence abstractSequenceBuilder,
            IProperty<TProperty> property, IEasing defaultEasing) :
            base(property, defaultEasing) {
            _abstractSequenceBuilder = abstractSequenceBuilder;
        }

        public PropertyKeyPercentToBuilder<TProperty> Duration(float duration) {
            AllStepsDuration = duration;
            return this;
        }

        public PropertyKeyPercentToBuilder<TProperty> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyKeyPercentToBuilder<TProperty> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyKeyPercentToBuilder<TProperty> KeyframeTo(float percentage, TProperty to,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            return KeyframeTo(percentage, _ => to, easing, callbackNode);
        }

        public PropertyKeyPercentToBuilder<TProperty> KeyframeTo(float percentage, Func<Node, TProperty> to,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            if (percentage == 0f) {
                From(to);
            }
            var animationStepPropertyTweener =
                new AnimationKeyPercentTo<TProperty>(percentage, to, easing, callbackNode);
            Steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyPercentToBuilder<TProperty> SetDebugSteps(
            List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public Sequence EndAnimate() {
            return _abstractSequenceBuilder;
        }
    }

    public class PropertyKeyPercentOffsetBuilder<TProperty> : PropertyKeyPercentTweener<TProperty>
        {
        private readonly Sequence _abstractSequenceBuilder;

        internal PropertyKeyPercentOffsetBuilder(Sequence abstractSequenceBuilder,
            IProperty<TProperty> property, IEasing? defaultEasing, bool relativeToFrom) :
            base(property, defaultEasing) {
            _abstractSequenceBuilder = abstractSequenceBuilder;
            RelativeToFrom = relativeToFrom;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty> Duration(float duration) {
            AllStepsDuration = duration;
            return this;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty> From(Func<Node, TProperty> fromFunction) {
            FromFunction = fromFunction;
            return this;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty> From(TProperty from) {
            FromFunction = node => from;
            return this;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty> KeyframeOffset(float percentage, TProperty offset,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            return KeyframeOffset(percentage, _ => offset, easing, callbackNode);
        }

        public PropertyKeyPercentOffsetBuilder<TProperty> KeyframeOffset(float percentage,
            Func<Node, TProperty> offset,
            IEasing? easing = null, Action<Node>? callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyPercentOffset<TProperty>(percentage, offset, easing, callbackNode);
            Steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty> SetDebugSteps(
            List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public Sequence EndAnimate() {
            return _abstractSequenceBuilder;
        }
    }

    public class DebugStep<TProperty> {
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