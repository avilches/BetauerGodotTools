using System;
using Godot;

namespace Betauer.Animation.Easing {
    public static class EasingFunctions {
        // Adapted from source : http://www.robertpenner.com/TransitionType/

        public static float Ease(float linearStep, float acceleration, Godot.Tween.TransitionType type) {
            var easedStep = acceleration > 0 ? EaseIn(linearStep, type) :
                acceleration < 0 ? EaseOut(linearStep, type) : linearStep;

            return Mathf.Lerp(linearStep, easedStep, Math.Abs(acceleration));
        }

        public static float EaseIn(float linearStep, Godot.Tween.TransitionType type) {
            switch (type) {
                case Godot.Tween.TransitionType.Linear: return linearStep;
                case Godot.Tween.TransitionType.Sine: return Sine.EaseIn(linearStep);
                case Godot.Tween.TransitionType.Quad: return Power.EaseIn(linearStep, 2);
                case Godot.Tween.TransitionType.Cubic: return Power.EaseIn(linearStep, 3);
                case Godot.Tween.TransitionType.Quart: return Power.EaseIn(linearStep, 4);
                case Godot.Tween.TransitionType.Quint: return Power.EaseIn(linearStep, 5);

                // case Godot.Tween.TransitionType.Expo:    return Power.EaseIn(linearStep, 5); // MAL!!!!
                // case Godot.Tween.TransitionType.Elastic:    return Power.EaseIn(linearStep, 5); // MAL!!!!
                // case Godot.Tween.TransitionType.Circ:    return Power.EaseIn(linearStep, 5); // MAL!!!!
                // case Godot.Tween.TransitionType.Bounce:    return Power.EaseIn(linearStep, 5); // MAL!!!!
                // case Godot.Tween.TransitionType.Back:    return Power.EaseIn(linearStep, 5); // MAL!!!!
            }
            throw new NotImplementedException();
        }

        public static float EaseOut(float linearStep, Godot.Tween.TransitionType type) {
            switch (type) {
                case Godot.Tween.TransitionType.Linear: return linearStep;
                case Godot.Tween.TransitionType.Sine: return Sine.EaseOut(linearStep);
                case Godot.Tween.TransitionType.Quad: return Power.EaseOut(linearStep, 2);
                case Godot.Tween.TransitionType.Cubic: return Power.EaseOut(linearStep, 3);
                case Godot.Tween.TransitionType.Quart: return Power.EaseOut(linearStep, 4);
                case Godot.Tween.TransitionType.Quint: return Power.EaseOut(linearStep, 5);

                // case Godot.Tween.TransitionType.Expo:    return Power.EaseOut(linearStep, 5); // MAL!!!!
                // case Godot.Tween.TransitionType.Elastic:    return Power.EaseOut(linearStep, 5); // MAL!!!!
                // case Godot.Tween.TransitionType.Circ:    return Power.EaseOut(linearStep, 5); // MAL!!!!
                // case Godot.Tween.TransitionType.Bounce:    return Power.EaseOut(linearStep, 5); // MAL!!!!
                // case Godot.Tween.TransitionType.Back:    return Power.EaseOut(linearStep, 5); // MAL!!!!
            }
            throw new NotImplementedException();
        }

        public static float EaseInOut(float linearStep, Godot.Tween.TransitionType easeIn, Godot.Tween.TransitionType easeOut) {
            return linearStep < 0.5 ? EaseInOut(linearStep, easeIn) : EaseInOut(linearStep, easeOut);
        }

        public static float EaseInOut(float linearStep, Godot.Tween.TransitionType type) {
            switch (type) {
                case Godot.Tween.TransitionType.Linear: return linearStep;
                case Godot.Tween.TransitionType.Sine: return Sine.EaseInOut(linearStep);
                case Godot.Tween.TransitionType.Quad: return Power.EaseInOut(linearStep, 2);
                case Godot.Tween.TransitionType.Cubic: return Power.EaseInOut(linearStep, 3);
                case Godot.Tween.TransitionType.Quart: return Power.EaseInOut(linearStep, 4);
                case Godot.Tween.TransitionType.Quint: return Power.EaseInOut(linearStep, 5);

                // case Godot.Tween.TransitionType.Expo:    return Power.EaseInOut(linearStep, 5); // MAL!!!!
                // case Godot.Tween.TransitionType.Elastic:    return Power.EaseInOut(linearStep, 5); // MAL!!!!
                // case Godot.Tween.TransitionType.Circ:    return Power.EaseInOut(linearStep, 5); // MAL!!!!
                // case Godot.Tween.TransitionType.Bounce:    return Power.EaseInOut(linearStep, 5); // MAL!!!!
                // case Godot.Tween.TransitionType.Back:    return Power.EaseInOut(linearStep, 5); // MAL!!!!
            }
            throw new NotImplementedException();
        }

        static class Sine {
            private const float HalfPi = Mathf.Pi / 2;

            public static float EaseIn(float s) {
                return (float)Math.Sin(s * HalfPi - HalfPi) + 1;
            }

            public static float EaseOut(float s) {
                return (float)Math.Sin(s * HalfPi);
            }

            public static float EaseInOut(float s) {
                return (float)(Math.Sin(s * Mathf.Pi - HalfPi) + 1) / 2;
            }
        }

        static class Power {
            public static float EaseIn(float s, int power) {
                return (float)Math.Pow(s, power);
            }

            public static float EaseOut(float s, int power) {
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign * (Math.Pow(s - 1, power) + sign));
            }

            public static float EaseInOut(float s, int power) {
                s *= 2;
                if (s < 1) return EaseIn(s, power) / 2;
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (Math.Pow(s - 2, power) + sign * 2));
            }
        }
    }
}