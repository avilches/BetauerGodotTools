namespace Betauer.Core.Easing;

public class BiasGainEasing : IEasing {
    public float Bias { get; set; } = 0.5f;
    public float Offset { get; set; } = 0.5f;
    public float GetY(float t) => BiasGainFunctions.Gain(t, Bias, Offset);
}