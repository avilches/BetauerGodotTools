using System;
using System.Collections.Generic;
using System.IO;
using Betauer.Animation.Easing;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation.Tween {
    
    public abstract class PropertyTweener {
        protected static readonly Logger Logger = LoggerFactory.GetLogger(typeof(PropertyTweener));
    }

    public abstract class PropertyTweener<TProperty> : PropertyTweener {
        protected readonly IProperty<TProperty> Property;
        protected readonly IEasing? DefaultEasing;
        protected Func<Node, TProperty>? FromFunction;
        protected bool RelativeToFrom = false;
        protected List<DebugStep<TProperty>>? DebugSteps = null;

        internal PropertyTweener(IProperty<TProperty> property, IEasing? defaultEasing) {
            Property = property;
            DefaultEasing = defaultEasing;
        }

        protected bool Validate(int count, Node target, IProperty<TProperty> property) {
            if (count == 0) throw new Exception("Can't start an empty animation");
            if (!(property is CallbackProperty<TProperty>)) {
                // Callbacks properties don't need target
                if (target == null) {
                    throw new Exception("No target defined for the animation");
                }
                if (!Object.IsInstanceValid(target)) {
#if DEBUG
                    Logger.Warning($"Can't start {GetType()} using a freed target instance");
#endif
                    return false;
                }
                if (!Property.IsCompatibleWith(target)) {
                    throw new Exception(
                        $"Property {Property} is not compatible with target type {target.GetType().Name}");
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
#if DEBUG
            Logger.Info("\"" + target?.Name + "\" " + target?.GetType().Name + ":" + property +
                        " Interpolate(" +
                        from + ", " + to +
                        ") Scheduled from " + start.ToString("F") +
                        "s to " + end.ToString("F") +
                        "s (+" + duration.ToString("F") + "s) CurveBezier");
#endif
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
#if DEBUG
                // Logger.Debug(
                // $"\"{context.Target.Name}\" {context.Target.GetType().Name}.{property.GetPropertyName(context.Target)}:  Bezier({linearT})={curveY} value:{value}");
                // Console.WriteLine($"Play  From/To: {from}/{to} | Delta:+{(float)x.ElapsedMilliseconds/1000:0.0000} From/To: 0.00/{duration:0.00} (duration: {duration:0.00} Time:{((float)x2.ElapsedMilliseconds)/1000:0.0000} | t:{linearY:0.0000} y:{curveY:0000} Value: {value}");
#endif
                // TODO: there are no tests with bezier curves. No need to test the curve, need to test if the value is set
                context.UpdateValue(property, value);
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
#if DEBUG
                    // Logger.Info("\"" + context.Target.Name + "\" " + context.Target.GetType().Name + "." + property.GetPropertyName(target) + ": " 
                    // + " value:" + value+"");
#endif
                    context.UpdateValue(property, value);
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
}