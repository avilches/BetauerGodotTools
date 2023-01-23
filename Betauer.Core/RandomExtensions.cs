using System;

namespace Betauer.Core; 

public static class RandomExtensions {
    /// <summary>
    /// Random integer in range, both inclusive
    /// </summary>
    public static int Range(this Random random, int start, int end) => end == start ? start : random.Next(Math.Min(start, end), Math.Max(start, end) + 1);

    /// <summary>
    /// Random float in range, with a fixed number of decimals.
    /// random.Range(0.2f, 1.5f, 2) -> 0.20 to 1.50, only two decimals
    /// </summary>
    public static float Range(this Random random, float start, float end, int decimals) {
        var min = Math.Min(start, end);
        var max = Math.Max(start, end);
        var size = (int)Math.Pow(10, decimals);
        var imin = (int)(min * size);
        var imax = (int)(max * size);
        var result = random.Next(imin, imax + 1);
        return (float)result / size;
    }

    /// <summary>
    /// Random float in range, both inclusive
    /// </summary>
    public static long Range(this Random random, long start, long end) => end == start ? start : random.NextInt64(Math.Min(start, end), Math.Max(start, end) + 1);

    /// <summary>
    /// Random float in range
    /// </summary>
    public static float Range(this Random random, float start, float end) => (float)Range(random, (double)start, (double)end);
    
    /// <summary>
    /// Random double in range
    /// </summary>
    public static double Range(this Random random, double start, double end) {
        var min = Math.Min(start, end);
        var max = Math.Max(start, end);
        var range = max - min;
        var factor = random.NextDouble();
        var val = factor * range + min;
        return val;    
    }
    
}