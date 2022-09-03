using Godot;

namespace Betauer.Animation.Easing {
    public static class Easings {
        public static IEasing Linear = new GodotEasing(Tween.TransitionType.Linear, Tween.EaseType.InOut);

        public static IEasing SineIn = new GodotEasing(Tween.TransitionType.Sine, Tween.EaseType.In);
        public static IEasing SineOut = new GodotEasing(Tween.TransitionType.Sine, Tween.EaseType.Out);
        public static IEasing SineInOut = new GodotEasing(Tween.TransitionType.Sine, Tween.EaseType.InOut);

        public static IEasing QuintIn = new GodotEasing(Tween.TransitionType.Quint, Tween.EaseType.In);
        public static IEasing QuintOut = new GodotEasing(Tween.TransitionType.Quint, Tween.EaseType.Out);
        public static IEasing QuintInOut = new GodotEasing(Tween.TransitionType.Quint, Tween.EaseType.InOut);

        public static IEasing QuartIn = new GodotEasing(Tween.TransitionType.Quart, Tween.EaseType.In);
        public static IEasing QuartOut = new GodotEasing(Tween.TransitionType.Quart, Tween.EaseType.Out);
        public static IEasing QuartInOut = new GodotEasing(Tween.TransitionType.Quart, Tween.EaseType.InOut);

        public static IEasing QuadIn = new GodotEasing(Tween.TransitionType.Quad, Tween.EaseType.In);
        public static IEasing QuadOut = new GodotEasing(Tween.TransitionType.Quad, Tween.EaseType.Out);
        public static IEasing QuadInOut = new GodotEasing(Tween.TransitionType.Quad, Tween.EaseType.InOut);

        public static IEasing ExpoIn = new GodotEasing(Tween.TransitionType.Expo, Tween.EaseType.In);
        public static IEasing ExpoOut = new GodotEasing(Tween.TransitionType.Expo, Tween.EaseType.Out);
        public static IEasing ExpoInOut = new GodotEasing(Tween.TransitionType.Expo, Tween.EaseType.InOut);

        public static IEasing ElasticIn = new GodotEasing(Tween.TransitionType.Elastic, Tween.EaseType.In);
        public static IEasing ElasticOut = new GodotEasing(Tween.TransitionType.Elastic, Tween.EaseType.Out);
        public static IEasing ElasticInOut = new GodotEasing(Tween.TransitionType.Elastic, Tween.EaseType.InOut);

        public static IEasing CubicIn = new GodotEasing(Tween.TransitionType.Cubic, Tween.EaseType.In);
        public static IEasing CubicOut = new GodotEasing(Tween.TransitionType.Cubic, Tween.EaseType.Out);
        public static IEasing CubicInOut = new GodotEasing(Tween.TransitionType.Cubic, Tween.EaseType.InOut);

        public static IEasing CircIn = new GodotEasing(Tween.TransitionType.Circ, Tween.EaseType.In);
        public static IEasing CircOut = new GodotEasing(Tween.TransitionType.Circ, Tween.EaseType.Out);
        public static IEasing CircInOut = new GodotEasing(Tween.TransitionType.Circ, Tween.EaseType.InOut);

        public static IEasing BounceIn = new GodotEasing(Tween.TransitionType.Bounce, Tween.EaseType.In);
        public static IEasing BounceOut = new GodotEasing(Tween.TransitionType.Bounce, Tween.EaseType.Out);
        public static IEasing BounceInOut = new GodotEasing(Tween.TransitionType.Bounce, Tween.EaseType.InOut);

        public static IEasing BackIn = new GodotEasing(Tween.TransitionType.Back, Tween.EaseType.In);
        public static IEasing BackOut = new GodotEasing(Tween.TransitionType.Back, Tween.EaseType.Out);
        public static IEasing BackInOut = new GodotEasing(Tween.TransitionType.Back, Tween.EaseType.InOut);
    }
}