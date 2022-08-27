using System;
using Godot;

namespace Betauer.Animation {
    public class NewTransitionType {
        // Adapted from source : http://www.robertpenner.com/TransitionType/

        public static float Ease(double linearStep, float acceleration, Tween.TransitionType type) {
            float easedStep = acceleration > 0 ? EaseIn(linearStep, type) :
                acceleration < 0 ? EaseOut(linearStep, type) :
                (float)linearStep;

            return MathHelper.Lerp(linearStep, easedStep, Math.Abs(acceleration));
        }

        public static float EaseIn(double linearStep, Tween.TransitionType type) {
            switch (type) {
                // case Tween.TransitionType.Step:       return linearStep < 0.5 ? 0 : 1;
                case Tween.TransitionType.Linear: return (float)linearStep;
                case Tween.TransitionType.Sine: return Sine.EaseIn(linearStep);
                case Tween.TransitionType.Quad: return Power.EaseIn(linearStep, 2);
                case Tween.TransitionType.Cubic: return Power.EaseIn(linearStep, 3);
                case Tween.TransitionType.Quart: return Power.EaseIn(linearStep, 4);
                case Tween.TransitionType.Quint: return Power.EaseIn(linearStep, 5);

                // case Tween.TransitionType.Expo:    return Power.EaseIn(linearStep, 5); // MAL!!!!
                // case Tween.TransitionType.Elastic:    return Power.EaseIn(linearStep, 5); // MAL!!!!
                // case Tween.TransitionType.Circ:    return Power.EaseIn(linearStep, 5); // MAL!!!!
                // case Tween.TransitionType.Bounce:    return Power.EaseIn(linearStep, 5); // MAL!!!!
                // case Tween.TransitionType.Back:    return Power.EaseIn(linearStep, 5); // MAL!!!!
            }
            throw new NotImplementedException();
        }

        public static float EaseOut(double linearStep, Tween.TransitionType type) {
            switch (type) {
                // case Tween.TransitionType.Step:       return linearStep < 0.5 ? 0 : 1;
                case Tween.TransitionType.Linear: return (float)linearStep;
                case Tween.TransitionType.Sine: return Sine.EaseOut(linearStep);
                case Tween.TransitionType.Quad: return Power.EaseOut(linearStep, 2);
                case Tween.TransitionType.Cubic: return Power.EaseOut(linearStep, 3);
                case Tween.TransitionType.Quart: return Power.EaseOut(linearStep, 4);
                case Tween.TransitionType.Quint: return Power.EaseOut(linearStep, 5);

                // case Tween.TransitionType.Expo:    return Power.EaseOut(linearStep, 5); // MAL!!!!
                // case Tween.TransitionType.Elastic:    return Power.EaseOut(linearStep, 5); // MAL!!!!
                // case Tween.TransitionType.Circ:    return Power.EaseOut(linearStep, 5); // MAL!!!!
                // case Tween.TransitionType.Bounce:    return Power.EaseOut(linearStep, 5); // MAL!!!!
                // case Tween.TransitionType.Back:    return Power.EaseOut(linearStep, 5); // MAL!!!!
            }
            throw new NotImplementedException();
        }

        public static float EaseInOut(double linearStep, Tween.TransitionType easeIn, Tween.TransitionType easeOut) {
            return linearStep < 0.5 ? EaseInOut(linearStep, easeIn) : EaseInOut(linearStep, easeOut);
        }

        public static float EaseInOut(double linearStep, Tween.TransitionType type) {
            switch (type) {
                // case Tween.TransitionType.Step:       return linearStep < 0.5 ? 0 : 1;
                case Tween.TransitionType.Linear: return (float)linearStep;
                case Tween.TransitionType.Sine: return Sine.EaseInOut(linearStep);
                case Tween.TransitionType.Quad: return Power.EaseInOut(linearStep, 2);
                case Tween.TransitionType.Cubic: return Power.EaseInOut(linearStep, 3);
                case Tween.TransitionType.Quart: return Power.EaseInOut(linearStep, 4);
                case Tween.TransitionType.Quint: return Power.EaseInOut(linearStep, 5);

                // case Tween.TransitionType.Expo:    return Power.EaseInOut(linearStep, 5); // MAL!!!!
                // case Tween.TransitionType.Elastic:    return Power.EaseInOut(linearStep, 5); // MAL!!!!
                // case Tween.TransitionType.Circ:    return Power.EaseInOut(linearStep, 5); // MAL!!!!
                // case Tween.TransitionType.Bounce:    return Power.EaseInOut(linearStep, 5); // MAL!!!!
                // case Tween.TransitionType.Back:    return Power.EaseInOut(linearStep, 5); // MAL!!!!
            }
            throw new NotImplementedException();
        }

        static class Sine {
            public static float EaseIn(double s) {
                return (float)Math.Sin(s * MathHelper.HalfPi - MathHelper.HalfPi) + 1;
            }

            public static float EaseOut(double s) {
                return (float)Math.Sin(s * MathHelper.HalfPi);
            }

            public static float EaseInOut(double s) {
                return (float)(Math.Sin(s * MathHelper.Pi - MathHelper.HalfPi) + 1) / 2;
            }
        }

        static class Power {
            public static float EaseIn(double s, int power) {
                return (float)Math.Pow(s, power);
            }

            public static float EaseOut(double s, int power) {
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign * (Math.Pow(s - 1, power) + sign));
            }

            public static float EaseInOut(double s, int power) {
                s *= 2;
                if (s < 1) return EaseIn(s, power) / 2;
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (Math.Pow(s - 2, power) + sign * 2));
            }
        }
    }

    public static class MathHelper {
        public const float Pi = (float)Math.PI;
        public const float HalfPi = (float)(Math.PI / 2);

        public static float Lerp(double from, double to, double step) {
            return (float)((to - from) * step + from);
        }
    }
}