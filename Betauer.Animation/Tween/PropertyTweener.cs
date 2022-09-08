using System;
using System.Collections.Generic;
using Betauer.Animation.Easing;
using Betauer.Nodes.Property;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation.Tween {
    
    public abstract class PropertyTweener {
        protected static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener));
    }

    public abstract class PropertyTweener<TProperty> : PropertyTweener {
        protected readonly Func<Node, IProperty<TProperty>> PropertyFactory;
        protected readonly IEasing? DefaultEasing;
        protected Func<Node, TProperty>? FromFunction;
        protected bool RelativeToFrom = false;
        protected List<DebugStep<TProperty>>? DebugSteps = null;

        internal PropertyTweener(Func<Node, IProperty<TProperty>> propertyFactory, IEasing? defaultEasing) {
            PropertyFactory = propertyFactory;
            DefaultEasing = defaultEasing;
        }

        protected bool Validate(int count, Node target, IProperty<TProperty> property) {
            if (count == 0) throw new InvalidAnimationException("Can't start an empty animation");
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (!property.IsCompatibleWith(target))
                throw new NodeNotCompatibleWithPropertyException($"Property {property} is not compatible with target type {target.GetType().Name}");
            
            if (target == null) throw new InvalidAnimationException("No target defined for the animation");
            if (!Object.IsInstanceValid(target)) {
#if DEBUG
                Logger.Warning($"Can't start {GetType()} using a freed target instance");
#endif
                return false;
            }
            return true;
        }

        protected Tweener RunStep(
            SceneTreeTween sceneTreeTween,
            Node target, IProperty<TProperty> property, TProperty from, TProperty to, 
            float start, float duration, IEasing? easing) {
            
            easing ??= DefaultEasing ?? Easings.Linear;
            var end = start + duration;
#if DEBUG
            Logger.Info("\"" + target?.Name + "\" " + target?.GetType().Name + ":" + property +
                        " Interpolate(" +
                        from + ", " + to +
                        ") Scheduled from " + start.ToString("F") +
                        "s to " + end.ToString("F") +
                        "s (+" + duration.ToString("F") + "s) CurveBezier");
#endif
            DebugSteps?.Add(new DebugStep<TProperty>(target, from, to, start, duration, easing));

            return easing is BezierCurve bezierCurve ?
                RunCurveBezierStep(sceneTreeTween, target, property, from, to, start, duration, bezierCurve) :
                RunEasingStep(sceneTreeTween, target, property, from, to, start, duration, (GodotEasing)easing);
        }

        private static Tweener RunCurveBezierStep(
            SceneTreeTween sceneTreeTween,
            Node target, IProperty<TProperty> property,
            TProperty from, TProperty to, float start, float duration, BezierCurve bezierCurve) {
            // TODO: there are no tests with bezier curves. No need to test the curve, but it needs to test if the value is set
            return sceneTreeTween
                .Parallel()
                .TweenInterpolateAction(0f, 1f, duration,
                    (linearT) => {
                        var curveY = bezierCurve.GetY(linearT);
                        var value = (TProperty)VariantHelper.LerpVariant(from, to, curveY);
#if DEBUG
                        // Logger.Debug(
                        // $"\"{context.Target.Name}\" {context.Target.GetType().Name}.{property}:  Bezier({linearT})={curveY} value:{value}");
                        // Console.WriteLine($"Play  From/To: {from}/{to} | Delta:+{(float)x.ElapsedMilliseconds/1000:0.0000} From/To: 0.00/{duration:0.00} (duration: {duration:0.00} Time:{((float)x2.ElapsedMilliseconds)/1000:0.0000} | t:{linearY:0.0000} y:{curveY:0000} Value: {value}");
#endif
                        property.SetValue(target, value);
                    })
                .SetTrans(Godot.Tween.TransitionType.Linear)
                .SetEase(Godot.Tween.EaseType.InOut)
                .SetDelay(start);
        }

        private static Tweener RunEasingStep(
            SceneTreeTween sceneTreeTween,
            Node target, IProperty<TProperty> property,
            TProperty from, TProperty to, float start, float duration, GodotEasing godotEasing) {
            if (property is IIndexedProperty indexedProperty) {
                return sceneTreeTween
                    .Parallel()
                    .TweenProperty(target, indexedProperty.GetIndexedPropertyName(target), to, duration)
                    .From(@from)
                    .SetTrans(godotEasing.TransitionType)
                    .SetEase(godotEasing.EaseType)
                    .SetDelay(start);
            } else {
                return sceneTreeTween
                    .Parallel()
                    .TweenInterpolateAction(@from, to, duration,
                        (value) => {
#if DEBUG
                            // Logger.Info("\"" + context.Target.Name + "\" " + context.Target.GetType().Name + "." + property.GetPropertyName(target) + ": " 
                            // + " value:" + value+"");
#endif
                            property.SetValue(target, value);
                        })
                    .SetTrans(godotEasing.TransitionType)
                    .SetEase(godotEasing.EaseType)
                    .SetDelay(start);
            }
        }
    }
}