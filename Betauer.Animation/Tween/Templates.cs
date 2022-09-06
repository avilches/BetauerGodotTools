using System;
using System.Collections.Generic;

namespace Betauer.Animation.Tween {
    public static class Templates {
        public static KeyframeAnimation? Get(string name) => Get<KeyframeAnimation>(name);
        
        public static T? Get<T>(string name) where T : class => (Factories[name.ToLower()] as TemplateFactory<T>)?.Get();

        public static T? Get<TParam, T>(string name, TParam data) where T : class {
            var templateFactory = Factories[name.ToLower()];
            return templateFactory switch {
                TemplateFactory<TParam, T> templateFactoryTyped => templateFactoryTyped.Get(data),
                TemplateFactory<T> tp => tp.Get(),
                _ => null
            };
        }

        public static KeyframeAnimation Bounce => BounceFactory.Get();
        public static KeyframeAnimation Flash => FlashFactory.Get();
        public static KeyframeAnimation HeadShake => HeadShakeFactory.Get();
        public static KeyframeAnimation HeartBeat => HeartBeatFactory.Get();
        public static KeyframeAnimation Jello => JelloFactory.Get();
        public static KeyframeAnimation Pulse => PulseFactory.Get();
        public static KeyframeAnimation RubberBand => RubberBandFactory.Get();
        public static KeyframeAnimation Shake => ShakeFactory.Get();
        public static KeyframeAnimation ShakeX => ShakeXFactory.Get();
        public static KeyframeAnimation ShakeY => ShakeYFactory.Get();
        public static KeyframeAnimation Swing => SwingFactory.Get();
        public static KeyframeAnimation Tada => TadaFactory.Get();
        public static KeyframeAnimation Wobble => WobbleFactory.Get();

        public static KeyframeAnimation BackInUp => BackInUpFactory.Get();
        public static KeyframeAnimation BackInDown => BackInDownFactory.Get();
        public static KeyframeAnimation BackInLeft => BackInLeftFactory.Get();
        public static KeyframeAnimation BackInRight => BackInRightFactory.Get();

        public static KeyframeAnimation BackOutUp => BackOutUpFactory.Get();
        public static KeyframeAnimation BackOutDown => BackOutDownFactory.Get();
        public static KeyframeAnimation BackOutLeft => BackOutLeftFactory.Get();
        public static KeyframeAnimation BackOutRight => BackOutRightFactory.Get();

        public static KeyframeAnimation BounceIn => BounceInFactory.Get();
        public static KeyframeAnimation BounceInUp => BounceInUpFactory.Get();
        public static KeyframeAnimation BounceInDown => BounceInDownFactory.Get();
        public static KeyframeAnimation BounceInLeft => BounceInLeftFactory.Get();
        public static KeyframeAnimation BounceInRight => BounceInRightFactory.Get();

        public static KeyframeAnimation BounceOut => BounceOutFactory.Get();
        public static KeyframeAnimation BounceOutUp => BounceOutUpFactory.Get();
        public static KeyframeAnimation BounceOutDown => BounceOutDownFactory.Get();
        public static KeyframeAnimation BounceOutLeft => BounceOutLeftFactory.Get();
        public static KeyframeAnimation BounceOutRight => BounceOutRightFactory.Get();

        public static KeyframeAnimation FadeIn => FadeInFactory.Get();
        public static KeyframeAnimation FadeInUp => FadeInUpFactory.Get();
        public static KeyframeAnimation FadeInDown => FadeInDownFactory.Get();
        public static KeyframeAnimation FadeInLeft => FadeInLeftFactory.Get();
        public static KeyframeAnimation FadeInRight => FadeInRightFactory.Get();
        public static KeyframeAnimation FadeInTopLeft => FadeInTopLeftFactory.Get();
        public static KeyframeAnimation FadeInTopRight => FadeInTopRightFactory.Get();
        public static KeyframeAnimation FadeInBottomLeft => FadeInBottomLeftFactory.Get();
        public static KeyframeAnimation FadeInBottomRight => FadeInBottomRightFactory.Get();
        public static KeyframeAnimation FadeInUpBig => FadeInUpBigFactory.Get();
        public static KeyframeAnimation FadeInDownBig => FadeInDownBigFactory.Get();
        public static KeyframeAnimation FadeInLeftBig => FadeInLeftBigFactory.Get();
        public static KeyframeAnimation FadeInRightBig => FadeInRightBigFactory.Get();

        public static KeyframeAnimation FadeOut => FadeOutFactory.Get();
        public static KeyframeAnimation FadeOutUp => FadeOutUpFactory.Get();
        public static KeyframeAnimation FadeOutDown => FadeOutDownFactory.Get();
        public static KeyframeAnimation FadeOutLeft => FadeOutLeftFactory.Get();
        public static KeyframeAnimation FadeOutRight => FadeOutRightFactory.Get();
        public static KeyframeAnimation FadeOutTopLeft => FadeOutTopLeftFactory.Get();
        public static KeyframeAnimation FadeOutTopRight => FadeOutTopRightFactory.Get();
        public static KeyframeAnimation FadeOutBottomLeft => FadeOutBottomLeftFactory.Get();
        public static KeyframeAnimation FadeOutBottomRight => FadeOutBottomRightFactory.Get();
        public static KeyframeAnimation FadeOutUpBig => FadeOutUpBigFactory.Get();
        public static KeyframeAnimation FadeOutDownBig => FadeOutDownBigFactory.Get();
        public static KeyframeAnimation FadeOutLeftBig => FadeOutLeftBigFactory.Get();
        public static KeyframeAnimation FadeOutRightBig => FadeOutRightBigFactory.Get();

        public static KeyframeAnimation LightSpeedInLeft => LightSpeedInLeftFactory.Get();
        public static KeyframeAnimation LightSpeedInRight => LightSpeedInRightFactory.Get();
        public static KeyframeAnimation LightSpeedOutLeft => LightSpeedOutLeftFactory.Get();
        public static KeyframeAnimation LightSpeedOutRight => LightSpeedOutRightFactory.Get();

        public static KeyframeAnimation RotateIn => RotateInFactory.Get();
        public static KeyframeAnimation RotateInDownLeft => RotateInDownLeftFactory.Get();
        public static KeyframeAnimation RotateInDownRight => RotateInDownRightFactory.Get();
        public static KeyframeAnimation RotateInUpLeft => RotateInUpLeftFactory.Get();
        public static KeyframeAnimation RotateInUpRight => RotateInUpRightFactory.Get();

        public static KeyframeAnimation RotateOut => RotateOutFactory.Get();
        public static KeyframeAnimation RotateOutDownLeft => RotateOutDownLeftFactory.Get();
        public static KeyframeAnimation RotateOutDownRight => RotateOutDownRightFactory.Get();
        public static KeyframeAnimation RotateOutUpLeft => RotateOutUpLeftFactory.Get();
        public static KeyframeAnimation RotateOutUpRight => RotateOutUpRightFactory.Get();

        public static KeyframeAnimation Hinge => HingeFactory.Get();
        public static KeyframeAnimation JackInTheBox => JackInTheBoxFactory.Get();
        public static KeyframeAnimation RollOutLeft => RollOutLeftFactory.Get();
        public static KeyframeAnimation RollOutRight => RollOutRightFactory.Get();
        public static KeyframeAnimation RollInLeft => RollInLeftFactory.Get();
        public static KeyframeAnimation RollInRight => RollInRightFactory.Get();

        internal static TemplateFactory<T> CreateTemplateFactory<T>(string category, string name, Func<T> factory) where T : class {
            var template = new TemplateFactory<T>(category, name, factory);
            Factories[name.ToLower()] = template;
            return template;
        }
            
        internal static TemplateFactory<TParam, T> CreateTemplateFactory<TParam, T>(string category, string name, Func<T> factory, Func<TParam, T> factory1P) where T : class {
            var template = new TemplateFactory<TParam, T>(category, name, factory, factory1P);
            Factories[name.ToLower()] = template;
            return template;
        }
            
        internal static readonly Dictionary<string, TemplateFactory> Factories =
            new Dictionary<string, TemplateFactory>();

        public static ICollection<TemplateFactory> GetAllTemplates() => Factories.Values;

        private const string AttentionSeeker = "Attention Seeker";
        public static readonly TemplateFactory<KeyframeAnimation> BounceFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(Bounce), TemplateFactories.Bounce);
        public static readonly TemplateFactory<KeyframeAnimation> FlashFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(Flash), TemplateFactories.Flash);
        public static readonly TemplateFactory<KeyframeAnimation> HeadShakeFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(HeadShake), TemplateFactories.HeadShake);
        public static readonly TemplateFactory<KeyframeAnimation> HeartBeatFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(HeartBeat), TemplateFactories.HeartBeat);
        public static readonly TemplateFactory<KeyframeAnimation> JelloFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(Jello), TemplateFactories.Jello);
        public static readonly TemplateFactory<KeyframeAnimation> PulseFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(Pulse), TemplateFactories.Pulse);
        public static readonly TemplateFactory<KeyframeAnimation> RubberBandFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(RubberBand), TemplateFactories.RubberBand);
        public static readonly TemplateFactory<KeyframeAnimation> ShakeFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(Shake), TemplateFactories.Shake);
        public static readonly TemplateFactory<KeyframeAnimation> ShakeXFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(ShakeX), TemplateFactories.ShakeX);
        public static readonly TemplateFactory<KeyframeAnimation> ShakeYFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(ShakeY), TemplateFactories.ShakeY);
        public static readonly TemplateFactory<KeyframeAnimation> SwingFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(Swing), TemplateFactories.Swing);
        public static readonly TemplateFactory<KeyframeAnimation> TadaFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(Tada), TemplateFactories.Tada);
        public static readonly TemplateFactory<KeyframeAnimation> WobbleFactory = CreateTemplateFactory<KeyframeAnimation>(AttentionSeeker, nameof(Wobble), TemplateFactories.Wobble);

        private const string BackEntrances = "Back Entrance";
        public static readonly TemplateFactory<float, KeyframeAnimation> BackInUpFactory = CreateTemplateFactory<float, KeyframeAnimation>(BackEntrances, nameof(BackInUp), () => TemplateFactories.BackInUp(), TemplateFactories.BackInUp);
        public static readonly TemplateFactory<float, KeyframeAnimation> BackInDownFactory = CreateTemplateFactory<float, KeyframeAnimation>(BackEntrances, nameof(BackInDown), () => TemplateFactories.BackInDown(), TemplateFactories.BackInDown);
        public static readonly TemplateFactory<float, KeyframeAnimation> BackInLeftFactory = CreateTemplateFactory<float, KeyframeAnimation>(BackEntrances, nameof(BackInLeft), () => TemplateFactories.BackInLeft(), TemplateFactories.BackInLeft);
        public static readonly TemplateFactory<float, KeyframeAnimation> BackInRightFactory = CreateTemplateFactory<float, KeyframeAnimation>(BackEntrances, nameof(BackInRight), () => TemplateFactories.BackInRight(), TemplateFactories.BackInRight);

        private const string BackExits = "Back Exits";
        public static readonly TemplateFactory<float, KeyframeAnimation> BackOutUpFactory = CreateTemplateFactory<float, KeyframeAnimation>(BackExits, nameof(BackOutUp), () => TemplateFactories.BackOutUp(), TemplateFactories.BackOutUp);
        public static readonly TemplateFactory<float, KeyframeAnimation> BackOutDownFactory = CreateTemplateFactory<float, KeyframeAnimation>(BackExits, nameof(BackOutDown), () => TemplateFactories.BackOutDown(), TemplateFactories.BackOutDown);
        public static readonly TemplateFactory<float, KeyframeAnimation> BackOutLeftFactory = CreateTemplateFactory<float, KeyframeAnimation>(BackExits, nameof(BackOutLeft), () => TemplateFactories.BackOutLeft(), TemplateFactories.BackOutLeft);
        public static readonly TemplateFactory<float, KeyframeAnimation> BackOutRightFactory = CreateTemplateFactory<float, KeyframeAnimation>(BackExits, nameof(BackOutRight), () => TemplateFactories.BackOutRight(), TemplateFactories.BackOutRight);

        private const string BounceEntrances = "Bounce Entrance";
        public static readonly TemplateFactory<KeyframeAnimation> BounceInFactory = CreateTemplateFactory<KeyframeAnimation>(BounceEntrances, nameof(BounceIn), TemplateFactories.BounceIn);
        public static readonly TemplateFactory<float, KeyframeAnimation> BounceInUpFactory = CreateTemplateFactory<float, KeyframeAnimation>(BounceEntrances, nameof(BounceInUp), () => TemplateFactories.BounceInUp(), TemplateFactories.BounceInUp);
        public static readonly TemplateFactory<float, KeyframeAnimation> BounceInDownFactory = CreateTemplateFactory<float, KeyframeAnimation>(BounceEntrances, nameof(BounceInDown), () => TemplateFactories.BounceInDown(), TemplateFactories.BounceInDown);
        public static readonly TemplateFactory<float, KeyframeAnimation> BounceInLeftFactory = CreateTemplateFactory<float, KeyframeAnimation>(BounceEntrances, nameof(BounceInLeft), () => TemplateFactories.BounceInLeft(), TemplateFactories.BounceInLeft);
        public static readonly TemplateFactory<float, KeyframeAnimation> BounceInRightFactory = CreateTemplateFactory<float, KeyframeAnimation>(BounceEntrances, nameof(BounceInRight), () => TemplateFactories.BounceInRight(), TemplateFactories.BounceInRight);

        private const string BounceExits = "Bounce Exits";
        public static readonly TemplateFactory<KeyframeAnimation> BounceOutFactory = CreateTemplateFactory<KeyframeAnimation>(BounceExits, nameof(BounceOut), TemplateFactories.BounceOut);
        public static readonly TemplateFactory<float, KeyframeAnimation> BounceOutUpFactory = CreateTemplateFactory<float, KeyframeAnimation>(BounceExits, nameof(BounceOutUp), () => TemplateFactories.BounceOutUp(), TemplateFactories.BounceOutUp);
        public static readonly TemplateFactory<float, KeyframeAnimation> BounceOutDownFactory = CreateTemplateFactory<float, KeyframeAnimation>(BounceExits, nameof(BounceOutDown), () => TemplateFactories.BounceOutDown(), TemplateFactories.BounceOutDown);
        public static readonly TemplateFactory<float, KeyframeAnimation> BounceOutLeftFactory = CreateTemplateFactory<float, KeyframeAnimation>(BounceExits, nameof(BounceOutLeft), () => TemplateFactories.BounceOutLeft(), TemplateFactories.BounceOutLeft);
        public static readonly TemplateFactory<float, KeyframeAnimation> BounceOutRightFactory = CreateTemplateFactory<float, KeyframeAnimation>(BounceExits, nameof(BounceOutRight), () => TemplateFactories.BounceOutRight(), TemplateFactories.BounceOutRight);

        private const string FadingEntrances = "Fading Entrance";
        public static readonly TemplateFactory<KeyframeAnimation> FadeInFactory = CreateTemplateFactory<KeyframeAnimation>(FadingEntrances, nameof(FadeIn), TemplateFactories.FadeIn);
        public static readonly TemplateFactory<KeyframeAnimation> FadeInUpFactory = CreateTemplateFactory<KeyframeAnimation>(FadingEntrances, nameof(FadeInUp), TemplateFactories.FadeInUp);
        public static readonly TemplateFactory<KeyframeAnimation> FadeInDownFactory = CreateTemplateFactory<KeyframeAnimation>(FadingEntrances, nameof(FadeInDown), TemplateFactories.FadeInDown);
        public static readonly TemplateFactory<KeyframeAnimation> FadeInLeftFactory = CreateTemplateFactory<KeyframeAnimation>(FadingEntrances, nameof(FadeInLeft), TemplateFactories.FadeInLeft);
        public static readonly TemplateFactory<KeyframeAnimation> FadeInRightFactory = CreateTemplateFactory<KeyframeAnimation>(FadingEntrances, nameof(FadeInRight), TemplateFactories.FadeInRight);
        public static readonly TemplateFactory<KeyframeAnimation> FadeInTopLeftFactory = CreateTemplateFactory<KeyframeAnimation>(FadingEntrances, nameof(FadeInTopLeft), TemplateFactories.FadeInTopLeft);
        public static readonly TemplateFactory<KeyframeAnimation> FadeInTopRightFactory = CreateTemplateFactory<KeyframeAnimation>(FadingEntrances, nameof(FadeInTopRight), TemplateFactories.FadeInTopRight);
        public static readonly TemplateFactory<KeyframeAnimation> FadeInBottomLeftFactory = CreateTemplateFactory<KeyframeAnimation>(FadingEntrances, nameof(FadeInBottomLeft), TemplateFactories.FadeInBottomLeft);
        public static readonly TemplateFactory<KeyframeAnimation> FadeInBottomRightFactory = CreateTemplateFactory<KeyframeAnimation>(FadingEntrances, nameof(FadeInBottomRight), TemplateFactories.FadeInBottomRight);
        public static readonly TemplateFactory<float, KeyframeAnimation> FadeInUpBigFactory = CreateTemplateFactory<float, KeyframeAnimation>(FadingEntrances, nameof(FadeInUpBig), () => TemplateFactories.FadeInUpBig(), TemplateFactories.FadeInUpBig);
        public static readonly TemplateFactory<float, KeyframeAnimation> FadeInDownBigFactory = CreateTemplateFactory<float, KeyframeAnimation>(FadingEntrances, nameof(FadeInDownBig), () => TemplateFactories.FadeInDownBig(), TemplateFactories.FadeInDownBig);
        public static readonly TemplateFactory<float, KeyframeAnimation> FadeInLeftBigFactory = CreateTemplateFactory<float, KeyframeAnimation>(FadingEntrances, nameof(FadeInLeftBig), () => TemplateFactories.FadeInLeftBig(), TemplateFactories.FadeInLeftBig);
        public static readonly TemplateFactory<float, KeyframeAnimation> FadeInRightBigFactory = CreateTemplateFactory<float, KeyframeAnimation>(FadingEntrances, nameof(FadeInRightBig), () => TemplateFactories.FadeInRightBig(), TemplateFactories.FadeInRightBig);

        private const string FadingExits = "Fading Exits";
        public static readonly TemplateFactory<KeyframeAnimation> FadeOutFactory = CreateTemplateFactory<KeyframeAnimation>(FadingExits, nameof(FadeOut), TemplateFactories.FadeOut);
        public static readonly TemplateFactory<KeyframeAnimation> FadeOutUpFactory = CreateTemplateFactory<KeyframeAnimation>(FadingExits, nameof(FadeOutUp), TemplateFactories.FadeOutUp);
        public static readonly TemplateFactory<KeyframeAnimation> FadeOutDownFactory = CreateTemplateFactory<KeyframeAnimation>(FadingExits, nameof(FadeOutDown), TemplateFactories.FadeOutDown);
        public static readonly TemplateFactory<KeyframeAnimation> FadeOutLeftFactory = CreateTemplateFactory<KeyframeAnimation>(FadingExits, nameof(FadeOutLeft), TemplateFactories.FadeOutLeft);
        public static readonly TemplateFactory<KeyframeAnimation> FadeOutRightFactory = CreateTemplateFactory<KeyframeAnimation>(FadingExits, nameof(FadeOutRight), TemplateFactories.FadeOutRight);
        public static readonly TemplateFactory<KeyframeAnimation> FadeOutTopLeftFactory = CreateTemplateFactory<KeyframeAnimation>(FadingExits, nameof(FadeOutTopLeft), TemplateFactories.FadeOutTopLeft);
        public static readonly TemplateFactory<KeyframeAnimation> FadeOutTopRightFactory = CreateTemplateFactory<KeyframeAnimation>(FadingExits, nameof(FadeOutTopRight), TemplateFactories.FadeOutTopRight);
        public static readonly TemplateFactory<KeyframeAnimation> FadeOutBottomLeftFactory = CreateTemplateFactory<KeyframeAnimation>(FadingExits, nameof(FadeOutBottomLeft), TemplateFactories.FadeOutBottomLeft);
        public static readonly TemplateFactory<KeyframeAnimation> FadeOutBottomRightFactory = CreateTemplateFactory<KeyframeAnimation>(FadingExits, nameof(FadeOutBottomRight), TemplateFactories.FadeOutBottomRight);
        public static readonly TemplateFactory<float, KeyframeAnimation> FadeOutUpBigFactory = CreateTemplateFactory<float, KeyframeAnimation>(FadingExits, nameof(FadeOutUpBig), () => TemplateFactories.FadeOutUpBig(), TemplateFactories.FadeOutUpBig);
        public static readonly TemplateFactory<float, KeyframeAnimation> FadeOutDownBigFactory = CreateTemplateFactory<float, KeyframeAnimation>(FadingExits, nameof(FadeOutDownBig), () => TemplateFactories.FadeOutDownBig(), TemplateFactories.FadeOutDownBig);
        public static readonly TemplateFactory<float, KeyframeAnimation> FadeOutLeftBigFactory = CreateTemplateFactory<float, KeyframeAnimation>(FadingExits, nameof(FadeOutLeftBig), () => TemplateFactories.FadeOutLeftBig(), TemplateFactories.FadeOutLeftBig);
        public static readonly TemplateFactory<float, KeyframeAnimation> FadeOutRightBigFactory = CreateTemplateFactory<float, KeyframeAnimation>(FadingExits, nameof(FadeOutRightBig), () => TemplateFactories.FadeOutRightBig(), TemplateFactories.FadeOutRightBig);

        private const string Lightspeed = "Lightspeed";
        public static readonly TemplateFactory<KeyframeAnimation> LightSpeedInLeftFactory = CreateTemplateFactory<KeyframeAnimation>(Lightspeed, nameof(LightSpeedInLeft), TemplateFactories.LightSpeedInLeft);
        public static readonly TemplateFactory<KeyframeAnimation> LightSpeedInRightFactory = CreateTemplateFactory<KeyframeAnimation>(Lightspeed, nameof(LightSpeedInRight), TemplateFactories.LightSpeedInRight);
        public static readonly TemplateFactory<KeyframeAnimation> LightSpeedOutLeftFactory = CreateTemplateFactory<KeyframeAnimation>(Lightspeed, nameof(LightSpeedOutLeft), TemplateFactories.LightSpeedOutLeft);
        public static readonly TemplateFactory<KeyframeAnimation> LightSpeedOutRightFactory = CreateTemplateFactory<KeyframeAnimation>(Lightspeed, nameof(LightSpeedOutRight), TemplateFactories.LightSpeedOutRight);

        private const string RotatingEntrance = "Rotating Entrance";
        public static readonly TemplateFactory<KeyframeAnimation> RotateInFactory = CreateTemplateFactory<KeyframeAnimation>(RotatingEntrance, nameof(RotateIn), TemplateFactories.RotateIn);
        public static readonly TemplateFactory<KeyframeAnimation> RotateInDownLeftFactory = CreateTemplateFactory<KeyframeAnimation>(RotatingEntrance, nameof(RotateInDownLeft), TemplateFactories.RotateInDownLeft);
        public static readonly TemplateFactory<KeyframeAnimation> RotateInDownRightFactory = CreateTemplateFactory<KeyframeAnimation>(RotatingEntrance, nameof(RotateInDownRight), TemplateFactories.RotateInDownRight);
        public static readonly TemplateFactory<KeyframeAnimation> RotateInUpLeftFactory = CreateTemplateFactory<KeyframeAnimation>(RotatingEntrance, nameof(RotateInUpLeft), TemplateFactories.RotateInUpLeft);
        public static readonly TemplateFactory<KeyframeAnimation> RotateInUpRightFactory = CreateTemplateFactory<KeyframeAnimation>(RotatingEntrance, nameof(RotateInUpRight), TemplateFactories.RotateInUpRight);

        private const string RotatingExits = "Rotating Exits";
        public static readonly TemplateFactory<KeyframeAnimation> RotateOutFactory = CreateTemplateFactory<KeyframeAnimation>(RotatingExits, nameof(RotateOut), TemplateFactories.RotateOut);
        public static readonly TemplateFactory<KeyframeAnimation> RotateOutDownLeftFactory = CreateTemplateFactory<KeyframeAnimation>(RotatingExits, nameof(RotateOutDownLeft), TemplateFactories.RotateOutDownLeft);
        public static readonly TemplateFactory<KeyframeAnimation> RotateOutDownRightFactory = CreateTemplateFactory<KeyframeAnimation>(RotatingExits, nameof(RotateOutDownRight), TemplateFactories.RotateOutDownRight);
        public static readonly TemplateFactory<KeyframeAnimation> RotateOutUpLeftFactory = CreateTemplateFactory<KeyframeAnimation>(RotatingExits, nameof(RotateOutUpLeft), TemplateFactories.RotateOutUpLeft);
        public static readonly TemplateFactory<KeyframeAnimation> RotateOutUpRightFactory = CreateTemplateFactory<KeyframeAnimation>(RotatingExits, nameof(RotateOutUpRight), TemplateFactories.RotateOutUpRight);

        private const string Special = "Special";
        public static readonly TemplateFactory<KeyframeAnimation> HingeFactory = CreateTemplateFactory<KeyframeAnimation>(Special, nameof(Hinge), TemplateFactories.Hinge);
        public static readonly TemplateFactory<KeyframeAnimation> JackInTheBoxFactory = CreateTemplateFactory<KeyframeAnimation>(Special, nameof(JackInTheBox), TemplateFactories.JackInTheBox);
        public static readonly TemplateFactory<KeyframeAnimation> RollOutLeftFactory = CreateTemplateFactory<KeyframeAnimation>(Special, nameof(RollOutLeft), TemplateFactories.RollOutLeft);
        public static readonly TemplateFactory<KeyframeAnimation> RollOutRightFactory = CreateTemplateFactory<KeyframeAnimation>(Special, nameof(RollOutRight), TemplateFactories.RollOutRight);
        public static readonly TemplateFactory<KeyframeAnimation> RollInLeftFactory = CreateTemplateFactory<KeyframeAnimation>(Special, nameof(RollInLeft), TemplateFactories.RollInLeft);
        public static readonly TemplateFactory<KeyframeAnimation> RollInRightFactory = CreateTemplateFactory<KeyframeAnimation>(Special, nameof(RollInRight), TemplateFactories.RollInRight);

    }
}