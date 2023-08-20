using System;
using Godot;

namespace Betauer.Core;

/*
 * It takes a NoiseTexture2D with a gradient with InterpolationMode Constant and return the noise using the pixel from the image.
 * This works faster then use GetNoise2D or GetNoise2Dv, because it uses the image pixel directly which is already cached as a byte array and there is
 * no interop with Godot and C#
 * On the other hand, the noise values are more accurate noise than using the Noise class, so you can be sure the noise generated is exactly the same as image
 */
public class FastNoiseImageGradient : FastPixelImage {
    private readonly Color[] _colors;
    
    public FastNoiseImageGradient(NoiseTexture2D texture) : base(texture) {
        if (texture.ColorRamp == null || texture.ColorRamp.InterpolationMode != Gradient.InterpolationModeEnum.Constant) {
            throw new Exception("A Gradient with InterpolationMode Constant is expected");
        }
        _colors = texture.ColorRamp.Colors;
    }
    
    /// <summary>
    /// Using the colors from the gradient, returns the noise as a value from 0 to number of gradient colors - 1
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int GetNoise(int x, int y) {
        var pixelColor = GetPixel(x, y); // a faster version of:_image.GetPixel(x, y);
        var p = FindColor(pixelColor);
        return p;
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
