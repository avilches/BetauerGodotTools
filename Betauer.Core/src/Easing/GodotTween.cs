using System;
using Godot;

namespace Betauer.Core.Easing;

public class GodotTween : IInterpolation {
    public readonly Tween.EaseType EaseType;
    public readonly Tween.TransitionType TransitionType;
    public readonly Func<float, float> EasingFunction;

    public string Name { get; }

    internal GodotTween(Tween.TransitionType transitionType, Tween.EaseType easeType) {
        Name = $"{transitionType}{easeType}";
        TransitionType = transitionType;
        EaseType = easeType;
        EasingFunction = Functions.GetEaseFunc(transitionType, easeType);
    }

    public float Get(float t) {
        return EasingFunction(t);
    }
}