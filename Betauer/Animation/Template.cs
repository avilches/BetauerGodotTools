using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Animation {
    public static class Template {
        public static TweenSequenceTemplate Get(string name) => Factories[name.ToLower()].Get();

        public static TweenSequenceTemplate Get<T>(string name, T data) {
            var templateFactory = Factories[name.ToLower()];
            return templateFactory is TemplateFactory<T> templateFactoryTyped
                ? templateFactoryTyped.Get(data)
                : templateFactory.Get();
        }

        public static TweenSequenceTemplate Bounce => BounceFactory.Get();
        public static TweenSequenceTemplate Flash => FlashFactory.Get();
        public static TweenSequenceTemplate HeadShake => HeadShakeFactory.Get();
        public static TweenSequenceTemplate HeartBeat => HeartBeatFactory.Get();
        public static TweenSequenceTemplate Jello => JelloFactory.Get();
        public static TweenSequenceTemplate Pulse => PulseFactory.Get();
        public static TweenSequenceTemplate RubberBand => RubberBandFactory.Get();
        public static TweenSequenceTemplate Shake => ShakeFactory.Get();
        public static TweenSequenceTemplate ShakeX => ShakeXFactory.Get();
        public static TweenSequenceTemplate ShakeY => ShakeYFactory.Get();
        public static TweenSequenceTemplate Swing => SwingFactory.Get();
        public static TweenSequenceTemplate Tada => TadaFactory.Get();
        public static TweenSequenceTemplate Wobble => WobbleFactory.Get();

        public static TweenSequenceTemplate BackInUp => BackInUpFactory.Get();
        public static TweenSequenceTemplate BackInDown => BackInDownFactory.Get();
        public static TweenSequenceTemplate BackInLeft => BackInLeftFactory.Get();
        public static TweenSequenceTemplate BackInRight => BackInRightFactory.Get();

        public static TweenSequenceTemplate BackOutUp => BackOutUpFactory.Get();
        public static TweenSequenceTemplate BackOutDown => BackOutDownFactory.Get();
        public static TweenSequenceTemplate BackOutLeft => BackOutLeftFactory.Get();
        public static TweenSequenceTemplate BackOutRight => BackOutRightFactory.Get();

        public static TweenSequenceTemplate BounceIn => BounceInFactory.Get();
        public static TweenSequenceTemplate BounceInUp => BounceInUpFactory.Get();
        public static TweenSequenceTemplate BounceInDown => BounceInDownFactory.Get();
        public static TweenSequenceTemplate BounceInLeft => BounceInLeftFactory.Get();
        public static TweenSequenceTemplate BounceInRight => BounceInRightFactory.Get();

        private static readonly Dictionary<string, TemplateFactory> Factories =
            new Dictionary<string, TemplateFactory>();

        public static readonly TemplateFactory BounceFactory = new TemplateFactory(nameof(Bounce), Templates.Bounce);
        public static readonly TemplateFactory FlashFactory = new TemplateFactory(nameof(Flash), Templates.Flash);
        public static readonly TemplateFactory HeadShakeFactory = new TemplateFactory(nameof(HeadShake), Templates.HeadShake);
        public static readonly TemplateFactory HeartBeatFactory = new TemplateFactory(nameof(HeartBeat), Templates.HeartBeat);
        public static readonly TemplateFactory JelloFactory = new TemplateFactory(nameof(Jello), Templates.Jello);
        public static readonly TemplateFactory PulseFactory = new TemplateFactory(nameof(Pulse), Templates.Pulse);
        public static readonly TemplateFactory RubberBandFactory = new TemplateFactory(nameof(RubberBand), Templates.RubberBand);
        public static readonly TemplateFactory ShakeFactory = new TemplateFactory(nameof(Shake), Templates.Shake);
        public static readonly TemplateFactory ShakeXFactory = new TemplateFactory(nameof(ShakeX), Templates.ShakeX);
        public static readonly TemplateFactory ShakeYFactory = new TemplateFactory(nameof(ShakeY), Templates.ShakeY);
        public static readonly TemplateFactory SwingFactory = new TemplateFactory(nameof(Swing), Templates.Swing);
        public static readonly TemplateFactory TadaFactory = new TemplateFactory(nameof(Tada), Templates.Tada);
        public static readonly TemplateFactory WobbleFactory = new TemplateFactory(nameof(Wobble), Templates.Wobble);

        public static readonly TemplateFactory<float> BackInUpFactory = new TemplateFactory<float>(nameof(BackInUp), () => Templates.BackInUp(), Templates.BackInUp);
        public static readonly TemplateFactory<float> BackInDownFactory = new TemplateFactory<float>(nameof(BackInDown), () => Templates.BackInDown(), Templates.BackInDown);
        public static readonly TemplateFactory<float> BackInLeftFactory = new TemplateFactory<float>(nameof(BackInLeft), () => Templates.BackInLeft(), Templates.BackInLeft);
        public static readonly TemplateFactory<float> BackInRightFactory = new TemplateFactory<float>(nameof(BackInRight), () => Templates.BackInRight(), Templates.BackInRight);

        public static readonly TemplateFactory<float> BackOutUpFactory = new TemplateFactory<float>(nameof(BackOutUp), () => Templates.BackOutUp(), Templates.BackOutUp);
        public static readonly TemplateFactory<float> BackOutDownFactory = new TemplateFactory<float>(nameof(BackOutDown), () => Templates.BackOutDown(), Templates.BackOutDown);
        public static readonly TemplateFactory<float> BackOutLeftFactory = new TemplateFactory<float>(nameof(BackOutLeft), () => Templates.BackOutLeft(), Templates.BackOutLeft);
        public static readonly TemplateFactory<float> BackOutRightFactory = new TemplateFactory<float>(nameof(BackOutRight), () => Templates.BackOutRight(), Templates.BackOutRight);

        public static readonly TemplateFactory BounceInFactory = new TemplateFactory(nameof(BounceIn), Templates.BounceIn);
        public static readonly TemplateFactory<float> BounceInUpFactory = new TemplateFactory<float>(nameof(BounceInUp), () => Templates.BounceInUp(), Templates.BounceInUp);
        public static readonly TemplateFactory<float> BounceInDownFactory = new TemplateFactory<float>(nameof(BounceInDown), () => Templates.BounceInDown(), Templates.BounceInDown);
        public static readonly TemplateFactory<float> BounceInLeftFactory = new TemplateFactory<float>(nameof(BounceInLeft), () => Templates.BounceInLeft(), Templates.BounceInLeft);
        public static readonly TemplateFactory<float> BounceInRightFactory = new TemplateFactory<float>(nameof(BounceInRight), () => Templates.BounceInRight(), Templates.BounceInRight);


        public class TemplateFactory {
            private readonly Func<TweenSequenceTemplate> _factory;
            public readonly string Name;
            private TweenSequenceTemplate _cached;

            public TemplateFactory(string name, Func<TweenSequenceTemplate> factory) {
                Name = name;
                _factory = factory;
                Factories[name.ToLower()] = this;
            }

            public TweenSequenceTemplate Get() => _cached ??= _factory();
        }

        public class TemplateFactory<T> : TemplateFactory {
            private readonly Func<TweenSequenceTemplate> _factory;
            private readonly Func<T, TweenSequenceTemplate> _factory1P;
            private Dictionary<object, TweenSequenceTemplate> _cached;

            public TemplateFactory(string name, Func<TweenSequenceTemplate> factory, Func<T, TweenSequenceTemplate> factory1P) : base(name, factory) {
                _factory1P = factory1P;
            }

            public TweenSequenceTemplate Get(T data) {
                _cached ??= new Dictionary<object, TweenSequenceTemplate>();
                _cached.TryGetValue(data, out TweenSequenceTemplate template);
                if (template != null) return template;
                return _cached[data] = _factory1P(data);
            }
        }

        internal static class Templates {
            private static readonly Dictionary<string, BezierCurve> Beziers = new Dictionary<string, BezierCurve>();

            private static BezierCurve Bezier(float p1x, float p1y, float p2x, float p2y) {
                var bezierCurve = BezierCurve.Create(p1x, p1y, p2x, p2y);
                if (Beziers.ContainsKey(bezierCurve.Name)) {
                    return Beziers[bezierCurve.Name];
                }
                Beziers[bezierCurve.Name] = bezierCurve;
                return bezierCurve;
            }

            internal static TweenSequenceTemplate Bounce() {
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/bounce.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateRelativeKeys(property: Property.PositionY)
                    .KeyframeOffset(0.20f, 0, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(0.4f, -30, Bezier(0.755f, 0.05f, 0.855f, 0.06f))
                    .KeyframeOffset(0.43f, -30, Bezier(0.755f, 0.05f, 0.855f, 0.06f))
                    .KeyframeOffset(0.53f, 0, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(0.7f, -15, Bezier(0.755f, 0.05f, 0.855f, 0.06f))
                    .KeyframeOffset(0.8f, 0, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(0.9f, -4)
                    .KeyframeOffset(1, 0, Bezier(0.215f, 0.61f, 0.355f, 1))
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.ScaleY)
                    .KeyframeTo(0.20f, 1, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.4f, 1.1f, Bezier(0.755f, 0.05f, 0.855f, 0.06f))
                    .KeyframeTo(0.43f, 1.1f, Bezier(0.755f, 0.05f, 0.855f, 0.06f))
                    .KeyframeTo(0.53f, 1, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.7f, 1.05f, Bezier(0.755f, 0.05f, 0.855f, 0.06f))
                    .KeyframeTo(0.8f, 0.95f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.9f, 1.02f)
                    .KeyframeTo(1, 1, Bezier(0.215f, 0.61f, 0.355f, 1))
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate Flash() {
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/flash.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateKeys(property: Property.Opacity)
                    .From(1)
                    .KeyframeTo(0.25f, 0)
                    .KeyframeTo(0.50f, 1)
                    .KeyframeTo(0.75f, 0)
                    .KeyframeTo(1.00f, 1)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate HeadShake() {
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/headShake.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateRelativeKeys(property: Property.PositionX)
                    .KeyframeOffset(0.065f, -6)
                    .KeyframeOffset(0.185f, +5)
                    .KeyframeOffset(0.315f, -3)
                    .KeyframeOffset(0.435f, +2)
                    .KeyframeOffset(0.500f, 0)
                    .KeyframeOffset(1.000f, 0)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.RotateCenter)
                    .KeyframeOffset(0.000f, 0, node => node.SetPivotCenter())
                    .KeyframeOffset(0.065f, -9)
                    .KeyframeOffset(0.185f, +7)
                    .KeyframeOffset(0.315f, -5)
                    .KeyframeOffset(0.435f, +3)
                    .KeyframeOffset(0.500f, 0)
                    .KeyframeOffset(1.000f, 0)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate HeartBeat() {
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/headShake.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.00f, Vector2.One, node => node.SetPivotCenter())
                    .KeyframeTo(0.14f, new Vector2(1.3f, 1.3f))
                    .KeyframeTo(0.28f, Vector2.One)
                    .KeyframeTo(0.42f, new Vector2(1.3f, 1.3f))
                    .KeyframeTo(0.70f, Vector2.One)
                    .KeyframeTo(1.00f, Vector2.One)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate Jello() {
                // Ported from the Ceceppa/Anima animation:
                // https://github.com/ceceppa/anima/blob/master/addons/anima/animations/attention_seeker/jello.gd
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateRelativeKeys(property: Property.SkewX)
                    .KeyframeOffset(0.111f, 0f)
                    .KeyframeOffset(0.222f, -0.3f)
                    .KeyframeOffset(0.333f, +0.265f)
                    .KeyframeOffset(0.444f, -0.1325f)
                    .KeyframeOffset(0.555f, +0.06625f)
                    .KeyframeOffset(0.666f, -0.033125f)
                    .KeyframeOffset(0.777f, +0.0165625f)
                    .KeyframeOffset(0.888f, -0.00828125f)
                    .KeyframeOffset(1.000f, 0f) // a relative offset 0 returns to the original value
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.SkewY)
                    .KeyframeOffset(0.111f, 0f)
                    .KeyframeOffset(0.222f, -0.3f)
                    .KeyframeOffset(0.333f, +0.265f)
                    .KeyframeOffset(0.444f, -0.1325f)
                    .KeyframeOffset(0.555f, +0.06625f)
                    .KeyframeOffset(0.666f, -0.033125f)
                    .KeyframeOffset(0.777f, +0.0165625f)
                    .KeyframeOffset(0.888f, -0.00828125f)
                    .KeyframeOffset(1.000f, 0f) // a relative offset 0 returns to the original value
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate Pulse() {
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/pulse.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.0f, Vector2.One, node => node.SetPivotCenter())
                    .KeyframeTo(0.5f, new Vector2(1.05f, 1.05f))
                    .KeyframeTo(1.0f, Vector2.One)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate RubberBand() {
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/rubberBand.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.00f, Vector2.One, node => node.SetPivotCenter())
                    .KeyframeTo(0.30f, new Vector2(1.25f, 0.75f))
                    .KeyframeTo(0.40f, new Vector2(0.75f, 1.25f))
                    .KeyframeTo(0.50f, new Vector2(1.15f, 0.85f))
                    .KeyframeTo(0.65f, new Vector2(0.95f, 1.05f))
                    .KeyframeTo(0.75f, new Vector2(1.05f, 0.95f))
                    .KeyframeTo(1.00f, Vector2.One)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate Shake() {
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/shake.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateRelativeKeys(property: Property.Position2D)
                    .KeyframeOffset(0.10f, new Vector2(-10f, -10f))
                    .KeyframeOffset(0.20f, new Vector2(+10f, +10f))
                    .KeyframeOffset(0.30f, new Vector2(-10f, -10f))
                    .KeyframeOffset(0.40f, new Vector2(+10f, +10f))
                    .KeyframeOffset(0.50f, new Vector2(-10f, -10f))
                    .KeyframeOffset(0.60f, new Vector2(+10f, +10f))
                    .KeyframeOffset(0.70f, new Vector2(-10f, -10f))
                    .KeyframeOffset(0.80f, new Vector2(+10f, +10f))
                    .KeyframeOffset(0.90f, new Vector2(-10f, -10f))
                    .KeyframeOffset(1.00f, Vector2.Zero) // a relative offset 0 returns to the original value
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate ShakeX() {
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/shakeX.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateRelativeKeys(property: Property.PositionX)
                    .KeyframeOffset(0.10f, -10f)
                    .KeyframeOffset(0.20f, +10f)
                    .KeyframeOffset(0.30f, -10f)
                    .KeyframeOffset(0.40f, +10f)
                    .KeyframeOffset(0.50f, -10f)
                    .KeyframeOffset(0.60f, +10f)
                    .KeyframeOffset(0.70f, -10f)
                    .KeyframeOffset(0.80f, +10f)
                    .KeyframeOffset(0.90f, -10f)
                    .KeyframeOffset(1.00f, 0f) // a relative offset 0 returns to the original value
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate ShakeY() {
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/shakeY.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateRelativeKeys(property: Property.PositionY)
                    .KeyframeOffset(0.10f, -10f)
                    .KeyframeOffset(0.20f, +10f)
                    .KeyframeOffset(0.30f, -10f)
                    .KeyframeOffset(0.40f, +10f)
                    .KeyframeOffset(0.50f, -10f)
                    .KeyframeOffset(0.60f, +10f)
                    .KeyframeOffset(0.70f, -10f)
                    .KeyframeOffset(0.80f, +10f)
                    .KeyframeOffset(0.90f, -10f)
                    .KeyframeOffset(1.00f, 0f) // a relative offset 0 returns to the original value
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate Swing() {
                // TODO: it uses SetPivotTopCenter, so it's only compatible with Sprite and Control, not Node2D as RotateCenter is validatin
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/swing.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateRelativeKeys(property: Property.RotateCenter)
                    .KeyframeOffset(0.0f, 0, node => node.SetPivotTopCenter())
                    .KeyframeOffset(0.2f, +15)
                    .KeyframeOffset(0.4f, -10)
                    .KeyframeOffset(0.6f, +5)
                    .KeyframeOffset(0.8f, -5)
                    .KeyframeOffset(1.0f, 0)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate Tada() {
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/tada.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.0f, Vector2.One, node => node.SetPivotCenter())
                    .KeyframeTo(0.1f, new Vector2(0.9f, 0.9f))
                    .KeyframeTo(0.3f, new Vector2(1.1f, 1.1f))
                    .KeyframeTo(1.0f, Vector2.One)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.RotateCenter)
                    .KeyframeOffset(0.1f, -3f)
                    .KeyframeOffset(0.2f, -3f)
                    .KeyframeOffset(0.3f, +3f)
                    .KeyframeOffset(0.4f, -3f)
                    .KeyframeOffset(0.5f, +3f)
                    .KeyframeOffset(0.6f, -3f)
                    .KeyframeOffset(0.7f, +3f)
                    .KeyframeOffset(0.8f, -3f)
                    .KeyframeOffset(0.9f, +3f)
                    .KeyframeOffset(1.0f, 0)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate Wobble() {
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/wobble.css
                // TODO: it uses SetPivotTopCenter, so it's only compatible with Sprite and Control (not really needed because PositionXCenter already validate it...)
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateRelativeKeys(property: Property.PositionXPercent)
                    .KeyframeOffset(0.00f, +0.00f, node => node.SetPivotTopCenter())
                    .KeyframeOffset(0.15f, -0.25f)
                    .KeyframeOffset(0.30f, +0.20f)
                    .KeyframeOffset(0.45f, -0.15f)
                    .KeyframeOffset(0.60f, +0.10f)
                    .KeyframeOffset(0.75f, -0.05f)
                    .KeyframeOffset(1.00f, +0.00f)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.RotateCenter)
                    .KeyframeOffset(0.15f, -5f)
                    .KeyframeOffset(0.30f, +3f)
                    .KeyframeOffset(0.45f, -3f)
                    .KeyframeOffset(0.60f, +2f)
                    .KeyframeOffset(0.75f, -1f)
                    .KeyframeOffset(1.0f, 0)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BackInUp(float distance = 1200f) {
                // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInUp.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), node => node.SetPivotCenter())
                    .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                    .KeyframeTo(1.00f, Vector2.One)
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 0.7f)
                    .KeyframeTo(0.80f, 0.7f)
                    .KeyframeTo(1.00f, 1)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.PositionY)
                    .KeyframeOffset(0.00f, Math.Abs(distance))
                    .KeyframeOffset(0.80f, 0f)
                    .KeyframeOffset(1.00f, 0f)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BackInDown(float distance = 1200f) {
                // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInDown.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), node => node.SetPivotCenter())
                    .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                    .KeyframeTo(1.00f, Vector2.One)
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 0.7f)
                    .KeyframeTo(0.80f, 0.7f)
                    .KeyframeTo(1.00f, 1)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.PositionY)
                    .KeyframeOffset(0.00f, -Math.Abs(distance))
                    .KeyframeOffset(0.80f, 0f)
                    .KeyframeOffset(1.00f, 0f)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BackInLeft(float distance = 2000f) {
                // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInLeft.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), node => node.SetPivotCenter())
                    .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                    .KeyframeTo(1.00f, Vector2.One)
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 0.7f)
                    .KeyframeTo(0.80f, 0.7f)
                    .KeyframeTo(1.00f, 1)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.PositionX)
                    .KeyframeOffset(0.00f, -Math.Abs(distance))
                    .KeyframeOffset(0.80f, 0f)
                    .KeyframeOffset(1.00f, 0f)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BackInRight(float distance = 2000f) {
                // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backInRight.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.00f, new Vector2(0.7f, 0.7f), node => node.SetPivotCenter())
                    .KeyframeTo(0.80f, new Vector2(0.7f, 0.7f))
                    .KeyframeTo(1.00f, Vector2.One)
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 0.7f)
                    .KeyframeTo(0.80f, 0.7f)
                    .KeyframeTo(1.00f, 1)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.PositionX)
                    .KeyframeOffset(0.00f, Math.Abs(distance))
                    .KeyframeOffset(0.80f, 0f)
                    .KeyframeOffset(1.00f, 0f)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BackOutUp(float distance = 700f) {
                // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutUp.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.00f, Vector2.One, node => node.SetPivotCenter())
                    .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                    .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 1)
                    .KeyframeTo(0.20f, 0.7f)
                    .KeyframeTo(1.00f, 0.7f)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.PositionY)
                    .KeyframeOffset(0.00f, 0f)
                    .KeyframeOffset(0.20f, 0f)
                    .KeyframeOffset(1.00f, -Math.Abs(distance))
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BackOutDown(float distance = 700f) {
                // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutDown.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.00f, Vector2.One, node => node.SetPivotCenter())
                    .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                    .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 1)
                    .KeyframeTo(0.20f, 0.7f)
                    .KeyframeTo(1.00f, 0.7f)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.PositionY)
                    .KeyframeOffset(0.00f, 0f)
                    .KeyframeOffset(0.20f, 0f)
                    .KeyframeOffset(1.00f, Math.Abs(distance))
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BackOutLeft(float distance = 2000f) {
                // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutLeft.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.00f, Vector2.One, node => node.SetPivotCenter())
                    .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                    .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 1)
                    .KeyframeTo(0.20f, 0.7f)
                    .KeyframeTo(1.00f, 0.7f)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.PositionX)
                    .KeyframeOffset(0.00f, 0f)
                    .KeyframeOffset(0.20f, 0f)
                    .KeyframeOffset(1.00f, -Math.Abs(distance))
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BackOutRight(float distance = 2000f) {
                // https://github.com/animate-css/animate.css/blob/main/source/back_entrances/backOutRight.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.00f, Vector2.One, node => node.SetPivotCenter())
                    .KeyframeTo(0.20f, new Vector2(0.7f, 0.7f))
                    .KeyframeTo(1.00f, new Vector2(0.7f, 0.7f))
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 1)
                    .KeyframeTo(0.20f, 0.7f)
                    .KeyframeTo(1.00f, 0.7f)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.PositionX)
                    .KeyframeOffset(0.00f, 0f)
                    .KeyframeOffset(0.20f, 0f)
                    .KeyframeOffset(1.00f, Math.Abs(distance))
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BounceIn() {
                // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceIn.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.00f, new Vector2(0.30f, 0.30f), node => node.SetPivotCenter())
                    .KeyframeTo(0.20f, new Vector2(1.00f, 1.00f), Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.40f, new Vector2(0.90f, 0.90f), Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.60f, new Vector2(1.03f, 1.03f), Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.80f, new Vector2(0.97f, 0.97f), Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(1.00f, Vector2.One, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 0.0f)
                    .KeyframeTo(0.60f, 1.0f)
                    .KeyframeTo(1.00f, 1.0f)
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BounceInUp(float distance = 3000f) {
                // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceInUp.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.ScaleY)
                    .KeyframeTo(0.00f, 3.000f)
                    .KeyframeTo(0.60f, 0.900f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.75f, 0.950f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.90f, 0.985f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(1.00f, 1.000f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 0.0f)
                    .KeyframeTo(0.60f, 1.0f)
                    .KeyframeTo(1.00f, 1.0f)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.PositionY)
                    .KeyframeOffset(0.00f, Math.Abs(distance))
                    .KeyframeOffset(0.60f, -25f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(0.75f, 10f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(0.90f, -5f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(1.00f, 0f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BounceInDown(float distance = 3000f) {
                // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceInDown.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.ScaleY)
                    .KeyframeTo(0.00f, 3.000f)
                    .KeyframeTo(0.60f, 0.900f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.75f, 0.950f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.90f, 0.985f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(1.00f, 1.000f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 0.0f)
                    .KeyframeTo(0.60f, 1.0f)
                    .KeyframeTo(1.00f, 1.0f)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.PositionY)
                    .KeyframeOffset(0.00f, -Math.Abs(distance))
                    .KeyframeOffset(0.60f, 25f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(0.75f, -10f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(0.90f, 5f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(1.00f, 0f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BounceInLeft(float distance = 3000f) {
                // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceInLeft.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.ScaleY)
                    .KeyframeTo(0.00f, 3.000f)
                    .KeyframeTo(0.60f, 0.900f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.75f, 0.950f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.90f, 0.985f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(1.00f, 1.000f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 0.0f)
                    .KeyframeTo(0.60f, 1.0f)
                    .KeyframeTo(1.00f, 1.0f)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.PositionX)
                    .KeyframeOffset(0.00f, -Math.Abs(distance))
                    .KeyframeOffset(0.60f, 25f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(0.75f, -10f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(0.90f, 5f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(1.00f, 0f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .EndAnimate()
                    .BuildTemplate();
            }

            internal static TweenSequenceTemplate BounceInRight(float distance = 3000f) {
                // https://github.com/animate-css/animate.css/blob/main/source/bouncing_entrances/bounceInRight.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(1f)
                    .AnimateKeys(property: Property.ScaleY)
                    .KeyframeTo(0.00f, 3.000f)
                    .KeyframeTo(0.60f, 0.900f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.75f, 0.950f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(0.90f, 0.985f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeTo(1.00f, 1.000f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys(property: Property.Opacity)
                    .KeyframeTo(0.00f, 0.0f)
                    .KeyframeTo(0.60f, 1.0f)
                    .KeyframeTo(1.00f, 1.0f)
                    .EndAnimate()
                    .Parallel()
                    .AnimateRelativeKeys(property: Property.PositionX)
                    .KeyframeOffset(0.00f, Math.Abs(distance))
                    .KeyframeOffset(0.60f, -25f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(0.75f, 10f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(0.90f, -5f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .KeyframeOffset(1.00f, 0f, Bezier(0.215f, 0.61f, 0.355f, 1f))
                    .EndAnimate()
                    .BuildTemplate();
            }

        }
    }
}