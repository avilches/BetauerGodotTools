using System;
using System.Collections.Generic;
using Godot;
using Object = Godot.Object;

namespace Tools.Animation {
    public interface ITweener {
        float Start(Tween tween, float initialDelay, Node defaultTarget, Property defaultProperty,
            float sequenceDuration);
    }

    public interface ITweener<TProperty> {
        public float Start(Tween tween, float initialDelay, Node defaultTarget, Property<TProperty> defaultProperty,
            float defaultDuration);
    }

    public abstract class TweenerAdapter<TProperty> : ITweener, ITweener<TProperty> {
        public float Start(Tween tween, float initialDelay, Node defaultTarget, Property defaultProperty,
            float sequenceDuration) {
            return Start(tween, initialDelay, defaultTarget, (Property<TProperty>)defaultProperty, sequenceDuration);
        }

        public abstract float Start(Tween tween, float initialDelay, Node defaultTarget, Property<TProperty> defaultProperty,
            float defaultDuration);
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
            Property ignoredDefaultProperty, float ignoredDefaultDuration) {
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
            Node ignoredDefaultTarget, Property ignoredDefaultProperty, float ignoredDefaultDuration) {
            var delayEndTime = _delay + initialDelay;
            Logger.Info("Adding a delay of " + _delay + "s. Scheduled from " + initialDelay.ToString("F") + " to " +
                        delayEndTime.ToString("F"));
            return _delay;
        }
    }

    public abstract class PropertyTweener<TProperty> : TweenerAdapter<TProperty> {
        protected static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener<>));

        protected readonly Node _target;
        protected readonly Property<TProperty> _defaultProperty;
        protected readonly Easing _defaultEasing;
        // protected readonly Callback _defaultCallback; // TODO: implement, why not?

        protected TProperty _from;
        protected bool _liveFrom = true;

        internal PropertyTweener(Node target, Property<TProperty> defaultProperty, Easing defaultEasing) {
            _target = target;
            _defaultProperty = defaultProperty;
            _defaultEasing = defaultEasing;
            _from = default;
            _liveFrom = true;
        }

        protected TProperty GetFirstFromValue(Node target) {
            return _liveFrom ? _defaultProperty.GetValue(target) : _from;
        }

        protected bool Validate(int count, Node target, Property<TProperty> property) {
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

        protected void RunStep(Tween tween, Node target, Property<TProperty> property,
            TProperty from, TProperty to, float start, float duration, Easing easing, CallbackNode callbackNode) {
            if (duration > 0 && !from.Equals(to)) {
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
            }
            if (callbackNode != null) {
                // TODO: no reference at all to this holder... could be disposed by GC before it's executed?
                new DelayedCallbackNodeHolder(callbackNode, tween).Start(tween, start);
            }
        }

        private static void RunCurveBezierStep(Tween tween, Node target, Property<TProperty> property, TProperty @from, TProperty to,
            float start,
            float duration, BezierCurve bezierCurve) {
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

        private static void RunEasingStep(Tween tween, Node target, Property<TProperty> property, TProperty @from, TProperty to, float start,
            float duration, GodotEasing godotEasing) {
            if (property is IndexedProperty<TProperty> basicProperty) {
                tween.InterpolateProperty(target, basicProperty.GetIndexedProperty(target), @from, to, duration,
                    godotEasing.TransitionType, godotEasing.EaseType, start);
            } else {
                TweenPropertyMethodHolder<TProperty> tweenPropertyMethodHolder = new TweenPropertyMethodHolder<TProperty>(
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
        protected readonly ICollection<AnimationKeyStep<TProperty>> _steps = new SimpleLinkedList<AnimationKeyStep<TProperty>>();

        public List<AnimationKeyStep<TProperty>> CreateStepList() => new List<AnimationKeyStep<TProperty>>(_steps);

        internal PropertyKeyStepTweener(Node target, Property<TProperty> defaultProperty, Easing defaultEasing) :
            base(target, defaultProperty, defaultEasing) {
        }

        public override float Start(Tween tween, float initialDelay, Node defaultTarget, Property<TProperty> defaultProperty,
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

        public List<AnimationKeyPercent<TProperty>> CreateStepList() => new List<AnimationKeyPercent<TProperty>>(_steps);

        public float AllStepsDuration = 0;

        internal PropertyKeyPercentTweener(Node target, Property<TProperty> defaultProperty, Easing defaultEasing) :
            base(target, defaultProperty, defaultEasing) {
        }

        public override float Start(Tween tween, float initialDelay, Node defaultTarget, Property<TProperty> defaultProperty,
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
                RunStep(tween, target, property, from, to, initialDelay + startTime, keyDuration, easing,
                    step.CallbackNode);
                from = to;
                // percentStart = percentEnd;
                startTime = endTime;
            }
            return allStepsDuration;
        }
    }

    public class PropertyKeyStepTweenerBuilder<TProperty, TBuilder> : PropertyKeyStepTweener<TProperty> where TBuilder : class {
        private readonly AbstractTweenSequenceBuilder<TBuilder> _abstractTweenSequenceBuilder;

        internal PropertyKeyStepTweenerBuilder(AbstractTweenSequenceBuilder<TBuilder> abstractTweenSequenceBuilder, Node target,
            Property<TProperty> defaultProperty,
            Easing defaultEasing) : base(target, defaultProperty, defaultEasing) {
            _abstractTweenSequenceBuilder = abstractTweenSequenceBuilder;
        }

        public PropertyKeyStepTweenerBuilder<TProperty, TBuilder> From(TProperty from) {
            _from = from;
            _liveFrom = false;
            return this;
        }

        public PropertyKeyStepTweenerBuilder<TProperty, TBuilder> To(TProperty to, float duration, Easing easing = null,
            CallbackNode callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyStepTo<TProperty>(to, duration, easing ?? _defaultEasing, callbackNode);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyStepTweenerBuilder<TProperty, TBuilder> Offset(TProperty offset, float duration, Easing easing = null,
            CallbackNode callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyStepOffset<TProperty>(offset, duration, easing ?? _defaultEasing, callbackNode);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyStepTweenerBuilder<TProperty, TBuilder> SetDebugSteps(List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public TBuilder EndAnimate() {
            return _abstractTweenSequenceBuilder as TBuilder;
        }

    }

    public class PropertyKeyPercentTweenerBuilder<TProperty, TBuilder> : PropertyKeyPercentTweener<TProperty> where TBuilder : class {
        private readonly AbstractTweenSequenceBuilder<TBuilder> _abstractTweenSequenceBuilder;

        internal PropertyKeyPercentTweenerBuilder(AbstractTweenSequenceBuilder<TBuilder> abstractTweenSequenceBuilder, Node target,
            Property<TProperty> defaultProperty,
            Easing defaultEasing) : base(target, defaultProperty, defaultEasing) {
            _abstractTweenSequenceBuilder = abstractTweenSequenceBuilder;
        }

        public PropertyKeyPercentTweenerBuilder<TProperty, TBuilder> Duration(float duration) {
            AllStepsDuration = duration;
            return this;
        }

        public PropertyKeyPercentTweenerBuilder<TProperty, TBuilder> From(TProperty from) {
            _from = from;
            _liveFrom = false;
            return this;
        }

        public PropertyKeyPercentTweenerBuilder<TProperty, TBuilder> KeyframeTo(float percentage, TProperty to, Easing easing = null,
            CallbackNode callbackNode = null) {
            if (percentage == 0f) {
                From(to);
            }
            var animationStepPropertyTweener =
                new AnimationKeyPercentTo<TProperty>(percentage, to, easing ?? _defaultEasing, callbackNode);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyPercentTweenerBuilder<TProperty, TBuilder> KeyframeOffset(float percentage, TProperty offset,
            Easing easing = null,
            CallbackNode callbackNode = null) {
            var animationStepPropertyTweener =
                new AnimationKeyPercentOffset<TProperty>(percentage, offset, easing, callbackNode);
            _steps.Add(animationStepPropertyTweener);
            return this;
        }

        public PropertyKeyPercentTweenerBuilder<TProperty, TBuilder> SetDebugSteps(List<DebugStep<TProperty>> debugSteps) {
            DebugSteps = debugSteps;
            return this;
        }

        public TBuilder EndAnimate() {
            return _abstractTweenSequenceBuilder as TBuilder;
        }
    }


    /**
     * Special thanks to Alessandro Senese (Ceceppa)
     *
     * All the tricks to set pivots in Control nodes and create fake pivot in Sprite nodes are possible because
     * of his work in the wonderful library Anima: https://github.com/ceceppa/anima
     *
     * Thank you man! :)
     */
    public class PropertyTools {
        public static Vector2 GetSpriteSize(Sprite sprite) {
            return sprite.Texture.GetSize() * sprite.Scale;
        }

        public static void SetSpritePivot(Sprite node2D, Vector2 offset) {
            var position = node2D.GlobalPosition;
            node2D.Offset = offset;
            node2D.GlobalPosition = position - node2D.Offset;
        }

        public static void SetPivotTopCenter(Node node) {
            if (node is Control control) {
                // node.set_pivot_offset(Vector2(size.x / 2, 0))
                control.RectPivotOffset = new Vector2(control.RectSize.x / 2, 0);
            } else if (node is Sprite sprite) {
                // node.offset = Vector2(0, size.y / 2)
                SetSpritePivot(sprite, new Vector2(0, GetSpriteSize(sprite).y / 2));
            }
        }

        public static void SetPivotTopLeft(Node node) {
            if (node is Control control) {
                // node.set_pivot_offset(Vector2(0, 0))
                control.RectPivotOffset = Vector2.Zero;
            } else if (node is Sprite sprite) {
                // node.offset = Vector2(size.x / 2, 0)
                SetSpritePivot(sprite, new Vector2(GetSpriteSize(sprite).x / 2, 0));
            }
        }

        public static void SetPivotCenter(Node node) {
            if (node is Control control) {
                // node.set_pivot_offset(size / 2)
                control.RectPivotOffset = control.RectSize / 2;
            }
        }

        public static void SetPivotCenterBottom(Node node) {
            if (node is Control control) {
                // node.set_pivot_offset(Vector2(size.x / 2, size.y / 2))
                var size = control.RectSize;
                control.RectPivotOffset = new Vector2(size.x / 2, size.y / 2);
            } else if (node is Sprite sprite) {
                // node.offset = Vector2(0, -size.y / 2)
                SetSpritePivot(sprite, new Vector2(0, -GetSpriteSize(sprite).y / 2));
            }
        }

        public static void SetPivotLeftBottom(Node node) {
            if (node is Control control) {
                // node.set_pivot_offset(Vector2(0, size.y))
                control.RectPivotOffset = new Vector2(0, control.RectSize.y);
            } else if (node is Sprite sprite) {
                var size = GetSpriteSize(sprite);
                // node.offset = Vector2(size.x / 2, size.y)
                SetSpritePivot(sprite, new Vector2(size.x / 2, size.y));
            }
        }

        public static void SetPivotRightBottom(Node node) {
            if (node is Control control) {
                // node.set_pivot_offset(Vector2(size.x, size.y / 2))
                var size = control.RectSize;
                control.RectPivotOffset = new Vector2(size.x, size.y / 2);
            } else if (node is Sprite sprite) {
                var size = GetSpriteSize(sprite);
                // node.offset = Vector2(-size.x / 2, size.y / 2)
                SetSpritePivot(sprite, new Vector2(-size.x / 2, -size.y / 2));
            }
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

        public static readonly Property<float> RotateCenter = new RotationProperty();
    }

    public abstract class Property<TProperty> : Property {
        protected Property() {
        }

        public abstract TProperty GetValue(Node node);
        public abstract void SetValue(Node node, TProperty value);
    }

    public abstract class IndexedProperty<TProperty> : Property<TProperty> {
        public abstract string GetIndexedProperty(Node node);

        public override TProperty GetValue(Node node) {
            return (TProperty)node.GetIndexed(GetIndexedProperty(node));
        }

        public override void SetValue(Node node, TProperty value) {
            node.SetIndexed(GetIndexedProperty(node), value);
        }
    }

    public class BasicProperty<TProperty> : IndexedProperty<TProperty> {
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
            return node is Control ? "rect_scale:" + _key : "scale:" + _key;
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

    public class RotationProperty : IndexedProperty<float> {
        public override string GetIndexedProperty(Node node) {
            return node switch {
                Control control => "rect_rotation",
                Node2D node2D => "rotation_degrees",
                _ => "rotation" // TODO: this case is not tested... 3D?
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