namespace Betauer.Core.Easing;

public static class BiasGainFunctions {
    public static float Bias(float time, float bias) {
        return time / ((1.0f / bias - 2.0f) * (1.0f - time) + 1.0f);
    }

    public static float Gain(float time, float bias, float offset) {
        if (time < offset)
            return Bias(time / offset, bias) * offset;
        return Bias((time - offset) / (1 - offset), 1 - bias) * (1 - offset) + offset;
    }
}