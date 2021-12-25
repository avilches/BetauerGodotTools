using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private static readonly Dictionary<string, BezierCurve> Beziers = new Dictionary<string, BezierCurve>();

        private static BezierCurve Bezier(float p1x, float p1y, float p2x, float p2y) {
            var bezierCurve = BezierCurve.Create(p1x, p1y, p2x, p2y);
            if (Beziers.ContainsKey(bezierCurve.Name)) {
                return Beziers[bezierCurve.Name];
            }
            Beziers[bezierCurve.Name] = bezierCurve;
            return bezierCurve;
        }
    }

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

        internal static readonly Dictionary<string, TemplateFactory> Factories =
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
    }

    public class TemplateFactory {
        private readonly Func<TweenSequenceTemplate> _factory;
        public readonly string Name;
        private TweenSequenceTemplate _cached;

        public TemplateFactory(string name, Func<TweenSequenceTemplate> factory) {
            Name = name;
            _factory = factory;
            Template.Factories[name.ToLower()] = this;
        }

        public TweenSequenceTemplate Get() => _cached ??= _factory();
    }

    public class TemplateFactory<T> : TemplateFactory {
        private readonly Func<TweenSequenceTemplate> _factory;
        private readonly Func<T, TweenSequenceTemplate> _factory1P;
        private Dictionary<object, TweenSequenceTemplate> _cached;

        public TemplateFactory(string name, Func<TweenSequenceTemplate> factory,
            Func<T, TweenSequenceTemplate> factory1P) : base(name, factory) {
            _factory1P = factory1P;
        }

        public TweenSequenceTemplate Get(T data) {
            _cached ??= new Dictionary<object, TweenSequenceTemplate>();
            _cached.TryGetValue(data, out TweenSequenceTemplate template);
            if (template != null) return template;
            return _cached[data] = _factory1P(data);
        }
    }
}