using System;
using System.Collections.Generic;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    public interface ITweener {
        float Start(Tween tween, float initialDelay, Node defaultTarget, IProperty defaultProperty,
            float sequenceDuration);
    }

    public interface ITweener<TProperty> {
        public float Start(Tween tween, float initialDelay, Node defaultTarget, IProperty<TProperty> defaultProperty,
            float defaultDuration);
    }

    public abstract class TweenerAdapter<TProperty> : ITweener, ITweener<TProperty> {
        public float Start(Tween tween, float initialDelay, Node defaultTarget, IProperty defaultProperty,
            float sequenceDuration) {
            return Start(tween, initialDelay, defaultTarget, (IProperty<TProperty>)defaultProperty, sequenceDuration);
        }

        public abstract float Start(Tween tween, float initialDelay, Node defaultTarget,
            IProperty<TProperty> defaultProperty, float defaultDuration);
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
        public readonly string Name;

        internal CallbackTweener(float delay, Action callback, string name = null) {
            _delay = delay;
            _callback = callback;
            Name = name;
        }

        public float Start(Tween tween, float initialDelay, Node ignoredDefaultTarget,
            IProperty ignoredDefaultProperty, float ignoredDefaultDuration) {
            return Start(tween, initialDelay);
        }


        public float Start(Tween tween, float initialDelay) {
            if (!IsInstanceValid(tween)) {
                Logger.Warning("Can't create a CallbackTweener from a freed tween instance");
                return 0;
            }
            var start = _delay + initialDelay;
            var name = Name != null ? Name + " " : "";
            Logger.Info("Adding callback " + name + "with " + _delay + "s delay. Scheduled: " + start.ToString("F"));
            tween.InterpolateCallback(this, start, nameof(Call));
            return _delay;
        }

        internal void Call() {
            _callback?.Invoke();
        }
    }

    internal class PauseTweener : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));
        private readonly float _delay;

        internal PauseTweener(float delay) {
            _delay = delay;
        }

        public float Start(Tween tween, float initialDelay,
            Node ignoredDefaultTarget, IProperty ignoredDefaultProperty, float ignoredDefaultDuration) {
            var delayEndTime = _delay + initialDelay;
            Logger.Info("Adding a delay of " + _delay + "s. Scheduled from " + initialDelay.ToString("F") + " to " +
                        delayEndTime.ToString("F"));
            return _delay;
        }
    }

    public class DebugStep<TProperty> {
        public readonly Node Node;
        public readonly TProperty From;
        public readonly TProperty To;
        public readonly float Start;
        public readonly float Duration;
        public readonly Easing Easing;
        public readonly CallbackNode CallbackNode;

        public DebugStep(Node node, TProperty from, TProperty to, float start, float duration, Easing easing,
            CallbackNode callbackNode) {
            Node = node;
            From = from;
            To = to;
            Start = start;
            Duration = duration;
            Easing = easing;
            CallbackNode = callbackNode;
        }
    }

    public abstract class PropertyTweener<TProperty> : TweenerAdapter<TProperty> {
        protected static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));

        protected readonly Node _target;
        protected readonly IProperty<TProperty> _defaultProperty;
        protected readonly Easing _defaultEasing;

        protected Func<Node, TProperty> _fromFunction;
        protected bool _relativeToFrom = false;

        public List<DebugStep<TProperty>> DebugSteps = null;

        internal PropertyTweener(Node target, IProperty<TProperty> defaultProperty, Easing defaultEasing) {
            _target = target;
            _defaultProperty = defaultProperty;
            _defaultEasing = defaultEasing;
        }

        protected TProperty GetFirstFromValue(Node target) {
            return _fromFunction != null ? _fromFunction(target) : _defaultProperty.GetValue(target);
        }

        protected bool Validate(int count, Node target, IProperty<TProperty> property) {
            if (count == 0) {
                throw new Exception("Cant' start an animation with 0 steps or keys");
            }
            if (target == null) {
                throw new Exception("No target defined for the tween");
            }
            if (property == null) {
                throw new Exception("No property defined for the tween");
            }
            if (!Object.IsInstanceValid(target)) {
                Logger.Warning("Can't create InterpolateProperty in a freed target instance");
                return false;
            }
            if (!property.IsCompatibleWith(target)) {
                Logger.Warning(
                    $"Can't create InterpolateProperty in a not compatible node {target.GetType()} with the property {property}");
                return false;
            }
            return true;
        }

        protected void RunStep(Tween tween, Node target, IProperty<TProperty> property,
            TProperty from, TProperty to, float start, float duration, Easing easing, CallbackNode callbackNode) {
            if (duration > 0 && !from.Equals(to)) {
                easing ??= Easing.LinearInOut;
                var end = start + duration;
                Logger.Info("\"" + target.Name + "\" " + target.GetType().Name + "." + property + ": " +
                            from + " to " + to +
                            " Scheduled from " + start.ToString("F") +
                            " to " + end.ToString("F") +
                            " (+" + duration.ToString("F") + ") " + easing.Name);

                if (easing is GodotEasing godotEasing) {
                    RunEasingStep(tween, target, property, @from, to, start, duration, godotEasing);
                } else if (easing is BezierCurve bezierCurve) {
                    RunCurveBezierStep(tween, target, property, @from, to, start, duration, bezierCurve);
                }
                if (DebugSteps != null) {
                    DebugSteps.Add(new DebugStep<TProperty>(target, from, to, start, duration, easing, callbackNode));
                }
            }
            if (callbackNode != null) {
                // TODO: no reference at all to this holder... could be disposed by GC before it's executed?
                new DelayedCallbackNodeHolder(callbackNode, target).Start(tween, start);
            }
        }

        private static void RunCurveBezierStep(Tween tween, Node target, IProperty<TProperty> property, TProperty @from,
            TProperty to, float start, float duration, BezierCurve bezierCurve) {
            TweenPropertyMethodHolder<float> tweenPropertyMethodHolder = new TweenPropertyMethodHolder<float>(
                delegate(float linearY) {
                    var curveY = bezierCurve.GetY(linearY);
                    var value = (TProperty)GodotTools.LerpVariant(@from, to, curveY);
                    // Logger.Debug(target.Name + "." + property + ": " + typeof(TProperty).Name + " t:" + value + " y:" + value);
                    property.SetValue(target, value);
                });
            tween.InterpolateMethod(tweenPropertyMethodHolder, nameof(TweenPropertyMethodHolder<TProperty>.Call),
                0f, 1f, duration, Tween.TransitionType.Linear, Tween.EaseType.InOut, start);
        }

        private static void RunEasingStep(Tween tween, Node target, IProperty<TProperty> property, TProperty @from,
            TProperty to, float start, float duration, GodotEasing godotEasing) {
            if (property is IIndexedProperty<TProperty> basicProperty) {
                tween.InterpolateProperty(target, basicProperty.GetIndexedProperty(target), @from, to, duration,
                    godotEasing.TransitionType, godotEasing.EaseType, start);
            } else {
                TweenPropertyMethodHolder<TProperty> tweenPropertyMethodHolder =
                    new TweenPropertyMethodHolder<TProperty>(
                        delegate(TProperty value) {
                            // Logger.Debug(target.Name + "." + property + ": " + typeof(TProperty).Name + " t:" + value + " y:" + value);
                            property.SetValue(target, value);
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

        internal PropertyKeyStepTweener(Node target, IProperty<TProperty> defaultProperty, Easing defaultEasing) :
            base(target, defaultProperty, defaultEasing) {
        }

        public override float Start(Tween tween, float initialDelay, Node defaultTarget,
            IProperty<TProperty> defaultProperty,
            float defaultDuration) {
            var target = _target ?? defaultTarget;
            var property = _defaultProperty ?? defaultProperty;
            if (!Validate(_steps.Count, target, property)) return 0;
            // TODO: defaultDuration could be % or absolute or nothing
            var from = GetFirstFromValue(target);
            var initialFrom = from;
            var startTime = 0f;
            // var totalDuration = _steps.Sum(step => step.Duration);
            foreach (var step in _steps) {
                var to = step.GetTo(_relativeToFrom ? initialFrom : from);
                var duration = step.Duration;
                // var percentStart = startTime / totalDuration;
                // var percentEnd = (startTime + duration) / totalDuration;
                var easing = step.Easing ?? _defaultEasing;
                RunStep(tween, target, property, from, to, initialDelay + startTime, duration, easing,
                    step.CallbackNode);
                from = to;
                startTime += duration;
            }
            return startTime;
        }
    }

    public class PropertyKeyPercentTweener<TProperty> : PropertyTweener<TProperty> {
        protected readonly ICollection<AnimationKeyPercent<TProperty>> _steps =
            new SimpleLinkedList<AnimationKeyPercent<TProperty>>();

        public List<AnimationKeyPercent<TProperty>> CreateStepList() =>
            new List<AnimationKeyPercent<TProperty>>(_steps);

        public float AllStepsDuration = 0;

        internal PropertyKeyPercentTweener(Node target, IProperty<TProperty> defaultProperty, Easing defaultEasing) :
            base(target, defaultProperty, defaultEasing) {
        }

        public override float Start(Tween tween, float initialDelay, Node defaultTarget,
            IProperty<TProperty> defaultProperty, float defaultDuration) {
            var allStepsDuration = AllStepsDuration > 0f ? AllStepsDuration : defaultDuration;
            if (allStepsDuration <= 0)
                throw new Exception("Keyframe animation duration should be more than 0");

            var target = _target ?? defaultTarget;
            var property = _defaultProperty ?? defaultProperty;
            if (!Validate(_steps.Count, target, property)) return 0;
            var from = GetFirstFromValue(target);
            var initialFrom = from;
            var startTime = 0f;
            // var percentStart = 0f;
            foreach (var step in _steps) {
                var to = step.GetTo(_relativeToFrom ? initialFrom : from);
                var endTime = step.Percent * allStepsDuration;
                var keyDuration = endTime - startTime;
                var easing = step.Easing ?? _defaultEasing;
                // var percentEnd = step.Percent;
                RunStep(tween, target, property, from, to, initialDelay + startTime, keyDuration, easing,
                    step.CallbackNode);
                from = to;
                // percentStart = percentEnd;
                startTime = endTime;
            }
            return allStepsDuration;
        }
    }

    public class PropertyKeyStepToBuilder<TProperty, TBuilder> : PropertyKeyStepTweener<TProperty>
        where TBuilder : class {
        private readonly AbstractTweenSequenceBuilder<TBuilder> _abstractTweenSequenceBuilder;

        internal PropertyKeyStepToBuilder(AbstractTweenSequenceBuilder<TBuilder> abstractTweenSequenceBuilder,
            Node target,
            IProperty<TProperty> defaultProperty,
            Easing defaultEasing) : base(target, defaultProperty, defaultEasing) {
            _abstractTweenSequenceBuilder = abstractTweenSequenceBuilder;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> From(Func<Node, TProperty> fromFunction) {
            _fromFunction = fromFunction;
            return this;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> From(TProperty from) {
            _fromFunction = node => from;
            return this;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder>
            To(TProperty to, float duration, CallbackNode callbackNode) {
            return To(to, duration, null, callbackNode);
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> To(TProperty to, float duration, Easing easing = null,
            CallbackNode callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyStepTo<TProperty>(to, duration, easing ?? _defaultEasing, callbackNode);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> SetDebugSteps(List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public TBuilder EndAnimate() {
            return _abstractTweenSequenceBuilder as TBuilder;
        }
    }

    public class PropertyKeyStepOffsetBuilder<TProperty, TBuilder> : PropertyKeyStepTweener<TProperty>
        where TBuilder : class {
        private readonly AbstractTweenSequenceBuilder<TBuilder> _abstractTweenSequenceBuilder;

        internal PropertyKeyStepOffsetBuilder(AbstractTweenSequenceBuilder<TBuilder> abstractTweenSequenceBuilder,
            Node target, IProperty<TProperty> defaultProperty, Easing defaultEasing, bool relativeToFrom) :
            base(target, defaultProperty, defaultEasing) {
            _abstractTweenSequenceBuilder = abstractTweenSequenceBuilder;
            _relativeToFrom = relativeToFrom;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> From(Func<Node, TProperty> fromFunction) {
            _fromFunction = fromFunction;
            return this;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> From(TProperty from) {
            _fromFunction = node => from;
            return this;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> Offset(TProperty offset, float duration,
            CallbackNode callbackNode) {
            return Offset(offset, duration, null, callbackNode);
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> Offset(TProperty offset, float duration,
            Easing easing = null, CallbackNode callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyStepOffset<TProperty>(offset, duration, easing ?? _defaultEasing, callbackNode);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> SetDebugSteps(List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public TBuilder EndAnimate() {
            return _abstractTweenSequenceBuilder as TBuilder;
        }
    }

    public class PropertyKeyPercentToBuilder<TProperty, TBuilder> : PropertyKeyPercentTweener<TProperty>
        where TBuilder : class {
        private readonly AbstractTweenSequenceBuilder<TBuilder> _abstractTweenSequenceBuilder;

        internal PropertyKeyPercentToBuilder(AbstractTweenSequenceBuilder<TBuilder> abstractTweenSequenceBuilder,
            Node target, IProperty<TProperty> defaultProperty, Easing defaultEasing) :
            base(target, defaultProperty, defaultEasing) {
            _abstractTweenSequenceBuilder = abstractTweenSequenceBuilder;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> Duration(float duration) {
            AllStepsDuration = duration;
            return this;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> From(Func<Node, TProperty> fromFunction) {
            _fromFunction = fromFunction;
            return this;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> From(TProperty from) {
            _fromFunction = node => from;
            return this;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> KeyframeTo(float percentage, TProperty to,
            CallbackNode callbackNode) {
            return KeyframeTo(percentage, to, null, callbackNode);
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> KeyframeTo(float percentage, TProperty to,
            Easing easing = null, CallbackNode callbackNode = null) {
            if (percentage == 0f) {
                From(to);
            }
            var animationStepPropertyTweener =
                new AnimationKeyPercentTo<TProperty>(percentage, to, easing ?? _defaultEasing, callbackNode);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> SetDebugSteps(
            List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public TBuilder EndAnimate() {
            return _abstractTweenSequenceBuilder as TBuilder;
        }
    }

    public class PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> : PropertyKeyPercentTweener<TProperty>
        where TBuilder : class {
        private readonly AbstractTweenSequenceBuilder<TBuilder> _abstractTweenSequenceBuilder;

        internal PropertyKeyPercentOffsetBuilder(AbstractTweenSequenceBuilder<TBuilder> abstractTweenSequenceBuilder,
            Node target, IProperty<TProperty> defaultProperty, Easing defaultEasing, bool relativeToFrom) :
            base(target, defaultProperty, defaultEasing) {
            _abstractTweenSequenceBuilder = abstractTweenSequenceBuilder;
            _relativeToFrom = relativeToFrom;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> Duration(float duration) {
            AllStepsDuration = duration;
            return this;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> From(Func<Node, TProperty> fromFunction) {
            _fromFunction = fromFunction;
            return this;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> From(TProperty from) {
            _fromFunction = node => from;
            return this;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> KeyframeOffset(float percentage, TProperty offset,
            CallbackNode callbackNode) {
            return KeyframeOffset(percentage, offset, null, callbackNode);
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> KeyframeOffset(float percentage, TProperty offset,
            Easing easing = null, CallbackNode callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyPercentOffset<TProperty>(percentage, offset, easing, callbackNode);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> SetDebugSteps(
            List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public TBuilder EndAnimate() {
            return _abstractTweenSequenceBuilder as TBuilder;
        }
    }
}