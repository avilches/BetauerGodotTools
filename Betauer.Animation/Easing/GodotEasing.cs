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
            if (EaseType == Godot.Tween.EaseType.In) {
                return EasingFunctions.EaseIn(t, TransitionType);
            } else if (EaseType == Godot.Tween.EaseType.Out) {
                return EasingFunctions.EaseOut(t, TransitionType);
            } else if (EaseType == Godot.Tween.EaseType.InOut) {
                return EasingFunctions.EaseInOut(t, TransitionType);
            }
            throw new NotImplementedException();
        }
    }
}