using System;
using Godot;

namespace Betauer.Core.Easing;

public class Interpolation : IInterpolation {
    public readonly Func<float, float> Function;

    private Interpolation(Func<float, float> function) {
        Function = function;
    }

    public float Get(float t) { 
        return Function(t);
    }
        
    public static readonly IInterpolation Linear = new GodotTween(Tween.TransitionType.Linear, Tween.EaseType.InOut);

    public static readonly IInterpolation SineIn = new GodotTween(Tween.TransitionType.Sine, Tween.EaseType.In);
    public static readonly IInterpolation SineOut = new GodotTween(Tween.TransitionType.Sine, Tween.EaseType.Out);
    public static readonly IInterpolation SineInOut = new GodotTween(Tween.TransitionType.Sine, Tween.EaseType.InOut);

    public static readonly IInterpolation QuintIn = new GodotTween(Tween.TransitionType.Quint, Tween.EaseType.In);
    public static readonly IInterpolation QuintOut = new GodotTween(Tween.TransitionType.Quint, Tween.EaseType.Out);
    public static readonly IInterpolation QuintInOut = new GodotTween(Tween.TransitionType.Quint, Tween.EaseType.InOut);

    public static readonly IInterpolation QuartIn = new GodotTween(Tween.TransitionType.Quart, Tween.EaseType.In);
    public static readonly IInterpolation QuartOut = new GodotTween(Tween.TransitionType.Quart, Tween.EaseType.Out);
    public static readonly IInterpolation QuartInOut = new GodotTween(Tween.TransitionType.Quart, Tween.EaseType.InOut);

    public static readonly IInterpolation QuadIn = new GodotTween(Tween.TransitionType.Quad, Tween.EaseType.In);
    public static readonly IInterpolation QuadOut = new GodotTween(Tween.TransitionType.Quad, Tween.EaseType.Out);
    public static readonly IInterpolation QuadInOut = new GodotTween(Tween.TransitionType.Quad, Tween.EaseType.InOut);

    public static readonly IInterpolation ExpoIn = new GodotTween(Tween.TransitionType.Expo, Tween.EaseType.In);
    public static readonly IInterpolation ExpoOut = new GodotTween(Tween.TransitionType.Expo, Tween.EaseType.Out);
    public static readonly IInterpolation ExpoInOut = new GodotTween(Tween.TransitionType.Expo, Tween.EaseType.InOut);

    public static readonly IInterpolation ElasticIn = new GodotTween(Tween.TransitionType.Elastic, Tween.EaseType.In);
    public static readonly IInterpolation ElasticOut = new GodotTween(Tween.TransitionType.Elastic, Tween.EaseType.Out);
    public static readonly IInterpolation ElasticInOut = new GodotTween(Tween.TransitionType.Elastic, Tween.EaseType.InOut);

    public static readonly IInterpolation CubicIn = new GodotTween(Tween.TransitionType.Cubic, Tween.EaseType.In);
    public static readonly IInterpolation CubicOut = new GodotTween(Tween.TransitionType.Cubic, Tween.EaseType.Out);
    public static readonly IInterpolation CubicInOut = new GodotTween(Tween.TransitionType.Cubic, Tween.EaseType.InOut);

    public static readonly IInterpolation CircIn = new GodotTween(Tween.TransitionType.Circ, Tween.EaseType.In);
    public static readonly IInterpolation CircOut = new GodotTween(Tween.TransitionType.Circ, Tween.EaseType.Out);
    public static readonly IInterpolation CircInOut = new GodotTween(Tween.TransitionType.Circ, Tween.EaseType.InOut);

    public static readonly IInterpolation BounceIn = new GodotTween(Tween.TransitionType.Bounce, Tween.EaseType.In);
    public static readonly IInterpolation BounceOut = new GodotTween(Tween.TransitionType.Bounce, Tween.EaseType.Out);
    public static readonly IInterpolation BounceInOut = new GodotTween(Tween.TransitionType.Bounce, Tween.EaseType.InOut);

    public static readonly IInterpolation BackIn = new GodotTween(Tween.TransitionType.Back, Tween.EaseType.In);
    public static readonly IInterpolation BackOut = new GodotTween(Tween.TransitionType.Back, Tween.EaseType.Out);
    public static readonly IInterpolation BackInOut = new GodotTween(Tween.TransitionType.Back, Tween.EaseType.InOut);

    public static readonly IInterpolation SpringIn = new GodotTween(Tween.TransitionType.Spring, Tween.EaseType.In);
    public static readonly IInterpolation SpringOut = new GodotTween(Tween.TransitionType.Spring, Tween.EaseType.Out);
    public static readonly IInterpolation SpringInOut = new GodotTween(Tween.TransitionType.Spring, Tween.EaseType.InOut);

    public static IInterpolation Create(Func<float, float> interpolation) => new Interpolation(interpolation);
    
    public static IInterpolation Mirror(IInterpolation interpolation) => new Interpolation((t) => Functions.Mirror(t, interpolation));

    public static IInterpolation Bias(float bias) => new Interpolation((t) => Functions.Bias(t, bias));
    
    public static IInterpolation Gain(float bias, float gain) => new Interpolation((t) => Functions.Gain(t, bias, gain));

    public static IInterpolation Shift(IInterpolation interpolation, float start = 0f, float end = 1f) {
        return new Interpolation((t) => Functions.Shift(start, end, t, interpolation.Get));
    }

    public static IInterpolation Get(Tween.TransitionType type, Tween.EaseType easeType) {
        return type switch {
            Tween.TransitionType.Linear => Linear,
            Tween.TransitionType.Sine => easeType switch {
                Tween.EaseType.In => SineIn,
                Tween.EaseType.Out => SineOut,
                Tween.EaseType.InOut => SineInOut,
                Tween.EaseType.OutIn => SineInOut,
            },
            Tween.TransitionType.Quint => easeType switch {
                Tween.EaseType.In => QuintIn,
                Tween.EaseType.Out => QuintOut,
                Tween.EaseType.InOut => QuintInOut,
                Tween.EaseType.OutIn => QuintInOut,
            },
            Tween.TransitionType.Quart => easeType switch {
                Tween.EaseType.In => QuartIn,
                Tween.EaseType.Out => QuartOut,
                Tween.EaseType.InOut => QuartInOut,
                Tween.EaseType.OutIn => QuartInOut,
            },
            Tween.TransitionType.Quad => easeType switch {
                Tween.EaseType.In => QuadIn,
                Tween.EaseType.Out => QuadOut,
                Tween.EaseType.InOut => QuadInOut,
                Tween.EaseType.OutIn => QuadInOut,
            },
            Tween.TransitionType.Expo => easeType switch {
                Tween.EaseType.In => ExpoIn,
                Tween.EaseType.Out => ExpoOut,
                Tween.EaseType.InOut => ExpoInOut,
                Tween.EaseType.OutIn => ExpoInOut,
            },
            Tween.TransitionType.Elastic => easeType switch {
                Tween.EaseType.In => ElasticIn,
                Tween.EaseType.Out => ElasticOut,
                Tween.EaseType.InOut => ElasticInOut,
                Tween.EaseType.OutIn => ElasticInOut,
            },
            Tween.TransitionType.Cubic => easeType switch {
                Tween.EaseType.In => CubicIn,
                Tween.EaseType.Out => CubicOut,
                Tween.EaseType.InOut => CubicInOut,
                Tween.EaseType.OutIn => CubicInOut,
            },
            Tween.TransitionType.Circ => easeType switch {
                Tween.EaseType.In => CircIn,
                Tween.EaseType.Out => CircOut,
                Tween.EaseType.InOut => CircInOut,
                Tween.EaseType.OutIn => CircInOut,
            },
            Tween.TransitionType.Bounce => easeType switch {
                Tween.EaseType.In => BounceIn,
                Tween.EaseType.Out => BounceOut,
                Tween.EaseType.InOut => BounceInOut,
                Tween.EaseType.OutIn => BounceInOut,
            },
            Tween.TransitionType.Back => easeType switch {
                Tween.EaseType.In => BackIn,
                Tween.EaseType.Out => BackOut,
                Tween.EaseType.InOut => BackInOut,
                Tween.EaseType.OutIn => BackInOut,
            },
            Tween.TransitionType.Spring => easeType switch {
                Tween.EaseType.In => SpringIn,
                Tween.EaseType.Out => SpringOut,
                Tween.EaseType.InOut => SpringInOut,
                Tween.EaseType.OutIn => SpringInOut,
            },
        };
    }
}