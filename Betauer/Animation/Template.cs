using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Animation {
    public static class Template {
        public static TweenSequenceTemplate Get(string name) => Factories[name.ToLower()].Get();

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

        private static readonly TemplateFactory BounceFactory;
        private static readonly TemplateFactory FlashFactory;
        private static readonly TemplateFactory HeadShakeFactory;
        private static readonly TemplateFactory HeartBeatFactory;
        private static readonly TemplateFactory JelloFactory;
        private static readonly TemplateFactory PulseFactory;
        private static readonly TemplateFactory RubberBandFactory;
        private static readonly TemplateFactory ShakeFactory;
        private static readonly TemplateFactory ShakeXFactory;
        private static readonly TemplateFactory ShakeYFactory;
        private static readonly TemplateFactory SwingFactory;

        private static readonly Dictionary<string, TemplateFactory> Factories =
            new Dictionary<string, TemplateFactory>();

        static Template() {
            BounceFactory = new TemplateFactory(nameof(Bounce), Templates.Bounce);
            BounceFactory = new TemplateFactory(nameof(Bounce), Templates.Bounce);
            FlashFactory = new TemplateFactory(nameof(Flash), Templates.Flash);
            HeadShakeFactory = new TemplateFactory(nameof(HeadShake), Templates.HeadShake);
            HeartBeatFactory = new TemplateFactory(nameof(HeartBeat), Templates.HeartBeat);
            JelloFactory = new TemplateFactory(nameof(Jello), Templates.Jello);
            PulseFactory = new TemplateFactory(nameof(Pulse), Templates.Pulse);
            RubberBandFactory = new TemplateFactory(nameof(RubberBand), Templates.RubberBand);
            ShakeFactory = new TemplateFactory(nameof(Shake), Templates.Shake);
            ShakeXFactory = new TemplateFactory(nameof(ShakeX), Templates.ShakeX);
            ShakeYFactory = new TemplateFactory(nameof(ShakeY), Templates.ShakeY);
            SwingFactory = new TemplateFactory(nameof(Swing), Templates.Swing);
        }

        private class TemplateFactory {
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

        private static class Templates {
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
        }
    }
}