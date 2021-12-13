using System;
using System.Collections.Generic;
using Godot;
using Object = Godot.Object;

namespace Tools.Animation {
    public interface ITweener {
        float Start(Tween tween, float initialDelay, Node defaultTarget, Property defaultProperty, float sequenceDuration);
    }

    public interface ITweener<T> {
        public float Start(Tween tween, float initialDelay, Node defaultTarget, Property<T> defaultProperty, float defaultDuration);
    }

    public abstract class TweenerAdapter<T> : ITweener, ITweener<T> {
        public float Start(Tween tween, float initialDelay, Node defaultTarget, Property defaultProperty, float sequenceDuration) {
            return Start(tween, initialDelay, defaultTarget, (Property<T>)defaultProperty, sequenceDuration);
        }

        public abstract float Start(Tween tween, float initialDelay, Node defaultTarget, Property<T> defaultProperty, float defaultDuration);
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

        public float Start(Tween tween, float initialDelay, Node defaultTarget, Property defaultProperty,
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

    public abstract class PropertyTweener<T> : TweenerAdapter<T> {
        protected static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));

        protected readonly Node _target;
        protected readonly Property<T> _defaultProperty;
        protected readonly Easing _defaultEasing;
        protected readonly Callback _defaultCallback; // TODO: implement, why not?


        protected T _from;
        protected bool _liveFrom = true;

        internal PropertyTweener(Node target, Property<T> defaultProperty, Easing defaultEasing) {
            _target = target;
            _defaultProperty = defaultProperty;
            _defaultEasing = defaultEasing;
            _from = default;
            _liveFrom = true;
        }

        protected T GetFirstFromValue(Node target) {
            return _liveFrom ? _defaultProperty.GetValue(target) : _from;
        }

        protected bool Validate(int count, Node target, Property<T> property) {
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
            return true;
        }

        public void RunStep(Tween tween, Node target, Property<T> property,
            T from, T to, float start, float duration, Easing easing, TweenCallback callback) {
            var end = start + duration;
            Logger.Info("\"" + target.Name + "\" " + target.GetType().Name + "." + property + ": " +
                        from + " to " + to +
                        " Start: " + start.ToString("F") +
                        " End: " + end.ToString("F") +
                        " (+" + duration.ToString("F") + ") " + easing.Name);

            if (easing is GodotEasing godotEasing) {
                if (property is IndexedProperty<T> basicProperty) {
                    tween.InterpolateProperty(target, basicProperty.GetIndexedProperty(target), from, to, duration,
                        godotEasing.TransitionType, godotEasing.EaseType, start);
                } else {
                    TweenPropertyMethodHolder<T> tweenPropertyMethodHolder = new TweenPropertyMethodHolder<T>(
                        delegate(T value) {
                            // Logger.Debug(target.Name + "." + property + ": " + typeof(T).Name + " t:"+value+" value:"+value);
                            property.SetValue(target, value);
                        });
                    tween.InterpolateMethod(tweenPropertyMethodHolder, nameof(TweenPropertyMethodHolder<T>.Call),
                        from, to, duration, godotEasing.TransitionType, godotEasing.EaseType, start);
                }
            } else if (easing is BezierCurve bezierCurve) {
                TweenPropertyMethodHolder<float> tweenPropertyMethodHolder = new TweenPropertyMethodHolder<float>(
                    delegate(float linearY) {
                        var curveY = bezierCurve.GetY(linearY);
                        var value = (T)GodotTools.LerpVariant(from, to, curveY);
                        // Logger.Debug(target.Name + "." + property + ": " + typeof(T).Name + " t:"+value+" y:"+lerp);
                        property.SetValue(target, value);
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

    public class PropertyKeyStepTweener<T> : PropertyTweener<T> {
        protected readonly ICollection<AnimationKeyStep<T>> _steps = new SimpleLinkedList<AnimationKeyStep<T>>();

        internal PropertyKeyStepTweener(Node target, Property<T> defaultProperty, Easing defaultEasing) :
            base(target, defaultProperty, defaultEasing) {
        }

        public override float Start(Tween tween, float initialDelay, Node defaultTarget, Property<T> defaultProperty,
            float defaultDuration) {
            var target = _target ?? defaultTarget;
            var property = _defaultProperty ?? defaultProperty;
            if (!Validate(_steps.Count, target, property)) return 0;
            // TODO: defaultDuration could be % or absolute or nothing
            var from = GetFirstFromValue(target);
            var startTime = 0f;
            // var totalDuration = _steps.Sum(step => step.Duration);
            foreach (var step in _steps) {
                var to = step.GetTo(from);
                var duration = step.Duration;
                // var percentStart = startTime / totalDuration;
                // var percentEnd = (startTime + duration) / totalDuration;
                var easing = step.Easing ?? _defaultEasing ?? Easing.LinearInOut;
                RunStep(tween, target, property, from, to, initialDelay + startTime, duration, easing, step.Callback);
                from = to;
                startTime += duration;
            }
            return startTime;
        }
    }

    public class PropertyKeyPercentTweener<T> : PropertyTweener<T> {
        protected readonly ICollection<AnimationKeyPercent<T>> _steps =
            new SimpleLinkedList<AnimationKeyPercent<T>>();

        protected float AllStepsDuration = 0;

        internal PropertyKeyPercentTweener(Node target, Property<T> defaultProperty, Easing defaultEasing) :
            base(target, defaultProperty, defaultEasing) {
        }

        public override float Start(Tween tween, float initialDelay, Node defaultTarget, Property<T> defaultProperty,
            float defaultDuration) {
            var allStepsDuration = AllStepsDuration > 0f ? AllStepsDuration : defaultDuration;
            if (allStepsDuration <= 0)
                throw new Exception("Keyframe animation duration should be more than 0");

            var target = _target ?? defaultTarget;
            var property = _defaultProperty ?? defaultProperty;
            if (!Validate(_steps.Count, target, property)) return 0;
            var from = GetFirstFromValue(target);
            var startTime = 0f;
            // var percentStart = 0f;
            foreach (var step in _steps) {
                var to = step.GetTo(from);
                var endTime = step.Percent * allStepsDuration;
                var keyDuration = endTime - startTime;
                var easing = step.Easing ?? _defaultEasing ?? Easing.LinearInOut;
                // var percentEnd = step.Percent;
                RunStep(tween, target, property, from, to, initialDelay + startTime, keyDuration, easing, step.Callback);
                from = to;
                // percentStart = percentEnd;
                startTime = endTime;
            }
            return allStepsDuration;
        }
    }

    public class PropertyKeyStepTweenerBuilder<T> : PropertyKeyStepTweener<T> {
        private readonly TweenSequenceBuilder _tweenSequenceBuilder;

        internal PropertyKeyStepTweenerBuilder(TweenSequenceBuilder tweenSequenceBuilder, Node target,
            Property<T> defaultProperty,
            Easing defaultEasing) : base(target, defaultProperty, defaultEasing) {
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

        internal PropertyKeyPercentTweenerBuilder(TweenSequenceBuilder tweenSequenceBuilder, Node target,
            Property<T> defaultProperty,
            Easing defaultEasing) : base(target, defaultProperty, defaultEasing) {
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


    public class Property {
        public static readonly Property<Color> Modulate = new BasicProperty<Color>("modulate");
        public static readonly Property<float> ModulateR = new BasicProperty<float>("modulate:r");
        public static readonly Property<float> ModulateG = new BasicProperty<float>("modulate:g");
        public static readonly Property<float> ModulateB = new BasicProperty<float>("modulate:b");
        public static readonly Property<float> Opacity = new BasicProperty<float>("modulate:a");

        public static readonly Property<Vector2> Position2D = new Position2DProperty();
        public static readonly Property<float> PositionX = new PositionProperty("x");
        public static readonly Property<float> PositionY = new PositionProperty("y");
        public static readonly Property<float> PositionZ = new PositionProperty("z");

        public static readonly Property<Vector2> Scale2D = new Scale2DProperty();
        public static readonly Property<float> ScaleX = new ScaleProperty("x");
        public static readonly Property<float> ScaleY = new ScaleProperty("y");
        public static readonly Property<float> ScaleZ = new ScaleProperty("z");
    }

    public abstract class Property<T> : Property {
        protected Property() {
        }

        public abstract T GetValue(Node node);
        public abstract void SetValue(Node node, T value);
    }

    public abstract class IndexedProperty<T> : Property<T> {
        public abstract string GetIndexedProperty(Node node);

        public override T GetValue(Node node) {
            return (T)node.GetIndexed(GetIndexedProperty(node));
        }

        public override void SetValue(Node node, T value) {
            node.SetIndexed(GetIndexedProperty(node), value);
        }
    }

    public class BasicProperty<T> : IndexedProperty<T> {
        public readonly string IndexedProperty;

        public BasicProperty(string indexedProperty) {
            IndexedProperty = indexedProperty;
        }

        public override string GetIndexedProperty(Node node) {
            return IndexedProperty;
        }
    }

    public class Scale2DProperty : IndexedProperty<Vector2> {
        public override string GetIndexedProperty(Node node) {
            return node is Control ? "rect_scale" : "scale";
        }
    }

    public class ScaleProperty : IndexedProperty<float> {
        private readonly string _key;

        public ScaleProperty(string key) {
            _key = key;
        }

        public override string GetIndexedProperty(Node node) {
            return node is Control ? "rect_scale:" + _key : "scale" + _key;
        }
    }

    public class Position2DProperty : IndexedProperty<Vector2> {
        public override string GetIndexedProperty(Node node) {
            return node switch {
                Control control => "rect_position",
                Node2D node2D => "position",
                _ => "global_transform:origin" // TODO: this case is not tested... 3D?
            };
        }
    }

    public class PositionProperty : IndexedProperty<float> {
        private readonly string _key;

        public PositionProperty(string key) {
            _key = key;
        }

        public override string GetIndexedProperty(Node node) {
            return node switch {
                Control control => "rect_position:" + _key,
                Node2D node2D => "position:" + _key,
                _ => "global_transform:origin:" + _key // TODO: this case is not tested... 3D?
            };
        }
    }
}