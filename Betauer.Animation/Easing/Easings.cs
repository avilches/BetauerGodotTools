using Godot;

namespace Betauer.Animation.Easing {
    public static class Easings {
        public static IEasing Linear = new GodotEasing(Godot.Tween.TransitionType.Linear, Godot.Tween.EaseType.InOut);

        public static IEasing SineIn = new GodotEasing(Godot.Tween.TransitionType.Sine, Godot.Tween.EaseType.In);
        public static IEasing SineOut = new GodotEasing(Godot.Tween.TransitionType.Sine, Godot.Tween.EaseType.Out);
        public static IEasing SineInOut = new GodotEasing(Godot.Tween.TransitionType.Sine, Godot.Tween.EaseType.InOut);

        public static IEasing QuintIn = new GodotEasing(Godot.Tween.TransitionType.Quint, Godot.Tween.EaseType.In);
        public static IEasing QuintOut = new GodotEasing(Godot.Tween.TransitionType.Quint, Godot.Tween.EaseType.Out);
        public static IEasing QuintInOut = new GodotEasing(Godot.Tween.TransitionType.Quint, Godot.Tween.EaseType.InOut);

        public static IEasing QuartIn = new GodotEasing(Godot.Tween.TransitionType.Quart, Godot.Tween.EaseType.In);
        public static IEasing QuartOut = new GodotEasing(Godot.Tween.TransitionType.Quart, Godot.Tween.EaseType.Out);
        public static IEasing QuartInOut = new GodotEasing(Godot.Tween.TransitionType.Quart, Godot.Tween.EaseType.InOut);

        public static IEasing QuadIn = new GodotEasing(Godot.Tween.TransitionType.Quad, Godot.Tween.EaseType.In);
        public static IEasing QuadOut = new GodotEasing(Godot.Tween.TransitionType.Quad, Godot.Tween.EaseType.Out);
        public static IEasing QuadInOut = new GodotEasing(Godot.Tween.TransitionType.Quad, Godot.Tween.EaseType.InOut);

        public static IEasing ExpoIn = new GodotEasing(Godot.Tween.TransitionType.Expo, Godot.Tween.EaseType.In);
        public static IEasing ExpoOut = new GodotEasing(Godot.Tween.TransitionType.Expo, Godot.Tween.EaseType.Out);
        public static IEasing ExpoInOut = new GodotEasing(Godot.Tween.TransitionType.Expo, Godot.Tween.EaseType.InOut);

        public static IEasing ElasticIn = new GodotEasing(Godot.Tween.TransitionType.Elastic, Godot.Tween.EaseType.In);
        public static IEasing ElasticOut = new GodotEasing(Godot.Tween.TransitionType.Elastic, Godot.Tween.EaseType.Out);
        public static IEasing ElasticInOut = new GodotEasing(Godot.Tween.TransitionType.Elastic, Godot.Tween.EaseType.InOut);

        public static IEasing CubicIn = new GodotEasing(Godot.Tween.TransitionType.Cubic, Godot.Tween.EaseType.In);
        public static IEasing CubicOut = new GodotEasing(Godot.Tween.TransitionType.Cubic, Godot.Tween.EaseType.Out);
        public static IEasing CubicInOut = new GodotEasing(Godot.Tween.TransitionType.Cubic, Godot.Tween.EaseType.InOut);

        public static IEasing CircIn = new GodotEasing(Godot.Tween.TransitionType.Circ, Godot.Tween.EaseType.In);
        public static IEasing CircOut = new GodotEasing(Godot.Tween.TransitionType.Circ, Godot.Tween.EaseType.Out);
        public static IEasing CircInOut = new GodotEasing(Godot.Tween.TransitionType.Circ, Godot.Tween.EaseType.InOut);

        public static IEasing BounceIn = new GodotEasing(Godot.Tween.TransitionType.Bounce, Godot.Tween.EaseType.In);
        public static IEasing BounceOut = new GodotEasing(Godot.Tween.TransitionType.Bounce, Godot.Tween.EaseType.Out);
        public static IEasing BounceInOut = new GodotEasing(Godot.Tween.TransitionType.Bounce, Godot.Tween.EaseType.InOut);

        public static IEasing BackIn = new GodotEasing(Godot.Tween.TransitionType.Back, Godot.Tween.EaseType.In);
        public static IEasing BackOut = new GodotEasing(Godot.Tween.TransitionType.Back, Godot.Tween.EaseType.Out);
        public static IEasing BackInOut = new GodotEasing(Godot.Tween.TransitionType.Back, Godot.Tween.EaseType.InOut);
    }
}