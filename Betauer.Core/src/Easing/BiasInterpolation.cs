namespace Betauer.Core.Easing;

public class BiasInterpolation : IInterpolation {
    public float Bias { get; set; } = 0.5f;

    public BiasInterpolation(float bias) {
        Bias = bias;
    }

    public float Get(float t) => Functions.Bias(t, Bias);
}