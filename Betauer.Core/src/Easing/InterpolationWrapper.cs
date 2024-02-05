namespace Betauer.Core.Easing;

public class InterpolationWrapper : IInterpolation {
    public IInterpolation Interpolation { get; }

    public InterpolationWrapper(IInterpolation interpolation) {
        Interpolation = interpolation;
    }

    public float GetY(float t) => Interpolation.GetY(t);
}