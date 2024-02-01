using Godot;

namespace Betauer.Core.Easing; 

public static class Easings {
    public static readonly IEasing Linear = new GodotEasing(Tween.TransitionType.Linear, Tween.EaseType.InOut);

    public static readonly IEasing SineIn = new GodotEasing(Tween.TransitionType.Sine, Tween.EaseType.In);
    public static readonly IEasing SineOut = new GodotEasing(Tween.TransitionType.Sine, Tween.EaseType.Out);
    public static readonly IEasing SineInOut = new GodotEasing(Tween.TransitionType.Sine, Tween.EaseType.InOut);

    public static readonly IEasing QuintIn = new GodotEasing(Tween.TransitionType.Quint, Tween.EaseType.In);
    public static readonly IEasing QuintOut = new GodotEasing(Tween.TransitionType.Quint, Tween.EaseType.Out);
    public static readonly IEasing QuintInOut = new GodotEasing(Tween.TransitionType.Quint, Tween.EaseType.InOut);

    public static readonly IEasing QuartIn = new GodotEasing(Tween.TransitionType.Quart, Tween.EaseType.In);
    public static readonly IEasing QuartOut = new GodotEasing(Tween.TransitionType.Quart, Tween.EaseType.Out);
    public static readonly IEasing QuartInOut = new GodotEasing(Tween.TransitionType.Quart, Tween.EaseType.InOut);

    public static readonly IEasing QuadIn = new GodotEasing(Tween.TransitionType.Quad, Tween.EaseType.In);
    public static readonly IEasing QuadOut = new GodotEasing(Tween.TransitionType.Quad, Tween.EaseType.Out);
    public static readonly IEasing QuadInOut = new GodotEasing(Tween.TransitionType.Quad, Tween.EaseType.InOut);

    public static readonly IEasing ExpoIn = new GodotEasing(Tween.TransitionType.Expo, Tween.EaseType.In);
    public static readonly IEasing ExpoOut = new GodotEasing(Tween.TransitionType.Expo, Tween.EaseType.Out);
    public static readonly IEasing ExpoInOut = new GodotEasing(Tween.TransitionType.Expo, Tween.EaseType.InOut);

    public static readonly IEasing ElasticIn = new GodotEasing(Tween.TransitionType.Elastic, Tween.EaseType.In);
    public static readonly IEasing ElasticOut = new GodotEasing(Tween.TransitionType.Elastic, Tween.EaseType.Out);
    public static readonly IEasing ElasticInOut = new GodotEasing(Tween.TransitionType.Elastic, Tween.EaseType.InOut);

    public static readonly IEasing CubicIn = new GodotEasing(Tween.TransitionType.Cubic, Tween.EaseType.In);
    public static readonly IEasing CubicOut = new GodotEasing(Tween.TransitionType.Cubic, Tween.EaseType.Out);
    public static readonly IEasing CubicInOut = new GodotEasing(Tween.TransitionType.Cubic, Tween.EaseType.InOut);

    public static readonly IEasing CircIn = new GodotEasing(Tween.TransitionType.Circ, Tween.EaseType.In);
    public static readonly IEasing CircOut = new GodotEasing(Tween.TransitionType.Circ, Tween.EaseType.Out);
    public static readonly IEasing CircInOut = new GodotEasing(Tween.TransitionType.Circ, Tween.EaseType.InOut);

    public static readonly IEasing BounceIn = new GodotEasing(Tween.TransitionType.Bounce, Tween.EaseType.In);
    public static readonly IEasing BounceOut = new GodotEasing(Tween.TransitionType.Bounce, Tween.EaseType.Out);
    public static readonly IEasing BounceInOut = new GodotEasing(Tween.TransitionType.Bounce, Tween.EaseType.InOut);

    public static readonly IEasing BackIn = new GodotEasing(Tween.TransitionType.Back, Tween.EaseType.In);
    public static readonly IEasing BackOut = new GodotEasing(Tween.TransitionType.Back, Tween.EaseType.Out);
    public static readonly IEasing BackInOut = new GodotEasing(Tween.TransitionType.Back, Tween.EaseType.InOut);

    public static readonly IEasing SpringIn = new GodotEasing(Tween.TransitionType.Spring, Tween.EaseType.In);
    public static readonly IEasing SpringOut = new GodotEasing(Tween.TransitionType.Spring, Tween.EaseType.Out);
    public static readonly IEasing SpringInOut = new GodotEasing(Tween.TransitionType.Spring, Tween.EaseType.InOut);

    public static IEasing CreateBias(float bias) => new BiasEasing() {
        Bias = bias,
    };

    public static IEasing CreateBiasGain(float bias, float offset) => new BiasGainEasing() {
        Bias = bias,
        Offset = offset,
    };
}