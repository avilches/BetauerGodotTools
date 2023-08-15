using Godot;

namespace Betauer.Core;

public static class ColorExtensions {
    /// <summary>
    /// Returns the closest color position in the array of colors
    /// </summary>
    /// <param name="source"></param>
    /// <param name="colors"></param>
    /// <returns></returns>
    public static int FindClosestColorPosition(this Color source, Color[] colors) {
        var closestDistance = double.MaxValue;
        var found = 0;
        for (var idx = 0; idx < colors.Length; idx++) {
            var color = colors[idx];
            var distance = source.GetDistance(color);
            if (distance < closestDistance) {
                closestDistance = distance;
                found = idx;
            }
        }
        return found;
    }

    /// <summary>
    /// Return the color in the array of colors that is closest to the source color
    /// </summary>
    /// <param name="source"></param>
    /// <param name="colors"></param>
    /// <returns></returns>
    public static Color FindClosestColor(this Color source, Color[] colors) {
        var closestDistance = double.MaxValue;
        var found = colors[0];
        foreach (var color in colors) {
            var distance = source.GetDistance(color);
            if (distance < closestDistance) {
                closestDistance = distance;
                found = color;
            }
        }
        return found;
    }

    public static double GetDistance(this Color color1, Color color2) {
        var deltaR = color1.R - color2.R;
        var deltaG = color1.G - color2.G;
        var deltaB = color1.B - color2.B;
        return deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
    }	
	
}