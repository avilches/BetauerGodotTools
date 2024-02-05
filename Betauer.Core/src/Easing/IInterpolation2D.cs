namespace Betauer.Core.Easing; 

public interface IInterpolation2D {
    /// <summary>
    /// t is a number between 0 and 1
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public float Get(float x, float y);
}