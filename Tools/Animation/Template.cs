using System.Collections.Generic;
using Godot;

namespace Tools.Animation {
    public static class Template {
        public static TweenSequenceTemplate Get(string name) => TemplateHolder.Templates[name].Get();
        public static readonly TweenSequenceTemplate Bounce = TemplateHolder.Templates["bounce"].Get();
        public static readonly TweenSequenceTemplate Flash = TemplateHolder.Templates["flash"].Get();
        public static readonly TweenSequenceTemplate Headshake = TemplateHolder.Templates["headshake"].Get();

        private static class TemplateHolder {
            internal static readonly Dictionary<string, TemplateFactory> Templates;

            static TemplateHolder() {
                TemplateFactory[] factories = {
                    new TemplateFactory("bounce", Bounce),
                    new TemplateFactory("flash", Flash),
                    new TemplateFactory("headshake", Headshake),
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

            private static TweenSequenceTemplate Bounce() {
                return TweenSequenceBuilder.CreateTemplate()
                    .SetDuration(0.5f)
                    .AnimateKeys<float>(property: Property.ScaleY)
                    .From(1)
                    .KeyframeTo(0.20f, 1)
                    .KeyframeTo(0.40f, 1.1f, BezierCurve.Create(0.7555f, 0.5f, 0.8555f, 0.06f))
                    .KeyframeTo(0.43f, 1.1f, BezierCurve.Create(0.7555f, 0.5f, 0.8555f, 0.06f))
                    .KeyframeTo(0.53f, 1)
                    .KeyframeTo(0.70f, 1.05f, BezierCurve.Create(0.755f, 0.05f, 0.855f, 0.06f))
                    .KeyframeTo(0.80f, 0.95f)
                    .KeyframeTo(0.90f, 1.02f)
                    .KeyframeTo(1, 1f)
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys<float>(property: Property.PositionY)
                    .KeyframeOffset(0.20f, 0f)
                    .KeyframeOffset(0.40f, -30f, BezierCurve.Create(0.7555f, 0.5f, 0.8555f, 0.06f))
                    .KeyframeOffset(0.43f, 0f, BezierCurve.Create(0.7555f, 0.5f, 0.8555f, 0.06f))
                    .KeyframeOffset(0.53f, +30f)
                    .KeyframeOffset(0.70f, -15f, BezierCurve.Create(0.755f, 0.05f, 0.855f, 0.06f))
                    .KeyframeOffset(0.80f, +15f)
                    .KeyframeOffset(0.90f, -4f)
                    .KeyframeOffset(1f, +4f)
                    .EndAnimate()
                    .Build();
            }

            private static TweenSequenceTemplate Flash() {
                return TweenSequenceBuilder.CreateTemplate()
                    .SetDuration(0.5f)
                    .AnimateKeys<float>(property: Property.Opacity)
                    .From(1)
                    .KeyframeTo(0.25f, 0)
                    .KeyframeTo(0.50f, 1)
                    .KeyframeTo(0.75f, 0)
                    .KeyframeTo(1.00f, 1)
                    .EndAnimate()
                    .Build();
            }

            private static TweenSequenceTemplate Headshake() {
                return TweenSequenceBuilder.CreateTemplate()
                    .SetDuration(0.5f)
                    .AnimateKeys<float>(property: Property.PositionY)
                    .KeyframeOffset(0.065f, -6)
                    .KeyframeOffset(0.185f, +5)
                    .KeyframeOffset(0.315f, -3)
                    .KeyframeOffset(0.435f, +2)
                    .KeyframeOffset(0.500f, 0)
                    .KeyframeOffset(1.000f, 0)
                    .EndAnimate()
                    .Parallel()
                    .AnimateKeys<float>(property: Property.RotateCenter)
                    .From(0)
                    .KeyframeTo(0.000f, 0)
                    .KeyframeTo(0.065f, -9)
                    .KeyframeTo(0.185f, +7)
                    .KeyframeTo(0.315f, -5)
                    .KeyframeTo(0.435f, +3)
                    .KeyframeTo(0.500f, 0)
                    .KeyframeTo(1.000f, 0)
                    .EndAnimate()
                    .Build();
            }
        }
    }
}