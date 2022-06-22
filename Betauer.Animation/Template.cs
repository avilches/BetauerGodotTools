using System;
using System.Collections.Generic;

namespace Betauer.Animation {
    public static class Template {
        public static SequenceTemplate Get(string name) => Factories[name.ToLower()].Get();

        public static SequenceTemplate Get<T>(string name, T data) {
            var templateFactory = Factories[name.ToLower()];
            return templateFactory is TemplateFactory<T> templateFactoryTyped
                ? templateFactoryTyped.Get(data)
                : templateFactory.Get();
        }

        public static SequenceTemplate Bounce => BounceFactory.Get();
        public static SequenceTemplate Flash => FlashFactory.Get();
        public static SequenceTemplate HeadShake => HeadShakeFactory.Get();
        public static SequenceTemplate HeartBeat => HeartBeatFactory.Get();
        public static SequenceTemplate Jello => JelloFactory.Get();
        public static SequenceTemplate Pulse => PulseFactory.Get();
        public static SequenceTemplate RubberBand => RubberBandFactory.Get();
        public static SequenceTemplate Shake => ShakeFactory.Get();
        public static SequenceTemplate ShakeX => ShakeXFactory.Get();
        public static SequenceTemplate ShakeY => ShakeYFactory.Get();
        public static SequenceTemplate Swing => SwingFactory.Get();
        public static SequenceTemplate Tada => TadaFactory.Get();
        public static SequenceTemplate Wobble => WobbleFactory.Get();

        public static SequenceTemplate BackInUp => BackInUpFactory.Get();
        public static SequenceTemplate BackInDown => BackInDownFactory.Get();
        public static SequenceTemplate BackInLeft => BackInLeftFactory.Get();
        public static SequenceTemplate BackInRight => BackInRightFactory.Get();

        public static SequenceTemplate BackOutUp => BackOutUpFactory.Get();
        public static SequenceTemplate BackOutDown => BackOutDownFactory.Get();
        public static SequenceTemplate BackOutLeft => BackOutLeftFactory.Get();
        public static SequenceTemplate BackOutRight => BackOutRightFactory.Get();

        public static SequenceTemplate BounceIn => BounceInFactory.Get();
        public static SequenceTemplate BounceInUp => BounceInUpFactory.Get();
        public static SequenceTemplate BounceInDown => BounceInDownFactory.Get();
        public static SequenceTemplate BounceInLeft => BounceInLeftFactory.Get();
        public static SequenceTemplate BounceInRight => BounceInRightFactory.Get();

        public static SequenceTemplate BounceOut => BounceOutFactory.Get();
        public static SequenceTemplate BounceOutUp => BounceOutUpFactory.Get();
        public static SequenceTemplate BounceOutDown => BounceOutDownFactory.Get();
        public static SequenceTemplate BounceOutLeft => BounceOutLeftFactory.Get();
        public static SequenceTemplate BounceOutRight => BounceOutRightFactory.Get();

        public static SequenceTemplate FadeIn => FadeInFactory.Get();
        public static SequenceTemplate FadeInUp => FadeInUpFactory.Get();
        public static SequenceTemplate FadeInDown => FadeInDownFactory.Get();
        public static SequenceTemplate FadeInLeft => FadeInLeftFactory.Get();
        public static SequenceTemplate FadeInRight => FadeInRightFactory.Get();
        public static SequenceTemplate FadeInTopLeft => FadeInTopLeftFactory.Get();
        public static SequenceTemplate FadeInTopRight => FadeInTopRightFactory.Get();
        public static SequenceTemplate FadeInBottomLeft => FadeInBottomLeftFactory.Get();
        public static SequenceTemplate FadeInBottomRight => FadeInBottomRightFactory.Get();
        public static SequenceTemplate FadeInUpBig => FadeInUpBigFactory.Get();
        public static SequenceTemplate FadeInDownBig => FadeInDownBigFactory.Get();
        public static SequenceTemplate FadeInLeftBig => FadeInLeftBigFactory.Get();
        public static SequenceTemplate FadeInRightBig => FadeInRightBigFactory.Get();

        public static SequenceTemplate FadeOut => FadeOutFactory.Get();
        public static SequenceTemplate FadeOutUp => FadeOutUpFactory.Get();
        public static SequenceTemplate FadeOutDown => FadeOutDownFactory.Get();
        public static SequenceTemplate FadeOutLeft => FadeOutLeftFactory.Get();
        public static SequenceTemplate FadeOutRight => FadeOutRightFactory.Get();
        public static SequenceTemplate FadeOutTopLeft => FadeOutTopLeftFactory.Get();
        public static SequenceTemplate FadeOutTopRight => FadeOutTopRightFactory.Get();
        public static SequenceTemplate FadeOutBottomLeft => FadeOutBottomLeftFactory.Get();
        public static SequenceTemplate FadeOutBottomRight => FadeOutBottomRightFactory.Get();
        public static SequenceTemplate FadeOutUpBig => FadeOutUpBigFactory.Get();
        public static SequenceTemplate FadeOutDownBig => FadeOutDownBigFactory.Get();
        public static SequenceTemplate FadeOutLeftBig => FadeOutLeftBigFactory.Get();
        public static SequenceTemplate FadeOutRightBig => FadeOutRightBigFactory.Get();

        public static SequenceTemplate LightSpeedInLeft => LightSpeedInLeftFactory.Get();
        public static SequenceTemplate LightSpeedInRight => LightSpeedInRightFactory.Get();
        public static SequenceTemplate LightSpeedOutLeft => LightSpeedOutLeftFactory.Get();
        public static SequenceTemplate LightSpeedOutRight => LightSpeedOutRightFactory.Get();

        public static SequenceTemplate RotateIn => RotateInFactory.Get();
        public static SequenceTemplate RotateInDownLeft => RotateInDownLeftFactory.Get();
        public static SequenceTemplate RotateInDownRight => RotateInDownRightFactory.Get();
        public static SequenceTemplate RotateInUpLeft => RotateInUpLeftFactory.Get();
        public static SequenceTemplate RotateInUpRight => RotateInUpRightFactory.Get();

        public static SequenceTemplate RotateOut => RotateOutFactory.Get();
        public static SequenceTemplate RotateOutDownLeft => RotateOutDownLeftFactory.Get();
        public static SequenceTemplate RotateOutDownRight => RotateOutDownRightFactory.Get();
        public static SequenceTemplate RotateOutUpLeft => RotateOutUpLeftFactory.Get();
        public static SequenceTemplate RotateOutUpRight => RotateOutUpRightFactory.Get();

        public static SequenceTemplate Hinge => HingeFactory.Get();
        public static SequenceTemplate JackInTheBox => JackInTheBoxFactory.Get();
        public static SequenceTemplate RollOutLeft => RollOutLeftFactory.Get();
        public static SequenceTemplate RollOutRight => RollOutRightFactory.Get();
        public static SequenceTemplate RollInLeft => RollInLeftFactory.Get();
        public static SequenceTemplate RollInRight => RollInRightFactory.Get();

        internal static readonly Dictionary<string, TemplateFactory> Factories =
            new Dictionary<string, TemplateFactory>();

        public static ICollection<TemplateFactory> GetAllTemplates() => Factories.Values;

        private const string AttentionSeeker = "Attention Seeker";
        public static readonly TemplateFactory BounceFactory = new TemplateFactory(AttentionSeeker, nameof(Bounce), Templates.Bounce);
        public static readonly TemplateFactory FlashFactory = new TemplateFactory(AttentionSeeker, nameof(Flash), Templates.Flash);
        public static readonly TemplateFactory HeadShakeFactory = new TemplateFactory(AttentionSeeker, nameof(HeadShake), Templates.HeadShake);
        public static readonly TemplateFactory HeartBeatFactory = new TemplateFactory(AttentionSeeker, nameof(HeartBeat), Templates.HeartBeat);
        public static readonly TemplateFactory JelloFactory = new TemplateFactory(AttentionSeeker, nameof(Jello), Templates.Jello);
        public static readonly TemplateFactory PulseFactory = new TemplateFactory(AttentionSeeker, nameof(Pulse), Templates.Pulse);
        public static readonly TemplateFactory RubberBandFactory = new TemplateFactory(AttentionSeeker, nameof(RubberBand), Templates.RubberBand);
        public static readonly TemplateFactory ShakeFactory = new TemplateFactory(AttentionSeeker, nameof(Shake), Templates.Shake);
        public static readonly TemplateFactory ShakeXFactory = new TemplateFactory(AttentionSeeker, nameof(ShakeX), Templates.ShakeX);
        public static readonly TemplateFactory ShakeYFactory = new TemplateFactory(AttentionSeeker, nameof(ShakeY), Templates.ShakeY);
        public static readonly TemplateFactory SwingFactory = new TemplateFactory(AttentionSeeker, nameof(Swing), Templates.Swing);
        public static readonly TemplateFactory TadaFactory = new TemplateFactory(AttentionSeeker, nameof(Tada), Templates.Tada);
        public static readonly TemplateFactory WobbleFactory = new TemplateFactory(AttentionSeeker, nameof(Wobble), Templates.Wobble);

        private const string BackEntrances = "Back Entrance";
        public static readonly TemplateFactory<float> BackInUpFactory = new TemplateFactory<float>(BackEntrances, nameof(BackInUp), () => Templates.BackInUp(), Templates.BackInUp);
        public static readonly TemplateFactory<float> BackInDownFactory = new TemplateFactory<float>(BackEntrances, nameof(BackInDown), () => Templates.BackInDown(), Templates.BackInDown);
        public static readonly TemplateFactory<float> BackInLeftFactory = new TemplateFactory<float>(BackEntrances, nameof(BackInLeft), () => Templates.BackInLeft(), Templates.BackInLeft);
        public static readonly TemplateFactory<float> BackInRightFactory = new TemplateFactory<float>(BackEntrances, nameof(BackInRight), () => Templates.BackInRight(), Templates.BackInRight);

        private const string BackExits = "Back Exits";
        public static readonly TemplateFactory<float> BackOutUpFactory = new TemplateFactory<float>(BackExits, nameof(BackOutUp), () => Templates.BackOutUp(), Templates.BackOutUp);
        public static readonly TemplateFactory<float> BackOutDownFactory = new TemplateFactory<float>(BackExits, nameof(BackOutDown), () => Templates.BackOutDown(), Templates.BackOutDown);
        public static readonly TemplateFactory<float> BackOutLeftFactory = new TemplateFactory<float>(BackExits, nameof(BackOutLeft), () => Templates.BackOutLeft(), Templates.BackOutLeft);
        public static readonly TemplateFactory<float> BackOutRightFactory = new TemplateFactory<float>(BackExits, nameof(BackOutRight), () => Templates.BackOutRight(), Templates.BackOutRight);

        private const string BounceEntrances = "Bounce Entrance";
        public static readonly TemplateFactory BounceInFactory = new TemplateFactory(BounceEntrances, nameof(BounceIn), Templates.BounceIn);
        public static readonly TemplateFactory<float> BounceInUpFactory = new TemplateFactory<float>(BounceEntrances, nameof(BounceInUp), () => Templates.BounceInUp(), Templates.BounceInUp);
        public static readonly TemplateFactory<float> BounceInDownFactory = new TemplateFactory<float>(BounceEntrances, nameof(BounceInDown), () => Templates.BounceInDown(), Templates.BounceInDown);
        public static readonly TemplateFactory<float> BounceInLeftFactory = new TemplateFactory<float>(BounceEntrances, nameof(BounceInLeft), () => Templates.BounceInLeft(), Templates.BounceInLeft);
        public static readonly TemplateFactory<float> BounceInRightFactory = new TemplateFactory<float>(BounceEntrances, nameof(BounceInRight), () => Templates.BounceInRight(), Templates.BounceInRight);

        private const string BounceExits = "Bounce Exits";
        public static readonly TemplateFactory BounceOutFactory = new TemplateFactory(BounceExits, nameof(BounceOut), Templates.BounceOut);
        public static readonly TemplateFactory<float> BounceOutUpFactory = new TemplateFactory<float>(BounceExits, nameof(BounceOutUp), () => Templates.BounceOutUp(), Templates.BounceOutUp);
        public static readonly TemplateFactory<float> BounceOutDownFactory = new TemplateFactory<float>(BounceExits, nameof(BounceOutDown), () => Templates.BounceOutDown(), Templates.BounceOutDown);
        public static readonly TemplateFactory<float> BounceOutLeftFactory = new TemplateFactory<float>(BounceExits, nameof(BounceOutLeft), () => Templates.BounceOutLeft(), Templates.BounceOutLeft);
        public static readonly TemplateFactory<float> BounceOutRightFactory = new TemplateFactory<float>(BounceExits, nameof(BounceOutRight), () => Templates.BounceOutRight(), Templates.BounceOutRight);

        private const string FadingEntrances = "Fading Entrance";
        public static readonly TemplateFactory FadeInFactory = new TemplateFactory(FadingEntrances, nameof(FadeIn), Templates.FadeIn);
        public static readonly TemplateFactory FadeInUpFactory = new TemplateFactory(FadingEntrances, nameof(FadeInUp), Templates.FadeInUp);
        public static readonly TemplateFactory FadeInDownFactory = new TemplateFactory(FadingEntrances, nameof(FadeInDown), Templates.FadeInDown);
        public static readonly TemplateFactory FadeInLeftFactory = new TemplateFactory(FadingEntrances, nameof(FadeInLeft), Templates.FadeInLeft);
        public static readonly TemplateFactory FadeInRightFactory = new TemplateFactory(FadingEntrances, nameof(FadeInRight), Templates.FadeInRight);
        public static readonly TemplateFactory FadeInTopLeftFactory = new TemplateFactory(FadingEntrances, nameof(FadeInTopLeft), Templates.FadeInTopLeft);
        public static readonly TemplateFactory FadeInTopRightFactory = new TemplateFactory(FadingEntrances, nameof(FadeInTopRight), Templates.FadeInTopRight);
        public static readonly TemplateFactory FadeInBottomLeftFactory = new TemplateFactory(FadingEntrances, nameof(FadeInBottomLeft), Templates.FadeInBottomLeft);
        public static readonly TemplateFactory FadeInBottomRightFactory = new TemplateFactory(FadingEntrances, nameof(FadeInBottomRight), Templates.FadeInBottomRight);
        public static readonly TemplateFactory<float> FadeInUpBigFactory = new TemplateFactory<float>(FadingEntrances, nameof(FadeInUpBig), () => Templates.FadeInUpBig(), Templates.FadeInUpBig);
        public static readonly TemplateFactory<float> FadeInDownBigFactory = new TemplateFactory<float>(FadingEntrances, nameof(FadeInDownBig), () => Templates.FadeInDownBig(), Templates.FadeInDownBig);
        public static readonly TemplateFactory<float> FadeInLeftBigFactory = new TemplateFactory<float>(FadingEntrances, nameof(FadeInLeftBig), () => Templates.FadeInLeftBig(), Templates.FadeInLeftBig);
        public static readonly TemplateFactory<float> FadeInRightBigFactory = new TemplateFactory<float>(FadingEntrances, nameof(FadeInRightBig), () => Templates.FadeInRightBig(), Templates.FadeInRightBig);

        private const string FadingExits = "Fading Exits";
        public static readonly TemplateFactory FadeOutFactory = new TemplateFactory(FadingExits, nameof(FadeOut), Templates.FadeOut);
        public static readonly TemplateFactory FadeOutUpFactory = new TemplateFactory(FadingExits, nameof(FadeOutUp), Templates.FadeOutUp);
        public static readonly TemplateFactory FadeOutDownFactory = new TemplateFactory(FadingExits, nameof(FadeOutDown), Templates.FadeOutDown);
        public static readonly TemplateFactory FadeOutLeftFactory = new TemplateFactory(FadingExits, nameof(FadeOutLeft), Templates.FadeOutLeft);
        public static readonly TemplateFactory FadeOutRightFactory = new TemplateFactory(FadingExits, nameof(FadeOutRight), Templates.FadeOutRight);
        public static readonly TemplateFactory FadeOutTopLeftFactory = new TemplateFactory(FadingExits, nameof(FadeOutTopLeft), Templates.FadeOutTopLeft);
        public static readonly TemplateFactory FadeOutTopRightFactory = new TemplateFactory(FadingExits, nameof(FadeOutTopRight), Templates.FadeOutTopRight);
        public static readonly TemplateFactory FadeOutBottomLeftFactory = new TemplateFactory(FadingExits, nameof(FadeOutBottomLeft), Templates.FadeOutBottomLeft);
        public static readonly TemplateFactory FadeOutBottomRightFactory = new TemplateFactory(FadingExits, nameof(FadeOutBottomRight), Templates.FadeOutBottomRight);
        public static readonly TemplateFactory<float> FadeOutUpBigFactory = new TemplateFactory<float>(FadingExits, nameof(FadeOutUpBig), () => Templates.FadeOutUpBig(), Templates.FadeOutUpBig);
        public static readonly TemplateFactory<float> FadeOutDownBigFactory = new TemplateFactory<float>(FadingExits, nameof(FadeOutDownBig), () => Templates.FadeOutDownBig(), Templates.FadeOutDownBig);
        public static readonly TemplateFactory<float> FadeOutLeftBigFactory = new TemplateFactory<float>(FadingExits, nameof(FadeOutLeftBig), () => Templates.FadeOutLeftBig(), Templates.FadeOutLeftBig);
        public static readonly TemplateFactory<float> FadeOutRightBigFactory = new TemplateFactory<float>(FadingExits, nameof(FadeOutRightBig), () => Templates.FadeOutRightBig(), Templates.FadeOutRightBig);

        private const string Lightspeed = "Lightspeed";
        public static readonly TemplateFactory LightSpeedInLeftFactory = new TemplateFactory(Lightspeed, nameof(LightSpeedInLeft), Templates.LightSpeedInLeft);
        public static readonly TemplateFactory LightSpeedInRightFactory = new TemplateFactory(Lightspeed, nameof(LightSpeedInRight), Templates.LightSpeedInRight);
        public static readonly TemplateFactory LightSpeedOutLeftFactory = new TemplateFactory(Lightspeed, nameof(LightSpeedOutLeft), Templates.LightSpeedOutLeft);
        public static readonly TemplateFactory LightSpeedOutRightFactory = new TemplateFactory(Lightspeed, nameof(LightSpeedOutRight), Templates.LightSpeedOutRight);

        private const string RotatingEntrance = "Rotating Entrance";
        public static readonly TemplateFactory RotateInFactory = new TemplateFactory(RotatingEntrance, nameof(RotateIn), Templates.RotateIn);
        public static readonly TemplateFactory RotateInDownLeftFactory = new TemplateFactory(RotatingEntrance, nameof(RotateInDownLeft), Templates.RotateInDownLeft);
        public static readonly TemplateFactory RotateInDownRightFactory = new TemplateFactory(RotatingEntrance, nameof(RotateInDownRight), Templates.RotateInDownRight);
        public static readonly TemplateFactory RotateInUpLeftFactory = new TemplateFactory(RotatingEntrance, nameof(RotateInUpLeft), Templates.RotateInUpLeft);
        public static readonly TemplateFactory RotateInUpRightFactory = new TemplateFactory(RotatingEntrance, nameof(RotateInUpRight), Templates.RotateInUpRight);

        private const string RotatingExits = "Rotating Exits";
        public static readonly TemplateFactory RotateOutFactory = new TemplateFactory(RotatingExits, nameof(RotateOut), Templates.RotateOut);
        public static readonly TemplateFactory RotateOutDownLeftFactory = new TemplateFactory(RotatingExits, nameof(RotateOutDownLeft), Templates.RotateOutDownLeft);
        public static readonly TemplateFactory RotateOutDownRightFactory = new TemplateFactory(RotatingExits, nameof(RotateOutDownRight), Templates.RotateOutDownRight);
        public static readonly TemplateFactory RotateOutUpLeftFactory = new TemplateFactory(RotatingExits, nameof(RotateOutUpLeft), Templates.RotateOutUpLeft);
        public static readonly TemplateFactory RotateOutUpRightFactory = new TemplateFactory(RotatingExits, nameof(RotateOutUpRight), Templates.RotateOutUpRight);

        private const string Special = "Special";
        public static readonly TemplateFactory HingeFactory = new TemplateFactory(Special, nameof(Hinge), Templates.Hinge);
        public static readonly TemplateFactory JackInTheBoxFactory = new TemplateFactory(Special, nameof(JackInTheBox), Templates.JackInTheBox);
        public static readonly TemplateFactory RollOutLeftFactory = new TemplateFactory(Special, nameof(RollOutLeft), Templates.RollOutLeft);
        public static readonly TemplateFactory RollOutRightFactory = new TemplateFactory(Special, nameof(RollOutRight), Templates.RollOutRight);
        public static readonly TemplateFactory RollInLeftFactory = new TemplateFactory(Special, nameof(RollInLeft), Templates.RollInLeft);
        public static readonly TemplateFactory RollInRightFactory = new TemplateFactory(Special, nameof(RollInRight), Templates.RollInRight);

    }

    public class TemplateFactory {
        private readonly Func<SequenceTemplate> _factory;
        public readonly string Name;
        public readonly string? Category;
        private SequenceTemplate? _cached;

        public TemplateFactory(string name, Func<SequenceTemplate> factory) : this(null, name, factory) {
        }

        public TemplateFactory(string? category, string name, Func<SequenceTemplate> factory) {
            Name = name;
            Category = category;
            _factory = factory;
            Template.Factories[name.ToLower()] = this;
        }

        public SequenceTemplate Get() => _cached ??= _factory();
    }

    public class TemplateFactory<T> : TemplateFactory {
        private readonly Func<T, SequenceTemplate> _factoryWithOneParameter;
        private Dictionary<object, SequenceTemplate>? _cached;

        public TemplateFactory(string name, Func<SequenceTemplate> factory,
            Func<T, SequenceTemplate> factoryWithOneParameter) : this(null, name, factory, factoryWithOneParameter) {
        }

        public TemplateFactory(string? category, string name, Func<SequenceTemplate> factory,
            Func<T, SequenceTemplate> factoryWithOneParameter) : base(category, name, factory) {
            _factoryWithOneParameter = factoryWithOneParameter;
        }

        public SequenceTemplate Get(T data) {
            _cached ??= new Dictionary<object, SequenceTemplate>();
            _cached.TryGetValue(data, out SequenceTemplate template);
            if (template != null) return template;
            return _cached[data] = _factoryWithOneParameter(data);
        }
    }
}