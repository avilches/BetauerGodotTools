using System;

namespace Betauer.Core.Easing; 

public interface IInterpolationGrid2D {

    public int Width { get; }
    public int Height { get; }

    /// <summary>
    /// return a number between 0 and 1 based on the coords
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public float Get(int x, int y);

    public void Loop(Action<int, int, float> action);

}