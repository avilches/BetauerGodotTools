using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    public interface ITweener {
        float Start(Tween tween, float initialDelay, Node defaultTarget, float duration);
        public abstract Node Target { get; }
    }

    public delegate void CallbackNode(Node node);

    internal class TweenPropertyMethodHolder<TProperty> : Object {
        public delegate void PropertyMethodCallback(TProperty value);

        private readonly PropertyMethodCallback _callback;

        public TweenPropertyMethodHolder(PropertyMethodCallback callback) {
            _callback = callback;
        }

        internal void Call(TProperty value) {
            _callback.Invoke(value);
        }
    }

    internal class DelayedCallbackNodeHolder : Object {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));
        private readonly CallbackNode _stepCallbackNode;
        private readonly Node _node;

        internal DelayedCallbackNodeHolder(CallbackNode callbackNode, Node node) {
            _stepCallbackNode = callbackNode;
            _node = node;
        }

        internal void Start(Tween tween, float start) {
            if (!IsInstanceValid(_node)) {
                Logger.Warning("Can't create a Delayed Callback from a freed target instance");
                return;
            }
            tween.InterpolateCallback(this, start, nameof(Call));
        }

        internal void Call() {
            _stepCallbackNode?.Invoke(_node);
        }
    }

    internal class CallbackTweener : Object, ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));
        private readonly Action _callback;
        private readonly float _delay;
        public Node Target { get; } // Not needed, not used!

        internal CallbackTweener(float delay, Action callback) {
            _delay = delay;
            _callback = callback;
        }

        public float Start(Tween tween, float initialDelay, Node ignoredDefaultTarget, float ignoredDuration) {
            if (!IsInstanceValid(tween)) {
                Logger.Warning("Can't create a CallbackTweener from a freed tween instance");
                return 0;
            }
            var start = _delay + initialDelay;
            Logger.Info("Adding anonymous callback with " + _delay + "s delay. Scheduled: " + start.ToString("F"));
            tween.InterpolateCallback(this, start, nameof(Call));
            return _delay;
        }

        internal void Call() {
            _callback?.Invoke();
        }
    }

    internal class MethodCallbackTweener : Object, ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));
        private readonly string _methodName;
        private readonly float _delay;
        private readonly object _p1;
        private readonly object _p2;
        private readonly object _p3;
        private readonly object _p4;
        private readonly object _p5;

        public Node Target { get; }

        public MethodCallbackTweener(float delay, Node target, string methodName, object p1 = null, object p2 = null,
            object p3 = null,
            object p4 = null, object p5 = null) {
            Target = target;
            _methodName = methodName;
            _delay = delay;
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;
            _p4 = p4;
            _p5 = p5;
        }

        public float Start(Tween tween, float initialDelay, Node defaultTarget, float ignoredDuration) {
            if (!IsInstanceValid(tween)) {
                Logger.Warning("Can't create a MethodCallbackTweener from a freed tween instance");
                return 0;
            }
            var start = _delay + initialDelay;
            Logger.Info("Adding method callback " + _methodName + "with " + _delay + "s delay. Scheduled: " +
                        start.ToString("F"));
            tween.InterpolateCallback(Target ?? defaultTarget, start, _methodName, _p1, _p2, _p3, _p4, _p5);
            return _delay;
        }
    }

    internal class PauseTweener : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));
        private readonly float _delay;
        public Node Target { get; } // Not needed, not used!

        internal PauseTweener(float delay) {
            _delay = delay;
        }

        public float Start(Tween tween, float initialDelay, Node ignoredDefaultTarget, float ignoredDuration) {
            var delayEndTime = _delay + initialDelay;
            Logger.Info("Adding a delay of " + _delay + "s. Scheduled from " + initialDelay.ToString("F") + " to " +
                        delayEndTime.ToString("F"));
            return _delay;
        }
    }

    public abstract class PropertyTweener<TProperty> : ITweener {
        protected static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));

        public Node Target { get; }
        protected readonly IProperty<TProperty> Property;
        protected readonly Easing DefaultEasing;


        protected Func<Node, TProperty> FromFunction;
        protected bool RelativeToFrom = false;

        public List<DebugStep<TProperty>> DebugSteps = null;

        internal PropertyTweener(Node target, IProperty<TProperty> property, Easing defaultEasing) {
            Target = target;
            Property = property;
            DefaultEasing = defaultEasing;
        }

        public abstract float Start(Tween tween, float initialDelay, Node defaultTarget, float duration);

        protected bool Validate(int count, Node target, IProperty<TProperty> property) {
            if (count == 0) {
                throw new InvalidDataException("Cant' start an animation with 0 steps or keys");
            }
            if (target == null) {
                throw new InvalidDataException("No target defined for the animation");
            }
            if (property == null) {
                throw new InvalidDataException("No property defined for the animation");
            }
            if (!Object.IsInstanceValid(target)) {
                Logger.Warning("Can't create InterpolateProperty in a freed target instance");
                return false;
            }
            return true;
        }

        protected void RunStep(Tween tween, Node target, IProperty<TProperty> property, TProperty initial,
            TProperty from, TProperty to, float start, float duration, Easing easing) {
            easing ??= DefaultEasing ?? Easing.LinearInOut;
            var end = start + duration;
            Logger.Info("\"" + target.Name + "\" " + target.GetType().Name + "." + property + ": " +
                        from + " to " + to +
                        " Scheduled from " + start.ToString("F") +
                        " to " + end.ToString("F") +
                        " (+" + duration.ToString("F") + ") " + easing.Name);

            if (easing is GodotEasing godotEasing) {
                RunEasingStep(tween, target, property, initial, from, to, start, duration, godotEasing);
            } else if (easing is BezierCurve bezierCurve) {
                RunCurveBezierStep(tween, target, property, initial, from, to, start, duration, bezierCurve);
            }
            if (DebugSteps != null) {
                DebugSteps.Add(new DebugStep<TProperty>(target, from, to, start, duration, easing));
            }
        }

        private static void RunCurveBezierStep(Tween tween, Node target, IProperty<TProperty> property,
            TProperty initial,
            TProperty from, TProperty to, float start, float duration, BezierCurve bezierCurve) {
            TweenPropertyMethodHolder<float> tweenPropertyMethodHolder = new TweenPropertyMethodHolder<float>(
                delegate(float linearY) {
                    var curveY = bezierCurve.GetY(linearY);
                    var value = (TProperty)GodotTools.LerpVariant(@from, to, curveY);
                    // Logger.Debug(target.Name + "." + property + ": " + typeof(TProperty).Name + " t:" + value + " y:" + value);
                    property.SetValue(target, initial, value);
                });
            tween.InterpolateMethod(tweenPropertyMethodHolder, nameof(TweenPropertyMethodHolder<TProperty>.Call),
                0f, 1f, duration, Tween.TransitionType.Linear, Tween.EaseType.InOut, start);
        }

        private static void RunEasingStep(Tween tween, Node target, IProperty<TProperty> property, TProperty initial,
            TProperty from, TProperty to, float start, float duration, GodotEasing godotEasing) {
            if (property is IIndexedProperty<TProperty> basicProperty) {
                tween.InterpolateProperty(target, basicProperty.GetIndexedProperty(target), @from, to, duration,
                    godotEasing.TransitionType, godotEasing.EaseType, start);
            } else {
                TweenPropertyMethodHolder<TProperty> tweenPropertyMethodHolder =
                    new TweenPropertyMethodHolder<TProperty>(
                        delegate(TProperty value) {
                            // Logger.Debug(target.Name + "." + property + ": " + typeof(TProperty).Name + " t:" + value + " y:" + value);
                            property.SetValue(target, initial, value);
                        });
                tween.InterpolateMethod(tweenPropertyMethodHolder, nameof(TweenPropertyMethodHolder<TProperty>.Call),
                    @from, to, duration, godotEasing.TransitionType, godotEasing.EaseType, start);
            }
        }
    }

    public class PropertyKeyStepTweener<TProperty> : PropertyTweener<TProperty> {
        protected readonly ICollection<AnimationKeyStep<TProperty>> _steps =
            new SimpleLinkedList<AnimationKeyStep<TProperty>>();

        public List<AnimationKeyStep<TProperty>> CreateStepList() => new List<AnimationKeyStep<TProperty>>(_steps);

        internal PropertyKeyStepTweener(Node target, IProperty<TProperty> property, Easing defaultEasing) :
            base(target, property, defaultEasing) {
        }

        public override float Start(Tween tween, float initialDelay, Node defaultTarget, float duration) {
            var target = Target ?? defaultTarget;
            if (!Validate(_steps.Count, target, Property)) return 0;
            // TODO: duration is ignored. It should be % or absolute or nothing
            var initialValue = Property.IsCompatibleWith(target) ? Property.GetValue(target) : default;
            var from = FromFunction != null ? FromFunction(target) : initialValue;
            var initialFrom = from;
            var startTime = 0f;
            // var totalDuration = _steps.Sum(step => step.Duration);
            foreach (var step in _steps) {
                var to = step.GetTo(target, RelativeToFrom ? initialFrom : from);
                var durationStep = step.Duration;
                // var percentStart = startTime / totalDuration;
                // var percentEnd = (startTime + duration) / totalDuration;
                var start = initialDelay + startTime;
                if (durationStep > 0 && !from.Equals(to) && Property.IsCompatibleWith(target)) {
                    RunStep(tween, target, Property, initialValue, from, to, start, durationStep, step.Easing);
                }
                if (step.CallbackNode != null) {
                    // TODO: no reference at all to this holder... could be disposed by GC before it's executed?
                    new DelayedCallbackNodeHolder(step.CallbackNode, target).Start(tween, start);
                }
                from = to;
                startTime += durationStep;
            }
            return startTime;
        }
    }

    public class PropertyKeyPercentTweener<TProperty> : PropertyTweener<TProperty> {
        protected readonly ICollection<AnimationKeyPercent<TProperty>> Steps =
            new SimpleLinkedList<AnimationKeyPercent<TProperty>>();

        public List<AnimationKeyPercent<TProperty>> CreateStepList() =>
            new List<AnimationKeyPercent<TProperty>>(Steps);

        public float AllStepsDuration = 0;

        internal PropertyKeyPercentTweener(Node target, IProperty<TProperty> property, Easing defaultEasing) :
            base(target, property, defaultEasing) {
        }

        public override float Start(Tween tween, float initialDelay, Node defaultTarget, float duration) {
            var target = Target ?? defaultTarget;
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
                    RunStep(tween, target, Property, initialValue, from, to, start, keyDuration, step.Easing);
                }
                if (step.CallbackNode != null) {
                    // TODO: no reference at all to this holder... could be disposed by GC before it's executed?
                    new DelayedCallbackNodeHolder(step.CallbackNode, target).Start(tween, start);
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
            Node target, IProperty<TProperty> property, Easing defaultEasing) : base(target, property, defaultEasing) {
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
            Easing easing = null, CallbackNode callbackNode = null) {
            return To(_ => to, duration, easing, callbackNode);
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> To(Func<Node, TProperty> to, float duration,
            Easing easing = null, CallbackNode callbackNode = null) {
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
            Node defaultTarget, IProperty<TProperty> property, Easing defaultEasing, bool relativeToFrom) :
            base(defaultTarget, property, defaultEasing) {
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
            Easing easing = null, CallbackNode callbackNode = null) {
            return Offset(_ => offset, duration, easing, callbackNode);
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> Offset(Func<Node, TProperty> offset, float duration,
            Easing easing = null, CallbackNode callbackNode = null) {
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
            Node defaultTarget, IProperty<TProperty> property, Easing defaultEasing) :
            base(defaultTarget, property, defaultEasing) {
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
            Easing easing = null, CallbackNode callbackNode = null) {
            return KeyframeTo(percentage, _ => to, easing, callbackNode);
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> KeyframeTo(float percentage, Func<Node, TProperty> to,
            Easing easing = null, CallbackNode callbackNode = null) {
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
            Node defaultTarget, IProperty<TProperty> property, Easing defaultEasing, bool relativeToFrom) :
            base(defaultTarget, property, defaultEasing) {
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
            Easing easing = null, CallbackNode callbackNode = null) {
            return KeyframeOffset(percentage, _ => offset, easing, callbackNode);
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> KeyframeOffset(float percentage,
            Func<Node, TProperty> offset,
            Easing easing = null, CallbackNode callbackNode = null) {
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