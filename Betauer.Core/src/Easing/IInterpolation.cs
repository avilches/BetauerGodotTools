namespace Betauer.Core.Easing; 

public interface IInterpolation {
    /// <summary>
    /// t is a number between 0 and 1
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public float GetY(float t);
}