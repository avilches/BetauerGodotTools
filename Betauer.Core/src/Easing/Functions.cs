using System;
using Godot;

namespace Betauer.Core.Easing; 

public static class Functions {
    // Adapted from source : http://robertpenner.com/easing/

    public static Func<float, float> GetEaseFunc(Tween.TransitionType type, Tween.EaseType easeType) {
        if (type == Tween.TransitionType.Linear) return (t) => t;
        return easeType switch {
            Tween.EaseType.In => GetEaseInFunc(type),
            Tween.EaseType.Out => GetEaseOutFunc(type),
            Tween.EaseType.InOut => GetEaseInOutFunc(type),
            Tween.EaseType.OutIn => GetEaseOutInFunc(type),
        };
    }

    public static Func<float, float> GetEaseInFunc(Tween.TransitionType type) {
        return type switch {
            Tween.TransitionType.Linear => (t) => t,
            Tween.TransitionType.Sine => Sine.EaseIn,
            Tween.TransitionType.Quad => (t) => Power.EaseIn(t, 2),
            Tween.TransitionType.Cubic => (t) => Power.EaseIn(t, 3),
            Tween.TransitionType.Quart => (t) => Power.EaseIn(t, 4),
            Tween.TransitionType.Quint => (t) => Power.EaseIn(t, 5),
            Tween.TransitionType.Expo => Expo.EaseIn,
            Tween.TransitionType.Elastic => Elastic.EaseIn,
            Tween.TransitionType.Circ => Circle.EaseIn,
            Tween.TransitionType.Bounce => Bounce.EaseIn,
            Tween.TransitionType.Back => Back.EaseIn,
            Tween.TransitionType.Spring => Spring.EaseIn,
        };
    }

    public static Func<float, float> GetEaseOutFunc(Tween.TransitionType type) {
        return type switch {
            Tween.TransitionType.Linear => (t) => t,
            Tween.TransitionType.Sine => Sine.EaseOut,
            Tween.TransitionType.Quad => (t) => Power.EaseOut(t, 2),
            Tween.TransitionType.Cubic => (t) => Power.EaseOut(t, 3),
            Tween.TransitionType.Quart => (t) => Power.EaseOut(t, 4),
            Tween.TransitionType.Quint => (t) => Power.EaseOut(t, 5),
            Tween.TransitionType.Expo => Expo.EaseOut,
            Tween.TransitionType.Elastic => Elastic.EaseOut,
            Tween.TransitionType.Circ => Circle.EaseOut,
            Tween.TransitionType.Bounce => Bounce.EaseOut,
            Tween.TransitionType.Back => Back.EaseOut,
            Tween.TransitionType.Spring => Spring.EaseOut,
        };
    }

    public static Func<float, float> GetEaseInOutFunc(Tween.TransitionType type) {
        return type switch {
            Tween.TransitionType.Linear => (t) => t,
            Tween.TransitionType.Sine => Sine.EaseInOut,
            Tween.TransitionType.Quad => (t) => Power.EaseInOut(t, 2),
            Tween.TransitionType.Cubic => (t) => Power.EaseInOut(t, 3),
            Tween.TransitionType.Quart => (t) => Power.EaseInOut(t, 4),
            Tween.TransitionType.Quint => (t) => Power.EaseInOut(t, 5),
            Tween.TransitionType.Expo => Expo.EaseInOut,
            Tween.TransitionType.Elastic => Elastic.EaseInOut,
            Tween.TransitionType.Circ => Circle.EaseInOut,
            Tween.TransitionType.Bounce => Bounce.EaseInOut,
            Tween.TransitionType.Back => Back.EaseInOut,
            Tween.TransitionType.Spring => Spring.EaseInOut,
        };
    }

    public static Func<float, float> GetEaseOutInFunc(Tween.TransitionType type) {
        return type switch {
            Tween.TransitionType.Linear => (t) => t,
            Tween.TransitionType.Sine => Sine.EaseOutIn,
            Tween.TransitionType.Quad => (t) => Power.EaseOutIn(t, 2),
            Tween.TransitionType.Cubic => (t) => Power.EaseOutIn(t, 3),
            Tween.TransitionType.Quart => (t) => Power.EaseOutIn(t, 4),
            Tween.TransitionType.Quint => (t) => Power.EaseOutIn(t, 5),
            Tween.TransitionType.Expo => Expo.EaseOutIn,
            Tween.TransitionType.Elastic => Elastic.EaseOutIn,
            Tween.TransitionType.Circ => Circle.EaseOutIn,
            Tween.TransitionType.Bounce => Bounce.EaseOutIn,
            Tween.TransitionType.Back => Back.EaseOutIn,
            Tween.TransitionType.Spring => Spring.EaseOutIn,
        };
    }

    public static float EaseIn(Tween.TransitionType type, float t) {
        if (type == Tween.TransitionType.Linear) return t;
        return GetEaseInFunc(type).Invoke(t);
    }

    public static float EaseOut(Tween.TransitionType type, float t) {
        if (type == Tween.TransitionType.Linear) return t;
        return GetEaseOutFunc(type).Invoke(t);
    }

    public static float EaseInOut(Tween.TransitionType type, float t) {
        if (type == Tween.TransitionType.Linear) return t;
        return GetEaseInOutFunc(type).Invoke(t);
    }

    public static float EaseOutIn(Tween.TransitionType type, float t) {
        if (type == Tween.TransitionType.Linear) return t;
        return GetEaseOutInFunc(type).Invoke(t);
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
    
    public static float Bias(float time, float bias) {
        return time / ((1.0f / bias - 2.0f) * (1.0f - time) + 1.0f);
    }

    public static float Gain(float time, float bias, float offset) {
        if (time < offset)
            return Bias(time / offset, bias) * offset;
        return Bias((time - offset) / (1 - offset), 1 - bias) * (1 - offset) + offset;
    }
    
    /// <summary>
    /// Returns a logistic curve with a S form that can be adjusted
    /// </summary>
    public static float Logistic(float time, float exp, float offset) {
        var pow = Mathf.Pow(time, exp);
        return pow / (pow + Mathf.Pow(offset - offset * time, exp));
    }
    
    /// <summary>
    /// Shift a easing curve where the start and end are the points where the curve starts and ends. If t &lt; start it returns 0, if t &gt; end it returns 1
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="t"></param>
    /// <param name="easingFunction"></param>
    /// <returns></returns>
    public static float Shift(float start, float end, float t, Func<float, float> easingFunction) {
        if (start >= end) {
            (start, end) = (end, start);
        }
        if (t <= start) return 0f;
        if (t >= end) return 1f;
        // Shift a easing curve where the start and end are the points where the curve starts and ends. If t > start it returns 0, if t < end it returns 1
        var length = end - start;
        var tt = (t - start) / length;
        return easingFunction.Invoke(tt);
    }

    public static float GetRect2D(float width, float height, float centerX, float centerY, float x, float y) {
        var maxDistanceX = Math.Max(centerX, width - 1 - centerX);
        var maxDistanceY = Math.Max(centerY, height - 1 - centerY);
        var distanceX = Math.Abs(x - centerX);
        var distanceY = Math.Abs(y - centerY);
        var valueX = 1f - distanceX / maxDistanceX;
        var valueY = 1f - distanceY / maxDistanceY;
        return Math.Min(valueX, valueY);
    }

    public static float GetRect2D(float width, float height, float x, float y) {
        var centerX = width / 2;
        var centerY = height / 2;
        var distanceX = Math.Abs(x - centerX);
        var distanceY = Math.Abs(y - centerY);
        var valueX = 1f - distanceX / centerX;
        var valueY = 1f - distanceY / centerY;
        return Math.Min(valueX, valueY);
    }
    
    // Return a value from 0 to 1, where 1 is when the point x,y is in the center of the ellipse and 0 when it's in the border
    public static float GetEllipse(float rx, float ry, float x, float y) {
        return Mathf.Clamp(1 - ((x * x) / (rx * rx) + (y * y) / (float)(ry * ry)), 0f, 1f);
    }

    // Return a value from 0 to 1, where 1 is when the point x,y is in the center of the circle and 0 when it's in the border
    public static float GetCircle(float r, float x, float y) {
        return Math.Clamp(1 - ((x * x + y * y) / (r * r)), 0f, 1f);
    }

}