using System;
using Godot;

namespace Betauer.Animation.Easing {
    public class GodotEasing : IEasing {
        public readonly Tween.EaseType EaseType;
        public readonly Tween.TransitionType TransitionType;
        
        public string Name { get; }

        internal GodotEasing(Tween.TransitionType transitionType, Tween.EaseType easeType) {
            Name = $"{transitionType}{easeType}";
            TransitionType = transitionType;
            EaseType = easeType;
        }

        public float GetY(float t) {
            if (EaseType == Tween.EaseType.In) {
                return EasingFunctions.EaseIn(t, TransitionType);
            } else if (EaseType == Tween.EaseType.Out) {
                return EasingFunctions.EaseOut(t, TransitionType);
            } else if (EaseType == Tween.EaseType.InOut) {
                return EasingFunctions.EaseInOut(t, TransitionType);
            }
            throw new NotImplementedException();
        }
    }
}