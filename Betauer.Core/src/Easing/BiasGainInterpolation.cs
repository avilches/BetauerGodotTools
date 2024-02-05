namespace Betauer.Core.Easing;

public struct BiasGainInterpolation : IInterpolation {
    public float Bias { get; set; } = 0.5f;
    public float Offset { get; set; } = 0.5f;

    public BiasGainInterpolation(float bias, float offset) {
        Bias = bias;
        Offset = offset;
    }

    public float GetY(float t) => Functions.Gain(t, Bias, Offset);
}