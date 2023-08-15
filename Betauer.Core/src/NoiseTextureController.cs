using System;
using Betauer.Core;
using Godot;

namespace Betauer.Core;

/*
 * It takes a NoiseTexture2D with a gradient with InterpolationMode Constant and return the noise using the pixel from the image
 * This results in a more accurate noise than using the Noise class, so you can be sure the noise generated is exactly the same as image
 */
public class NoiseTextureController : FastPixelTextureController {
    private readonly Color[] _colors;
    
    public NoiseTextureController(NoiseTexture2D noiseTexture) : base(noiseTexture) {
        if (noiseTexture.ColorRamp == null || noiseTexture.ColorRamp.InterpolationMode != Gradient.InterpolationModeEnum.Constant) {
            throw new Exception("A Gradient with InterpolationMode Constant is expected");
        }
        _colors = noiseTexture.ColorRamp.Colors;
    }
    
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
