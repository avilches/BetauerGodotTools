using System;
using Godot;

namespace Betauer.Core.Easing; 

public class Interpolation : IInterpolation {
    public readonly Tween.EaseType EaseType;
    public readonly Tween.TransitionType TransitionType;
    public readonly Func<float, float> EasingFunction;

    public string Name { get; }

    private Interpolation(Tween.TransitionType transitionType, Tween.EaseType easeType) {
        Name = $"{transitionType}{easeType}";
        TransitionType = transitionType;
        EaseType = easeType;
        EasingFunction = Functions.GetEaseFunc(transitionType, easeType);
    }

    public float GetY(float t) { 
        return EasingFunction(t);
    }
        
    public static readonly Interpolation Linear = new Interpolation(Tween.TransitionType.Linear, Tween.EaseType.InOut);

    public static readonly Interpolation SineIn = new Interpolation(Tween.TransitionType.Sine, Tween.EaseType.In);
    public static readonly Interpolation SineOut = new Interpolation(Tween.TransitionType.Sine, Tween.EaseType.Out);
    public static readonly Interpolation SineInOut = new Interpolation(Tween.TransitionType.Sine, Tween.EaseType.InOut);

    public static readonly Interpolation QuintIn = new Interpolation(Tween.TransitionType.Quint, Tween.EaseType.In);
    public static readonly Interpolation QuintOut = new Interpolation(Tween.TransitionType.Quint, Tween.EaseType.Out);
    public static readonly Interpolation QuintInOut = new Interpolation(Tween.TransitionType.Quint, Tween.EaseType.InOut);

    public static readonly Interpolation QuartIn = new Interpolation(Tween.TransitionType.Quart, Tween.EaseType.In);
    public static readonly Interpolation QuartOut = new Interpolation(Tween.TransitionType.Quart, Tween.EaseType.Out);
    public static readonly Interpolation QuartInOut = new Interpolation(Tween.TransitionType.Quart, Tween.EaseType.InOut);

    public static readonly Interpolation QuadIn = new Interpolation(Tween.TransitionType.Quad, Tween.EaseType.In);
    public static readonly Interpolation QuadOut = new Interpolation(Tween.TransitionType.Quad, Tween.EaseType.Out);
    public static readonly Interpolation QuadInOut = new Interpolation(Tween.TransitionType.Quad, Tween.EaseType.InOut);

    public static readonly Interpolation ExpoIn = new Interpolation(Tween.TransitionType.Expo, Tween.EaseType.In);
    public static readonly Interpolation ExpoOut = new Interpolation(Tween.TransitionType.Expo, Tween.EaseType.Out);
    public static readonly Interpolation ExpoInOut = new Interpolation(Tween.TransitionType.Expo, Tween.EaseType.InOut);

    public static readonly Interpolation ElasticIn = new Interpolation(Tween.TransitionType.Elastic, Tween.EaseType.In);
    public static readonly Interpolation ElasticOut = new Interpolation(Tween.TransitionType.Elastic, Tween.EaseType.Out);
    public static readonly Interpolation ElasticInOut = new Interpolation(Tween.TransitionType.Elastic, Tween.EaseType.InOut);

    public static readonly Interpolation CubicIn = new Interpolation(Tween.TransitionType.Cubic, Tween.EaseType.In);
    public static readonly Interpolation CubicOut = new Interpolation(Tween.TransitionType.Cubic, Tween.EaseType.Out);
    public static readonly Interpolation CubicInOut = new Interpolation(Tween.TransitionType.Cubic, Tween.EaseType.InOut);

    public static readonly Interpolation CircIn = new Interpolation(Tween.TransitionType.Circ, Tween.EaseType.In);
    public static readonly Interpolation CircOut = new Interpolation(Tween.TransitionType.Circ, Tween.EaseType.Out);
    public static readonly Interpolation CircInOut = new Interpolation(Tween.TransitionType.Circ, Tween.EaseType.InOut);

    public static readonly Interpolation BounceIn = new Interpolation(Tween.TransitionType.Bounce, Tween.EaseType.In);
    public static readonly Interpolation BounceOut = new Interpolation(Tween.TransitionType.Bounce, Tween.EaseType.Out);
    public static readonly Interpolation BounceInOut = new Interpolation(Tween.TransitionType.Bounce, Tween.EaseType.InOut);

    public static readonly Interpolation BackIn = new Interpolation(Tween.TransitionType.Back, Tween.EaseType.In);
    public static readonly Interpolation BackOut = new Interpolation(Tween.TransitionType.Back, Tween.EaseType.Out);
    public static readonly Interpolation BackInOut = new Interpolation(Tween.TransitionType.Back, Tween.EaseType.InOut);

    public static readonly Interpolation SpringIn = new Interpolation(Tween.TransitionType.Spring, Tween.EaseType.In);
    public static readonly Interpolation SpringOut = new Interpolation(Tween.TransitionType.Spring, Tween.EaseType.Out);
    public static readonly Interpolation SpringInOut = new Interpolation(Tween.TransitionType.Spring, Tween.EaseType.InOut);
    
    public static IInterpolation Combine(IInterpolation interpolation) => new InterpolationWrapper(interpolation);
    
    public static IInterpolation Combine(Func<float, float> interpolation) => new InterpolationFunc(interpolation);

    public static Interpolation Get(Tween.TransitionType type, Tween.EaseType easeType) {
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