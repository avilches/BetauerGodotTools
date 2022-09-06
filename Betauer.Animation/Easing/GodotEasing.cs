using System;
using Godot;

namespace Betauer.Animation.Easing {
    public class GodotEasing : IEasing {
        public readonly Godot.Tween.EaseType EaseType;
        public readonly Godot.Tween.TransitionType TransitionType;
        
        public string Name { get; }

        internal GodotEasing(Godot.Tween.TransitionType transitionType, Godot.Tween.EaseType easeType) {
            Name = $"{transitionType}{easeType}";
            TransitionType = transitionType;
            EaseType = easeType;
        }

        public float GetY(float t) {
            return EaseType switch {
                Godot.Tween.EaseType.In => EasingFunctions.EaseIn(t, TransitionType),
                Godot.Tween.EaseType.Out => EasingFunctions.EaseOut(t, TransitionType),
                Godot.Tween.EaseType.InOut => EasingFunctions.EaseInOut(t, TransitionType),
                Godot.Tween.EaseType.OutIn => throw new NotImplementedException(),
            };
        }
    }
}