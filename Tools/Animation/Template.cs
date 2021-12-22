using System.Collections.Generic;
using Godot;

namespace Tools.Animation {
    public static class Template {
        public static TweenSequenceTemplate Get(string name) => TemplateHolder.Templates[name].Get();
        public static readonly TweenSequenceTemplate Bounce = TemplateHolder.Templates["bounce"].Get();
        public static readonly TweenSequenceTemplate Flash = TemplateHolder.Templates["flash"].Get();
        public static readonly TweenSequenceTemplate Headshake = TemplateHolder.Templates["headshake"].Get();
        public static readonly TweenSequenceTemplate Heartbeat = TemplateHolder.Templates["heartbeat"].Get();
        public static readonly TweenSequenceTemplate Jello = TemplateHolder.Templates["jello"].Get();

        private static class TemplateHolder {
            internal static readonly Dictionary<string, TemplateFactory> Templates;

            static TemplateHolder() {
                TemplateFactory[] factories = {
                    new TemplateFactory("bounce", Bounce),
                    new TemplateFactory("flash", Flash),
                    new TemplateFactory("headshake", Headshake),
                    new TemplateFactory("heartbeat", Heartbeat),
                    new TemplateFactory("jello", Jello),
                };
                Templates = new Dictionary<string, TemplateFactory>(factories.Length);
                foreach (var templateFactory in factories) {
                    Templates[templateFactory.Name] = templateFactory;
                }
            }

            internal delegate TweenSequenceTemplate TweenSequenceFactory();

            internal class TemplateFactory {
                private readonly TweenSequenceFactory _factory;
                public readonly string Name;
                private TweenSequenceTemplate _cached;

                public TemplateFactory(string name, TweenSequenceFactory factory) {
                    Name = name;
                    _factory = factory;
                }

                public TweenSequenceTemplate Get() => _cached ??= _factory();
            }

            private static readonly Dictionary<string, BezierCurve> Beziers = new Dictionary<string, BezierCurve>();

            private static BezierCurve Bezier(float p1x, float p1y, float p2x, float p2y) {
                var bezierCurve = BezierCurve.Create(p1x, p1y, p2x, p2y);
                if (Beziers.ContainsKey(bezierCurve.Name)) {
                    return Beziers[bezierCurve.Name];
                }
                Beziers[bezierCurve.Name] = bezierCurve;
                return bezierCurve;
            }

            private static TweenSequenceTemplate Bounce() {
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

            private static TweenSequenceTemplate Flash() {
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

            private static TweenSequenceTemplate Headshake() {
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
                    .AnimateKeys(property: Property.RotateCenter)
                    .From(0)
                    .KeyframeTo(0.000f, 0, Easing.LinearInOut, node => node.SetPivotCenter())
                    .KeyframeTo(0.065f, -9)
                    .KeyframeTo(0.185f, +7)
                    .KeyframeTo(0.315f, -5)
                    .KeyframeTo(0.435f, +3)
                    .KeyframeTo(0.500f, 0)
                    .KeyframeTo(1.000f, 0)
                    .EndAnimate()
                    .BuildTemplate();
            }

            private static TweenSequenceTemplate Heartbeat() {
                // https://github.com/animate-css/animate.css/blob/main/source/attention_seekers/headShake.css
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateKeys(property: Property.Scale2D)
                    .KeyframeTo(0.00f, Vector2.One, Easing.LinearIn, node => node.SetPivotCenter())
                    .KeyframeTo(0.14f, new Vector2(1.3f, 1.3f))
                    .KeyframeTo(0.28f, Vector2.One)
                    .KeyframeTo(0.42f, new Vector2(1.3f, 1.3f))
                    .KeyframeTo(0.70f, Vector2.One)
                    .KeyframeTo(1.00f, Vector2.One)
                    .EndAnimate()
                    .BuildTemplate();
            }

            private static TweenSequenceTemplate Jello() {
                // Ported from the Ceceppa/Anima animation:
                // https://github.com/ceceppa/anima/blob/master/addons/anima/animations/attention_seeker/jello.gd
                return TweenSequenceBuilder.Create()
                    .SetDuration(0.5f)
                    .AnimateRelativeKeys(property: Property.SkewX)
                    .KeyframeOffset(0.000f, 0f, Easing.LinearInOut, node => node.SetPivotCenter())
                    .KeyframeOffset(0.111f, 0f)
                    .KeyframeOffset(0.222f, -0.3f)
                    .KeyframeOffset(0.333f, +0.265f)
                    .KeyframeOffset(0.444f, -0.1325f)
                    .KeyframeOffset(0.555f, +0.06625f)
                    .KeyframeOffset(0.666f, -0.033125f)
                    .KeyframeOffset(0.777f, +0.0165625f)
                    .KeyframeOffset(0.888f, -0.00828125f)
                    .KeyframeOffset(1.000f, 0f)
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
                    .KeyframeOffset(1.000f, 0f)
                    .EndAnimate()
                    .BuildTemplate();
            }
        }
    }
}