using System.Collections.Generic;

namespace Tools.Animation {
    public static class Template {
        public static TweenSequenceTemplate Get(string name) => TemplateHolder._templates[name].Get();
        public static readonly TweenSequenceTemplate Bounce = TemplateHolder._templates["bounce"].Get();

        private static class TemplateHolder {
            internal static readonly Dictionary<string, TemplateFactory> _templates;
            static TemplateHolder() {
                TemplateFactory[] factories = {
                    new TemplateFactory("bounce", CreateBounce),
                    new TemplateFactory("bounce2", CreateBounce),
                    new TemplateFactory("bounce3", CreateBounce),
                    new TemplateFactory("bounce4", CreateBounce),
                };
                _templates = new Dictionary<string, TemplateFactory>(factories.Length);
                foreach (var templateFactory in factories) {
                    _templates[templateFactory.Name] = templateFactory;
                }
            }

            internal delegate TweenSequenceTemplate TweenSequenceFactory();

            internal class TemplateFactory {
                private readonly TweenSequenceFactory _factory;
                public string Name;
                private TweenSequenceTemplate _cached;

                public TemplateFactory(string name, TweenSequenceFactory factory) {
                    Name = name;
                    _factory = factory;
                }

                public TweenSequenceTemplate Get() => _cached ??= _factory();
            }

            private static TweenSequenceTemplate CreateBounce() {
                return TweenSequenceBuilder.CreateTemplate()
                    .SetDuration(0.5f)
                    .AnimateKeys<float>(property: "scale:y")
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
                    .AnimateKeys<float>(property: "position:y")
                    .From(0f)
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
        }
    }
}