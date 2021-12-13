using System;
using System.Collections.Generic;
using Godot;
using Object = Godot.Object;

namespace Tools.Animation {

    public interface ITweener {
        public float Start(Tween tween, float initialDelay, Node defaultTarget, string defaultMember,
            float defaultDuration);
    }

    public delegate void TweenCallback();

    internal class TweenCallbackHolder : Object {
        protected readonly TweenCallback Callback;

        internal TweenCallbackHolder(TweenCallback callback) {
            Callback = callback;
        }

        internal void Call() {
            Callback?.Invoke();
        }
    }

    internal class TweenPropertyMethodHolder<T> : Object {
        public delegate void PropertyMethodCallback(T value);

        private readonly PropertyMethodCallback _callback;

        public TweenPropertyMethodHolder(PropertyMethodCallback callback) {
            _callback = callback;
        }

        internal void Call(T value) {
            _callback.Invoke(value);
        }
    }

    internal class CallbackTweener : TweenCallbackHolder, ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));

        private readonly float _delay;

        internal CallbackTweener(float delay, TweenCallback callback) : base(callback) {
            _delay = delay;
        }

        public float Start(Tween tween, float initialDelay, Node defaultTarget, string defaultMember,
            float defaultDuration) {
            var start = _delay + initialDelay;
            if (Callback != null) {
                Logger.Info("Scheduling callback with " + _delay + "s delay. Start: " + start.ToString("F"));
            } else {
                Logger.Info("Scheduling " + _delay + "s delay. Start: " + initialDelay.ToString("F") + " End: " +
                            start.ToString("F"));
            }
            tween.InterpolateCallback(this, start, nameof(Call));
            return _delay;
        }
    }

    public abstract class PropertyTweener<T> {
        protected static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));

        protected readonly Node _target;
        protected readonly string _member; // TODO: create method/delegate interpolator
        protected readonly Easing _defaultEasing;
        protected readonly Callback _defaultCallback; // TODO: implement, why not?


        protected T _from;
        protected bool _liveFrom = true;

        internal PropertyTweener(Node target, string member, Easing defaultEasing) {
            _target = target;
            _member = member;
            _defaultEasing = defaultEasing;
            _from = default;
            _liveFrom = true;
        }

        protected T GetFirstFromValue() => _liveFrom ? (T)_target.GetIndexed(_member) : _from;

        protected bool Validate(int count, Node target, string member) {
            if (count == 0) {
                throw new Exception("Cant' start an animation with 0 steps or keys");
            }
            if (target == null) {
                throw new Exception("No target defined for the tween");
            }
            if (member == null) {
                throw new Exception("No member defined for the tween");
            }
            if (!Object.IsInstanceValid(target)) {
                Logger.Warning("Can't create InterpolateProperty in a freed target instance");
                return false;
            }
            return true;
        }

        public void RunStep(Tween tween, Node target, string property,
            T from, T to, float start, float duration, Easing easing, TweenCallback callback) {
            var end = start + duration;
            Logger.Info("\"" + target.Name + "\" " + target.GetType().Name + "." + property + ": " +
                        from + " to " + to +
                        " Start: " + start.ToString("F") +
                        " End: " + end.ToString("F") +
                        " (+" + duration.ToString("F") + ") " + easing.Name);

            if (easing is GodotEasing godotEasing) {
                tween.InterpolateProperty(target, property, from, to, duration,
                    godotEasing.TransitionType, godotEasing.EaseType, start);
            } else if (easing is BezierCurve bezierCurve) {
                TweenPropertyMethodHolder<float> tweenPropertyMethodHolder = new TweenPropertyMethodHolder<float>(
                    delegate(float value) {
                        var y = bezierCurve.GetY(value);
                        var lerp = GodotTools.LerpVariant(from, to, y);
                        // Logger.Debug(target.Name + "." + property + ": " + typeof(T).Name + " t:"+value+" y:"+lerp);
                        target.SetIndexed(property, lerp);
                    });
                tween.InterpolateMethod(tweenPropertyMethodHolder, nameof(TweenPropertyMethodHolder<T>.Call),
                    0f, 1f, duration, Tween.TransitionType.Linear, Tween.EaseType.InOut, start);
            }
            if (callback != null) {
                TweenCallbackHolder holder = new TweenCallbackHolder(callback);
                tween.InterpolateCallback(holder, start, nameof(TweenCallbackHolder.Call));
            }
        }
    }

    public class PropertyKeyStepTweener<T> : PropertyTweener<T>, ITweener {
        protected readonly ICollection<AnimationKeyStep<T>> _steps = new SimpleLinkedList<AnimationKeyStep<T>>();

        internal PropertyKeyStepTweener(Node target, string member, bool memberIsProperty, Easing defaultEasing) :
            base(target, member, defaultEasing) {
        }

        public float Start(Tween tween, float initialDelay, Node defaultTarget, string defaultMember,
            float defaultDuration) {
            var target = _target ?? defaultTarget;
            var member = _member ?? defaultMember;
            if (!Validate(_steps.Count, target, member)) return 0;
            // TODO: defaultDuration could be % or absolute or nothing
            var from = GetFirstFromValue();
            var startTime = 0f;
            // var totalDuration = _steps.Sum(step => step.Duration);
            foreach (var step in _steps) {
                var to = step.GetTo(from);
                var duration = step.Duration;
                // var percentStart = startTime / totalDuration;
                // var percentEnd = (startTime + duration) / totalDuration;
                var easing = step.Easing ?? _defaultEasing ?? Easing.LinearInOut;
                RunStep(tween, target, member, from, to, initialDelay + startTime, duration, easing, step.Callback);
                from = to;
                startTime += duration;
            }
            return startTime;
        }
    }

    public class PropertyKeyPercentTweener<T> : PropertyTweener<T>, ITweener {
        protected readonly ICollection<AnimationKeyPercent<T>> _steps =
            new SimpleLinkedList<AnimationKeyPercent<T>>();

        protected float AllStepsDuration = 0;

        internal PropertyKeyPercentTweener(Node target, string member, bool memberIsProperty, Easing defaultEasing) :
            base(
                target, member, defaultEasing) {
        }

        public float Start(Tween tween, float initialDelay, Node defaultTarget, string defaultMember,
            float defaultDuration) {
            var target = _target ?? defaultTarget;
            var member = _member ?? defaultMember;
            if (!Validate(_steps.Count, target, member)) return 0;
            var allStepsDuration = AllStepsDuration > 0f ? AllStepsDuration : defaultDuration;
            if (allStepsDuration <= 0)
                throw new Exception("Keyframe animation duration should be more than 0");
            var from = GetFirstFromValue();
            var startTime = 0f;
            // var percentStart = 0f;
            foreach (var step in _steps) {
                var to = step.GetTo(from);
                var endTime = step.Percent * allStepsDuration;
                var keyDuration = endTime - startTime;
                var easing = step.Easing ?? _defaultEasing ?? Easing.LinearInOut;
                // var percentEnd = step.Percent;
                RunStep(tween, target, member, from, to, initialDelay + startTime, keyDuration, easing, step.Callback);
                from = to;
                // percentStart = percentEnd;
                startTime = endTime;
            }
            return allStepsDuration;
        }
    }

    public class PropertyKeyStepTweenerBuilder<T> : PropertyKeyStepTweener<T> {
        private readonly TweenSequenceBuilder _tweenSequenceBuilder;

        internal PropertyKeyStepTweenerBuilder(TweenSequenceBuilder tweenSequenceBuilder, Node target, string member,
            bool memberIsProperty, Easing defaultEasing) : base(target, member, memberIsProperty, defaultEasing) {
            _tweenSequenceBuilder = tweenSequenceBuilder;
        }

        public PropertyKeyStepTweenerBuilder<T> From(T from) {
            _from = from;
            _liveFrom = false;
            return this;
        }

        public PropertyKeyStepTweenerBuilder<T> To(T to, float duration, Easing easing = null,
            TweenCallback callback = null) {
            var animationStepPropertyTweener =
                new AnimationKeyStepTo<T>(to, duration, easing ?? _defaultEasing, callback);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyStepTweenerBuilder<T> Offset(T offset, float duration, Easing easing = null,
            TweenCallback callback = null) {
            var animationStepPropertyTweener =
                new AnimationKeyStepOffset<T>(offset, duration, easing ?? _defaultEasing, callback);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public TweenSequenceBuilder EndAnimate() {
            return _tweenSequenceBuilder;
        }
    }

    public class PropertyKeyPercentTweenerBuilder<T> : PropertyKeyPercentTweener<T> {
        private readonly TweenSequenceBuilder _tweenSequenceBuilder;

        internal PropertyKeyPercentTweenerBuilder(TweenSequenceBuilder tweenSequenceBuilder, Node target, string member,
            bool memberIsProperty, Easing defaultEasing) : base(target, member, memberIsProperty, defaultEasing) {
            _tweenSequenceBuilder = tweenSequenceBuilder;
        }

        public PropertyKeyPercentTweenerBuilder<T> Duration(float duration) {
            AllStepsDuration = duration;
            return this;
        }

        public PropertyKeyPercentTweenerBuilder<T> From(T from) {
            _from = from;
            _liveFrom = false;
            return this;
        }

        public PropertyKeyPercentTweenerBuilder<T> KeyframeTo(float percentage, T to, Easing easing = null,
            TweenCallback callback = null) {
            if (percentage == 0f) {
                From(to);
            } else {
                var animationStepPropertyTweener =
                    new AnimationKeyPercentTo<T>(percentage, to, easing ?? _defaultEasing, callback);
                _steps.Add(animationStepPropertyTweener);
            }
            return this;
        }

        public PropertyKeyPercentTweenerBuilder<T> KeyframeOffset(float percentage, T offset, Easing easing = null,
            TweenCallback callback = null) {
            if (percentage == 0) {
                throw new Exception(
                    $"Can't set a 0% keyframe with offset. Use KeyframeTo(0,{offset}) or From({offset}) instead");
            }
            var animationStepPropertyTweener =
                new AnimationKeyPercentOffset<T>(percentage, offset, easing, callback);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public TweenSequenceBuilder EndAnimate() {
            return _tweenSequenceBuilder;
        }
    }
}