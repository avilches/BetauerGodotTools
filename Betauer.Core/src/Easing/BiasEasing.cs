namespace Betauer.Core.Easing;

public class BiasEasing : IEasing {
    public float Bias { get; set; } = 0.5f;
    public float GetY(float t) => BiasGainFunctions.Bias(t, Bias);
}