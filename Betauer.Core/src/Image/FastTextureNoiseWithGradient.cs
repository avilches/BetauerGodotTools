using System;
using Godot;

namespace Betauer.Core.Image;

/**
 * It takes a NoiseTexture2D with a constant gradient.
 * Then, the image as array of bytes is loaded in the C# memory, so we can access to the values directly, which is faster than using GetNoise2D.
 * It takes more memory than FastNoise, but it's still as faster as it.
 * It allows to query pixels and return a gradient color position. This allow to generate content based on the noise and it will look exactly the same as the texture.
 */
public class FastTextureNoiseWithGradient : FastImage {
    private readonly Color[] _colors;
    
    public FastTextureNoiseWithGradient(NoiseTexture2D texture) : base(texture) {
        if (texture.ColorRamp != null) {
            if (texture.ColorRamp.InterpolationMode != Gradient.InterpolationModeEnum.Constant) {
                throw new Exception("A Gradient with InterpolationMode Constant is expected. If you only want to get the noise, use FastTextureNoise instead");
            }
            _colors = texture.ColorRamp.Colors;
        }
    }

    /// <summary>
    /// Using the colors from the gradient, returns the noise as a value from 0 to (number of gradient colors - 1)
    /// So, gradient with 3 colors will return values from 0 to 2
    /// If there is no constant gradient, it will return 0
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int GetNoiseGradient(int x, int y) {
        if (_colors == null) return 0;
        var pixelColor = GetPixel(x, y);
        var p = FindColor(pixelColor);
        return p;
    }

    public float GetNoise(int x, int y) {
        return GetPixel(x, y).Luminance;
    }

    private int FindColor(Color pixelColor) {
        for (var index = 0; index < _colors.Length; index++) {
            var color = _colors[index];
            if (color.R == pixelColor.R &&
                color.G == pixelColor.G &&
                color.B == pixelColor.B &&
                color.A == pixelColor.A) {
                return index;
            }
        }
        // Sometimes, gradient color are slightly different from the pixel color. This is a workaround to find the closest color
        // and fix the gradient color, so next time it will be found.
        var pos = pixelColor.FindClosestColorPosition(_colors);
        _colors[pos] = pixelColor;
        return pos;
    }
}
