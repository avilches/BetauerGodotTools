using System;
using Godot;

namespace Betauer.Animation.Easing {
    public static class EasingFunctions {
        // Adapted from source : http://www.robertpenner.com/TransitionType/

        public static float Ease(Tween.TransitionType type, float linearStep, float acceleration) {
            var easedStep = acceleration > 0 ? EaseIn(type, linearStep) :
                acceleration < 0 ? EaseOut(type, linearStep) : linearStep;

            return Mathf.Lerp(linearStep, easedStep, Mathf.Abs(acceleration));
        }

        public static float EaseIn(Tween.TransitionType type, float t) {
            return type switch {
                Tween.TransitionType.Linear => t,
                Tween.TransitionType.Sine => Sine.EaseIn(t),
                Tween.TransitionType.Quad => Power.EaseIn(t, 2),
                Tween.TransitionType.Cubic => Power.EaseIn(t, 3),
                Tween.TransitionType.Quart => Power.EaseIn(t, 4),
                Tween.TransitionType.Quint => Power.EaseIn(t, 5),
                Tween.TransitionType.Expo => Expo.EaseIn(t),
                Tween.TransitionType.Elastic => Elastic.EaseIn(t),
                Tween.TransitionType.Circ => Circle.EaseIn(t),
                Tween.TransitionType.Bounce => Bounce.EaseIn(t),
                Tween.TransitionType.Back => Back.EaseIn(t),
                Tween.TransitionType.Spring => Spring.EaseIn(t),
            };
        }

        public static float EaseOut(Tween.TransitionType type, float t) {
            return type switch {
                Tween.TransitionType.Linear => t,
                Tween.TransitionType.Sine => Sine.EaseOut(t),
                Tween.TransitionType.Quad => Power.EaseOut(t, 2),
                Tween.TransitionType.Cubic => Power.EaseOut(t, 3),
                Tween.TransitionType.Quart => Power.EaseOut(t, 4),
                Tween.TransitionType.Quint => Power.EaseOut(t, 5),
                Tween.TransitionType.Expo => Expo.EaseOut(t),
                Tween.TransitionType.Elastic => Elastic.EaseOut(t),
                Tween.TransitionType.Circ => Circle.EaseOut(t),
                Tween.TransitionType.Bounce => Bounce.EaseOut(t),
                Tween.TransitionType.Back => Back.EaseOut(t),
                Tween.TransitionType.Spring => Spring.EaseOut(t),
            };
        }

        public static float EaseInOut(float t, Tween.TransitionType type) {
            return t < 0.5f ? EaseIn(type, t * 2) * 0.5f : 0.5f + EaseOut(type, (t - 0.5f) * 2f) * 0.5f;
        }

        public static float EaseOutIn(float t, Tween.TransitionType type) {
            return t < 0.5f ? EaseOut(type, t * 2) * 0.5f : 0.5f + EaseIn(type, (t - 0.5f) * 2f) * 0.5f;
        }

        public static class Sine {
            private const float HalfPi = Mathf.Pi / 2;

            public static float EaseIn(float t) {
                return 1f - Mathf.Sin((1f - t) * HalfPi);
            }

            public static float EaseOut(float t) {
                return Mathf.Sin(t * HalfPi);
            }

            public static float EaseInOut(float t) {
                return (Mathf.Sin(t * Mathf.Pi - HalfPi) + 1) / 2;
            }

            public static float EaseOutIn(float t) {
                return t < 0.5f ? 0.5f * Mathf.Sin(t * Mathf.Pi) : 0.5f + 0.5f * Mathf.Sin((t - 0.5f) * Mathf.Pi);
            }
        }

        public static class Power {
            public static float EaseIn(float t, float power) {
                return Mathf.Pow(t, power);
            }

            public static float EaseOut(float t, float power) {
                return 1f - Mathf.Pow(1f - t, power);
            }

            public static float EaseInOut(float t, float power) {
                return t < 0.5f ? EaseIn(t * 2f, power) * 0.5f : 0.5f + EaseOut((t - 0.5f) * 2f, power) * 0.5f;
            }

            public static float EaseOutIn(float t, float power) {
                return t < 0.5f ? EaseOut(t * 2f, power) * 0.5f : 0.5f + EaseIn((t - 0.5f) * 2f, power) * 0.5f;
            }
        }

        public static class Expo {
            public static float EaseIn(float t) => Mathf.Pow(2, 10 * (t - 1));
            public static float EaseOut(float t) => 1 - EaseIn(1 - t);

            public static float EaseInOut(float t) {
                return t < 0.5f ? EaseIn(t * 2f) * 0.5f : 0.5f + EaseOut((t - 0.5f) * 2f) * 0.5f;
            }

            public static float EaseOutIn(float t) {
                return t < 0.5f ? EaseOut(t * 2f) * 0.5f : 0.5f + EaseIn((t - 0.5f) * 2f) * 0.5f;
            }
        }

        public static class Elastic {
            private const float P = 0.3f;
            public static float EaseIn(float t) => 1 - EaseOut(1 - t);

            public static float EaseOut(float t) {
                return Mathf.Pow(2, -10 * t) * (float)Mathf.Sin((t - P / 4) * (2 * Mathf.Pi) / P) + 1;
            }

            public static float EaseInOut(float t) {
                return t < 0.5f ? EaseIn(t * 2f) * 0.5f : 0.5f + EaseOut((t - 0.5f) * 2f) * 0.5f;
            }

            public static float EaseOutIn(float t) {
                return t < 0.5f ? EaseOut(t * 2f) * 0.5f : 0.5f + EaseIn((t - 0.5f) * 2f) * 0.5f;
            }
        }

        public static class Circle {
            public static float EaseIn(float t) => -(Mathf.Sqrt(1 - t * t) - 1);
            public static float EaseOut(float t) => 1 - EaseIn(1 - t);

            public static float EaseInOut(float t) {
                return t < 0.5f ? EaseIn(t * 2f) * 0.5f : 0.5f + EaseOut((t - 0.5f) * 2f) * 0.5f;
            }

            public static float EaseOutIn(float t) {
                return t < 0.5f ? EaseOut(t * 2f) * 0.5f : 0.5f + EaseIn((t - 0.5f) * 2f) * 0.5f;
            }
        }

        public static class Back {
            private const float S = 1.70158f;
            public static float EaseIn(float t) {
                return t * t * ((S + 1) * t - S);
            }

            public static float EaseOut(float t) => 1 - EaseIn(1 - t);

            public static float EaseInOut(float t) {
                return t < 0.5f ? EaseIn(t * 2f) * 0.5f : 0.5f + EaseOut((t - 0.5f) * 2f) * 0.5f;
            }

            public static float EaseOutIn(float t) {
                return t < 0.5f ? EaseOut(t * 2f) * 0.5f : 0.5f + EaseIn((t - 0.5f) * 2f) * 0.5f;
            }
        }

        public static class Bounce {
            private const float Div = 2.75f;
            private const float Mult = 7.5625f;
            public static float EaseIn(float t) => 1 - EaseOut(1 - t);

            public static float EaseOut(float t) {
                if (t < 1 / Div) {
                    return Mult * t * t;
                } else if (t < 2 / Div) {
                    t -= 1.5f / Div;
                    return Mult * t * t + 0.75f;
                } else if (t < 2.5 / Div) {
                    t -= 2.25f / Div;
                    return Mult * t * t + 0.9375f;
                } else {
                    t -= 2.625f / Div;
                    return Mult * t * t + 0.984375f;
                }
            }

            public static float EaseInOut(float t) {
                return t < 0.5f ? EaseIn(t * 2f) * 0.5f : 0.5f + EaseOut((t - 0.5f) * 2f) * 0.5f;
            }

            public static float EaseOutIn(float t) {
                return t < 0.5f ? EaseOut(t * 2f) * 0.5f : 0.5f + EaseIn((t - 0.5f) * 2f) * 0.5f;
            }
        }

        public static class Spring {
            public static float EaseIn(float t) {
                return 1f - EaseOut(1f - t);
            }

            public static float EaseOut(float t) {
                return 1f - Mathf.Exp(-8f * t) * Mathf.Cos(12f * t);
            }

            public static float EaseInOut(float t) {
                return t < 0.5f ? EaseIn(t * 2f) * 0.5f : 0.5f + EaseOut((t - 0.5f) * 2f) * 0.5f;
            }

            public static float EaseOutIn(float t) {
                return t < 0.5f ? EaseOut(t * 2f) * 0.5f : 0.5f + EaseIn((t - 0.5f) * 2f) * 0.5f;
            }
        }
    }
}